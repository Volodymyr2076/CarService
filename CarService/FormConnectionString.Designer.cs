namespace CarService
{
    partial class FormConnectionString
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
            this.textBoxConnectionString = new System.Windows.Forms.TextBox();
            this.buttonChangeConnectionString = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxConnectionString
            // 
            this.textBoxConnectionString.Location = new System.Drawing.Point(12, 22);
            this.textBoxConnectionString.Name = "textBoxConnectionString";
            this.textBoxConnectionString.Size = new System.Drawing.Size(460, 20);
            this.textBoxConnectionString.TabIndex = 0;
            // 
            // buttonChangeConnectionString
            // 
            this.buttonChangeConnectionString.Location = new System.Drawing.Point(397, 48);
            this.buttonChangeConnectionString.Name = "buttonChangeConnectionString";
            this.buttonChangeConnectionString.Size = new System.Drawing.Size(75, 23);
            this.buttonChangeConnectionString.TabIndex = 1;
            this.buttonChangeConnectionString.Text = "Изменить";
            this.buttonChangeConnectionString.UseVisualStyleBackColor = true;
            this.buttonChangeConnectionString.Click += new System.EventHandler(this.buttonChangeConnectionString_Click);
            // 
            // FormConnectionString
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 111);
            this.Controls.Add(this.buttonChangeConnectionString);
            this.Controls.Add(this.textBoxConnectionString);
            this.Name = "FormConnectionString";
            this.Text = "FormConnectionString";
            this.Load += new System.EventHandler(this.FormConnectionString_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxConnectionString;
        private System.Windows.Forms.Button buttonChangeConnectionString;
    }
}