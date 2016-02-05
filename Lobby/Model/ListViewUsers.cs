using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lobby.Model
{
    public class ListViewUser
    {
        string _name;
        string _color;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public string Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
            }
        }

        public ListViewUser(string name, string color)
        {
            _name = name;
            _color = color;
        }
    }
}
