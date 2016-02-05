using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrairieService.Models
{
    /// <summary>
    /// Représente le type de la carte.
    /// </summary>
    public enum CardType
    {
        /// <summary>
        /// Une carte de valeur (en kilomètres).
        /// </summary>
        VALUE,

        /// <summary>
        /// Une carte d'effet positive.
        /// 
        /// Annule l'effet de la carte négative.
        /// </summary>
        EFFECT_POSITIVE,

        /// <summary>
        /// Une carte d'effet négative.
        /// 
        /// Permet de poser obstacle à un adversaire.
        /// </summary>
        EFFECT_NEGATIVE,

        /// <summary>
        /// Une carte atout/override.
        /// 
        /// Permet d'annuler l'effet de la carte négative et de jouer une seconde
        /// fois dans le même tour.
        /// </summary>
        EFFECT_INVINCIBLE
    }

}
