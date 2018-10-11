using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ContentAlignment = System.Drawing.ContentAlignment;

namespace SudokuMaster4
{
    public partial class Form1
    {
        #region Class Level Variables

        public string FileContent { get; set; }

        // dimension of each cell in the grid   
        public readonly int CellWidth = 64;
        public readonly int CellHeight = 64;

        public int SelectedNumber = 0;

        // offset from the top left corner of the window   
        public readonly int XOffset = -40;
        public readonly int YOffset = -4;

        //  color for original puzzle values  
        public readonly Color FixedBackColor = Color.LightSteelBlue;
        public readonly Color FixedForeColor = Color.SteelBlue;
        public readonly Color UserBackColor = Color.LightYellow;
        public readonly Color UserForeColor = Color.Black;

        public bool ShowHints { get; set; } = true;

        //  keep track of file name to save to  
        public string SaveFileName = string.Empty;

        //  the number currently selected for insertion  
        // has the game started?  
        public bool GameStarted;

        //  used to keep track of elapsed time  
        public int Seconds;

        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // initialize the status bar 
            toolStripStatusLabel1.Text = string.Empty;
            toolStripStatusLabel2.Text = string.Empty;

            // draw the board 
            DrawBoard();
        }

        public void DrawBoard()
        {
            if (tableLayoutPanel1.Controls.Count >= 81) throw new Exception("The board has already been drawn.");

            // used to store the location of the cell 
            var location = new Point();
            // draws the cells
            for (var row = 1; row <= 9; row++)
            {
                for (var col = 1; col <= 9; col++)
                {
                    location.X = col * (CellWidth + 1) + XOffset - 8;
                    location.Y = row * (CellHeight + 1) + YOffset - 28;
                    var subgrid = SudokuPuzzle.GetRegion(col, row);
                    var label = new SudokuLabel
                    {
                        Name = $"Label{col}{row}{subgrid}",
                        Tag = $"Label{col}{row}{subgrid}",
                        Font = new Font("Consolas", 8, FontStyle.Bold),
                        BorderStyle = BorderStyle.Fixed3D,
                        Location = location,
                        Width = CellWidth,
                        Height = CellHeight,
                        Margin = new Padding(0),
                        Padding = new Padding(0),
                        TextAlign = ContentAlignment.MiddleCenter,
                        BackColor = UserBackColor,
                        ForeColor = UserForeColor,
                        Text = $@"{col}{row}{subgrid}",
                        Column = col,
                        Row = row,
                        SubGrid = SudokuPuzzle.GetRegion(col, row)
                    };

                    if (tableLayoutPanel1.Controls.Count < 81) tableLayoutPanel1.Controls.Add(label);
                }
            }
        }

        public void StartNewGame()
        {
            SaveFileName = string.Empty;
            Seconds = 0;
            GameStarted = true;
            timer1.Enabled = true;
            ButtonNotes.Visible = true;
            toolStripStatusLabel1.Text = @"New game started";
            toolTip1.RemoveAll();
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (GameStarted)
                {
                    var dr = ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        new SudokuPuzzle().SaveGameToDisk(false);
                    }
                    else if (dr == DialogResult.Cancel)
                    {
                        return;
                    }
                }
            }
            catch (Exception)
            {
                Trace.WriteLine(typeof(Exception).ToString());
            }

            // load the game from disk 
            var filter = @"SS files (easy1.ss)|easy1.ss|SDO files (*.sdo)|*.sdo|All files (*.*)|*.*";
            var dialog = new OpenFileDialog
            {
                Filter = filter,
                FilterIndex = 1,
                RestoreDirectory = false
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                FileContent = File.ReadAllText(dialog.FileName).Replace("X", "0").Replace("\r", "").Replace("\n", "");
                if (FileContent.Length != 81)
                {
                    throw new InvalidDataException("Input data was not formatted correctly.");
                }

                Text = dialog.FileName;
                SaveFileName = dialog.FileName;

            }
            else
            {
                return;
            }


            StartNewGame();

            // initialize the board 
            var counter = 0;
            foreach (var row in Enumerable.Range(1, 9))
            {
                foreach (var col in Enumerable.Range(1, 9))
                {
                    var i = counter++;
                    var value = int.Parse(FileContent[i].ToString());
                    SetCell(col, row, value);
                }
            }

            DisplayCandidatesForAllCells();

        }

        public void DisplayCandidatesForCell(SudokuLabel label)
        {
            var candidates = "123456789";
            const string nl = "\r\n";

            // Get the SudokuLabels that can be changed by the user.
            foreach (var lbl in tableLayoutPanel1.Controls.OfType<SudokuLabel>().Where(lbl => lbl.Column == label.Column))
            {
                candidates = candidates.Replace(lbl.Value.ToString(), " ");
                label.Candidates = $@"{candidates.Substring(0, 3)}{nl}{candidates.Substring(3, 3)}{nl}{candidates.Substring(6, 3)}";
            }
            foreach (var lbl in tableLayoutPanel1.Controls.OfType<SudokuLabel>().Where(lbl => lbl.Row == label.Row))
            {
                candidates = candidates.Replace(lbl.Value.ToString(), " ");
                label.Candidates = $@"{candidates.Substring(0, 3)}{nl}{candidates.Substring(3, 3)}{nl}{candidates.Substring(6, 3)}";
            }
            foreach (var lbl in tableLayoutPanel1.Controls.OfType<SudokuLabel>().Where(lbl => lbl.SubGrid == label.SubGrid))
            {
                candidates = candidates.Replace(lbl.Value.ToString(), " ");
                label.Candidates = $@"{candidates.Substring(0, 3)}{nl}{candidates.Substring(3, 3)}{nl}{candidates.Substring(6, 3)}";
            }

            candidates = $"{candidates.Substring(0, 3)}{nl}{candidates.Substring(3, 3)}{nl}{candidates.Substring(6, 3)}";
            label.Text = candidates;
        }

        public void SetCell(int col, int row, int value)
        {
            // Find the Label control 
            var subgrid = SudokuPuzzle.GetRegion(col, row);
            var labelName = $"Label{col}{row}{subgrid}";
            var control = tableLayoutPanel1.Controls.Find(labelName, false).FirstOrDefault();
            var label = (SudokuLabel)control;
            if (label == null) return;

            if (value > 0)
            {
                label.Value = value;
                label.Font = new Font("Consolas", 12);
                label.BackColor = FixedBackColor;
                label.ForeColor = FixedForeColor;
                label.Text = $@"{value}";
                label.Enabled = false;
            }
            else if (value == 0)
            {
                // set the appearance for the Label control 
                label.Font = new Font("Consolas", 8);
                label.BackColor = UserBackColor;
                label.ForeColor = UserForeColor;
                label.MouseDown += SudokuLabel_MouseDown;
            }
        }

        private void DifficultMenuItem_Click(object sender, EventArgs e)
        {
            easyMenuItem.Checked = false;
            mediumMenuItem.Checked = false;
            extremelyDifficultMenuItem.Checked = false;
        }

        private void EasyMenuItem_Click(object sender, EventArgs e)
        {
            mediumMenuItem.Checked = false;
            difficultMenuItem.Checked = false;
            extremelyDifficultMenuItem.Checked = false;
        }

        private void ExtremelyDifficultMenuItem_Click(object sender, EventArgs e)
        {
            easyMenuItem.Checked = false;
            mediumMenuItem.Checked = false;
            difficultMenuItem.Checked = false;
        }

        private void MediumMenuItem_Click(object sender, EventArgs e)
        {
            easyMenuItem.Checked = false;
            difficultMenuItem.Checked = false;
            extremelyDifficultMenuItem.Checked = false;
        }

        private void OnMenuItemClick(object sender, EventArgs e)
        {
            if (sender is MenuItem item)
            {
                if (item.Parent is ContextMenu owner)
                {
                    var label = (SudokuLabel)owner.SourceControl;
                    Trace.WriteLine(label.Name);
                    label.Font = new Font("Consolas", 12);
                    if (item.Text.Contains("Exclude")) label.Text = item.Text.Substring(7);
                    if (item.Text.Contains("Make")) label.Text = item.Text.Substring(5);
                    SelectedNumber = label.Value = int.Parse(label.Text);

                    //todo: Create IsMoveValid function.
                    // check if move is valid
                    if (!new SudokuPuzzle().IsMoveValid(label.Column, label.Row, label.Value))
                    {
                        new SudokuPuzzle().DisplayActivity($"Invalid move at ({label.Column},{label.Row})", false);
                        return;
                    }

                    //todo: Create IsPuzzleSolved function.
                    if (new SudokuPuzzle().IsPuzzleSolved())
                    {
                        timer1.Enabled = false;
                        Console.Beep();
                        toolStripStatusLabel1.Text = @"*****Puzzle Solved*****";
                    }

                    label.Enabled = false;
                    DisplayCandidatesForAllCells();
                }
            }
        }

        private void SudokuLabel_MouseDown(object sender, MouseEventArgs e)
        {
            // Check to see if the game has started.
            if (!GameStarted)
            {
                TextBoxActivities.Text = @"Click File->New to start a new game or File->Open to load an existing game";
                return;
            }

            var label = (SudokuLabel)sender;
            if (label == null) return;

            if (!label.IsGiven)
            {
                var candidates = label.Candidates.Replace(" ", "").Replace("\r\n", "");
                var cm = new ContextMenu();
                cm.MenuItems.Clear();

                foreach (var c in candidates)
                {
                    var mi = new MenuItem($"Make {c}");
                    cm.MenuItems.Add(mi);
                    mi.Click += OnMenuItemClick;
                }

                if (candidates.Length > 1)
                {
                    var separator = new MenuItem("-");
                    cm.MenuItems.Add(separator);

                    foreach (var c in candidates)
                    {
                        var mi = new MenuItem($"Exclude {c}");
                        cm.MenuItems.Add(mi);
                        mi.Click += OnMenuItemClick;
                    }

                }
                label.ContextMenu = cm;
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel2.Text = $@"Elapsed time: {Seconds.ToString()} second(s)";
            Seconds++;
        }

        private void DisplayCandidatesForAllCells()
        {
            foreach (var label in tableLayoutPanel1.Controls.OfType<SudokuLabel>().Where(label => label.Value == 0))
            {
                if (ShowHints)
                {
                    label.ForeColor = UserForeColor;
                }
                else if (!ShowHints)
                {
                    label.ForeColor = UserBackColor;
                }

                DisplayCandidatesForCell(label);
            }

        }

        private void ButtonNotes_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            if (button.Text == @"Hide Candidates")
            {
                ShowHints = false;
                DisplayCandidatesForAllCells();
            }
            if (button.Text == @"Display Candidates")
            {
                ShowHints = true;
                DisplayCandidatesForAllCells();
            }

            if (ShowHints) button.Text = @"Hide Candidates";
            if (!ShowHints) button.Text = @"Display Candidates";
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GameStarted)
            {
                Visible = false;
                var dr = ShowDialog();
                MessageBox.Show(@"Do you want to save current game?");

                if (dr == DialogResult.OK)
                {
                    new SudokuPuzzle().SaveGameToDisk(false);
                }
                else if (dr == DialogResult.Cancel)
                {
                    return;
                }
            }

            Application.Exit();

        }

        private void ButtonClearTextBox_Click(object sender, EventArgs e)
        {

        }

        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void RedoToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!GameStarted)
            {
                new SudokuPuzzle().DisplayActivity("Game not started yet.", true);
                return;
            }

            new SudokuPuzzle().SaveGameToDisk(true);
        }

        private void CreatePuzzleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new PuzzleCreator();
            form.Show();
        }
    }
}