using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace LibrairieService.Models
{
    [DataContract]
    public enum PlayCardResult
    {
        [EnumMember]
        SUCCESS,

        [EnumMember]
        CANNOT_PLAY,

        [EnumMember]
        WRONG_TOKEN,

        [EnumMember]
        WRONG_TOKEN_PLAYER,

        [EnumMember]
        ALREADY_PLAYED,

        [EnumMember]
        WRONG_TURN,

        [EnumMember]
        NOT_ALL_PLAYERS_PRESENT,

        [EnumMember]
        WAITING_FOR_DECISION
    }
}
