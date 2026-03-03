namespace WinFormsApp1
{
    partial class GameForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // GameForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDarkDark;
            ClientSize = new Size(887, 581);
            Margin = new Padding(3, 2, 3, 2);
            Name = "GameForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Connect 4";
            MouseDown += GameForm_MouseClick;
            ResumeLayout(false);
        }

        #endregion
    }
}
