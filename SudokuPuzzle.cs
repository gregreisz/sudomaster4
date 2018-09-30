using System;
using System.Windows.Forms;

namespace SudokuMaster4
{
    public class SudokuPuzzle
    {
        #region Class level variables


        #endregion



        //==================================================
        // Check if move is valid
        //==================================================
        public bool IsMoveValid(int col, int row, int value)
        {
            return true;
        }

        //==================================================
        // Check if the puzzle is solved
        //==================================================
        public bool IsPuzzleSolved()
        {
            return true;
        }


        //==================================================
        // Displays a message in the Activities text box
        //==================================================
        public void DisplayActivity(string str, bool soundBeep)
        {
            var form1 = new Form1();
            if (soundBeep) Console.Beep();
            if (string.IsNullOrEmpty(str)) return;
            form1.TextBoxActivities.Text += str + Environment.NewLine;
        }

        //==================================================
        // Set the Tooltip for a Label control
        //==================================================
        public void SetToolTip(int col, int row, string possiblevalues)
        {
            // Locate the particular Label control 
            var form1 = new Form1();
            var lbl = form1.Controls.Find(col + row.ToString(), true);
            form1.toolTip1.SetToolTip((Label)lbl[0], possiblevalues);
        }
        //==================================================
        // Save the game to disk
        //==================================================
        public void SaveGameToDisk(bool saveAs)
        {
            var form1 = new Form1();
            // if saveFileName is empty, means game has not been saved before
            if (form1.SaveFileName == string.Empty || saveAs)
            {
                var saveFileDialog1 = new SaveFileDialog
                {
                    Filter = @"SDO files (*.sdo)|*.sdo|All files (*.*)|*.*",
                    FilterIndex = 1,
                    RestoreDirectory = false
                };
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    form1.SaveFileName = saveFileDialog1.FileName;
            }


        }

        public static int GetRegion(int col, int row)
        {
            switch (int.Parse($"{col}{row}"))
            {
                case 11:
                case 21:
                case 31:
                case 12:
                case 22:
                case 32:
                case 13:
                case 23:
                case 33:
                    return 1;
                case 41:
                case 51:
                case 61:
                case 42:
                case 52:
                case 62:
                case 43:
                case 53:
                case 63:
                    return 2;
                case 71:
                case 81:
                case 91:
                case 72:
                case 82:
                case 92:
                case 73:
                case 83:
                case 93:
                    return 3;
                case 14:
                case 24:
                case 34:
                case 15:
                case 25:
                case 35:
                case 16:
                case 26:
                case 36:
                    return 4;
                case 44:
                case 54:
                case 64:
                case 45:
                case 55:
                case 65:
                case 46:
                case 56:
                case 66:
                    return 5;
                case 74:
                case 84:
                case 94:
                case 75:
                case 85:
                case 95:
                case 76:
                case 86:
                case 96:
                    return 6;
                case 17:
                case 27:
                case 37:
                case 18:
                case 28:
                case 38:
                case 19:
                case 29:
                case 39:
                    return 7;
                case 47:
                case 57:
                case 67:
                case 48:
                case 58:
                case 68:
                case 49:
                case 59:
                case 69:
                    return 8;
                case 77:
                case 87:
                case 97:
                case 78:
                case 88:
                case 98:
                case 79:
                case 89:
                case 99:
                    return 9;
                default:
                    return 0;
            }
        }
    }
}