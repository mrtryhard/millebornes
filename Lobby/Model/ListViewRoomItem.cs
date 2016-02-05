using System.Windows.Controls;

namespace Lobby.Model
{
    /// <summary>
    /// Classe:         Model.ListViewRoomItem
    /// Date création:  2014-11-21
    /// Auteur:         Alexandre TryHard Leblanc
    /// Description:    Classe dérivée afin de conserver le Id de la salle.
    /// </summary>
    public class ListViewRoomItem : ListViewItem
    {
        /// <summary>
        /// Id de la salle
        /// </summary>
        public int RoomId { get; set; }

        /// <summary>
        /// Constructeur
        /// </summary>
        public ListViewRoomItem(int id)
        {
            RoomId = id;
        }
    }
}
