using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace LibrairieService.Models
{
    [DataContract]
    public class GamePlayer
    {
        [DataMember]
        public int Order { get; set; }

        [DataMember]
        public int TeamIndex { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}
