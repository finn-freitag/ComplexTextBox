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
            complexTextBox.Text = "Hallo,\nMein Name ist Finn und das hier ist ein Test mit einer besonders langen Zeile, die über den Rand des Fenster hinausragt.";
            this.Controls.Add(complexTextBox);
        }
    }
}
