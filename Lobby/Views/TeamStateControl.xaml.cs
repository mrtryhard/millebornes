using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lobby.Views
{
    /// <summary>
    /// Interaction logic for TeamStateControl.xaml
    /// </summary>
    public partial class TeamStateControl : UserControl
    {
        private class EffectCardDefinition
        {
            public Func<GameProxy.TeamState, bool> StateGetter { get; set; }
            public Func<GameProxy.TeamState, bool> InvincibilityGetter { get; set; }
            public GameProxy.EffectCardType CardEffect { get; set; }
            public bool FlipState { get; set; }

            public string PositiveImage { get; set; }
            public string NegativeImage { get; set; }

            public Func<TeamStateControl, Image> TargetImage { get; set; }
        }

        private class InvincibilityCardDefinition
        {
            public Func<GameProxy.TeamState, bool> InvincibilityGetter { get; set; }
            public string Image { get; set; }
            public Func<TeamStateControl, Image> TargetImage { get; set; }
        }

        private static EffectCardDefinition[] _effectDefinitions = new[] 
        {
            new EffectCardDefinition()
            {
                StateGetter = ts => ts.CanGo,
                InvincibilityGetter = ts => ts.InvinciblePriority,
                CardEffect = GameProxy.EffectCardType.TRAFFIC_LIGHT,
                FlipState = true,

                PositiveImage = "roll.png",
                NegativeImage = "stop.png",

                TargetImage = tsc => tsc.imgLight
            },
            new EffectCardDefinition()
            {
                StateGetter = ts => ts.IsUnderSpeedLimit,
                InvincibilityGetter = ts => ts.InvinciblePriority,
                CardEffect = GameProxy.EffectCardType.SPEED_LIMIT,
                FlipState = false,

                PositiveImage = "unlimited.png",
                NegativeImage = "limit.png",

                TargetImage = tsc => tsc.imgSpeedLimit
            },
            new EffectCardDefinition()
            {
                StateGetter = ts => ts.HasAccident,
                InvincibilityGetter = ts => ts.InvincibleToAccidents,
                CardEffect = GameProxy.EffectCardType.ACCIDENT,
                FlipState = false,

                PositiveImage = "repair.png",
                NegativeImage = "crash.png",

                TargetImage = tsc => tsc.imgAccident
            },
            new EffectCardDefinition()
            {
                StateGetter = ts => ts.IsOutOfFuel,
                InvincibilityGetter = ts => ts.InvincibleToFuel,
                CardEffect = GameProxy.EffectCardType.FUEL,
                FlipState = false,

                PositiveImage = "gas.png",
                NegativeImage = "empty.png",

                TargetImage = tsc => tsc.imgFuel
            },
            new EffectCardDefinition()
            {
                StateGetter = ts => ts.HasFlatTire,
                InvincibilityGetter = ts => ts.InvincibleToTire,
                CardEffect = GameProxy.EffectCardType.TIRE,
                FlipState = false,

                PositiveImage = "spare.png",
                NegativeImage = "flat.png",

                TargetImage = tsc => tsc.imgTires
            }
        };

        private static InvincibilityCardDefinition[] _invicibilityDefinitions = new[] 
        {
            new InvincibilityCardDefinition()
            {
                InvincibilityGetter = ts => ts.InvinciblePriority,
                Image = "emergency_priority.png",
                TargetImage = tsc => tsc.imgPriority
            },
            new InvincibilityCardDefinition()
            {
                InvincibilityGetter = ts => ts.InvincibleToAccidents,
                Image = "ace_safety.png",
                TargetImage = tsc => tsc.imgDrivingAce
            },
            new InvincibilityCardDefinition()
            {
                InvincibilityGetter = ts => ts.InvincibleToFuel,
                Image = "tanker.png",
                TargetImage = tsc => tsc.imgExtraTank
            },
            new InvincibilityCardDefinition()
            {
                InvincibilityGetter = ts => ts.InvincibleToTire,
                Image = "sealant.png",
                TargetImage = tsc => tsc.imgPunctureProof
            }
        };

        Image[] _allImageBoxes;
        public TeamStateControl()
        {
            InitializeComponent();

            _allImageBoxes = new[]
            {
                imgLight,
                imgSpeedLimit,
                imgAccident,
                imgFuel,
                imgTires,
                imgPriority,
                imgDrivingAce,
                imgExtraTank,
                imgPunctureProof
            };
        }

        private const string PLACEHOLDER_CARD = "component/Images/cardback.png";
        public void SetTeamState(GameProxy.TeamState teamState)
        {
            foreach (var def in _effectDefinitions)
            {
                bool hasState = def.StateGetter(teamState);
                bool hasInvincibility = def.InvincibilityGetter(teamState);
                Image targetImage = def.TargetImage(this);

                if (def.FlipState)
                {
                    hasState = !hasState;
                }

                if (!hasState)
                {
                    if (!hasInvincibility && teamState.PlayedCardEffects.HasFlag(def.CardEffect))
                    {
                        targetImage.Source = GetImageFromRessources(
                            "component/Images/" + def.PositiveImage
                        );
                    }
                    else
                    {
                        targetImage.Source = GetImageFromRessources(PLACEHOLDER_CARD);
                    }
                }
                else
                {
                    if (teamState.PlayedCardEffects.HasFlag(def.CardEffect))
                    {
                        targetImage.Source = GetImageFromRessources(
                            "component/Images/" + def.NegativeImage
                        );
                    }
                    else
                    {
                        targetImage.Source = GetImageFromRessources(PLACEHOLDER_CARD);
                    }
                }
            }

            foreach (var def in _invicibilityDefinitions)
            {
                bool hasInvincibility = def.InvincibilityGetter(teamState);
                Image targetImage = def.TargetImage(this);

                if (hasInvincibility)
                {
                    targetImage.Source = GetImageFromRessources(
                       "component/Images/" + def.Image
                   );
                }
                else 
                {
                    targetImage.Source = GetImageFromRessources(PLACEHOLDER_CARD);
                }
            }

            tbDistance.Text = string.Format("{0} km", teamState.DistanceTraveled);
        }

        public void ClearTeamState()
        {
            foreach (var img in _allImageBoxes)
            {
                img.Source = GetImageFromRessources(PLACEHOLDER_CARD);
            }

            tbDistance.Text = "-- km";
        }

        private ImageSource GetImageFromRessources(string path)
        {
            var uriSource = new Uri(@"/Lobby;" + path, UriKind.Relative);
            return new BitmapImage(uriSource);
        }
    }
}
