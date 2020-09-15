using System.Windows.Forms;

namespace Classroom_Client.UI
{
    public class LinkedButton<T> : Button
    {
        public readonly T LinkedData;

        public LinkedButton(T link) : base()
        {
            LinkedData = link;
        }
    }
}