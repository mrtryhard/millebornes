using System;
using System.Runtime.Serialization;

namespace LibrairieService.Models
{
    [DataContract]
    public class RoomInfo
    {
        [DataMember]
        public Guid Token { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string MasterName { get; set; }
    }
}
