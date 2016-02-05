using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace LibrairieService.Models
{
    /// <summary>
    /// Représente le type de l'effet pour les cartes ayant un effet.
    /// 
    /// L'énumération a l'attribut Flags parce que la carte Prioritaire a
    /// l'effet combiné d'une limite de vitesse et d'un feu.
    /// </summary>
    [Flags]
    [DataContract]
    public enum EffectCardType
    {
        /// <summary>
        /// Aucun type.
        /// </summary>
        [EnumMember]
        NONE = 0x0,

        /// <summary>
        /// Les effets reliées à l'accident.
        /// </summary>
        [EnumMember]
        ACCIDENT = 0x1,

        /// <summary>
        /// Les effets reliées à l'essence.
        /// </summary>
        [EnumMember]
        FUEL = 0x2,

        /// <summary>
        /// Les effets reliées à la roue.
        /// </summary>
        [EnumMember]
        TIRE = 0x4,

        /// <summary>
        /// Les effets reliées à la limite de vitesse.
        /// </summary>
        [EnumMember]
        SPEED_LIMIT = 0x8,

        /// <summary>
        /// Les effets reliées au feu (C'est-à-dire pouvoir rouler).
        /// </summary>
        [EnumMember]
        TRAFFIC_LIGHT = 0x10
    }
}
