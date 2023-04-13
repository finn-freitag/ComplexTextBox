using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComplexTextBoxTestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ComplexTextBox.ComplexTextBox complexTextBox = new ComplexTextBox.ComplexTextBox();
            complexTextBox.Location = new Point(0, 0);
            complexTextBox.Size = new Size(this.Width - 20, this.Height - 45);
            complexTextBox.Text = "Hello,\nMy name is Finn and this is a test with an especially long line, which protrudes over the border of this textbox!";
            complexTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            complexTextBox.BackColor = Color.Black;
            complexTextBox.ForeColor = Color.White;
            this.Controls.Add(complexTextBox);
        }
    }
}
