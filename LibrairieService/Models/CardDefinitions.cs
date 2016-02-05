using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrairieService.Models
{
    /// <summary>
    /// La classe regroupe les définitions des cartes et la définition du
    /// paquet de cartes.
    /// </summary>
    public class CardDefinitions
    {
        /// <summary>
        /// Obtient la définition des cartes du jeu.
        /// </summary>
        public static Card[] Cards
        {
            get
            {
                return _cardDefinitions;
            }
        }

        /// <summary>
        /// Obtient la distribution des cartes dans le paquet de cartes.
        /// </summary>
        public static int[] Deck
        {
            get
            {
                return _cardDeck;
            }
        }

        #region Définition des cartes
        // Le tableau des définitons des cartes.
        private static Card[] _cardDefinitions = new[]
        {
            // 5 cartes de valeur ou de distance.

            new Card()
            {
                CardId = 1,
                Name = "25km",
                CardType = CardType.VALUE,
                EffectType = EffectCardType.NONE,
                Value = 25
            },
            new Card()
            {
                CardId = 2,
                Name = "50km",
                CardType = CardType.VALUE,
                EffectType = EffectCardType.NONE,
                Value = 50
            },
            new Card()
            {
                CardId = 3,
                Name = "75km",
                CardType = CardType.VALUE,
                EffectType = EffectCardType.NONE,
                Value = 75
            },
            new Card()
            {
                CardId = 4,
                Name = "100km",
                CardType = CardType.VALUE,
                EffectType = EffectCardType.NONE,
                Value = 100
            },
            new Card()
            {
                CardId = 5,
                Name = "200km",
                CardType = CardType.VALUE,
                EffectType = EffectCardType.NONE,
                Value = 200
            },

            // 5 cartes d'effet positif.

            new Card()
            {
                CardId = 6,
                Name = "Repairs",
                CardType = CardType.EFFECT_POSITIVE,
                EffectType = EffectCardType.ACCIDENT,
                Value = 0
            },
            new Card()
            {
                CardId = 7,
                Name = "Fuel",
                CardType = CardType.EFFECT_POSITIVE,
                EffectType = EffectCardType.FUEL,
                Value = 0
            },
            new Card()
            {
                CardId = 8,
                Name = "SpareTire",
                CardType = CardType.EFFECT_POSITIVE,
                EffectType = EffectCardType.TIRE,
                Value = 0
            },
            new Card()
            {
                CardId = 9,
                Name = "LimitEnd",
                CardType = CardType.EFFECT_POSITIVE,
                EffectType = EffectCardType.SPEED_LIMIT,
                Value = 0
            },
            new Card()
            {
                CardId = 10,
                Name = "Roll",
                CardType = CardType.EFFECT_POSITIVE,
                EffectType = EffectCardType.TRAFFIC_LIGHT,
                Value = 0
            },

            // 5 cartes d'effet négatif ou d'attaque.

            new Card()
            {
                CardId = 11,
                Name = "Accident",
                CardType = CardType.EFFECT_NEGATIVE,
                EffectType = EffectCardType.ACCIDENT,
                Value = 0
            },
            new Card()
            {
                CardId = 12,
                Name = "OutOfFuel",
                CardType = CardType.EFFECT_NEGATIVE,
                EffectType = EffectCardType.FUEL,
                Value = 0
            },
            new Card()
            {
                CardId = 13,
                Name = "FlatTire",
                CardType = CardType.EFFECT_NEGATIVE,
                EffectType = EffectCardType.TIRE,
                Value = 0
            },
            new Card()
            {
                CardId = 14,
                Name = "SpeedLimit",
                CardType = CardType.EFFECT_NEGATIVE,
                EffectType = EffectCardType.SPEED_LIMIT,
                Value = 0
            },
            new Card()
            {
                CardId = 15,
                Name = "Stop",
                CardType = CardType.EFFECT_NEGATIVE,
                EffectType = EffectCardType.TRAFFIC_LIGHT,
                Value = 0
            },

            // 4 cartes d'atout ou d'invincibilité.

            new Card()
            {
                CardId = 16,
                Name = "DrivingAce",
                CardType = CardType.EFFECT_INVINCIBLE,
                EffectType = EffectCardType.ACCIDENT,
                Value = 0
            },
            new Card()
            {
                CardId = 17,
                Name = "ExtraTank",
                CardType = CardType.EFFECT_INVINCIBLE,
                EffectType = EffectCardType.FUEL,
                Value = 0
            },
            new Card()
            {
                CardId = 18,
                Name = "PunctureProof",
                CardType = CardType.EFFECT_INVINCIBLE,
                EffectType = EffectCardType.TIRE,
                Value = 0
            },
            new Card()
            {
                CardId = 19,
                Name = "RightOfWay",
                CardType = CardType.EFFECT_INVINCIBLE,
                EffectType = EffectCardType.SPEED_LIMIT | EffectCardType.TRAFFIC_LIGHT,
                Value = 0
            }
        };
        #endregion

        #region Définition du paquet
        // Le tableau de la définition du paquet de cartes.
        private static int[] _cardDeck;

        /// <summary>
        /// Insère une valeur dans une collection un nombre de fois donné.
        /// </summary>
        /// <typeparam name="T">Le type de la valeur de la collection et de la valeur à répéter.</typeparam>
        /// <param name="list">La collection dans laquelle placer les répétitions.</param>
        /// <param name="value">La valeur à répéter.</param>
        /// <param name="count">Le nombre de fois à effectuer.</param>
        private static void InsertRepeat<T>(ICollection<T> list, T value, int count)
        {
            for (int i = 0; i < count; i++)
            {
                list.Add(value);
            }
        }

        /// <summary>
        /// Constructeur statique pour initialiser le tableau du paquet de cartes.
        /// </summary>
        static CardDefinitions()
        {
            List<int> cardDeck = new List<int>();

            // 10 cartes de 25 km.
            InsertRepeat(cardDeck, 1, 10);

            // 10 cartes de 50 km.
            InsertRepeat(cardDeck, 2, 10);

            // 10 cartes de 75 km.
            InsertRepeat(cardDeck, 3, 10);

            // 12 cartes de 100 km.
            InsertRepeat(cardDeck, 4, 12);

            // 4 cartes de 200 km.
            InsertRepeat(cardDeck, 5, 4);

            // 6 cartes de réparation (effet positif sur l'accident).
            InsertRepeat(cardDeck, 6, 6);

            // 6 cartes d'essence (effet positif sur l'essence).
            InsertRepeat(cardDeck, 7, 6);

            // 6 cartes de roue de secours (effet positif sur la roue).
            InsertRepeat(cardDeck, 8, 6);

            // 6 cartes de fin de limite de vitesse (effet positif sur la limite).
            InsertRepeat(cardDeck, 9, 6);

            // 14 cartes de feu vert (effet positif sur le feu).
            InsertRepeat(cardDeck, 10, 14);

            // 3 cartes de d'accident (effet négatif sur l'accident).
            InsertRepeat(cardDeck, 11, 3);

            // 3 cartes de panne d'essence (effet négatif sur l'essence).
            InsertRepeat(cardDeck, 12, 3);

            // 3 cartes de crevaison (effet négatif sur la roue).
            InsertRepeat(cardDeck, 13, 3);

            // 4 cartes de limite de vitesse (effet négatif sur la limite).
            InsertRepeat(cardDeck, 14, 4);

            // 5 cartes de feu rouge (effet négatif sur le feu).
            InsertRepeat(cardDeck, 15, 14);

            // 1 carte d'as du volant (atout sur l'accident).
            InsertRepeat(cardDeck, 16, 1);

            // 1 carte de camion-citerne (atout sur l'essence).
            InsertRepeat(cardDeck, 17, 1);

            // 1 carte d'increvable (atout sur la roue).
            InsertRepeat(cardDeck, 18, 1);

            // 1 carte de prioritaire (atout sur la limite et le feu).
            InsertRepeat(cardDeck, 19, 1);

            _cardDeck = cardDeck.ToArray();
        }
        #endregion
    }
}
