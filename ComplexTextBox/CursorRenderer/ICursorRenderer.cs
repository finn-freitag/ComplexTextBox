using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexTextBox.CursorRenderer
{
    public interface ICursorRenderer
    {
        int UpdateTimeMillis { get; }
        void Update();
        void BringIntoView();
        void RenderCursor(Graphics g, int LineHeight, PointF point);
    }
}
