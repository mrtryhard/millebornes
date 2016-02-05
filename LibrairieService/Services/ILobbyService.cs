using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;
using System.ServiceModel;

namespace LibrairieService.Services
{
    [ServiceContract]
    public interface ILobbyService
    {
        [OperationContract]
        Guid? CreateRoom(string name, Guid roomMaster);

        [OperationContract]
        bool JoinRoom(Guid player, Guid room);

        [OperationContract]
        bool JoinRoomGameEnd(Guid player, Guid gameEndToken);

        [OperationContract]
        bool LeaveRoom(Guid player, Guid room);

        [OperationContract]
        bool SetReady(Guid player, bool ready);

        [OperationContract]
        bool SendRoomMessage(Guid player, Guid room, string message);

        [OperationContract]
        bool SetConfig(Guid player, Guid room);

        [OperationContract]
        bool SetName(Guid player, Guid room, string newName);

        [OperationContract]
        bool SetMaster(Guid player, Guid room, string newMaster);

        [OperationContract]
        List<Models.RoomInfo> GetOpenRoomsInfo();

        [OperationContract]
        Dictionary<string, byte> GetLoggedUsers();

        [OperationContract]
        List<string> GetRoomPlayers(Guid room);

        [OperationContract]
        List<Models.UserMessage> GetRoomMessages(Guid room, int limit);

        [OperationContract]
        string GetMasterName(Guid room);

        [OperationContract]
        bool SetTeam(Guid player, Guid room, int team);

        [OperationContract]
        int GetCurrentTeamIndex(Guid player);

        [OperationContract]
        bool SetRoomName(Guid room, string newName);

        [OperationContract]
        Guid? GetCurrentGameToken(Guid room);

        // Messagerie privée
        [OperationContract]
        List<Models.UserMessage> GetNewMessageFrom(Guid player, string name);

        [OperationContract]
        List<string> DoTheBigBastardThatWaveHisFlagHasNewMessageFromSomeoneIfSoTellMeLad(Guid player);

        [OperationContract]
        bool SendPrivateMessage(Guid from, string to, string message);

        [OperationContract]
        Models.PlayerConfigEntry[] GetPlayerConfig(Guid roomToken);
    }
}
