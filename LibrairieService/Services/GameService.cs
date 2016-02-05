using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel;

using LibrairieService.Models;

namespace LibrairieService.Services
{
    public class GameService : IGameService
    {
        public static event Action<LogLevel, string> EventOccurred;
        public static event Action<LogLevel, string> ErrorOccurred;

        public Guid CreateGame(Guid player, Guid room)
        {
            using (var context = new MilleBornesEntities())
            {
                var playerEntity = context.User
                    .Where(us => us.LoggedInUser != null)
                    .SingleOrDefault(us => us.LoggedInUser.Token == player);
                if (playerEntity == null)
                {
                    throw new FaultException("Le joueur n'est pas valide ou est hors-ligne.");
                }

                var roomEntity = context.Room
                    .SingleOrDefault(rm => rm.Token == room);
                if (roomEntity == null)
                {
                    throw new FaultException("La salle n'est pas trouvée.");
                }

                if (roomEntity.MasterUserId != playerEntity.UserId)
                {
                    throw new FaultException("Seul le maître de la salle peut démarrer la partie.");
                }

                var newGame = new Game()
                {
                    Token = Guid.NewGuid(),
                    StartDate = DateTime.UtcNow,
                    IsTakingDisconnectDecision = false
                };

                FillPlayerConfig(context, roomEntity, newGame);

                roomEntity.Game = newGame;
                roomEntity.Started = true;
                context.Game.Add(newGame);
                try
                {
                    context.SaveChanges();
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException dbue)
                {
                    throw;
                }

                InitializeGame(newGame);
                try
                {
                    context.SaveChanges();
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException dbue)
                {
                    throw;
                }

                return newGame.Token;
            }
        }

        private static void FillPlayerConfig(MilleBornesEntities context, Room roomEntity, Game newGame)
        {
            // Essayer en premier lieu avec le service, sinon
            // regarder directement dans la base de données.

            List<PlayerGame> playerGameSource = new List<PlayerGame>();
#if false
            try
            {
                using (var client = new LobbyGameProxy.LobbyServiceClient())
                {
                    var configEntries = client.GetPlayerConfig(roomEntity.Token);

                    foreach (var entry in configEntries)
                    {
                        var user = context.User
                            .Where(us => us.LoggedInUser != null)
                            .SingleOrDefault(us => us.LoggedInUser.Token == entry.UserToken);

                        if (user == null)
                        {
                            continue;
                        }

                        var playerGame = new PlayerGame()
                        {
                            Game = newGame,
                            UserId = user.UserId,
                            User = user,
                            LastHeartbeat = DateTime.UtcNow,
                            Order = entry.Order,
                            Team = entry.Team + 1,
                            HasJoined = user.UserId == roomEntity.MasterUserId
                        };
                    }
                }
            }
#endif
            //catch (Exception ex)
            //{
                playerGameSource = roomEntity.PlayerRoomState.Select((prs, inx) => new PlayerGame()
                {
                    Game = newGame,
                    UserId = prs.UserId,
                    User = prs.User,
                    LastHeartbeat = DateTime.UtcNow,
                    Order = inx,
                    Team = prs.Team + 1,
                    HasJoined = prs.UserId == roomEntity.MasterUserId
                })
                .ToList();
            //}

            newGame.PlayerGame = playerGameSource;
        }

        private void InitializeGame(Game game)
        {
            DrawInitialHands(game);

            var firstPlayer = GetPlayersInGame(game)
                .OrderBy(pg => pg.Order)
                .FirstOrDefault();

            ChangePlayer(game, firstPlayer.UserId);
        }

        public Guid JoinGame(Guid player, Guid room)
        {
            using (var context = new MilleBornesEntities())
            {
                var playerEntity = context.User
                    .Where(us => us.LoggedInUser != null)
                    .SingleOrDefault(us => us.LoggedInUser.Token == player);
                if (playerEntity == null)
                {
                    throw new FaultException("Le joueur n'est pas valide ou est hors-ligne.");
                }

                var roomEntity = context.Room
                    .SingleOrDefault(rm => rm.Token == room);
                if (roomEntity == null)
                {
                    throw new FaultException("La salle n'est pas trouvée.");
                }

                var game = roomEntity.Game;
                if (game == null)
                {
                    throw new FaultException("Celle salle n'est pas associée à une partie en cours.");
                }

                var playerGame = game.PlayerGame
                    .SingleOrDefault(pg => pg.UserId == playerEntity.UserId);
                if (playerGame == null)
                {
                    throw new FaultException("Le joueur n'est pas dans cette partie.");
                }

                if (playerGame.HasJoined)
                {
                    throw new FaultException("Le joueur a déja rejoint la partie.");
                }

                playerGame.HasJoined = true;
                playerGame.LastHeartbeat = DateTime.UtcNow;
                context.SaveChanges();

                return game.Token;
            }
        }

        public bool DoHeartbeat(Guid playerToken, Guid gameToken)
        {
            using (var context = new MilleBornesEntities())
            {
                var player = GetPlayerFromToken(context, playerToken);

                if (player == null)
                {
                    throw new FaultException("Le joueur n'est pas trouvé.");
                }

                var game = context.Game
                    .SingleOrDefault(gm => gm.Token == gameToken);

                if (game == null)
                {
                    throw new FaultException("La partie n'est pas trouvée.");
                }

                var playerGame = game.PlayerGame
                    .SingleOrDefault(pg => pg.UserId == player.UserId);
                if (playerGame == null)
                {
                    throw new FaultException("Le joueur n'est pas dans la partie.");
                }

                if (!playerGame.HasJoined)
                {
                    throw new FaultException("Le joueur n'a pas rejoint la partie.");
                }

                bool validPlayerHeartbeat = ValidatePlayerHeartbeat(playerGame);
                if (validPlayerHeartbeat)
                {
                    playerGame.LastHeartbeat = DateTime.UtcNow;
                    context.SaveChanges();
                }

                if (!CheckAllPlayersHeartbeat(game))
                {
                    context.SaveChanges();
                }

                return validPlayerHeartbeat;
            }
        }

        private bool CheckAllPlayersHeartbeat(Game game)
        {
            if (GetPlayersInGame(game)
                .Where(pg => pg.HasJoined)
                .Any(pg => !ValidatePlayerHeartbeat(pg)))
            {
                // S'assurer qu'il reste plus d'un joueur, sinon terminer la partie.
                if (GetPlayersInGame(game)
                    .Where(pg => pg.HasJoined)
                    .Where(pg => ValidatePlayerHeartbeat(pg))
                    .Count() <= 1)
                {
                    EndGame(game, GameEndReason.PLAYER_DISCONNECTION);

                    return false;
                }

                var masterPlayerId = game.Room.First().MasterUserId;

                // S'assurer que le maître de la salle est présent pour prendre
                // la décision, sionon terminer la partie aussi.
                if (GetPlayersInGame(game)
                    .Where(pg => pg.HasJoined)
                    .Where(pg => ValidatePlayerHeartbeat(pg))
                    .Where(pg => pg.UserId == masterPlayerId)
                    .Count() != 1)
                {
                    EndGame(game, GameEndReason.PLAYER_DISCONNECTION);

                    return false;
                }

                game.IsTakingDisconnectDecision = true;
                return false;
            }

            return true;
        }

        private bool ValidatePlayerHeartbeat(PlayerGame playerGame)
        {
            DateTime currentTime = DateTime.UtcNow;
            return (currentTime - playerGame.LastHeartbeat).TotalMilliseconds <= 20000;
        }

        public GameState GetState(Guid playerToken, Guid gameToken)
        {
            using (var context = new MilleBornesEntities())
            {
                var player = GetPlayerFromToken(context, playerToken);

                if (player == null)
                {
                    throw new FaultException("Le joueur n'est pas trouvé.");
                }

                var game = context.Game
                    .SingleOrDefault(gm => gm.Token == gameToken);

                if (game == null)
                {
                    throw new FaultException("La partie n'est pas trouvée.");
                }

                var playerGame = GetPlayersInGame(game)
                    .SingleOrDefault(pg => pg.UserId == player.UserId);
                if (playerGame == null)
                {
                    throw new FaultException("Le joueur n'est pas dans la partie.");
                }

                if (!playerGame.HasJoined)
                {
                    throw new FaultException("Le joueur n'a pas rejoint la partie.");
                }

                if (!ValidatePlayerHeartbeat(playerGame))
                {
                    throw new FaultException("Le heartbeat du joueur a expiré.");
                }

                var drawCardEvents = game.GameEvent
                    .Where(ge => ge.Type == GameEventType.DRAW_CARD)
                    .Cast<DrawCardEvent>()
                    .Where(dc => dc.PlayerId == player.UserId)
                    .Where(dc => dc.PlayCardEvent.Count == 0);

                var teamStates = GetTeamsState(game);

                var ownTeamState = teamStates
                    .Single(ts => ts.TeamIndex == playerGame.Team);
                var opponentsTeamStates = teamStates
                    .Where(ts => ts.TeamIndex != playerGame.Team)
                    .ToArray();

                var currentTurnPlayer = GetLastPlayerChangeEvent(game);
                var currentTurnPlayerGame = GetPlayersInGame(game)
                    .SingleOrDefault(pg => pg.UserId == currentTurnPlayer.NewPlayerId);

                GameState gameState = new GameState()
                {
                    GameEnd = game.EndReason.HasValue,
                    GameEndReason = game.EndReason,
                    WaitingForDecision = game.IsTakingDisconnectDecision,
                    CardsInHand = drawCardEvents.Select(dc => new GameCard()
                    {
                        CardId = dc.CardIndex,
                        Token = dc.Token
                    })
                    .ToArray(),
                    OwnTeamState = ownTeamState,
                    OpponentsTeamStates = opponentsTeamStates,
                    Players = game.PlayerGame.Select(pg => new GamePlayer()
                    {
                        Name = pg.User.Name,
                        Order = pg.Order,
                        TeamIndex = pg.Team
                    })
                    .ToArray(),
                    CurrentTeam = currentTurnPlayerGame.Team,
                    CurrentPlayer = currentTurnPlayerGame.Order,
                    IsOwnTeamTurn = currentTurnPlayerGame.Order == playerGame.Order,
                    IsOwnTurn = currentTurnPlayerGame.Team == playerGame.Team,
                    OwnTeamIndex = playerGame.Team,
                    OwnPlayerOrder = playerGame.Order
                };

                return gameState;
            }
        }

        public PlayCardResult PlayCard(Guid gameToken, Guid playerToken, Guid cardToken, int targetTeamIndex)
        {
            using (var context = new MilleBornesEntities())
            {
                var player = GetPlayerFromToken(context, playerToken);

                if (player == null)
                {
                    throw new FaultException("Le joueur n'est pas trouvé.");
                }

                var game = context.Game
                    .SingleOrDefault(gm => gm.Token == gameToken);

                if (game == null)
                {
                    throw new FaultException("La partie n'est pas trouvée.");
                }

                if (!GetPlayersInGame(game).All(pg => pg.HasJoined))
                {
                    return PlayCardResult.NOT_ALL_PLAYERS_PRESENT;
                }

                if (game.IsTakingDisconnectDecision)
                {
                    throw new FaultException("La partie est en attente de décison de déconnection d'un joueur.");
                }

                var playerGame = GetPlayersInGame(game)
                    .SingleOrDefault(pg => pg.UserId == player.UserId);
                if (playerGame == null)
                {
                    throw new FaultException("Le joueur n'est pas dans la partie.");
                }

                if (!playerGame.HasJoined)
                {
                    throw new FaultException("Le joueur n'a pas rejoint la partie.");
                }

                if (!ValidatePlayerHeartbeat(playerGame))
                {
                    throw new FaultException("Le heartbeat du joueur à expiré.");
                }

                if (game.EndReason != null)
                {
                    throw new FaultException("La partie est terminée.");
                }

                var currentPlayerId = GetCurrentPlayerUserId(game);
                if (currentPlayerId != player.UserId)
                {
                    LogPlayCardError(
                        "ce n'est pas le tour du joueur {0}({1})",
                        playerToken.ToString("B"),
                        player.Name
                    );

                    return PlayCardResult.WRONG_TURN;
                }

                var card = game.GameEvent
                    .Where(ge => ge.Type == GameEventType.DRAW_CARD)
                    .Cast<DrawCardEvent>()
                    .Where(dc => dc.Token == cardToken)
                    .SingleOrDefault();

                if (card == null)
                {
                    LogPlayCardError(
                        "le jeton de carte {0} est introuvable pour la partie {1}",
                        cardToken.ToString("B"),
                        gameToken.ToString("B")
                    );

                    return PlayCardResult.WRONG_TOKEN;
                }

                if (card.PlayerId != player.UserId)
                {
                    LogPlayCardError(
                        "le jeton de carte {0} est introuvable pour le joueur {1}",
                        cardToken.ToString("B"),
                        playerToken.ToString("B")
                    );

                    return PlayCardResult.WRONG_TOKEN_PLAYER;
                }

                if (card.PlayCardEvent.Count != 0)
                {
                    LogPlayCardError(
                        "le joueur {0} à joué la carte {1} déja jouée",
                        playerToken.ToString("B"),
                        cardToken.ToString("B")
                    );
                    return PlayCardResult.ALREADY_PLAYED;
                }

                if (!CanPlayCard(game, card, playerGame.Team, targetTeamIndex))
                {
                    LogPlayCardError(
                        "le joueur {0} à tenté de jouer la carte {1} sur l'équipe {2} dans un état invalide.",
                        playerToken.ToString("B"),
                        cardToken.ToString("B"),
                        targetTeamIndex
                    );
                    return PlayCardResult.CANNOT_PLAY;
                }

                PlayCardInGame(game, card, targetTeamIndex);
                context.SaveChanges();
                if (CheckGameEnd(game))
                {
                    context.SaveChanges();

                    LogMessage(
                        "Le joueur {0} joue la carte {1} sur l'équipe {2}",
                        playerToken.ToString("B"),
                        cardToken.ToString("B"),
                        targetTeamIndex
                    );
                    LogMessage(
                        "La partie {0} est terminée par la raison {1}",
                        game.Token,
                        game.EndReason
                    );

                    return PlayCardResult.SUCCESS;
                }

                DrawNewCardToPlayer(game, player);

                var newPlayer = player;
                var playedCardDef = CardDefinitions.Cards
                    .Single(cd => cd.CardId == card.CardIndex);
                if (playedCardDef.CardType != CardType.EFFECT_INVINCIBLE)
                {
                    newPlayer = GoToNextPlayer(game);
                }

                context.SaveChanges();
                LogMessage(
                    "Le joueur {0} joue la carte {1} sur l'équipe {2}, le nouveau joueur est {3}",
                    playerToken.ToString("B"),
                    cardToken.ToString("B"),
                    targetTeamIndex,
                    newPlayer.LoggedInUser.Token.ToString("B")
                );
                return PlayCardResult.SUCCESS;
            }
        }

        private bool CheckGameEnd(Game game)
        {
            // Finir la partie en gagnant.

            var teamStates = GetTeamsState(game);

            foreach (var teamState in teamStates)
            {
                if (teamState.DistanceTraveled == 1000)
                {
                    EndGame(game, GameEndReason.WON_THOUSAND_MILES);
                    return true;
                }
            }

            // Finir la partie en épuisant toutes les cartes.
            // (Dans la pile et celle dans les mains des joueurs)

            var playerHands = GetAllPlayersHands(game);

            if (playerHands.All(x => x.Item2.Count == 0))
            {
                EndGame(game, GameEndReason.EXHAUSTED_DECK);
                return true;
            }

            return false;
        }

        private Tuple<int, List<DrawCardEvent>>[] GetAllPlayersHands(Game game)
        {
            var playerHands = game.GameEvent
                .Where(ge => ge.Type == GameEventType.DRAW_CARD)
                .Cast<DrawCardEvent>()
                .Where(dc => dc.PlayCardEvent.Count == 0)
                .GroupBy(dc => dc.PlayerId)
                .Select(g => Tuple.Create(g.Key, g.ToList()))
                .ToArray();

            return playerHands;
        }

        private void EndGame(Game game, GameEndReason reason)
        {
            game.EndDate = DateTime.UtcNow;
            game.EndToken = Guid.NewGuid();
            game.EndReason = reason;
        }

        public bool SendGameMessage(Guid playerToken, Guid gameToken, string message)
        {
            using (var context = new MilleBornesEntities())
            {
                var player = GetPlayerFromToken(context, playerToken);

                if (player == null)
                {
                    throw new FaultException("Le joueur n'est pas trouvé.");
                }

                var game = context.Game
                    .SingleOrDefault(gm => gm.Token == gameToken);

                if (game == null)
                {
                    throw new FaultException("La partie n'est pas trouvée.");
                }

                if (!GetPlayersInGame(game).Any(pg => pg.UserId == player.UserId))
                {
                    throw new FaultException("Le joueur n'est pas dans la partie.");
                }

                game.Message.Add(new Message() 
                {
                    UserId = player.UserId,
                    Type = MessageType.GAME,
                    Date = DateTime.UtcNow,
                    Content = message
                });

                try
                {
                    context.SaveChanges();
                    return true;
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException dbue)
                {
                    return false;
                }
            }
        }

        public UserMessage[] GetAllGameMessages(Guid gameToken)
        {
            using (var context = new MilleBornesEntities())
            {
                var game = context.Game
                    .SingleOrDefault(gm => gm.Token == gameToken);

                if (game == null)
                {
                    throw new FaultException("La partie n'est pas trouvée.");
                }

                var messages = game.Message
                    .Select(msg => new UserMessage()
                    {
                        Username = msg.User.Name,
                        Date = msg.Date,
                        Content = msg.Content
                    })
                    .ToArray();

                return messages;
            }
        }

        public UserMessage[] GetGameMessagesSinceDate(Guid gameToken, DateTime sinceDateUtc)
        {
            using (var context = new MilleBornesEntities())
            {
                var game = context.Game
                    .SingleOrDefault(gm => gm.Token == gameToken);

                if (game == null)
                {
                    throw new FaultException("La partie n'est pas trouvée.");
                }

                var messagesSinceDate = game.Message
                    .Where(msg => msg.Date > sinceDateUtc)
                    .Select(msg => new UserMessage()
                    {
                        Username = msg.User.Name,
                        Date = msg.Date,
                        Content = msg.Content
                    })
                    .ToArray();

                return messagesSinceDate;
            }
        }

        public void TakePlayerDisconnectionDecision(Guid playerToken, Guid gameToken, bool continueGame)
        {
            using (var context = new MilleBornesEntities())
            {
                var playerEntity = context.User
                    .Where(us => us.LoggedInUser != null)
                    .SingleOrDefault(us => us.LoggedInUser.Token == playerToken);
                if (playerEntity == null)
                {
                    throw new FaultException("Le joueur n'est pas valide ou est hors-ligne.");
                }

                var game = context.Game
                    .SingleOrDefault(rm => rm.Token == gameToken);
                if (game == null)
                {
                    throw new FaultException("La partie n'est pas trouvée.");
                }

                if (game.Room == null)
                {
                    throw new FaultException("La partie n'est pas associée à une salle.");
                }

                if (game.Room.Count > 1)
                {
                    throw new FaultException("La partie est associée à plusieurs salles.");
                }

                if (game.Room.First().MasterUserId != playerEntity.UserId)
                {
                    throw new FaultException("Seul le maître de la salle peut prendre la décision.");
                }

                if (!game.IsTakingDisconnectDecision)
                {
                    throw new FaultException("La partie n'est pas en attente d'une décision.");
                }

                if (continueGame)
                {
                    var disconnectedPlayers = GetPlayersInGame(game)
                        .Where(pg => !ValidatePlayerHeartbeat(pg));

                    foreach (var player in disconnectedPlayers)
                    {
                        player.Order = -player.Order;
                    }

                    game.IsTakingDisconnectDecision = false;
                }
                else
                {
                    EndGame(game, GameEndReason.PLAYER_DISCONNECTION);
                    game.IsTakingDisconnectDecision = false;
                }

                context.SaveChanges();
            }
        }

        public Guid GetReturnToken(Guid room)
        {
            throw new NotImplementedException();
        }

        private void LogVerboseMessage(string format, params object[] args)
        {
            string str = string.Format(format, args);
            var del = EventOccurred;
            if (del != null)
            {
                del(LogLevel.VERBOSE, str);
            }
        }

        private void LogMessage(string format, params object[] args)
        {
            string str = string.Format(format, args);
            var del = EventOccurred;
            if (del != null)
            {
                del(LogLevel.INFO, str);
            }
        }

        private void LogErrorMessage(string format, params object[] args)
        {
            string str = string.Format(format, args);
            var del = ErrorOccurred;
            if (del != null)
            {
                del(LogLevel.ERROR, str);
            }
        }

        private void LogPlayCardError(string format, params object[] args)
        {
            LogErrorMessage(
                "Erreur lors du jeu de la carte: {0}",
                string.Format(format, args)
            );
        }

        private User GetPlayerFromToken(MilleBornesEntities context, Guid playerToken)
        {
            var loggedInUser = context.LoggedInUser
                .SingleOrDefault(lu => lu.Token == playerToken);

            if (loggedInUser == null)
            {
                return null;
            }

            return loggedInUser.User;
        }

        /// <summary>
        /// Obtient les joueurs qui sont connectés, soit ceux que leur ordre est positif.
        /// 
        /// Les joueurs déconnectés dans une partie qui continue ont comme ordre
        /// la valeur négative de celle-ci.
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        private IEnumerable<PlayerGame> GetPlayersInGame(Game game)
        {
            return game.PlayerGame
                .Where(pg => pg.Order >= 0);
        }

        private void DrawCard(Game game, User user, int cardId)
        {
            game.GameEvent.Add(new DrawCardEvent()
            {
                ServerEvent = true,
                Date = DateTime.UtcNow,
                Type = GameEventType.DRAW_CARD,

                CardIndex = cardId,
                Token = Guid.NewGuid(),
                User = user
            });
        }

        private void DrawInitialHands(Game game)
        {
            Random random = new Random();
            var remainingCards = CardDefinitions.Deck.ToList();

            foreach (var player in GetPlayersInGame(game))
            {
                for (int i = 0; i < 5; i++)
                {
                    int listIndex = random.Next(remainingCards.Count);

                    DrawCard(game, player.User, remainingCards[listIndex]);
                    remainingCards.RemoveAt(listIndex);
                }
            }
        }

        private void PlayCardInGame(Game game, DrawCardEvent drawEvent, int targetTeamIndex)
        {
            game.GameEvent.Add(new PlayCardEvent()
            {
                ServerEvent = false,
                Date = DateTime.UtcNow,
                Type = GameEventType.PLAY_CARD,

                TargetTeamIndex = targetTeamIndex,
                DrawCardEventId = drawEvent.GameEventId
            });
        }

        private bool CanPlayCard(Game game, DrawCardEvent drawEvent, int playerTeamIndex, int targetTeamIndex)
        {
            if (targetTeamIndex == 0)
            {
                return true;
            }

            var targetTeamState = GetTeamState(game, targetTeamIndex);
            var card = CardDefinitions.Cards
                .Single(cd => cd.CardId == drawEvent.CardIndex);

            switch (card.CardType)
            {
                case CardType.VALUE:
                {
                    if (playerTeamIndex != targetTeamIndex)
                    {
                        return false;
                    }

                    if (targetTeamState.CanGo && !targetTeamState.CurrentlyBrokenDown)
                    {
                        bool canPlayCard = true;
                        if (targetTeamState.IsUnderSpeedLimit)
                        {
                            canPlayCard = card.Value <= 50;
                        }

                        if (!canPlayCard)
                        {
                            return false;
                        }

                        return (targetTeamState.DistanceTraveled + card.Value) <= 1000;
                    }
                    else
                    {
                        return false;
                    }
                }
                case CardType.EFFECT_POSITIVE:
                {
                    if (playerTeamIndex != targetTeamIndex)
                    {
                        return false;
                    }

                    switch (card.EffectType)
                    {
                        case EffectCardType.ACCIDENT:
                        {
                            return targetTeamState.HasAccident;
                        }
                        case EffectCardType.FUEL:
                        {
                            return targetTeamState.IsOutOfFuel;
                        }
                        case EffectCardType.TIRE:
                        {
                            return targetTeamState.HasFlatTire;
                        }
                        case EffectCardType.SPEED_LIMIT:
                        {
                            return targetTeamState.IsUnderSpeedLimit;
                        }
                        case EffectCardType.TRAFFIC_LIGHT:
                        {
                            return !targetTeamState.CanGo && !targetTeamState.CurrentlyBrokenDown;
                        }
                        case EffectCardType.NONE:
                        default:
                        {
                            return false;
                        }
                    }
                }
                case CardType.EFFECT_NEGATIVE:
                {
                    if (playerTeamIndex == targetTeamIndex)
                    {
                        return false;
                    }

                    switch (card.EffectType)
                    {
                        case EffectCardType.ACCIDENT:
                        {
                            return !targetTeamState.CurrentlyBrokenDown &&
                                !targetTeamState.InvincibleToAccidents;
                        }
                        case EffectCardType.FUEL:
                        {
                            return !targetTeamState.CurrentlyBrokenDown &&
                                !targetTeamState.InvincibleToFuel;
                        }
                        case EffectCardType.TIRE:
                        {
                            return !targetTeamState.CurrentlyBrokenDown &&
                                !targetTeamState.InvincibleToTire;
                        }
                        case EffectCardType.SPEED_LIMIT:
                        {
                            return !targetTeamState.IsUnderSpeedLimit &&
                                !targetTeamState.InvinciblePriority;
                        }
                        case EffectCardType.TRAFFIC_LIGHT:
                        {
                            return targetTeamState.CanGo &&
                                !targetTeamState.InvinciblePriority;
                        }
                        case EffectCardType.NONE:
                        default:
                        {
                            return false;
                        }
                    }
                }
                case CardType.EFFECT_INVINCIBLE:
                {
                    switch (card.EffectType)
                    {
                        case EffectCardType.ACCIDENT:
                        {
                            return !targetTeamState.InvincibleToAccidents;
                        }
                        case EffectCardType.FUEL:
                        {
                            return !targetTeamState.InvincibleToFuel;
                        }
                        case EffectCardType.TIRE:
                        {
                            return !targetTeamState.InvincibleToTire;
                        }
                        case EffectCardType.SPEED_LIMIT | EffectCardType.TRAFFIC_LIGHT:
                        {
                            return !targetTeamState.InvinciblePriority;
                        }
                        case EffectCardType.NONE:
                        default:
                        {
                            return false;
                        }
                    }
                }
                default:
                {
                    return false;
                }
            }
        }

        private int[] GetRemainingCards(Game game)
        {
            var deck = CardDefinitions.Deck.ToList();

            var playedCards = game.GameEvent
                .Where(ge => ge.Type == GameEventType.DRAW_CARD)
                .Cast<DrawCardEvent>();

            foreach (var card in playedCards)
            {
                deck.Remove(card.CardIndex);
            }

            return deck.ToArray();
        }

        private void DrawNewCardToPlayer(Game game, User user)
        {
            var remainingDeck = GetRemainingCards(game);

            if (remainingDeck.Length <= 0)
            {
                return;
            }

            Random random = new Random();
            int listIndex = random.Next(remainingDeck.Length);
            DrawCard(game, user, remainingDeck[listIndex]);
        }

        private void ChangePlayer(Game game, int newPlayerId)
        {
            game.GameEvent.Add(new PlayerChangeEvent()
            {
                ServerEvent = true,
                Date = DateTime.UtcNow,
                Type = GameEventType.PLAYER_CHANGE,

                NewPlayerId = newPlayerId
            });
        }

        private PlayerChangeEvent GetLastPlayerChangeEvent(Game game)
        {
            return game.GameEvent
                .Where(ge => ge.Type == GameEventType.PLAYER_CHANGE)
                .Cast<PlayerChangeEvent>()
                .OrderByDescending(ge => ge.GameEventId)
                .FirstOrDefault();
        }

        private int GetCurrentPlayerUserId(Game game)
        {
            var lastPlayerChangeEvent = GetLastPlayerChangeEvent(game);

            return lastPlayerChangeEvent.NewPlayerId;
        }

        private User GetCurrentPlayer(Game game)
        {
            var lastPlayerChangeEvent = GetLastPlayerChangeEvent(game);

            return lastPlayerChangeEvent.User;
        }

        private User GoToNextPlayer(Game game)
        {
            var playerHands = GetAllPlayersHands(game);
            if (playerHands.All(x => x.Item2.Count == 0))
            {
                throw new Exception("Impossible de passer au prochain joueur comme toutes les cartes ont été jouées.");
            }

            var players = GetPlayersInGame(game)
                .OrderBy(pg => pg.Order)
                .ToArray();

            var playersWithHands = players
                .Join(playerHands,
                    pg => pg.UserId,
                    x => x.Item1,
                    (pg, x) => Tuple.Create(pg, x.Item2)
                )
                .ToArray();

            var currentPlayerId = GetCurrentPlayerUserId(game);

            var newPlayerIndex = players
                .Select((pg, inx) => new { pg, inx })
                .Single(x => x.pg.UserId == currentPlayerId)
                .inx;
            newPlayerIndex = (newPlayerIndex + 1) % playersWithHands.Length;

            var newPlayer = playersWithHands[newPlayerIndex];

            // Trouver un joueur qui a des cartes à jouer, sinon le jeu va
            // attendre qu'il joue une carte dans une main vide.
            while (newPlayer.Item2.Count == 0)
            {
                newPlayerIndex = (newPlayerIndex + 1) % playersWithHands.Length;
                newPlayer = playersWithHands[newPlayerIndex];
            }

            ChangePlayer(game, newPlayer.Item1.UserId);
            return newPlayer.Item1.User;
        }

        private TeamState[] GetTeamsState(Game game)
        {
            var teams = game.PlayerGame
               .Select(pg => pg.Team)
               .Distinct()
               .ToArray();

            TeamState[] teamStates = new TeamState[teams.Length];

            int index = 0;
            foreach (var team in teams)
            {
                teamStates[index] = GetTeamState(game, team);
                index++;
            }

            return teamStates;
        }

        private TeamState GetTeamState(Game game, int teamIndex)
        {
            if (teamIndex == 0)
            {
                return null;
            }

            var teams = game.PlayerGame
                .Select(pg => pg.Team)
                .Distinct()
                .ToArray();

            if (!teams.Contains(teamIndex))
            {
                return null;
            }

            var cardsPlayedOnTeam = game.GameEvent
                .Where(ge => ge.Type == GameEventType.PLAY_CARD)
                .Cast<PlayCardEvent>()
                .Where(pc => pc.TargetTeamIndex == teamIndex)
                .ToList()
                .Join(
                    CardDefinitions.Cards,
                    pc => pc.DrawCardEvent.CardIndex,
                    cd => cd.CardId,
                    (pc, cd) => Tuple.Create(pc, cd)
                )
                .OrderByDescending(x => x.Item1.GameEventId);

            var teamState = new TeamState()
            {
                TeamIndex = teamIndex
            };

            teamState.HasAccident = IsTeamUnderEffect(
                cardsPlayedOnTeam,
                EffectCardType.ACCIDENT
            );

            teamState.IsOutOfFuel = IsTeamUnderEffect(
                cardsPlayedOnTeam,
                EffectCardType.FUEL
            );

            teamState.HasFlatTire = IsTeamUnderEffect(
                cardsPlayedOnTeam,
                EffectCardType.TIRE
            );

            teamState.IsUnderSpeedLimit = IsTeamUnderEffect(
                cardsPlayedOnTeam,
                EffectCardType.SPEED_LIMIT
            );

            ComputePlayedCardEffects(cardsPlayedOnTeam, teamState);
            bool accidentPlayed = teamState.PlayedCardEffects.HasFlag(EffectCardType.ACCIDENT);
            bool outOfFuelPlayed = teamState.PlayedCardEffects.HasFlag(EffectCardType.FUEL);
            bool flatTirePlayed = teamState.PlayedCardEffects.HasFlag(EffectCardType.TIRE);

            // Une équipe peut partir si elle a joué un feu vert.
            // Elle doit aussi, pour chaque effet dans [accident, essence, pneus],
            // avoir joué la carte qui la contre si une carte d'effet négatif
            // a été jouée contre l'équipe (les trois ifs).
            //
            // Le booléen détermine aussi si une carte feu vert peut être jouée,
            // les ifs évitent qu'un feu vert peut être joué lorsqu'il y a bris
            // non-réglé.
            //
            teamState.CanGo = IsTeamUnderEffect(
                cardsPlayedOnTeam,
                EffectCardType.TRAFFIC_LIGHT
            );

            if (accidentPlayed)
            {
                if (teamState.HasAccident)
                {
                    teamState.CanGo = false;
                }
            }
            if (outOfFuelPlayed)
            {
                if (teamState.IsOutOfFuel)
                {
                    teamState.CanGo = false;
                }
            }
            if (flatTirePlayed)
            {
                if (teamState.HasFlatTire)
                {
                    teamState.CanGo = false;
                }
            }

            // Une équipe ne peut partir immédiatement après un bris.
            if (teamState.CanGo)
            {
                var cardsPlayedOnTeamWithOrder = cardsPlayedOnTeam
                    .OrderBy(tu => tu.Item1.GameEventId)
                    .Select((tu, inx) => Tuple.Create(tu.Item1, tu.Item2, inx));

                var lastAccident = GetLastCardEffectPlayedForTeam(
                    cardsPlayedOnTeamWithOrder,
                    EffectCardType.ACCIDENT
                );
                var lastFuel = GetLastCardEffectPlayedForTeam(
                    cardsPlayedOnTeamWithOrder,
                    EffectCardType.FUEL
                );
                var lastTire = GetLastCardEffectPlayedForTeam(
                    cardsPlayedOnTeamWithOrder,
                    EffectCardType.TIRE
                );

                var lastLight = GetLastCardEffectPlayedForTeam(
                    cardsPlayedOnTeamWithOrder,
                    EffectCardType.TRAFFIC_LIGHT
                );

                if (lastAccident != null && lastAccident.Item3 > lastLight.Item3)
                {
                    teamState.CanGo = false;
                }
                if (lastFuel != null && lastFuel.Item3 > lastLight.Item3)
                {
                    teamState.CanGo = false;
                }
                if (lastTire != null && lastTire.Item3 > lastLight.Item3)
                {
                    teamState.CanGo = false;
                }
            }

            teamState.IsBrokenDown = teamState.CurrentlyBrokenDown;

            teamState.DistanceTraveled = cardsPlayedOnTeam
                .Where(x => x.Item2.CardType == CardType.VALUE)
                .Sum(x => x.Item2.Value);

            CalculateInvincibility(cardsPlayedOnTeam, teamState);

            return teamState;
        }

        private void ComputePlayedCardEffects(
            IOrderedEnumerable<Tuple<PlayCardEvent, Card>> cardsPlayedOnTeam,
            TeamState teamState)
        {
            teamState.PlayedCardEffects = EffectCardType.NONE;

            if (IsCardEffectPlayedForTeam(
                cardsPlayedOnTeam,
                EffectCardType.ACCIDENT))
            {
                teamState.PlayedCardEffects |= EffectCardType.ACCIDENT;
            }

            if (IsCardEffectPlayedForTeam(
                cardsPlayedOnTeam,
                EffectCardType.FUEL))
            {
                teamState.PlayedCardEffects |= EffectCardType.FUEL;
            }

            if (IsCardEffectPlayedForTeam(
                cardsPlayedOnTeam,
                EffectCardType.TIRE))
            {
                teamState.PlayedCardEffects |= EffectCardType.TIRE;
            }

            if (IsCardEffectPlayedForTeam(
                cardsPlayedOnTeam,
                EffectCardType.SPEED_LIMIT))
            {
                teamState.PlayedCardEffects |= EffectCardType.SPEED_LIMIT;
            }

            if (IsCardEffectPlayedForTeam(
                cardsPlayedOnTeam,
                EffectCardType.TRAFFIC_LIGHT))
            {
                teamState.PlayedCardEffects |= EffectCardType.TRAFFIC_LIGHT;
            }
        }

        private bool IsTeamUnderEffect(
            IOrderedEnumerable<Tuple<PlayCardEvent, Card>> cardsPlayedOnTeam,
            EffectCardType effect)
        {
            var lastEffectCard = cardsPlayedOnTeam
                .Where(x => x.Item2.EffectType.HasFlag(effect))
                .FirstOrDefault();

            if (lastEffectCard == null)
            {
                return false;
            }
            else
            {
                bool isUnderEffect;
                switch (lastEffectCard.Item2.CardType)
                {
                    case CardType.EFFECT_NEGATIVE:
                    {
                        isUnderEffect = true;

                        break;
                    }
                    case CardType.VALUE:
                    case CardType.EFFECT_POSITIVE:
                    case CardType.EFFECT_INVINCIBLE:
                    default:
                    {
                        isUnderEffect = false;

                        break;
                    }
                }

                if (effect == EffectCardType.TRAFFIC_LIGHT)
                {
                    return !isUnderEffect;
                }

                return isUnderEffect;
            }
        }

        private bool IsCardEffectPlayedForTeam(
            IOrderedEnumerable<Tuple<PlayCardEvent, Card>> cardsPlayedOnTeam,
            EffectCardType effect)
        {
            return cardsPlayedOnTeam
                .Any(x => x.Item2.EffectType.HasFlag(effect));
        }

        private Tuple<PlayCardEvent, Card, int> GetLastCardEffectPlayedForTeam(
            IEnumerable<Tuple<PlayCardEvent, Card, int>> cardsPlayedOnTeamWithOrder,
            EffectCardType effect)
        {
            return cardsPlayedOnTeamWithOrder
                .Where(x => !(x.Item2.CardType == CardType.EFFECT_INVINCIBLE &&
                    x.Item2.EffectType != (EffectCardType.TRAFFIC_LIGHT | EffectCardType.SPEED_LIMIT)))
                .Where(x => x.Item2.EffectType.HasFlag(effect))
                .LastOrDefault();
        }

        private bool IsInvincibleCardEffectPlayedForTeam(
            IOrderedEnumerable<Tuple<PlayCardEvent, Card>> cardsPlayedOnTeam,
            EffectCardType effect)
        {
            return cardsPlayedOnTeam
                .Any(x => x.Item2.EffectType.HasFlag(effect) &&
                    x.Item2.CardType == CardType.EFFECT_INVINCIBLE
                );
        }

        private void CalculateInvincibility(
            IOrderedEnumerable<Tuple<PlayCardEvent, Card>> cardsPlayedOnTeam,
            TeamState teamState)
        {
            teamState.InvincibleToAccidents = IsInvincibleCardEffectPlayedForTeam(
                cardsPlayedOnTeam,
                EffectCardType.ACCIDENT
            );

            teamState.InvincibleToFuel = IsInvincibleCardEffectPlayedForTeam(
                cardsPlayedOnTeam,
                EffectCardType.FUEL
            );

            teamState.InvincibleToTire = IsInvincibleCardEffectPlayedForTeam(
                cardsPlayedOnTeam,
                EffectCardType.TIRE
            );

            teamState.InvinciblePriority = IsInvincibleCardEffectPlayedForTeam(
                cardsPlayedOnTeam,
                EffectCardType.TRAFFIC_LIGHT | EffectCardType.SPEED_LIMIT
            );
        }
    }
}
