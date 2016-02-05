using LibrairieService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LibrairieService.Services
{
    /// <summary>
    /// Classe:         Services.UserService
    /// Auteurs:        Michael Tran 
    ///                 Alexandre Leblanc
    /// Description:    Classe reliant les opérations lié directement / uniquement
    ///                 à l'utilisateur.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly int DELAY_MS_HEARTBEAT = 2000;

        /// <summary>
        /// Récupète les bytes d'une chaîne transformée en SHA1.
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static byte[] GetSHA1Hash(string inputString)
        {
            HashAlgorithm algorithm = SHA1.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        /// <summary>
        /// Récupère la chaîne transormée en SHA1.
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static string GetSHA1HashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetSHA1Hash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        /// <summary>
        /// Crée l'utilisateur en fonction des informations fournies.
        /// </summary>
        /// <param name="info"></param>
        /// <returns>True si créé avec succès. False autrement.</returns>
        public bool CreateUser(Models.CreateUserInfo info)
        {
            try
            {
                using (MilleBornesEntities context = new MilleBornesEntities())
                {
                    // Vérification basiques.
                    if (info.Email.Trim() == "" || !info.Email.Contains("@"))
                        return false;

                    if (info.Username.Trim().Length < 2 || info.Password.Length < 3)
                        return false;

                    User user = new User();
                    user.Name = info.Username.Trim();
                    user.PasswordHash = GetSHA1HashString(info.Password);
                    user.EmailAddress = info.Email.Trim();

                    // Si l'utilisateur est déjà présent, soit email soit name.
                    if (context.User.Where(p => p.Name == user.Name
                        || user.EmailAddress == p.EmailAddress).Count() > 0)
                    {
                        return false;
                    }


                    context.User.Add(user);
                    context.SaveChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Vérifie la connexion de l'utilisateur.
        /// </summary>
        /// <param name="username">Nom d'utilisateur</param>
        /// <param name="password">Mot de passe</param>
        /// <returns>Retourne le Guid de l'utilisateur.</returns>
        /// <remarks>Si cet appel retourne null, c'est que l'utilisateur n'a pas réussi à se logger.</remarks>
        public Guid? Login(string username, string password)
        {
            try
            {
                using (MilleBornesEntities context = new MilleBornesEntities())
                {
                    string hashedPassword = GetSHA1HashString(password);
                    var lstUsers = new List<User>();

                    lstUsers = new List<User>(context.User
                        .Where(p => p.Name == username.Trim()
                        && p.PasswordHash.ToLower() == hashedPassword.ToLower()));

                    if (lstUsers.Count() < 1)
                        return null;

                    User user = lstUsers.First();

                    LoggedInUser loggedInfo = new LoggedInUser();
                    loggedInfo.LoginDate = DateTime.UtcNow;
                    Guid guid = Guid.NewGuid();

                    loggedInfo.Token = guid;
                    loggedInfo.UserId = user.UserId;
                    DateTime hb = DateTime.UtcNow;
                    loggedInfo.LastHeartbeat = hb.AddMilliseconds(DELAY_MS_HEARTBEAT * 10);

                    LoggedInUser old = context.LoggedInUser.Find(loggedInfo.UserId);

                    if (old == null)
                    {
                        context.LoggedInUser.Add(loggedInfo);
                    }
                    else
                    {
                        old.Token = loggedInfo.Token;
                        old.LoginDate = DateTime.UtcNow;
                        old.LastHeartbeat = DateTime.UtcNow;
                    }

                    context.SaveChanges();
                    return guid;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Détermine si la connexion est toujours vivante.
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns>True si c'est le cas, false autrement.</returns>
        /// <remarks>Tolérance d'un skip de heartbeat. 
        /// Valeur par défaut 1 heartbeat == 2000ms</remarks>
        public bool Heartbeat(Guid userToken)
        {
            try
            {
                using (MilleBornesEntities context = new MilleBornesEntities())
                {
                    var lstLoggedUsers = context.LoggedInUser.Where(p => p.Token == userToken);
                    LoggedInUser liu = lstLoggedUsers.First();

                    // Différence de temps.
                    TimeSpan dt = DateTime.UtcNow - liu.LastHeartbeat;

                    liu.LastHeartbeat = DateTime.UtcNow;
                    context.SaveChanges();

                    return (dt.TotalMilliseconds <= DELAY_MS_HEARTBEAT * 10);
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Envoie un mess
        /// </summary>
        /// <param name="player"></param>
        /// <param name="room"></param>
        /// <param name="message"></param>
        /// <returns>True si envoyé avec succès.</returns>
        public bool SendGlobalMessage(Guid player, string message)
        {
            try
            {
                using (MilleBornesEntities context = new MilleBornesEntities())
                {
                    int userId = context.LoggedInUser.Where(p => p.Token == player)
                        .ToList().First().UserId;

                    Message msg = new Message();
                    msg.Content = message;
                    msg.Date = DateTime.UtcNow;
                    msg.UserId = userId;
                    msg.Type = MessageType.GLOBAL;

                    context.Message.Add(msg);
                    context.SaveChanges();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Récupère les messages globaux.
        /// </summary>
        /// <param name="limit"></param>
        /// <returns>Liste des messages.</returns>
        public List<Models.UserMessage> GetGlobalMessages(int limit)
        {
            List<Models.UserMessage> lstUMessages = new List<UserMessage>();

            try
            {
                using (MilleBornesEntities context = new MilleBornesEntities())
                {
                    var lstMessages = context.Message
                        .Where(p => p.Type == MessageType.GLOBAL)
                        .OrderByDescending(o => o.MessageId).ToList();

                    foreach (Message msg in lstMessages)
                    {
                        if (limit == 0)
                            break;

                        UserMessage uMsg = new UserMessage();
                        uMsg.Content = msg.Content;
                        uMsg.Date = msg.Date;
                        uMsg.Username = msg.User.Name;

                        lstUMessages.Add(uMsg);

                        limit--;
                    }
                }
            }
            catch
            {
            }

            return lstUMessages;
        }
    }
}
