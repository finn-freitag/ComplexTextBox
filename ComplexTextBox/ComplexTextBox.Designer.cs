namespace ComplexTextBox
{
    partial class ComplexTextBox
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ComplexTextBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "ComplexTextBox";
            this.Size = new System.Drawing.Size(735, 448);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ComplexTextBox_KeyPress);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ComplexTextBox_MouseClick);
            this.Resize += new System.EventHandler(this.ComplexTextBox_Resize);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
