using System;
using System.Runtime.Serialization;

namespace LibrairieService.Models
{
    [DataContract]
    public class UserMessage
    {
        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string Content { get; set; }

        [DataMember]
        public DateTime Date { get; set; }
    }
}
