using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace LibrairieService.Models
{
    [DataContract]
    public class PlayerConfigEntry
    {
        [DataMember]
        public Guid UserToken { get; set; }

        [DataMember]
        public int Order { get; set; }

        [DataMember]
        public int Team { get; set; }
    }
}
