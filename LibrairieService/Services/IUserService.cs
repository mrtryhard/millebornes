using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using LibrairieService.Models;

namespace LibrairieService.Services
{
    [ServiceContract]
    public interface IUserService
    {
        [OperationContract]
        bool CreateUser(CreateUserInfo info);

        [OperationContract]
        Guid? Login(string username, string password);

        [OperationContract]
        bool Heartbeat(Guid userToken);

        [OperationContract]
        bool SendGlobalMessage(Guid player, string message);

        [OperationContract]
        List<Models.UserMessage> GetGlobalMessages(int limit);
    }
}
