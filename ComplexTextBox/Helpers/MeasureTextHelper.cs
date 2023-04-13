using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComplexTextBox.Helpers
{
    public class MeasureTextHelper
    {
        public static Size MeasureText(string Text, Font Font)
        {
            // https://stackoverflow.com/a/6275131
            TextFormatFlags flags
              = TextFormatFlags.Left
              | TextFormatFlags.Top
              | TextFormatFlags.NoPadding
              | TextFormatFlags.NoPrefix;
            Size szProposed = new Size(int.MaxValue, int.MaxValue);
            Size sz1 = System.Windows.Forms.TextRenderer.MeasureText(".", Font, szProposed, flags);
            Size sz2 = System.Windows.Forms.TextRenderer.MeasureText(Text + ".", Font, szProposed, flags);
            return new Size(sz2.Width - sz1.Width, sz2.Height);
        }
    }
}
