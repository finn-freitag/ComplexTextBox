using ComplexTextBox.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexTextBox.TextRenderer
{
    public class SelectionRenderer : ITextRenderer
    {
        public Color SelectionBackColor = Color.FromArgb(150, 3, 78, 252);

        public void RenderText(Graphics g, Font font, PointF point, string text, Brush ForeColor, Brush BackColor)
        {
            Size textSize = MeasureTextHelper.MeasureText(text, font);
            g.FillRectangle(new SolidBrush(SelectionBackColor), new RectangleF(point, textSize));
            g.DrawString(text, font, ForeColor, point);
        }
    }
}
