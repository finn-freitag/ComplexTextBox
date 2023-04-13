using ComplexTextBox.CursorRenderer;
using ComplexTextBox.TextRenderer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace ComplexTextBox
{
    public partial class ComplexTextBox : UserControl
    {
        //private string text;
        private List<string> lines = new List<string>();

        public new string Text
        {
            get
            {
                string finalStr = "";
                for(int i = 0; i < lines.Count; i++)
                {
                    finalStr += Linebreak + lines[i];
                }
                return finalStr.Substring(2);
            }
            set
            {
                string[] lns = ReplaceLinebreak(value).Split(new string[] { Linebreak }, StringSplitOptions.None);
                lines.Clear();
                lines.AddRange(lns);
                if (TextChanged != null) TextChanged(this, EventArgs.Empty);
            }
        }

        private (int, int) curPos = (0, 0);

        public (int, int) CursorPos
        {
            get
            {
                return curPos;
            }
            set
            {
                curPos = value;
                if (CursorPositionChanged != null) CursorPositionChanged(this, EventArgs.Empty);
            }
        }

        public new event EventHandler TextChanged;
        public event EventHandler CursorPositionChanged;

        public string Linebreak = "\r\n";
        public string[] ConvertToLinebreak = new string[] { "\r", "\n" };

        public int LineHeight = 35;
        public int LineSpacing = 5;
        public bool DynamicLineHeight = true;

        public new bool AutoScroll = true;

        public bool LineNumbering = true;
        private Font LineNumberingFont = new Font(FontFamily.GenericMonospace, 12);
        public ITextRenderer LineRenderer = new PlainTextRenderer();
        public Color NumberTextSeparatorColor = Color.Gray;

        public int LeftDistance = 10;
        public int TopDistance = 10;

        public int ScrollBarThickness = 20;

        private new Font DefaultFont = new Font(FontFamily.GenericMonospace, 12);

        public ICursorRenderer CursorRenderer = new DefaultCursorRenderer();

        public bool CursorBlinkingActive
        {
            get
            {
                return UpdateTimer != null;
            }
            set
            {
                if (value)
                {
                    if(UpdateTimer == null)
                    {
                        UpdateTimer = new Timer();
                        UpdateTimer.Interval = CursorRenderer.UpdateTimeMillis;
                        UpdateTimer.Tick += UpdateTimer_Tick;
                        UpdateTimer.Start();
                    }
                }
                else
                {
                    if(UpdateTimer != null)
                    {
                        UpdateTimer.Stop();
                        UpdateTimer.Dispose();
                        UpdateTimer = null;
                        CursorRenderer.BringIntoView();
                    }
                }
            }
        }

        private HScrollBar horizontalScroll;
        private VScrollBar verticalScroll;

        private Timer UpdateTimer;

        private Size MaxSize = new Size(0, 0);

        public ComplexTextBox()
        {
            InitializeComponent();
            SetStyle(ControlStyles.SupportsTransparentBackColor |
             ControlStyles.Opaque |
             ControlStyles.UserPaint |
             ControlStyles.AllPaintingInWmPaint |
             ControlStyles.OptimizedDoubleBuffer, true);
            horizontalScroll = new HScrollBar();
            horizontalScroll.Maximum = 1;
            horizontalScroll.Height = ScrollBarThickness;
            horizontalScroll.Width = this.Width - ScrollBarThickness;
            horizontalScroll.Location = new Point(0, this.Height - ScrollBarThickness);
            horizontalScroll.Enabled = false;
            horizontalScroll.ValueChanged += (object sender, EventArgs e) => { this.Refresh(); };
            this.Controls.Add(horizontalScroll);

            verticalScroll = new VScrollBar();
            verticalScroll.Maximum = 1;
            verticalScroll.Width = ScrollBarThickness;
            verticalScroll.Height = this.Height - ScrollBarThickness;
            verticalScroll.Location = new Point(this.Width - ScrollBarThickness, 0);
            verticalScroll.Enabled = false;
            verticalScroll.ValueChanged += (object sender, EventArgs e) => { this.Refresh(); };
            this.Controls.Add(verticalScroll);

            CursorBlinkingActive = true;

            TextChanged += ComplexTextBox_TextChanged;
            CursorPositionChanged += ComplexTextBox_CursorPositionChanged;
        }

        private void ComplexTextBox_CursorPositionChanged(object sender, EventArgs e)
        {
            if (AutoScroll)
            {
                try
                {
                    int NumberSpacing = 0;
                    if (LineNumbering) NumberSpacing = MeasureText(Convert.ToString(lines.Count), LineNumberingFont).Width + 10;

                    int CurPosPixelsW = NumberSpacing + LeftDistance - horizontalScroll.Value + MeasureText(lines[CursorPos.Item1].Substring(0, CursorPos.Item2), DefaultFont).Width + 2;
                    if (CurPosPixelsW > this.Width - ScrollBarThickness - LeftDistance)
                    {
                        int difference = CurPosPixelsW - (this.Width - ScrollBarThickness - LeftDistance);
                        horizontalScroll.Value += difference;
                    }
                    if (CurPosPixelsW < LeftDistance + NumberSpacing)
                    {
                        int difference = -CurPosPixelsW + LeftDistance + NumberSpacing;
                        horizontalScroll.Value -= difference;
                    }

                    int CurPosPixelsH = TopDistance - verticalScroll.Value + (LineHeight + LineSpacing) * CursorPos.Item1;
                    if (CurPosPixelsH > this.Height - ScrollBarThickness * 2 - TopDistance)
                    {
                        int difference = CurPosPixelsH - this.Height + ScrollBarThickness * 2 + TopDistance;
                        verticalScroll.Value = Math.Min(verticalScroll.Value + difference, verticalScroll.Maximum);
                    }
                    if (CurPosPixelsH < TopDistance)
                    {
                        int difference = -CurPosPixelsH + TopDistance;
                        verticalScroll.Value -= difference;
                    }
                    //if (CursorPos.Item1 == 0) verticalScroll.Value = 0;
                    //if (CursorPos.Item1 == lines.Count - 1) verticalScroll.Value = verticalScroll.Maximum;
                }
                catch { }
            }
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            CursorRenderer.Update();
            this.Refresh();
        }

        private void ComplexTextBox_TextChanged(object sender, EventArgs e)
        {
            MaxSize = MeasureText(GetLongestLine(), DefaultFont);
            int NumberSpacing = 0;
            if (LineNumbering) NumberSpacing = MeasureText(Convert.ToString(lines.Count), LineNumberingFont).Width + 10;
            int textWidth = this.Width - ScrollBarThickness - LeftDistance - NumberSpacing;
            int textHeight = this.Height - ScrollBarThickness - TopDistance;
            if (MaxSize.Width > textWidth)
            {
                horizontalScroll.Enabled = true;
                horizontalScroll.Maximum = (MaxSize.Width - textWidth + 5);
            }
            else
            {
                horizontalScroll.Enabled = false;
            }
            int neededHeight = ((DynamicLineHeight ? MaxSize.Height : LineHeight) + LineSpacing) * lines.Count;
            if (neededHeight > textHeight)
            {
                verticalScroll.Enabled = true;
                verticalScroll.Maximum = (neededHeight - textHeight + 5);
            }
            else
            {
                verticalScroll.Enabled = false;
            }
            if (DynamicLineHeight) LineHeight = MaxSize.Height;
            ValidateCursor();
            this.Refresh();
        }

        private void ValidateCursor()
        {
            if (CursorPos.Item1 < 0) CursorPos = (0, 0);
            if (CursorPos.Item1 >= lines.Count) CursorPos = (lines.Count - 1, lines[lines.Count - 1].Length);
            if (CursorPos.Item2 < 0) CursorPos = (CursorPos.Item1, 0);
            if (CursorPos.Item2 > lines[CursorPos.Item1].Length) CursorPos = (CursorPos.Item1, lines[CursorPos.Item1].Length);
        }

        private void MoveCursor(int positions)
        {
            if (positions > 0)
            {
                for (int i = 0; i < positions; i++)
                {
                    int oldPos = CursorPos.Item2;
                    CursorPos = (CursorPos.Item1, CursorPos.Item2 + 1);
                    ValidateCursor();
                    if(oldPos == CursorPos.Item2)
                    {
                        try
                        {
                            CursorPos = (CursorPos.Item1 + 1, 0);
                            string s = lines[CursorPos.Item1];
                        }
                        catch
                        {
                            CursorPos = (lines.Count - 1, lines[lines.Count - 1].Length);
                        }
                        ValidateCursor();
                    }
                }
            }
            else
            {
                for (int i = 0; i < -positions; i++)
                {
                    int oldPos = CursorPos.Item2;
                    CursorPos = (CursorPos.Item1, CursorPos.Item2 - 1);
                    ValidateCursor();
                    if (oldPos == CursorPos.Item2)
                    {
                        try
                        {
                            CursorPos = (CursorPos.Item1 - 1, lines[CursorPos.Item1 - 1].Length);
                        }
                        catch
                        {
                            CursorPos = (0, 0);
                        }
                        ValidateCursor();
                    }
                }
            }
        }

        private void InsertLetters(string letters)
        {
            letters = ReplaceLinebreak(letters);
            string[] addLines = letters.Split(new string[] { Linebreak }, StringSplitOptions.None);
            if(addLines.Length > 1)
            {
                addLines[0] = lines[CursorPos.Item1].Substring(0, CursorPos.Item2) + addLines[0];
                int newCurPos = addLines[addLines.Length - 1].Length;
                addLines[addLines.Length - 1] = addLines[addLines.Length - 1] + lines[CursorPos.Item1].Substring(CursorPos.Item2, lines[CursorPos.Item1].Length - CursorPos.Item2);
                lines[CursorPos.Item1] = addLines[0];
                for(int i = 1; i < addLines.Length; i++)
                {
                    lines.Insert(CursorPos.Item1 + i, addLines[i]);
                }
                CursorPos = (CursorPos.Item1 + addLines.Length - 1, newCurPos);
                ValidateCursor();
            }
            else
            {
                lines[CursorPos.Item1] = lines[CursorPos.Item1].Insert(CursorPos.Item2, addLines[0]);
                MoveCursor(addLines[0].Length);
            }
        }

        private void RemoveLetters(int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (CursorPos.Item2 > 0)
                {
                    lines[CursorPos.Item1] = lines[CursorPos.Item1].Remove(CursorPos.Item2 - 1, 1);
                    CursorPos = (CursorPos.Item1, CursorPos.Item2 - 1);
                }
                else
                {
                    if(CursorPos.Item1 > 0)
                    {
                        int newCurPos = lines[CursorPos.Item1 - 1].Length;
                        lines[CursorPos.Item1 - 1] += lines[CursorPos.Item1];
                        lines.RemoveAt(CursorPos.Item1);
                        CursorPos = (CursorPos.Item1 - 1, newCurPos);
                    }
                }
                ValidateCursor();
            }
        }

        private string ReplaceLinebreak(string str)
        {
            List<int> positions = new List<int>();
            for(int i = 0; i < ConvertToLinebreak.Length; i++)
            {
                while (str.Contains(ConvertToLinebreak[i]))
                {
                    positions.Add(str.IndexOf(ConvertToLinebreak[i]));
                    str = str.Remove(positions[positions.Count - 1], ConvertToLinebreak[i].Length);
                }
            }
            for(int i = 0; i < positions.Count; i++)
            {
                str = str.Insert(positions[i] + (i * (Linebreak.Length - 1)), Linebreak);
            }
            return str;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Fill using backcolor
            e.Graphics.Clear(BackColor);

            // Draw line numbers
            int NumberSpacing = 0;
            if (LineNumbering)
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    LineRenderer.RenderText(e.Graphics, LineNumberingFont, new PointF(5 - horizontalScroll.Value, TopDistance + i * (LineHeight + LineSpacing) - verticalScroll.Value), Convert.ToString(i + 1), new SolidBrush(ForeColor), new SolidBrush(BackColor));
                }
                NumberSpacing = MeasureText(Convert.ToString(lines.Count), LineNumberingFont).Width + 10;
                e.Graphics.DrawLine(new Pen(new SolidBrush(NumberTextSeparatorColor), 1.5f), new Point(NumberSpacing - horizontalScroll.Value, 0), new Point(NumberSpacing - horizontalScroll.Value, this.Height));
            }

            // Render text
            ITextRenderer renderer = new PlainTextRenderer();
            for(int i = 0; i < lines.Count; i++)
            {
                renderer.RenderText(e.Graphics, DefaultFont, new PointF(LeftDistance - horizontalScroll.Value + NumberSpacing, TopDistance + i * (LineHeight + LineSpacing) - verticalScroll.Value), lines[i], new SolidBrush(ForeColor), new SolidBrush(BackColor));
            }

            // Render Cursor
            int strWidth = MeasureText(lines[CursorPos.Item1].Substring(0, CursorPos.Item2), DefaultFont).Width;
            CursorRenderer.RenderCursor(e.Graphics, MaxSize.Height, new PointF(LeftDistance - horizontalScroll.Value + NumberSpacing + strWidth + 2, (LineHeight + LineSpacing) * CursorPos.Item1 + TopDistance - verticalScroll.Value));

            // Overwrite Corner between scrollbars for better look
            e.Graphics.FillRectangle(new SolidBrush(BackColor), this.Width - ScrollBarThickness, this.Height - ScrollBarThickness, ScrollBarThickness, ScrollBarThickness);

            this.Focus();
        }

        private string GetLongestLine()
        {
            string longest = lines[0];
            for(int i = 1; i < lines.Count; i++)
            {
                if (lines[i].Length > longest.Length) longest = lines[i];
            }
            return longest;
        }

        private void ComplexTextBox_Resize(object sender, EventArgs e)
        {
            try
            {
                if(horizontalScroll != null && verticalScroll != null)
                {
                    horizontalScroll.Width = this.Width - ScrollBarThickness;
                    horizontalScroll.Location = new Point(0, this.Height - ScrollBarThickness);

                    verticalScroll.Height = this.Height - ScrollBarThickness;
                    verticalScroll.Location = new Point(this.Width - ScrollBarThickness, 0);
                }
            }
            catch { }
        }

        private void ComplexTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Right)
            {
                MoveCursor(1);
                Refresh();
                return true;
            }
            if (keyData == Keys.Left)
            {
                MoveCursor(-1);
                Refresh();
                return true;
            }
            if(keyData == Keys.Up)
            {
                CursorPos = (CursorPos.Item1 - 1, CursorPos.Item2);
                ValidateCursor();
                Refresh();
                return true;
            }
            if(keyData == Keys.Down)
            {
                CursorPos = (CursorPos.Item1 + 1, CursorPos.Item2);
                ValidateCursor();
                Refresh();
                return true;
            }
            if (keyData == Keys.Back)
            {
                RemoveLetters(1);
                if (TextChanged != null) TextChanged(this, EventArgs.Empty);
                return true;
            }
            if(keyData == Keys.Delete)
            {
                (int, int) oldPos = CursorPos;
                MoveCursor(1);
                if (oldPos != CursorPos) RemoveLetters(1);
                if (TextChanged != null) TextChanged(this, EventArgs.Empty);
                return true;
            }
            InsertLetters("" + KeyCodeToUnicode(keyData));
            if (TextChanged != null) TextChanged(this, EventArgs.Empty);
            return true;
        }

        private void ComplexTextBox_MouseClick(object sender, MouseEventArgs e)
        {
            this.Focus();
        }

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

        public string KeyCodeToUnicode(Keys key)
        {
            byte[] keyboardState = new byte[255];
            bool keyboardStateStatus = GetKeyboardState(keyboardState);

            if (!keyboardStateStatus)
            {
                return "";
            }

            uint virtualKeyCode = (uint)key;
            uint scanCode = MapVirtualKey(virtualKeyCode, 0);
            IntPtr inputLocaleIdentifier = GetKeyboardLayout(0);

            StringBuilder result = new StringBuilder();
            ToUnicodeEx(virtualKeyCode, scanCode, keyboardState, result, (int)5, (uint)0, inputLocaleIdentifier);

            return result.ToString();
        }

        [DllImport("user32.dll")]
        static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll")]
        static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")]
        static extern IntPtr GetKeyboardLayout(uint idThread);

        [DllImport("user32.dll")]
        static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);
    }
}
