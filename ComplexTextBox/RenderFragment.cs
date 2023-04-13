using ComplexTextBox.TextRenderer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexTextBox
{
    public class RenderFragment
    {
        public PointF Origin;
        public ITextRenderer Renderer;
        public string Text;

        public RenderFragment(PointF Origin, ITextRenderer Renderer, string Text)
        {
            this.Origin = Origin;
            this.Renderer = Renderer;
            this.Text = Text;
        }

        public void Render(Graphics g, Font font, Brush ForeColor, Brush BackColor)
        {
            Renderer.RenderText(g, font, Origin, Text, ForeColor, BackColor);
        }
    }
}
