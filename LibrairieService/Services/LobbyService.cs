using LibrairieService.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibrairieService.Services
{
    public class LobbyService : ILobbyService
    {
        private readonly int DELAY_MS_HEARTBEAT = 2000;
        /// <summary>
        /// Crée la room avec le master associé.
        /// </summary>
        /// <param name="name">Nom de la salle</param>
        /// <param name="roomMaster">Guid du créateur.</param>
        /// <returns>Le guid de la room. </returns>
        public Guid? CreateRoom(string name, Guid roomMaster)
        {
            try
            {
                using (MilleBornesEntities context = new MilleBornesEntities())
                {
                    // Fetch le master id. 
                    int mId = context.LoggedInUser.First(p => p.Token == roomMaster).UserId;
                    // Vérifie si déjà master d'une room ou membre d'une room.
                    bool alreadyInRoom = context.Room
                        .Where(p => p.MasterUserId == mId)
                        .Count() > 0;

                    if (alreadyInRoom)
                        return null;

                    alreadyInRoom = context.PlayerRoomState
                        .Where(p => p.UserId == mId)
                        .Count() > 0;

                    if (alreadyInRoom)
                        return null;

                    Guid roomGuid = Guid.NewGuid();
                    Room r = new Room();
                    r.Name = name;
                    r.Started = false;
                    r.Token = roomGuid;
                    r.MasterUserId = mId;
                    r.GameInProgressId = null;

                    // Crée le playerroomstate du roommaster
                    PlayerRoomState prs = new PlayerRoomState();
                    prs.Room = r;
                    prs.UserId = mId;
                    prs.IsReady = false;
                    prs.Order = 0;
                    prs.Team = 0;
                    prs.LastHeartbeat = DateTime.UtcNow;

                    context.Room.Add(r);
                    context.PlayerRoomState.Add(prs);
                    context.SaveChanges();

                    return roomGuid;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Permet de joindre une room!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// </summary>
        /// <param name="player"></param>
        /// <param name="room"></param>
        /// <returns>True si succès, false autrement.</returns>
        public bool JoinRoom(Guid player, Guid room)
        {
            try
            {
                using (MilleBornesEntities context = new MilleBornesEntities())
                {
                    LoggedInUser liu = context.LoggedInUser.First(p => p.Token == player);

                    // On vérifie si le user est dans la room... 
                    // vérifie si déjà dans une room
                    bool alreadyInRoom = context.PlayerRoomState
                        .Where(p => p.UserId == liu.UserId)
                        .Count() > 0;

                    Room r = context.Room.First(p => p.Token == room);

                    // Si il est dans une autre room.
                    if (alreadyInRoom)
                    {
                        if (r.MasterUserId == liu.UserId || r.PlayerRoomState.First(p => p.UserId == liu.UserId).Room.Token == room)
                        {
                            SendRoomMessage(Guid.ParseExact("00000000-0000-0000-0000-000000000000", "D"), room, "**Le joueur " + liu.User.Name + " revenu dans la partie ☺**");
                            return true;
                        }

                        return false;
                    }

                    PlayerRoomState prsUser = new PlayerRoomState();
                    prsUser.UserId = liu.UserId;
                    prsUser.RoomId = r.RoomId;
                    prsUser.IsReady = false;
                    prsUser.Order = 0;
                    prsUser.Team = 0;
                    prsUser.LastHeartbeat = DateTime.UtcNow;

                    context.PlayerRoomState.Add(prsUser);
                    context.SaveChanges();
                    SendRoomMessage(Guid.ParseExact("00000000-0000-0000-0000-000000000000", "D"), room, "**Le joueur " + liu.User.Name + " a join la partie ☺**");
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool JoinRoomGameEnd(Guid player, Guid gameEndToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Quitte la salle présentement dedans.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="room"></param>
        /// <returns>True si succès, false autrement</returns>
        public bool LeaveRoom(Guid player, Guid room)
        {
            try
            {
                using (MilleBornesEntities tities = new MilleBornesEntities())
                {
                    LoggedInUser u = tities.LoggedInUser.First(p => p.Token == player);

                    Room r = tities.Room.First(p => p.Token == room);

                    // Si c'est le master, on DÉTRUIT LA ROOOOOOOOOOOOOOOOOOOOOOOOOM
                    // Et tout les player state.
                    // Et tous les messages.
                    if (r.MasterUserId == u.UserId)
                    {
                        var lstPrs = tities.PlayerRoomState.Where(p => p.RoomId == r.RoomId).ToList();
                        tities.Message.RemoveRange(r.Message);
                        tities.PlayerRoomState.RemoveRange(lstPrs);
                        tities.Room.Remove(r);
                    }
                    else
                    {
                        // Sinon on quitte la salle, simplement.
                        PlayerRoomState prsUser = tities.PlayerRoomState.First(p => p.UserId == u.UserId);
                        tities.PlayerRoomState.Remove(prsUser);
                    }

                    tities.SaveChanges();

                    SendRoomMessage(Guid.ParseExact("00000000-0000-0000-0000-000000000000", "D"), room, "**Le joueur " + u.User.Name + " a quitté la partie ☹**");
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Set un joueur "prêt".
        /// </summary>
        /// <param name="player"></param>
        /// <param name="room"></param>
        /// <param name="ready"></param>
        /// <returns>True si succès, false autrement</returns>
        public bool SetReady(Guid player, bool ready)
        {
            try
            {
                using (MilleBornesEntities tities = new MilleBornesEntities())
                {
                    LoggedInUser liu = tities.LoggedInUser.First(p => p.Token == player);
                    PlayerRoomState prsUser = tities.PlayerRoomState.First(p => p.UserId == liu.UserId);
                    prsUser.IsReady = ready;

                    tities.SaveChanges();

                    if (ready)
                        SendRoomMessage(Guid.ParseExact("00000000-0000-0000-0000-000000000000", "D"), prsUser.Room.Token, "**" + prsUser.User.Name + " est prêt! ☺**");
                    else
                        SendRoomMessage(Guid.ParseExact("00000000-0000-0000-0000-000000000000", "D"), prsUser.Room.Token, "**" + prsUser.User.Name + " n'est pas prêt! ☹**");
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Parse la commande demandée par l'utilisateur dans le message.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>True si succès, false autrement</returns>
        /// o /teams                    => affiche les équipes.
        /// o /status                   => affiche le status des gens (prêt, pas prêt, j'y vais! :^)
        /// o /validate                 => Valide la composition et les équipes et retourne ce qui ne va pas!
        /// o /kick [name]; [reason]    => Kick le joueur "name" avec la raison "reason".
        private bool GetCommandOuput(Guid player, Guid room, string input)
        {
            try
            {
                using (MilleBornesEntities context = new MilleBornesEntities())
                {
                    User caller = context.LoggedInUser.First(p => p.Token == player).User;

                    var lstPlayers = context.PlayerRoomState
                            .Where(p => p.Room.Token == room)
                            .OrderBy(o => o.Team);

                    string output = "";

                    if (input.StartsWith("/teams"))
                    {
                        output = "**" + caller.Name + " a demandé à afficher les équipes.**\r\n";

                        foreach (PlayerRoomState prs in lstPlayers)
                            output += "   - [" + prs.Team.ToString() + "] " + prs.User.Name + "\r\n";
                    }
                    else if (input.StartsWith("/status"))
                    {
                        output = "**" + caller.Name + " a demandé à afficher les status:**\r\n";

                        foreach (PlayerRoomState prs in lstPlayers)
                            output += "   - [" + (prs.IsReady ? "Prêt" : "Pas prêt") + "] " + prs.User.Name + "\r\n";
                    }
                    else if (input.StartsWith("/validate"))
                    {
                        // Les équipes sont-elles balancées?
                        // Les joueurs sont-ils prêts?
                        if (lstPlayers.Where(p => p.IsReady == false).Count() > 0)
                            output += "  - Certains joueurs ne sont pas prêts. (/status)\r\n";

                        byte t1 = 0;
                        byte t2 = 0;
                        byte t3 = 0;

                        foreach (var user in lstPlayers)
                        {
                            if (user.Team == 0)
                                t1++;
                            else if (user.Team == 1)
                                t2++;
                            else
                                t3++;
                        }

                        // On regarde si il y a moins de 2 équipes
                        if (t1 == 0 && (
                            (t2 == 0 || t3 == 0) ||
                            (t1 == 0 || t3 == 0)
                            ))
                        {
                            output += "  - Il y a moins de 2 équipes! (/teams)\r\n";
                        }

                        // Si 2 équipe, et la troisième vide.
                        if ((t1 != t2 && t3 == 0) ||
                            (t2 != t3 && t1 == 0) ||
                            (t3 != t1 && t2 == 0))
                        {
                            output += "  - Équipes non équilibrées! (/teams)\r\n";
                        }

                        // Si aucune erreur
                        if (output == "")
                        {
                            output += "**Validation (/validate): Tous les éléments sont prêt pour commencer une belle partie!**";
                        }
                        else
                        {
                            output = "**Validation (/validate) échouée.**\r\nRaison:\r\n" + output;
                        }
                    }
                    else if (input.StartsWith("/kick ") && input.Length > "/kick ".Length + 1 && input.Contains(";"))
                    {
                        // Si le caller est le master.
                        if (context.Room.First(p => p.Token == room).MasterUserId == caller.UserId)
                        {
                            string kName = input.Substring("/kick ".Length, input.IndexOf(';') - "/kick ".Length);
                            string reason = input.Substring(input.IndexOf(';') + 1);

                            Guid pId = context.LoggedInUser.First(p => p.User.Name == kName).Token;
                            LeaveRoom(pId, room);

                            output = "**Le joueur " + kName + " a été kické de la salle.**\r\nRaison: " + reason;
                        }
                        else
                        {
                            output = "**Avertissement. Seul le maître peut kicker un joueur.**";
                        }
                    }
                    else
                    {
                        return false;
                    }

                    if (output.Length == 0)
                        return true;

                    Room r = context.Room.First(p => p.Token == room);
                    Message msg = new Message();
                    msg.Content = output;
                    msg.Date = DateTime.UtcNow;
                    msg.Type = MessageType.ROOM; // 1== room.
                    msg.UserId = 5; // sys
                    msg.Room.Add(r);

                    context.Message.Add(msg);
                    context.SaveChanges();

                    return true;
                }
            }
            catch
            {
            }

            return false;
        }

        /// <summary>
        /// Envoie une message dans une salle. 
        /// </summary>
        /// <param name="player">Guid du joueur.</param>
        /// <param name="room">Guid de la salle.</param>
        /// <param name="message">Contenu du message.</param>
        /// <returns>True si succès, false autrement.</returns>
        public bool SendRoomMessage(Guid player, Guid room, string message)
        {
            // Si c'est une commande.
            if (message.StartsWith("/"))
            {
                bool ok = GetCommandOuput(player, room, message);
                return ok;
            }

            try
            {
                using (MilleBornesEntities context = new MilleBornesEntities())
                {
                    //Message msg = new Message();
                    //msg.Content = message;
                    //msg.Date = DateTime.UtcNow;
                    //msg.Type = MessageType.ROOM; // 1== room.

                    //Room r = context.Room.First(p => p.Token == room);
                    //if (player != Guid.ParseExact("00000000-0000-0000-0000-000000000000", "D"))
                    //{
                    //    int userId = context.LoggedInUser.First(p => p.Token == player).UserId;
                    //    msg.UserId = userId;
                    //}
                    //else
                    //{
                    //    msg.UserId = 5; // id du système...
                    //}

                    //msg.Room.Add(r);

                    //context.Message.Add(msg);
                    //context.SaveChanges();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SetConfig(Guid player, Guid room)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Change le nom de la salle.
        /// </summary>
        /// <remarks>Le joueur doit être master.</remarks>
        /// <param name="player">Guid du joueur qui requiert.</param>
        /// <param name="room">Guid de la salle.</param>
        /// <param name="newName">Nouveau nom de la salle.</param>
        /// <returns>True si réussi, false autrement.</returns>
        public bool SetName(Guid player, Guid room, string newName)
        {
            try
            {
                using (MilleBornesEntities context = new MilleBornesEntities())
                {
                    var lstRooms = context.Room.Where(p => p.Token == room);

                    if (lstRooms.Count() < 1)
                        return false;

                    var lstUsers = context.LoggedInUser.Where(p => p.Token == player);

                    if (lstUsers.Count() < 1)
                        return false;

                    if (lstUsers.First().UserId != lstRooms.First().MasterUserId)
                        return false;

                    Room r = context.Room.Find(lstRooms.First().RoomId);

                    r.Name = newName;
                    context.SaveChanges();

                    SendRoomMessage(Guid.ParseExact("00000000-0000-0000-0000-000000000000", "D"), room, "**Le nom de la salle est désormais " + r.Name + " **");
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Défini le master de la salle.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="room"></param>
        /// <param name="newMaster"></param>
        /// <returns></returns>
        public bool SetMaster(Guid player, Guid room, string newMaster)
        {
            try
            {
                using (MilleBornesEntities context = new MilleBornesEntities())
                {
                    // Trouve l'id du master.
                    int masterId = context.Room.First(p => p.Token == room).MasterUserId;

                    // Trouve l'id du joueur qui fait la requête.
                    int playerId = context.LoggedInUser.First(p => p.Token == player).UserId;

                    // Si le player n'est pas le master.
                    if (masterId != playerId)
                        return false;

                    // Ok. Le joueur est le master.
                    // Trouve le nouveau master id :
                    int newMasterId = context.User.First(p => p.Name == newMaster).UserId;

                    // Swap.
                    context.Room.First(p => p.Token == room).MasterUserId = newMasterId;

                    context.SaveChanges();

                    SendRoomMessage(Guid.ParseExact("00000000-0000-0000-0000-000000000000", "D"), room, "**Le maître de la salle est désormais " + newMaster + "**");
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Obtient la liste des salles actuellement ouverte.
        /// </summary>
        /// <returns>Liste des salles.</returns>
        public List<Models.RoomInfo> GetOpenRoomsInfo()
        {
            List<Models.RoomInfo> lstRoomsInfo = new List<Models.RoomInfo>();

            try
            {
                using (MilleBornesEntities context = new MilleBornesEntities())
                {
                    var lstRooms = context.Room.Where(p => p.Started == false);

                    foreach (var room in lstRooms)
                    {
                        RoomInfo ri = new RoomInfo();
                        ri.MasterName = context.User.Find(room.MasterUserId).Name;
                        ri.Name = room.Name;
                        ri.Token = room.Token;

                        lstRoomsInfo.Add(ri);
                    }
                }
            }
            catch
            {
            }

            return lstRoomsInfo;
        }

        /// <summary>
        /// Détermine si on est toujours en ligne.
        /// </summary>
        /// <remarks>Contrairement à celui dans UserService, celui là ne rafraichi pas le hearbeat.</remarks>
        /// <param name="userToken"></param>
        /// <returns></returns>
        private bool GetHeartBeat(Guid userToken)
        {
            try
            {
                using (MilleBornesEntities context = new MilleBornesEntities())
                {
                    LoggedInUser loggedUser = context.LoggedInUser.First(p => p.Token == userToken);

                    // Différence de temps.
                    TimeSpan dt = DateTime.UtcNow - loggedUser.LastHeartbeat;
                    return (dt.TotalMilliseconds <= DELAY_MS_HEARTBEAT * 10);
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Obtient les utilisateurs connectés.
        /// </summary>
        /// <returns>[UserName] => [Connected?]</returns>
        public Dictionary<string, byte> GetLoggedUsers()
        {
            Dictionary<string, byte> users = new Dictionary<string, byte>();

            try
            {
                using (MilleBornesEntities context = new MilleBornesEntities())
                {
                    IQueryable<LoggedInUser> lstLogged = context.LoggedInUser;
                    Dictionary<string, byte> lstUsers = new Dictionary<string, byte>();

                    foreach (LoggedInUser u in lstLogged)
                    {
                        KeyValuePair<string, byte> kp;

                        if (GetHeartBeat(u.Token))
                        {
                            if (u.User.PlayerGame.Count(p => (DateTime.UtcNow - p.LastHeartbeat).TotalMilliseconds < DELAY_MS_HEARTBEAT * 10) > 0)
                                kp = new KeyValuePair<string, byte>(u.User.Name, 2);
                            else if (u.User.PlayerRoomState.Count > 0)
                                kp = new KeyValuePair<string, byte>(u.User.Name, 1);
                            else
                                kp = new KeyValuePair<string, byte>(u.User.Name, 0);

                            lstUsers.Add(kp.Key, kp.Value);
                        }
                    }

                    foreach (var uu in lstUsers.OrderBy(p => p.Value))
                        users.Add(uu.Key, uu.Value);
                }
            }
            catch
            {
            }

            return users;
        }

        /// <summary>
        /// Obtient les messages de la salle sélectionnée.
        /// </summary>
        /// <param name="room"></param>
        /// <param name="limit"></param>
        /// <returns>Liste de message.</returns>
        public List<UserMessage> GetRoomMessages(Guid room, int limit)
        {
            List<Models.UserMessage> lstUMessages = new List<UserMessage>();

            try
            {
                using (MilleBornesEntities context = new MilleBornesEntities())
                {
                    //var lstMessages = context.Room.First(p => p.Token == room)
                    //    .Message;

                    //    //.OrderByDescending(o => o.MessageId)
                    //    //.ToList();
                    
                    //foreach (Message msg in lstMessages)
                    //{
                    //    if (limit == 0)
                    //        break;

                    //    UserMessage uMsg = new UserMessage();
                    //    uMsg.Content = msg.Content;
                    //    uMsg.Date = msg.Date;
                    //    uMsg.Username = msg.User.Name;

                    //    lstUMessages.Add(uMsg);

                    //    limit--;
                    //}
                }
            }
            catch
            {
            }

            return lstUMessages;
        }

        /// <summary>
        /// Obtient la liste des joueurs dans la salle spécifiée
        /// </summary>
        /// <param name="room"></param>
        /// <returns>Liste des joueurs</returns>
        public List<string> GetRoomPlayers(Guid room)
        {
            List<string> lstPlayers = new List<string>();

            try
            {
                using (MilleBornesEntities context = new MilleBornesEntities())
                {
                    int rId = context.Room.First(p => p.Token == room).RoomId;
                    var lstPrs = context.PlayerRoomState.Where(p => p.RoomId == rId);

                    foreach (PlayerRoomState prs in lstPrs)
                        lstPlayers.Add(prs.User.Name);
                }
            }
            catch
            {
            }

            return lstPlayers;
        }

        /// <summary>
        /// Obtient le nom du master muahahahahaha
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public string GetMasterName(Guid room)
        {
            try
            {
                using (MilleBornesEntities tities = new MilleBornesEntities())
                {
                    int mid = tities.Room.First(p => p.Token == room).MasterUserId;

                    return tities.User
                        .First(p => p.UserId == mid)
                        .Name;
                }
            }
            catch
            {
            }

            return "";
        }

        /// <summary>
        /// Défini l'équipe laquelle le joueur join.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="room"></param>
        /// <param name="team"></param>
        /// <returns></returns>
        public bool SetTeam(Guid player, Guid room, int team)
        {
            if (team > 2)
                return false;

            try
            {
                using (MilleBornesEntities context = new MilleBornesEntities())
                {
                    var user = context.LoggedInUser
                        .First(p => p.Token == player);

                    PlayerRoomState prs = context.PlayerRoomState.First(p => p.UserId == user.UserId);
                    prs.Team = team;

                    context.SaveChanges();

                    SendRoomMessage(Guid.ParseExact("00000000-0000-0000-0000-000000000000", "D"),
                        room,
                        "**Le joueur " + user.User.Name + " a rejoint l'équipe #" + (team + 1).ToString() + ". **");

                    return true;
                }
            }
            catch
            {
            }

            return false;
        }

        /// <summary>
        /// Obtient l'index de l'équipe courante (0-1-2)
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public int GetCurrentTeamIndex(Guid player)
        {
            try
            {
                using (MilleBornesEntities context = new MilleBornesEntities())
                {
                    int uId = context.LoggedInUser
                        .First(p => p.Token == player)
                        .UserId;

                    return context.PlayerRoomState.First(p => p.UserId == uId).Team;
                }
            }
            catch
            {
            }

            return -1;
        }

        /// <summary>
        /// Définit le nom de la salle.
        /// </summary>
        /// <param name="room"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public bool SetRoomName(Guid room, string newName)
        {
            try
            {
                using (MilleBornesEntities context = new MilleBornesEntities())
                {
                    context.Room.First(p => p.Token == room).Name = newName;
                    context.SaveChanges();

                    return true;
                }
            }
            catch
            {
            }

            return false;
        }

        /// <summary>
        /// Obtient le token de la game en cours.
        /// </summary>
        /// <param name="room">Room</param>
        /// <returns>null si aucune partie. Sinon le Guid :^)</returns>
        public Guid? GetCurrentGameToken(Guid room)
        {
            try
            {
                using (MilleBornesEntities context = new MilleBornesEntities())
                {
                    Room r = context.Room.First(p => p.Token == room);

                    if (!r.Started)
                        return null;

                    var lstPRS = r.PlayerRoomState;

                    foreach (PlayerRoomState prs in lstPRS)
                    {
                        DateTime hb = context.LoggedInUser.First(p => p.UserId == prs.UserId).LastHeartbeat;
                        hb.AddSeconds(30);
                    }

                    return r.Game.Token;
                }
            }
            catch
            {
            }

            return null;
        }

        /// <summary>
        /// Obtient les nouveaux messages de name
        /// </summary>
        /// <param name="name">Utilisateur</param>
        /// <returns>Liste des nouveaux messages.</returns>
        public List<UserMessage> GetNewMessageFrom(Guid player, string name)
        {
            List<UserMessage> lstMessages = new List<UserMessage>();
            
            try
            {
                using (MilleBornesEntities context = new MilleBornesEntities())
                {
                    // Récupère les ids...
                    LoggedInUser receiver = context.LoggedInUser.First(p => p.Token == player);
                    User sender = context.User.First(p => p.Name.Trim() == name.Trim());

                    IQueryable<PrivateMessage> lstNewMessages = context.PrivateMessage.Where(p => p.ReceiverUserId == receiver.UserId
                                                                && p.SenderUserId == sender.UserId
                                                                && p.Read == false);

                    foreach (PrivateMessage pm in lstNewMessages)
                    {
                        UserMessage um = new UserMessage();
                        um.Content = pm.Message;
                        um.Date = pm.SentTime;
                        um.Username = name;

                        lstMessages.Add(um);
                        pm.Read = true;
                    }

                    context.SaveChanges();
                }
            }
            catch
            {
            }

            return lstMessages;
        }

        /// <summary>
        /// Vérifie si il y a de nouveaux messages pour l'utilisateur.
        /// </summary>
        /// <param name="player"></param>
        /// <returns>Renvoie une liste de username de gens qui ont des messages.</returns>
        public List<string> DoTheBigBastardThatWaveHisFlagHasNewMessageFromSomeoneIfSoTellMeLad(Guid player)
        {
            List<string> lstUsers = new List<string>();
            
            try
            {
                using (MilleBornesEntities context = new MilleBornesEntities())
                {
                    LoggedInUser liu = context.LoggedInUser.First(p => p.Token == player);

                    var pms = context.PrivateMessage.Where(p => p.Receiver.UserId == liu.UserId
                                                 && p.Read == false);

                    HashSet<string> temp = new HashSet<string>();

                    foreach (PrivateMessage pm in pms)
                    {
                        temp.Add(pm.Sender.Name);
                    }

                    lstUsers = temp.ToList();
                }
            }
            catch
            {
            }

            return lstUsers;
        }

        /// <summary>
        /// Envoie un nouveau message.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="message"></param>
        /// <returns>True si succès.</returns>
        public bool SendPrivateMessage(Guid from, string to, string message)
        {
            bool ok = false;

            try
            {
                using (MilleBornesEntities context = new MilleBornesEntities())
                {
                    LoggedInUser user = context.LoggedInUser.First(p => p.Token == from);
                    int receiverId = context.User.First(p => p.Name.Trim() == to.Trim()).UserId;

                    PrivateMessage pm = new PrivateMessage();
                    pm.Message = message.Trim();
                    pm.Read = false;
                    pm.ReceiverUserId = receiverId;
                    pm.SenderUserId = user.UserId;
                    pm.SentTime = DateTime.UtcNow;

                    context.PrivateMessage.Add(pm);
                    context.SaveChanges();

                    ok = true;
                }
            }
            catch 
            {
            }

            return ok;
        }

        public PlayerConfigEntry[] GetPlayerConfig(Guid roomToken)
        {
            // L'implémentation est dans LobbyGameService.
            throw new NotImplementedException();
        }
    }
}
