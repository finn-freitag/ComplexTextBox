using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexTextBox.TextRenderer
{
    public class PlainTextRenderer : ITextRenderer
    {
        public void RenderText(Graphics g, Font font, PointF point, string text, Brush ForeColor, Brush BackColor)
        {
            g.DrawString(text, font, ForeColor, point);
        }
    }
}
