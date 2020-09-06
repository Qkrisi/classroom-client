using System;
using System.Windows.Forms;
using static Classroom_Client.ClassroomResolver;

namespace Classroom_Client
{
    class MainProgram
    {
        public static void Main(string[] args)
        {
            Settings.RefreshSettings();
            Resolve();
        }
    }
}
