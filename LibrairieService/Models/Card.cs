using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace LibrairieService.Models
{
    /// <summary>
    /// Représente les attributs d'une carte du jeu.
    /// </summary>
    public class Card
    {
        /// <summary>
        /// Le id de la carte.
        /// </summary>
        public int CardId { get; set; }

        /// <summary>
        /// Le nom de la carte.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Le type de la carte (valeur ou effet)
        /// </summary>
        public CardType CardType { get; set; }

        /// <summary>
        /// Le type de l'effet (accident, essence, ...)
        /// </summary>
        public EffectCardType EffectType { get; set; }
        
        /// <summary>
        /// La valeur de la carte pour celles de valeur.
        /// </summary>
        public int Value { get; set; }
    }
}
