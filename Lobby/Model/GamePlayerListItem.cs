using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lobby.Model
{
    public class GamePlayerListItem
    {
        public bool IsCurrent { get; set; }
        public int TeamIndex { get; set; }
        public string Name { get; set; }
        public bool IsSelf { get; set; }
        public bool IsConnected { get; set; }

        public string SelfLabel 
        {
            get
            {
                return string.Format("Vous ({0})", Name);
            }
        }
    }
}
