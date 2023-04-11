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
            complexTextBox.Size = this.Size;
            this.Controls.Add(complexTextBox);
        }
    }
}
