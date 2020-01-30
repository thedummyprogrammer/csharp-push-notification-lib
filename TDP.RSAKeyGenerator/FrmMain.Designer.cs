namespace TDP.RSAKeyGenerator
{
    partial class FrmMain
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
            this.BtnGenerate = new System.Windows.Forms.Button();
            this.TxtKey = new System.Windows.Forms.TextBox();
            this.BtnGenerateForAppConfig = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BtnGenerate
            // 
            this.BtnGenerate.Location = new System.Drawing.Point(313, 159);
            this.BtnGenerate.Name = "BtnGenerate";
            this.BtnGenerate.Size = new System.Drawing.Size(75, 23);
            this.BtnGenerate.TabIndex = 0;
            this.BtnGenerate.Text = "Generate";
            this.BtnGenerate.UseVisualStyleBackColor = true;
            this.BtnGenerate.Click += new System.EventHandler(this.BtnGenerate_Click);
            // 
            // TxtKey
            // 
            this.TxtKey.Location = new System.Drawing.Point(15, 17);
            this.TxtKey.Multiline = true;
            this.TxtKey.Name = "TxtKey";
            this.TxtKey.Size = new System.Drawing.Size(535, 136);
            this.TxtKey.TabIndex = 1;
            // 
            // BtnGenerateForAppConfig
            // 
            this.BtnGenerateForAppConfig.Location = new System.Drawing.Point(394, 159);
            this.BtnGenerateForAppConfig.Name = "BtnGenerateForAppConfig";
            this.BtnGenerateForAppConfig.Size = new System.Drawing.Size(156, 23);
            this.BtnGenerateForAppConfig.TabIndex = 2;
            this.BtnGenerateForAppConfig.Text = "Generate for app.config";
            this.BtnGenerateForAppConfig.UseVisualStyleBackColor = true;
            this.BtnGenerateForAppConfig.Click += new System.EventHandler(this.BtnGenerateForAppConfig_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(568, 199);
            this.Controls.Add(this.BtnGenerateForAppConfig);
            this.Controls.Add(this.TxtKey);
            this.Controls.Add(this.BtnGenerate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RSA Key Generator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnGenerate;
        private System.Windows.Forms.TextBox TxtKey;
        private System.Windows.Forms.Button BtnGenerateForAppConfig;
    }
}

