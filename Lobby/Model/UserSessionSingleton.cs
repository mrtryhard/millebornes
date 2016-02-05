using System;

namespace Lobby.Model
{
    /// <summary>
    /// Classe:         Model.UserSessionSingleton
    /// Auteur:         Alexandre "TryHard" Leblanc
    /// Description:    Session de l'utilisateur
    /// </summary>
    internal class UserSessionSingleton
    {
        private static UserSessionSingleton _instance;

        /// <summary>
        /// Obtient ou définie la token de l'utilisateur actuel.
        /// </summary>
        /// <remarks>Nulle signifie non connecté / déconnecté.</remarks>
        public Guid? UserToken
        {
            get;
            set;
        }

        /// <summary>
        /// Obtient ou définie la token de la partie actuelle.
        /// </summary>
        /// <remarks>Null représente aucune partie en cours.</remarks>
        public Guid? CurrentGameToken
        {
            get;
            set;
        }

        /// <summary>
        /// Obtient le nom d'affichage.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Récupère l'instance.
        /// </summary>
        public static UserSessionSingleton Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UserSessionSingleton();

                return _instance;
            }
        }

        /// <summary>
        /// Obtient le hearbeat.
        /// </summary>
        /// <returns>True si le heartbeat est valide.</returns>
        public bool GetHeartBeat()
        {
            if (UserToken == null)
                return false;

            using (UserService.UserServiceClient svcClient = new UserService.UserServiceClient())
            {
                return svcClient.Heartbeat(UserToken.Value);
            }
        }

        /// <summary>
        /// Constructeur privé.
        /// </summary>
        private UserSessionSingleton()
        {
            UserToken = null;
            CurrentGameToken = null;
            Name = "";
        }
    }
}
