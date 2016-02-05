using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace LibrairieService.Models
{
    /// <summary>
    /// Représente une carte donnée à un joueur.
    /// </summary>
    [DataContract]
    public class GameCard
    {
        /// <summary>
        /// Le id de la carte pour permettre au client d'afficher une
        /// représentation de celle-ci.
        /// </summary>
        [DataMember]
        public int CardId { get; set; }

        /// <summary>
        /// Le jeton qui permettera de jouer la carte.
        /// </summary>
        [DataMember]
        public Guid Token { get; set; }
    }
}
