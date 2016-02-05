using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace LibrairieService.Models
{
    [DataContract]
    public enum GameEndReason : short
    {
        [EnumMember]
        WON_THOUSAND_MILES = 1,

        [EnumMember]
        EXHAUSTED_DECK = 2,

        [EnumMember]
        PLAYER_DISCONNECTION = 3
    }
}
