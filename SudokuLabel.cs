using System.Drawing;
using System.Windows.Forms;

namespace SudokuMaster4
{
    public class SudokuLabel : Label
    {
        public int Column { get; set; }

        public int Row { get; set; }

        public int SubGrid { get; set; }

        public string Candidates { get; set; }

        public int Value { get; set; }

        public bool IsGiven { get; set; }

    }
}