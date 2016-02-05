using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using System.Collections.ObjectModel;
using System.Collections.Concurrent;

namespace Lobby.Views
{
    /// <summary>
    /// Logique d'interaction pour GameView.xaml
    /// </summary>
    public partial class GameView : Window
    {
        private const string MSG_PLAYER_DISCONNECTION_MASTER = "Un joueur s'est déconnecté, veuillez taper la commande \"/continuer\" pour continuer la partie sans lui, sinon \"/arreter\" pour arrêter la partie.";
        private const string MSG_PLAYER_DISCONNECTION_PLAYER = "Un joueur s'est déconnecté, veuillez attendre la décision du maître du jeu.";

        public Guid GameToken { get; set; }
        public bool IsGameMaster { get; set; }

        public GameView()
        {
            InitializeComponent();
        }

        private Guid? _currentCardToken;

        private DateTime _lastMessageTimestamp;

        private ObservableCollection<GameProxy.UserMessage> _messageList = new ObservableCollection<GameProxy.UserMessage>();
        private ConcurrentQueue<GameProxy.UserMessage> _pendingMessages = new ConcurrentQueue<GameProxy.UserMessage>();
        private System.Windows.Threading.DispatcherTimer _pendingMessagesTimer = new System.Windows.Threading.DispatcherTimer();

        private bool _connected = true;
        private bool _windowClosed = false;

        private GameProxy.GameState _gameState = null;
        private bool _lastDecisionValue = false;
        private bool _lastEndGameValue = false;

        /// <summary>
        /// Quand un clique est appuyé. 
        /// </summary>
        /// <remarks>On gère le double-clic manuellement.</remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imgCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Image img = sender as Image;
                
                Guid? token = img.DataContext as Guid?;
                if (token != null && token.HasValue)
                {
                    _currentCardToken = token.Value;
                }
            }
        }

        /// <summary>
        /// Lorsqu'on clique sur "Envoyer"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSendChat_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void SendMessage()
        {
            if (!_connected)
            {
                return;
            }

            if (string.IsNullOrEmpty(txtMessage.Text))
            {
                return;
            }

            using (var client = new GameProxy.GameServiceClient())
            {
                string trimmedMessage = txtMessage.Text.Trim();
                bool isContinueCommand = trimmedMessage == "/continuer";
                bool isStopCommand = trimmedMessage == "/arreter";
                if (isContinueCommand || isStopCommand)
                {
                    if (_lastDecisionValue)
                    {
                        try
                        {
                            client.TakePlayerDisconnectionDecision(
                                Model.UserSessionSingleton.Instance.UserToken.Value,
                                GameToken,
                                isContinueCommand
                            );

                            if (isContinueCommand)
                            {
                                SendSystemMessage("Décision prise, la partie va continuer.");
                            }
                            else
                            {
                                SendSystemMessage("Décision prise, la partie va s'arrêter.");
                            }

                            txtMessage.Text = "";
                        }
                        catch (System.ServiceModel.FaultException fe)
                        {
                            SendSystemMessage("Impossible de prendre la décision: " + fe);
                        }
                    }
                    else
                    {
                        SendSystemMessage("La commande n'est pas valide dans le cas courant.");
                    }
                }
                else
                {
                    bool success = client.SendGameMessage(
                        Model.UserSessionSingleton.Instance.UserToken.Value,
                        GameToken,
                        txtMessage.Text
                    );

                    if (!success)
                    {
                        MessageBox.Show(
                            "L'envoi du message n'a pas réussi. Veuillez réessayer.",
                            "Erreur d'envoi");
                    }
                    else
                    {
                        txtMessage.Text = "";
                    }
                }
            }
        }

        /// <summary>
        /// Interrompt la fermeture du jeu (demande au joueur) et envoie les données au serveur.
        /// </summary>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!_lastEndGameValue)
            {
                if (MessageBox.Show("Quitter la partie en cours?", "Quitter", MessageBoxButton.YesNoCancel) != MessageBoxResult.Yes)
                {
                    e.Cancel = true;
                }
            }

            if (!e.Cancel)
            {
                _windowClosed = true;
            }

            base.OnClosing(e);
        }

        TeamStateControl[] _opponentsTeamStateControls;
        Image[] _cardsInHandImages;
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            _opponentsTeamStateControls = new[] { tscOpponent1, tscOpponent2 };
            _cardsInHandImages = new[] { imgCard1, imgCard2, imgCard3, imgCard4, imgCard5 };
            tbDiscard.DataContext = 0;

            lstGameMessages.ItemsSource = _messageList;

            UpdateHeartbeat();
            UpdateState();
            LoadInitialMessages();
            UpdateLoop();

            _pendingMessagesTimer.Tick += pendingMessagesTimer_Tick;
            _pendingMessagesTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            _pendingMessagesTimer.Start(); 
        }

        void pendingMessagesTimer_Tick(object sender, EventArgs e)
        {
            GameProxy.UserMessage pendingMessage;
            while (_pendingMessages.TryDequeue(out pendingMessage))
            {
                _messageList.Add(pendingMessage);
            }

            if (_messageList.Count > 0 && pendingMessage != null)
            {
                lstGameMessages.ScrollIntoView(_messageList.Last());
            }
        }

        private void LoadInitialMessages()
        {
            _lastMessageTimestamp = DateTime.UtcNow;

            using (var client = new GameProxy.GameServiceClient())
            {
                var messages = client.GetAllGameMessages(GameToken);
                if (messages.Length > 0)
                {
                    foreach (var message in messages)
                    {
                        _messageList.Add(new GameProxy.UserMessage() 
                        {
                            Username = message.Username,
                            Date = message.Date.ToLocalTime(),
                            Content = message.Content
                        });
                    }

                    _lastMessageTimestamp = messages.Max(um => um.Date);
                    lstGameMessages.ScrollIntoView(_messageList.Last());
                }
            }
        }

        private void UpdateState()
        {
            using (var client = new GameProxy.GameServiceClient())
            {
                _gameState = client.GetState(
                    Model.UserSessionSingleton.Instance.UserToken.Value,
                    GameToken
                );
            }
        }

        private void UpdateMessages()
        {
            using (var client = new GameProxy.GameServiceClient())
            {
                DateTime lastMessageTimestamp;

                lastMessageTimestamp = _lastMessageTimestamp;

                var messages = client.GetGameMessagesSinceDate(GameToken, lastMessageTimestamp);
                if (messages.Length > 0)
                {
                    foreach (var message in messages)
                    {
                        _pendingMessages.Enqueue(new GameProxy.UserMessage()
                        {
                            Username = message.Username,
                            Date = message.Date.ToLocalTime(),
                            Content = message.Content
                        });
                    }

                    _lastMessageTimestamp = messages.Max(um => um.Date);
                }
            }
        }

        private void SetGameState(GameProxy.GameState gameState)
        {
            _gameState = gameState;

            tscOwnTeam.SetTeamState(gameState.OwnTeamState);
            tscOwnTeam.DataContext = gameState.OwnTeamState.TeamIndex;

            FillOpponentsTeamStates(gameState);
            FillCards(gameState);
            FillPlayers(gameState);
        }

        private void FillPlayers(GameProxy.GameState gameState)
        {
            dgPlayers.ItemsSource = gameState.Players.Select(gp => new Model.GamePlayerListItem() 
            {
                IsCurrent = gp.Order == gameState.CurrentPlayer,
                TeamIndex = gp.TeamIndex,
                Name = gp.Name,
                IsSelf = gp.Order == gameState.OwnPlayerOrder,
                IsConnected = gp.Order >= 0
            });
        }

        private const string PLACEHOLDER_CARD = "component/Images/cardback.png";
        private void FillCards(GameProxy.GameState gameState)
        {
            for (int i = 0; i < _cardsInHandImages.Length; i++)
            {
                var image = _cardsInHandImages[i];

                if (i < gameState.CardsInHand.Length)
                {
                    var card = gameState.CardsInHand[i];
                    if (_imageLookup.ContainsKey(card.CardId))
                    {
                        image.Source = GetImageFromRessources(
                            "component/Images/" + _imageLookup[card.CardId]
                        );
                        image.DataContext = card.Token;
                    }
                    else
                    {
                        image.Source = GetImageFromRessources(PLACEHOLDER_CARD);
                        image.DataContext = null;
                    }
                }
                else
                {
                    image.Source = GetImageFromRessources(PLACEHOLDER_CARD);
                    image.DataContext = null;
                }
            }
        }

        private void FillOpponentsTeamStates(GameProxy.GameState gameState)
        {
            for (int i = 0; i < _opponentsTeamStateControls.Length; i++)
            {
                var control = _opponentsTeamStateControls[i];

                if (i < gameState.OpponentsTeamStates.Length)
                {
                    var opponentState = gameState.OpponentsTeamStates[i];
                    control.SetTeamState(opponentState);
                    control.DataContext = opponentState.TeamIndex;
                }
                else
                {
                    control.ClearTeamState();
                    control.DataContext = null;
                }
            }
        }

        private ImageSource GetImageFromRessources(string path)
        {
            var uriSource = new Uri(@"/Lobby;" + path, UriKind.Relative);
            return new BitmapImage(uriSource);
        }

        private static Dictionary<int, string> _imageLookup = new Dictionary<int, string>()
        {
            {1, "dist25.png"},
            {2, "dist50.png"},
            {3, "dist75.png"},
            {4, "dist100.png"},
            {5, "dist200.png"},

            {6, "repair.png"},
            {7, "gas.png"},
            {8, "spare.png"},
            {9, "unlimited.png"},
            {10, "roll.png"},

            {11, "crash.png"},
            {12, "empty.png"},
            {13, "flat.png"},
            {14, "limit.png"},
            {15, "stop.png"},

            {16, "ace_safety.png"},
            {17, "tanker.png"},
            {18, "sealant.png"},
            {19, "emergency_priority.png"}
        };

        private void teamState_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_connected)
            {
                return;
            }

            if (_lastEndGameValue)
            {
                return;
            }

            if (_currentCardToken == null)
            {
                return;
            }

            int teamIndex;

            FrameworkElement element = sender as FrameworkElement;
            if (element == null)             
            {
                return;
            }

            int? elementData = element.DataContext as int?;
            if (elementData == null)
            {
                return;
            }

            teamIndex = elementData.Value;

            using (var client = new GameProxy.GameServiceClient())
            {
                var result = client.PlayCard(
                    GameToken,
                    Model.UserSessionSingleton.Instance.UserToken.Value,
                    _currentCardToken.Value,
                    teamIndex
                );

                if (result == GameProxy.PlayCardResult.SUCCESS)
                {
                    _currentCardToken = null;

                    SetGameState(client.GetState(
                        Model.UserSessionSingleton.Instance.UserToken.Value,
                        GameToken
                    ));
                }
                else
                {
                    string message;

                    switch (result)
                    {
                        case Lobby.GameProxy.PlayCardResult.CANNOT_PLAY:
                        {
                            message = "Vous ne pouvez pas jouer cette carte dans l'état actuel.";
                            break;
                        }
                        case Lobby.GameProxy.PlayCardResult.WRONG_TURN:
                        {
                            message = "Ce n'est pas votre tour à jouer.";
                            break;
                        }
                        case Lobby.GameProxy.PlayCardResult.NOT_ALL_PLAYERS_PRESENT:
                        {
                            message = "Les joueurs n'ont pas tous rejoint la partie.";
                            break;
                        }
                        case Lobby.GameProxy.PlayCardResult.SUCCESS:
                        case Lobby.GameProxy.PlayCardResult.WRONG_TOKEN:
                        case Lobby.GameProxy.PlayCardResult.WRONG_TOKEN_PLAYER:
                        case Lobby.GameProxy.PlayCardResult.ALREADY_PLAYED:
                        default:
                        {
                            message = "Une erreur non spécifiée s'est produite lors du jeu de la carte.";
                            break;
                        }
                    }

                    MessageBox.Show(message, "Erreur");
                }
            }
        }

        private async void UpdateLoop()
        {
            while (_connected && !_windowClosed)
            {
                await Task.Run(() =>
                {
                    UpdateHeartbeat();
                    UpdateState();
                    UpdateMessages();
                });

                SetGameState(_gameState);
                UpdateDisconnectionDecision();
                UpdateEndGame();

                await Task.Delay(500);
            }
        }

        private void UpdateHeartbeat()
        {
            using (var client = new GameProxy.GameServiceClient())
            {
                bool result = client.DoHeartbeat(
                    Model.UserSessionSingleton.Instance.UserToken.Value,
                    GameToken
                );

                if (!result)
                {
                    _connected = false;
                    MessageBox.Show(
                        "Vous avez été déconnecté de la partie.",
                        "Erreur",
                        MessageBoxButton.OK
                    );
                }
            }
        }

        private void UpdateDisconnectionDecision()
        {
            if (_lastDecisionValue == _gameState.WaitingForDecision)
            {
                return;
            }

            _lastDecisionValue = _gameState.WaitingForDecision;

            if (_gameState.WaitingForDecision)
            {
                SendSystemMessage(IsGameMaster ?
                    MSG_PLAYER_DISCONNECTION_MASTER :
                    MSG_PLAYER_DISCONNECTION_PLAYER
                );
            }
            else
            {
                SendSystemMessage(_gameState.GameEnd ?
                    "La décision du maître du jeu est de terminer la partie." :
                    "La décision du maître du jeu est de continuer la partie."
               );
            }
        }

        private void UpdateEndGame()
        {
            if (_lastEndGameValue == _gameState.GameEnd)
            {
                return;
            }

            _lastEndGameValue = _gameState.GameEnd;

            if (_gameState.GameEnd)
            {
                string message = "La partie est terminée pour la raison suivante: " + GameEndEnumToString(_gameState.GameEndReason ?? 0);
                SendSystemMessage(message);

                MessageBox.Show(
                   message,
                   "Fin de la partie",
                   MessageBoxButton.OK
               );
            }
        }

        private static string GameEndEnumToString(GameProxy.GameEndReason value)
        {
            switch (value)
            {
                case Lobby.GameProxy.GameEndReason.WON_THOUSAND_MILES:
                {
                    return "Une équipe à parcouru 1000 bornes.";
                }
                case Lobby.GameProxy.GameEndReason.EXHAUSTED_DECK:
                {
                    return "Toutes les cartes ont été jouées.";
                }
                case Lobby.GameProxy.GameEndReason.PLAYER_DISCONNECTION:
                {
                    return "Un joueur s'est déconnecté et la partie s'est arrêtée ou il ne reste pas assez de joueurs.";
                }
                default:
                {
                    return "La raison de fin est inconnue.";
                }
            }
        }

        private void SendSystemMessage(string content)
        {
            var message = new GameProxy.UserMessage()
            {
                Username = "Système",
                Date = DateTime.Now,
                Content = content
            };

            _pendingMessages.Enqueue(message);
        }

        private void txtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                SendMessage();
            }
        }
    }
}
