namespace uPD
{
    partial class Vhdl_File
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txt_Vhdl = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txt_Vhdl
            // 
            this.txt_Vhdl.BackColor = System.Drawing.Color.Gainsboro;
            this.txt_Vhdl.Location = new System.Drawing.Point(12, 12);
            this.txt_Vhdl.Multiline = true;
            this.txt_Vhdl.Name = "txt_Vhdl";
            this.txt_Vhdl.ReadOnly = true;
            this.txt_Vhdl.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txt_Vhdl.Size = new System.Drawing.Size(614, 584);
            this.txt_Vhdl.TabIndex = 0;
            // 
            // Vhdl_File
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(638, 608);
            this.Controls.Add(this.txt_Vhdl);
            this.Name = "Vhdl_File";
            this.Text = "VHDL Component";
            this.Load += new System.EventHandler(this.Vhdl_File_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt_Vhdl;
    }
}