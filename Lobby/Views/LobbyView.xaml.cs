using Lobby.GameProxy;
using Lobby.LobbyService;
using Lobby.Model;
using Lobby.UserService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Lobby.Views
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class LobbyView : Window
    {
        private bool _cmbMasterChanged = false;
        private bool _isQuitting = false;
        private bool _madeHimQuit = false;
        private bool _gameStart = false;
        private bool _joinedGame = false;
        private bool _changedRoom = false;
        private List<PrivateMessageView> _lstChats = new List<PrivateMessageView>();

        public LobbyView()
        {
            InitializeComponent();
            LobbyServiceClient c = new LobbyServiceClient();
            c.Open();

            tabCurrentRoom.Visibility = System.Windows.Visibility.Hidden;
            tabCurrentRoom.IsSelected = false;
        }

        public void StartUpdating()
        {
            Updating();
        }

        /// <summary>
        /// Met à jour les informations de lobby.
        /// </summary>
        /// <remarks>On vérifie aussi le heartbeat.</remarks>
        private async void Updating()
        {
            while (!_isQuitting && await IsStillConnected())
            {
                var start = DateTime.Now;

                if (GetGameGuid() != null)
                {
                    _gameStart = true;
                    Close();
                }

                if (tabRoomsList.IsSelected)
                {
                    lstRooms.ItemsSource = await LoadRooms();
                }
                else if (tabGeneralChat.IsSelected)
                {
                    lstMessages.ItemsSource = await LoadGlobalMessages();

                    if (lstMessages.Items.Count > 0)
                        lstMessages.ScrollIntoView(lstMessages.Items[lstMessages.Items.Count - 1]);
                }
                else if (tabCurrentRoom.IsSelected)
                {
                    lstRoomMessages.ItemsSource = await LoadCurrentRoomMessage(tabCurrentRoom.DataContext as Guid?);
                    LoadSelectedRoom((Guid)tabCurrentRoom.DataContext);

                    if (lstRoomMessages.Items.Count > 0)
                        lstRoomMessages.ScrollIntoView(lstRoomMessages.Items[lstRoomMessages.Items.Count - 1]);
                }

                lstPlayers.ItemsSource = await LoadUsers(tabCurrentRoom.IsSelected, tabCurrentRoom.DataContext as Guid?);

                AnyPrivateMessage();

                double diff = (DateTime.Now - start).TotalMilliseconds;

                if (diff < 2000 && diff > 0)
                    await Task.Delay((int)diff);
            }

            _madeHimQuit = true;

            if (!_isQuitting)
                Close();
        }

        /// <summary>
        /// Vérifie si il n'y a pas de nouveaux messages.
        /// </summary>
        private async void AnyPrivateMessage()
        {
            using (LobbyServiceClient client = new LobbyServiceClient())
            {
                List<string> lstSenders = await Task.Run(() => client.DoTheBigBastardThatWaveHisFlagHasNewMessageFromSomeoneIfSoTellMeLad(UserSessionSingleton.Instance.UserToken.Value));

                foreach (string name in lstSenders)
                {
                    if (_lstChats.Where(p => p.ChattingTo.Trim() == name.Trim()).Count() < 1)
                    {
                        PrivateMessageView pmv = new PrivateMessageView(this, name);
                        pmv.Show();
                        _lstChats.Add(pmv);
                        pmv.Focus();
                    }
                }
            }
        }

        /// <summary>
        /// Obtient le Guid de la partie.
        /// </summary>
        /// <returns></returns>
        private Guid? GetGameGuid()
        {
            if (tabCurrentRoom.DataContext == null)
                return null;

            using (LobbyServiceClient svcClient = new LobbyServiceClient())
            {
                Guid? guid = svcClient.GetCurrentGameToken((Guid)tabCurrentRoom.DataContext);
                UserSessionSingleton.Instance.CurrentGameToken = guid;
                return guid;
            }
        }

        /// <summary>
        /// Détermine si le user est encore connecté.
        /// </summary>
        private async Task<bool> IsStillConnected()
        {
            if (_gameStart)
                return true;

            // Vérifie si l'utilisateur est loggé.
            bool ok = await Task<bool>.Run(
                () => UserSessionSingleton.Instance.GetHeartBeat()
                );

            if (!ok)
            {
                try
                {
                    if (tabCurrentRoom.DataContext != null)
                    {
                        using (var svcClient = new LobbyServiceClient())
                        {
                            svcClient.LeaveRoom(UserSessionSingleton.Instance.UserToken.Value, (Guid)tabCurrentRoom.DataContext);
                        }
                    }
                }
                catch
                {
                }

                if (_madeHimQuit)
                {
                    tabGeneralChat.IsSelected = true;
                    UserService.UserMessage um = new UserService.UserMessage();
                    um.Content = "Vous avez été kické de la salle par le maître de jeu.";
                    um.Date = DateTime.Now;
                    um.Username = "Millebornes";
                    lstMessages.Items.Add(um);
                }
            }

            return ok;
        }

        /// <summary>
        /// Charge les users connectés.
        /// </summary>
        /// <returns>Dictionnaire avec les status.</returns>
        private async Task<List<ListViewUser>> LoadUsers(bool room, Guid? roomGuid = null)
        {
            List<ListViewUser> lstUsers = new List<ListViewUser>();

            using (var svcClient = new LobbyServiceClient())
            {
                if (!room)
                {
                    var users = await Task.Run(() => svcClient.GetLoggedUsers());

                    foreach (var kp in users)
                    {
                        if (kp.Value == 0)
                            lstUsers.Add(new ListViewUser(kp.Key, "Green"));
                        else if (kp.Value == 1)
                            lstUsers.Add(new ListViewUser(kp.Key, "Orange"));
                        else
                            lstUsers.Add(new ListViewUser(kp.Key, "Red"));
                    }
                }
                else
                {
                    if (roomGuid != null)
                    {
                        var users = await Task.Run(() => svcClient.GetRoomPlayers(roomGuid.Value));

                        foreach (string username in users)
                            lstUsers.Add(new ListViewUser(username, "Orange"));
                    }
                }
            }

            return lstUsers;
        }

        /// <summary>
        /// Charge les salles de jeu.
        /// </summary>
        /// <returns></returns>
        private async Task<List<RoomInfo>> LoadRooms()
        {
            List<RoomInfo> openedRooms = new List<RoomInfo>();

            using (var svcClient = new LobbyServiceClient())
            {
                openedRooms = await Task.Run(() => svcClient.GetOpenRoomsInfo());
            }

            return openedRooms;
        }

        /// <summary>
        /// Obtient les messages généraux.
        /// </summary>
        /// <returns></returns>
        private async Task<List<UserService.UserMessage>> LoadGlobalMessages()
        {
            List<UserService.UserMessage> lstMessagesColl = new List<UserService.UserMessage>();

            using (var svcClient = new UserServiceClient())
            {
                lstMessagesColl = await Task.Run(() => svcClient.GetGlobalMessages(20)
                        .Select(um => new UserService.UserMessage()
                        {
                            Content = um.Content,
                            Date = um.Date.ToLocalTime(),
                            Username = um.Username
                        })
                        .ToList());
            }

            lstMessagesColl.Reverse();

            return lstMessagesColl;
        }

        /// <summary>
        /// Obtient les messages de la room courante.
        /// </summary>
        /// <returns></returns>
        private async Task<List<LobbyService.UserMessage>> LoadCurrentRoomMessage(Guid? room)
        {
            List<LobbyService.UserMessage> lstRoomMessagesColl = new List<LobbyService.UserMessage>();

            if (room == null)
                return lstRoomMessagesColl;

            using (var svcClient = new LobbyServiceClient())
            {
                lstRoomMessagesColl = await Task.Run(() => svcClient.GetRoomMessages(room.Value, 20)
                        .Select(um => new LobbyService.UserMessage()
                        {
                            Content = um.Content,
                            Date = um.Date.ToLocalTime(),
                            Username = um.Username
                        })
                        .ToList());
            }

            lstRoomMessagesColl.Reverse();

            return lstRoomMessagesColl;
        }

        /// <summary>
        /// Quitte l'application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>Quitte la room actuelle si dans une room.</remarks>
        private void mnuQuit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Override le onClosing pour permettre de confirmer avec l'utilisateur si c'est bien ce qu'il veut.
        /// </summary>
        /// <remarks>Quitte également la room (important) Et aussi le loggin.</remarks>
        /// <param name="e"></param>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            // Cacher la fenêtre appele le OnClosing aussi, donc _joinedGame est
            // vrai quand on veut cacher la fenêtre.
            if (_joinedGame)
            {
                e.Cancel = true;
                return;
            }

            _isQuitting = true;

            if (_madeHimQuit && !_gameStart)
            {
                MessageBox.Show("Vous avez été déconnecté, veuillez vous reconnecter!", "Erreur", MessageBoxButton.OK);

                LoginView logView = new LoginView();
                logView.Show();
            }
            else if (_gameStart && !_joinedGame)
            {
                if (!_changedRoom)
                {
                    e.Cancel = true;
                    _isQuitting = false;
                    return;
                }

                _joinedGame = true;
                // Hehe
                MessageBox.Show(new Window(), "La partie commence");

                Guid? gameToken = null;
                using (var client = new GameProxy.GameServiceClient())
                {
                    try
                    {
                        gameToken = client.JoinGame(
                            UserSessionSingleton.Instance.UserToken.Value,
                            (tabCurrentRoom.DataContext as Guid?).Value
                        );
                    }
                    catch (System.ServiceModel.FaultException fe)
                    {
                        MessageBox.Show(
                            "Impossible de joindre la partie: " + fe.Message
                        );
                    }
                }

                if (gameToken != null)
                {
                    GameView gv = new GameView();
                    gv.Closed += ShowLobby;
                    gv.GameToken = gameToken.Value;
                    gv.IsGameMaster = false;
                    gv.Show();

                    e.Cancel = true;
                    _isQuitting = false;
                    _joinedGame = true;
                    _changedRoom = false;
                    Hide();
                }
            }
            else if (MessageBox.Show("Vous êtes sur le point de quitter MilleBornes en ligne. Continuer ?", "Quitter MilleBornes en ligne",
                                MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (tabCurrentRoom.DataContext != null)
                {
                    try
                    {
                        using (var svcClient = new LobbyServiceClient())
                        {
                            Guid guid = (Guid)tabCurrentRoom.DataContext;
                            svcClient.LeaveRoom(UserSessionSingleton.Instance.UserToken.Value, guid);
                        }
                    }
                    catch
                    {
                    }
                }

                Application.Current.Shutdown(0);
            }
            else
            {
                e.Cancel = true;
                _isQuitting = false;
            }
        }

        /// <summary>
        /// Envoie le message sur le serveur.
        /// </summary>
        /// <remarks>Va envoyer en fonction du tab ouvert</remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            if (tcLobbyTabs.SelectedItem != tabGeneralChat && tcLobbyTabs.SelectedItem != tabCurrentRoom)
            {
                MessageBox.Show("Sélectionner l'onglet où envoyer le message!");
                return;
            }

            txtMessage.IsEnabled = false;
            btnSend.IsEnabled = false;

            bool ok = false;

            // Dispatch le message à la bonne endroit.
            if (tcLobbyTabs.SelectedItem == tabGeneralChat)
            {
                using (var svcClient = new UserServiceClient())
                    ok = svcClient.SendGlobalMessage(UserSessionSingleton.Instance.UserToken.Value,
                        txtMessage.Text.Trim());
            }
            else if (tcLobbyTabs.SelectedItem == tabCurrentRoom)
            {
                using (var svcClient = new LobbyServiceClient())
                    ok = svcClient.SendRoomMessage(UserSessionSingleton.Instance.UserToken.Value,
                        (Guid)tabCurrentRoom.DataContext, txtMessage.Text.Trim());
            }

            if (!ok)
            {
                MessageBox.Show("L'envoie du message n'a pas réussi. Veuillez réessayer.",
                    "Erreur d'envoie");
            }
            else
                txtMessage.Text = "";

            txtMessage.IsEnabled = true;
            btnSend.IsEnabled = true;
        }

        /// <summary>
        /// Lorsqu'on appuie sur enter ça envoie le message (évènement click).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtMessage_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                btnSend_Click(sender, new RoutedEventArgs());
            }
        }

        /// <summary>
        /// Affiche les informations du client
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuInfos_Click(object sender, RoutedEventArgs e)
        {
            ClientInfosView civ = new ClientInfosView();
            civ.ShowDialog();
        }

        /// <summary>
        /// Lorsqu'on clique sur ce bouton, on quitte la salle courante.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreateRoom_Click(object sender, RoutedEventArgs e)
        {
            using (var svcClient = new LobbyServiceClient())
            {
                Guid? room = svcClient.CreateRoom("Nouvelle partie", UserSessionSingleton.Instance.UserToken.Value);

                if (room == null)
                {
                    MessageBox.Show("La création de la salle a échouée. Veuillez réessayer.", "Erreur");
                    return;
                }
                RoomInfo ri = new RoomInfo();
                ri.MasterName = UserSessionSingleton.Instance.Name;
                ri.Name = "Nouvelle partie";
                ri.Token = room.Value;

                lstRooms.SelectedItem = ri;
                // Si le guid est existant donc on peut ouvrir l'onglet.
                LoadSelectedRoom(room.Value);
                tabCurrentRoom.Visibility = System.Windows.Visibility.Visible;
                tabCurrentRoom.IsSelected = true;
            }

            btnQuitRoom.IsEnabled = true;
            btnCreateRoom.IsEnabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="room"></param>
        private void LoadSelectedRoom(Guid room)
        {
            tabCurrentRoom.DataContext = room;

            if (lstRooms.SelectedItem is RoomInfo || (tabCurrentRoom.DataContext != null && tabCurrentRoom.Visibility == System.Windows.Visibility.Visible))
            {
                using (var svcClient = new LobbyServiceClient())
                {                   
                    string masterName = svcClient.GetMasterName((Guid)tabCurrentRoom.DataContext);

                    if (masterName != UserSessionSingleton.Instance.Name)
                    {
                        btnStart.IsEnabled = false;
                        btnStart.Visibility = System.Windows.Visibility.Hidden;
                        btnRoomApply.Visibility = System.Windows.Visibility.Hidden;
                        btnRoomApply.IsEnabled = false;
                        cmbRoomMaster.IsEnabled = false;
                        txtRoomName.IsEnabled = false;
                    }

                    var lstPlayersMasters = svcClient.GetRoomPlayers((Guid)tabCurrentRoom.DataContext);
                    List<ListViewUser> lstLvus = new List<ListViewUser>();

                    foreach (string p in lstPlayersMasters)
                    {
                        ListViewUser lvu = new ListViewUser(p, "Green");
                        lstLvus.Add(lvu);
                    }

                    cmbRoomMaster.ItemsSource = lstLvus;

                    if (!_cmbMasterChanged)
                    {
                        txtRoomName.Text = ((RoomInfo)lstRooms.SelectedItem).Name;
                    }

                    cmbTeamChosen.SelectedIndex = svcClient.GetCurrentTeamIndex(UserSessionSingleton.Instance.UserToken.Value);
                }
            }
        }

        /// <summary>
        /// Lorsqu'on clique sur ce bouton, on quitte la salle courante.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuitRoom_Click(object sender, RoutedEventArgs e)
        {
            if (tabCurrentRoom.DataContext == null)
            {
                MessageBox.Show("Vous n'êtes dans aucune salle ;^)");
                return;
            }

            using (var svcClient = new LobbyServiceClient())
            {
                Guid guid;
                bool ok = false;

                try
                {
                    guid = (Guid)tabCurrentRoom.DataContext;
                    ok = svcClient.LeaveRoom(UserSessionSingleton.Instance.UserToken.Value, guid);
                }
                catch
                {
                    MessageBox.Show("Impossible de quitter la salle", "Erreur");
                    return;
                }

                if (!ok)
                {
                    MessageBox.Show("Impossible de quitter la salle");
                    return;
                }
            }

            tabCurrentRoom.DataContext = null;
            tabCurrentRoom.Visibility = System.Windows.Visibility.Hidden;
            tabRoomsList.IsSelected = true;
            btnCreateRoom.IsEnabled = true;
            btnQuitRoom.IsEnabled = false;
        }

        /// <summary>
        /// Lorsqu'on double clique sur une room.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstRooms_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (tabCurrentRoom.Visibility != System.Windows.Visibility.Hidden)
            {
                MessageBox.Show("Vous ne pouvez que joindre une seule partie à la fois.", "Erreur");
                return;
            }

            using (var svcClient = new LobbyServiceClient())
            {
                ListView lvRoomsColl = sender as ListView;
                RoomInfo item = (RoomInfo)lvRoomsColl.SelectedItem;
  
                bool ok = svcClient.JoinRoom(UserSessionSingleton.Instance.UserToken.Value, item.Token);

                if (!ok)
                {
                    MessageBox.Show("Impossible de joindre la partie sélectionnée.", "Erreur");
                }
                else
                {
                    LoadSelectedRoom(item.Token);
                    tabCurrentRoom.Visibility = System.Windows.Visibility.Visible;
                    tabCurrentRoom.IsSelected = true;
                    btnQuitRoom.IsEnabled = true;

                    _changedRoom = true;
                }
            }
        }

        /// <summary>
        /// HEHEHEHEHE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbRoomMaster_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _cmbMasterChanged = true;
        }

        /// <summary>
        /// Applique les changements.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRoomApply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var svcClient = new LobbyServiceClient())
                {
                    bool ok = false;

                    if (_cmbMasterChanged)
                    {
                        ok = svcClient.SetMaster(UserSessionSingleton.Instance.UserToken.Value,
                            (Guid)tabCurrentRoom.DataContext, ((ListViewItem)cmbRoomMaster.SelectedItem).Name);

                        if (txtRoomName.Text.Trim().Length > 0)
                        {
                           ok = svcClient.SetName(UserSessionSingleton.Instance.UserToken.Value,
                                (Guid)tabCurrentRoom.DataContext, txtRoomName.Text);
                        }

                        _cmbMasterChanged = false;
                    }

                    if (!ok && _cmbMasterChanged)
                    {
                        MessageBox.Show("Impossible d'appliquer les changements, veuillez réessayer.", "Erreur");
                    }
                }
            }
            catch
            {
                MessageBox.Show("Impossible d'appliquer les changements, veuillez réessayer.", "Erreur");
                return;
            }
        }

        /// <summary>
        /// Détermine que le joueur est prêt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReady_Click(object sender, RoutedEventArgs e)
        {
            bool ok = false;
            try
            {
                using (var svcClient = new LobbyServiceClient())
                {
                    ok = svcClient.SetReady(UserSessionSingleton.Instance.UserToken.Value, (string)btnReady.Content == "Prêt");
                }
            }
            catch
            {
            }

            if (!ok)
            {
                MessageBox.Show("Erreur du serveur, veuillez réessayer", "Erreur");
                return;
            }

            if ((string)btnReady.Content == "Prêt")
                btnReady.Content = "Prêt ✔";
            else
                btnReady.Content = "Prêt";
        }

        /// <summary>
        /// Débute la game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var svcClient = new GameServiceClient())
                {
                    GameView gView = new GameView();

                    UserSessionSingleton.Instance.CurrentGameToken =
                        svcClient.CreateGame(
                        UserSessionSingleton.Instance.UserToken.Value,
                        (Guid)tabCurrentRoom.DataContext);

                    gView.Closed += ShowLobby;
                    gView.GameToken = UserSessionSingleton.Instance.CurrentGameToken.Value;
                    gView.IsGameMaster = true;
                    gView.Show();

                    _changedRoom = false;
                    _joinedGame = true;
                    Hide();
                }
            }
            catch
            {
                MessageBox.Show("Un problème s'est produit pour débuter la partie :^(");
            }
        }

        /// <summary>
        /// Change la sélection de l'équipe pour le joueur actuel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbTeamChosen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabCurrentRoom.IsSelected && !_isQuitting)
            {
                using (var svcClient = new LobbyServiceClient())
                {
                    bool ok = svcClient.SetTeam(
                        UserSessionSingleton.Instance.UserToken.Value,
                        (Guid)tabCurrentRoom.DataContext,
                        cmbTeamChosen.SelectedIndex);

                    if (!ok)
                    {
                        MessageBox.Show("Le serveur a été incapable de vous changer d'équipe (vraiment), veuillez réessayer!");
                    }
                }
            }
        }

        /// <summary>
        /// Lorsque le nom de la room change, on arrête de l'updater, on laisse l'gars écrire.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtRoomName_TextChanged(object sender, TextChangedEventArgs e)
        {
            _cmbMasterChanged = true;
        }

        /// <summary>
        /// Lorsqu'on double clique sur un joueur, ouvre une fenêtre de chat.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewItemUsers_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string name = ((ListViewUser)((ListViewItem)sender).Content).Name.Trim();

            // On ne peut pas se parler à soit-même.
            if (name == UserSessionSingleton.Instance.Name.Trim())
                return;

            if (_lstChats.Where(p => p.ChattingTo.Trim() == name).Count() < 1)
            {
                PrivateMessageView pmv = new PrivateMessageView(this, name);
                pmv.Show();
                _lstChats.Add(pmv);
            }
            else
            {
                _lstChats.Find(p => p.ChattingTo.Trim() == name).Focus();
            }
        }

        /// <summary>
        /// Retire une vue de la liste.
        /// </summary>
        /// <param name="pmv"></param>
        public void RemovePrivateMessageWindow(PrivateMessageView pmv)
        {
            _lstChats.Remove(pmv);
        }

        private void ShowLobby(object sender, EventArgs e)
        {
            _joinedGame = false;
            _gameStart = false;
            Show();
        }
    }
}
