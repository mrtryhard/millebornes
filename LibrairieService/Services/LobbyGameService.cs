using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel;

using LibrairieService.Models;

namespace LibrairieService.Services
{
    // Ce service implémente seulement GetPlayerConfig et c'est voulu.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class LobbyGameService : ILobbyService
    {
        public Guid? CreateRoom(string name, Guid roomMaster)
        {
            throw new NotImplementedException();
        }

        public bool JoinRoom(Guid player, Guid room)
        {
            throw new NotImplementedException();
        }

        public bool JoinRoomGameEnd(Guid player, Guid gameEndToken)
        {
            throw new NotImplementedException();
        }

        public bool LeaveRoom(Guid player, Guid room)
        {
            throw new NotImplementedException();
        }

        public bool SetReady(Guid player, bool ready)
        {
            throw new NotImplementedException();
        }

        public bool SendRoomMessage(Guid player, Guid room, string message)
        {
            throw new NotImplementedException();
        }

        public bool SetConfig(Guid player, Guid room)
        {
            throw new NotImplementedException();
        }

        public bool SetName(Guid player, Guid room, string newName)
        {
            throw new NotImplementedException();
        }

        public bool SetMaster(Guid player, Guid room, string newMaster)
        {
            throw new NotImplementedException();
        }

        public List<Models.RoomInfo> GetOpenRoomsInfo()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, byte> GetLoggedUsers()
        {
            throw new NotImplementedException();
        }

        public List<string> GetRoomPlayers(Guid room)
        {
            throw new NotImplementedException();
        }

        public List<Models.UserMessage> GetRoomMessages(Guid room, int limit)
        {
            throw new NotImplementedException();
        }

        public string GetMasterName(Guid room)
        {
            throw new NotImplementedException();
        }

        public bool SetTeam(Guid player, Guid room, int team)
        {
            throw new NotImplementedException();
        }

        public int GetCurrentTeamIndex(Guid player)
        {
            throw new NotImplementedException();
        }

        public bool SetRoomName(Guid room, string newName)
        {
            throw new NotImplementedException();
        }

        public Guid? GetCurrentGameToken(Guid room)
        {
            throw new NotImplementedException();
        }

        public List<Models.UserMessage> GetNewMessageFrom(Guid player, string name)
        {
            throw new NotImplementedException();
        }

        public List<string> DoTheBigBastardThatWaveHisFlagHasNewMessageFromSomeoneIfSoTellMeLad(Guid player)
        {
            throw new NotImplementedException();
        }

        public bool SendPrivateMessage(Guid from, string to, string message)
        {
            throw new NotImplementedException();
        }

        public Models.PlayerConfigEntry[] GetPlayerConfig(Guid roomToken)
        {
            using (var context = new MilleBornesEntities())
            {
                var room = context.Room
                    .SingleOrDefault(rm => rm.Token == roomToken);
                if (room == null)
                {
                    throw new FaultException("La salle n'est pas trouvée.");
                }

                var playerConfig = room.PlayerRoomState
                    .Select((prs, inx) => new PlayerConfigEntry()
                    {
                        UserToken = prs.User.LoggedInUser.Token,
                        Order = prs.Order,
                        Team = prs.Team
                    })
                    .ToArray();

                return playerConfig;
            }
        }
    }
}
