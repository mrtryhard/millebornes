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
    
    public partial class PlayerChangeEvent : GameEvent
    {
        public int NewPlayerId { get; set; }
    
        public virtual User User { get; set; }
    }
}
