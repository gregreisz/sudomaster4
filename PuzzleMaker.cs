using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SudokuMaster4.Properties;

namespace SudokuMaster4
{
    public partial class PuzzleMaker : Form
    {
        private string _fileContent = new string('.', 81);
        private string _filePath = Settings.Default.Path;
        private string _prefix = Settings.Default.Prefix;
        private string _differentiator = "1";
        private string _extension = Settings.Default.Extension;


        public PuzzleMaker()
        {
            InitializeComponent();
        }

        private void PuzzleMaker_Load(object sender, EventArgs e)
        {
            foreach (var i in Enumerable.Range(0, 81))
            {
                var tb = new TextBox
                {
                    Name = $"TextBox{i}",
                    Text = @".",
                    Tag = i,
                    BorderStyle = BorderStyle.FixedSingle,
                    TabIndex = i,
                    Size = new Size(30, 30),
                    Font = new Font("Consolas", 12, FontStyle.Regular),
                    Margin = new Padding(0),
                    Padding = new Padding(0),
                    TextAlign = HorizontalAlignment.Center,
                    MaxLength = 1
                };
                tb.KeyPress += Tb_KeyPress;
                tb.Enter += Tb_Enter;
                LayoutPanel1.Controls.Add(tb);
            }

            var sb = new StringBuilder();

            sb.AppendLine($@"{_fileContent.Substring(0, 9)}{Environment.NewLine}");
            sb.AppendLine($@"{_fileContent.Substring(9, 9)}{Environment.NewLine}");
            sb.AppendLine($@"{_fileContent.Substring(18, 9)}{Environment.NewLine}");
            sb.AppendLine($@"{_fileContent.Substring(27, 9)}{Environment.NewLine}");
            sb.AppendLine($@"{_fileContent.Substring(36, 9)}{Environment.NewLine}");
            sb.AppendLine($@"{_fileContent.Substring(45, 9)}{Environment.NewLine}");
            sb.AppendLine($@"{_fileContent.Substring(54, 9)}{Environment.NewLine}");
            sb.AppendLine($@"{_fileContent.Substring(63, 9)}{Environment.NewLine}");
            sb.AppendLine($@"{_fileContent.Substring(72, 9)}");

            TextBoxDifferentiator.Text = _differentiator;
            TextBoxExtension.Text = _extension;
            TextBoxPrefix.Text = _prefix;
            TextBoxPath.Text = $@"{_filePath}{_prefix} {_differentiator}{_extension}";
        }

        private static void Tb_Enter(object sender, EventArgs e)
        {
            var control = (Control) sender;
            if (control is TextBox tb)
            {
                tb.Text = @".";
            }
        }

        private void Tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            SelectNextControl((Control)sender, true, true, true, true);
        }

        private void ClearPuzzle()
        {
            foreach (var control in LayoutPanel1.Controls)
            {
                if (control is TextBox tb)
                {
                    tb.Text = @".";
                }
            }
        }

        private void ButtonSavePuzzle_Click(object sender, EventArgs e)
        {
            var sb = new StringBuilder();
            var counter = 0;
            foreach (var control in LayoutPanel1.Controls)
            {
                if (control is TextBox tb)
                {
                    sb.Append(tb.Text);
                    if (++counter % 9 == 0) sb.AppendLine();
                }
            }

            using (var writer = new StreamWriter($@"{_filePath}{_prefix} {_differentiator}{_extension}"))
            {
                writer.WriteLine(sb.ToString());
            }
        }

        private void TextBoxDifferentiator_TextChanged(object sender, EventArgs e)
        {
            var tb = (TextBox)sender;
            _differentiator = tb.Text;
            TextBoxPath.Text = $@"{_filePath}{_prefix} {_differentiator}{_extension}";
        }

        private void ButtonClearPuzzle_Click(object sender, EventArgs e)
        {
            ClearPuzzle();
        }

        private void PuzzleMaker_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.Path = TextBoxPath.Text;
            Settings.Default.Prefix = TextBoxPrefix.Text;
            Settings.Default.Extension = TextBoxExtension.Text;
            Settings.Default.Save();
        }
    }
}
