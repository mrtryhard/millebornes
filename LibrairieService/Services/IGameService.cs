using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;
using System.ServiceModel;

using LibrairieService.Models;

namespace LibrairieService.Services
{
    [ServiceContract]
    public interface IGameService
    {
        [OperationContract]
        Guid CreateGame(Guid player, Guid room);

        [OperationContract]
        Guid JoinGame(Guid player, Guid room);

        [OperationContract]
        bool DoHeartbeat(Guid playerToken, Guid gameToken);

        [OperationContract]
        GameState GetState(Guid playerToken, Guid gameToken);

        [OperationContract]
        PlayCardResult PlayCard(Guid gameToken, Guid playerToken, Guid cardToken, int targetTeamIndex);

        [OperationContract]
        bool SendGameMessage(Guid playerToken, Guid gameToken, string message);

        [OperationContract]
        UserMessage[] GetAllGameMessages(Guid gameToken);

        [OperationContract]
        UserMessage[] GetGameMessagesSinceDate(Guid gameToken, DateTime sinceDateUtc);

        [OperationContract]
        void TakePlayerDisconnectionDecision(Guid playerToken, Guid gameToken, bool continueGame);

        [OperationContract]
        Guid GetReturnToken(Guid room);
    }
}
