using System;
using System.Threading;
using System.Windows.Forms;

namespace SudokuMaster4
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        ///

        private static string appGuid = "c3722be4-cfae-42b8-b079-e1e31018fd8b";

        [STAThread]
        static void Main()
        {
            Application.SetCompatibleTextRenderingDefault(false);
            Application.EnableVisualStyles();

            Application.Run(new Form1());
        }
    }
}
