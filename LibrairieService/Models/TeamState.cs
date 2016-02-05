using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace LibrairieService.Models
{
    [DataContract]
    public class TeamState
    {
        [DataMember]
        public int TeamIndex { get; set; }

        [DataMember]
        public bool HasAccident { get; set; }

        [DataMember]
        public bool IsOutOfFuel { get; set; }

        [DataMember]
        public bool HasFlatTire { get; set; }

        [DataMember]
        public bool IsBrokenDown { get; set; }

        public bool CurrentlyBrokenDown
        {
            get
            {
                return HasAccident || IsOutOfFuel || HasFlatTire;
            }
        }

        [DataMember]
        public bool IsUnderSpeedLimit { get; set; }

        [DataMember]
        public bool CanGo { get; set; }

        [DataMember]
        public int DistanceTraveled { get; set; }

        [DataMember]
        public bool InvincibleToAccidents { get; set; }

        [DataMember]
        public bool InvincibleToFuel { get; set; }

        [DataMember]
        public bool InvincibleToTire { get; set; }

        [DataMember]
        public bool InvinciblePriority { get; set; }

        /// <summary>
        /// Indique les effets des cartes qui ont été jouées sur cette équipe.
        /// 
        /// Sert à distinguer le cas de l'effet pas joué de l'effet positif
        /// joué pour pouvoir afficher un placeholder côté client. La raison
        /// est que les booléens d'effet plus haut, quand ils sont à false,
        /// ne permettent pas de distinguer les deux cas.
        /// </summary>
        [DataMember]
        public EffectCardType PlayedCardEffects { get; set; }
    }
}
