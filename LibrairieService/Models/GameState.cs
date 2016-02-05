using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace LibrairieService.Models
{
    [DataContract]
    public class GameState
    {
        [DataMember]
        public bool GameEnd { get; set; }

        [DataMember]
        public GameEndReason? GameEndReason { get; set; }

        [DataMember]
        public bool WaitingForDecision { get; set; }

        [DataMember]
        public GameCard[] CardsInHand { get; set; }

        [DataMember]
        public TeamState OwnTeamState { get; set; }

        [DataMember]
        public TeamState[] OpponentsTeamStates { get; set; }

        [DataMember]
        public GamePlayer[] Players { get; set; }

        [DataMember]
        public int CurrentTeam { get; set; }

        [DataMember]
        public int CurrentPlayer { get; set; }

        [DataMember]
        public bool IsOwnTeamTurn { get; set; }

        [DataMember]
        public bool IsOwnTurn { get; set; }

        [DataMember]
        public int OwnTeamIndex { get; set; }

        [DataMember]
        public int OwnPlayerOrder { get; set; }
    }
}
