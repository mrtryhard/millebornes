//------------------------------------------------------------------------------
// <auto-generated>
//    Ce code a été généré à partir d'un modèle.
//
//    Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//    Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LibrairieService.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PrivateMessage
    {
        public int PrivateMessageId { get; set; }
        public int SenderUserId { get; set; }
        public int ReceiverUserId { get; set; }
        public string Message { get; set; }
        public bool Read { get; set; }
        public System.DateTime SentTime { get; set; }
    
        public virtual User Receiver { get; set; }
        public virtual User Sender { get; set; }
    }
}
