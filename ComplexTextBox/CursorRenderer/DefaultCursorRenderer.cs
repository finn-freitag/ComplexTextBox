using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexTextBox.CursorRenderer
{
    public class DefaultCursorRenderer : ICursorRenderer
    {
        bool Visible = true;

        public int UpdateTimeMillis { get { return 500; } }

        public void Update()
        {
            Visible = !Visible;
        }

        public void BringIntoView()
        {
            Visible = true;
        }

        public void RenderCursor(Graphics g, int LineHeight, PointF point)
        {
            if (Visible) g.DrawLine(new Pen(Brushes.Gray, 1.5f), point, new PointF(point.X, point.Y + LineHeight));
        }
    }
}
