namespace QuickWinstall
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // Controls
        private System.Windows.Forms.Label lblLanguage;
        private System.Windows.Forms.ComboBox cmbLanguage;
        private System.Windows.Forms.Label lblDefaultLocation;
        private System.Windows.Forms.TextBox txtDefaultLocation;
        private System.Windows.Forms.Button btnBrowse;
        
        private System.Windows.Forms.Button btnAbout;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;

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
            this.lblLanguage = new System.Windows.Forms.Label();
            this.cmbLanguage = new System.Windows.Forms.ComboBox();
            this.lblDefaultLocation = new System.Windows.Forms.Label();
            this.txtDefaultLocation = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnAbout = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            
            this.SuspendLayout();
            
            // Setup main form
            this.SetupForm();
            this.SetupControls();
            this.SetupEventHandlers();
            
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void SetupForm()
        {
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(400, 200);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
        }

        private void SetupControls()
        {
            const int padding = 20;
            const int labelHeight = 23;
            const int controlHeight = 27;
            const int spacing = 15;
            const int buttonWidth = 80;
            const int buttonHeight = 30;
            
            int yPos = padding;
            
            // Language
            this.lblLanguage.Text = "Language:";
            this.lblLanguage.Location = new System.Drawing.Point(padding, yPos);
            this.lblLanguage.Size = new System.Drawing.Size(100, labelHeight);
            
            this.cmbLanguage.Location = new System.Drawing.Point(130, yPos - 2);
            this.cmbLanguage.Size = new System.Drawing.Size(200, controlHeight);
            this.cmbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            
            // Add language options
            var languages = LanguageManager.Instance.GetAvailableLanguages();
            this.cmbLanguage.DisplayMember = "NativeName";
            this.cmbLanguage.ValueMember = "Code";
            this.cmbLanguage.DataSource = languages;
            
            yPos += controlHeight + spacing;
            
            // Default Location
            this.lblDefaultLocation.Text = "XML Default Location:";
            this.lblDefaultLocation.Location = new System.Drawing.Point(padding, yPos);
            this.lblDefaultLocation.Size = new System.Drawing.Size(150, labelHeight);
            
            yPos += labelHeight + 5;
            
            this.txtDefaultLocation.Location = new System.Drawing.Point(padding, yPos);
            this.txtDefaultLocation.Size = new System.Drawing.Size(250, controlHeight);
            this.txtDefaultLocation.ReadOnly = true;
            this.txtDefaultLocation.BackColor = System.Drawing.Color.White;
            
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.Location = new System.Drawing.Point(280, yPos - 2);
            this.btnBrowse.Size = new System.Drawing.Size(80, controlHeight + 4);
            this.btnBrowse.BackColor = System.Drawing.Color.White;
            this.btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowse.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnBrowse.UseVisualStyleBackColor = false;
            this.btnBrowse.Cursor = System.Windows.Forms.Cursors.Hand;
            
            // Buttons
            int buttonY = this.ClientSize.Height - buttonHeight - padding;
            
            // About button (bottom left)
            this.btnAbout.Text = "About";
            this.btnAbout.Location = new System.Drawing.Point(padding, buttonY);
            this.btnAbout.Size = new System.Drawing.Size(buttonWidth, buttonHeight);
            this.btnAbout.BackColor = System.Drawing.Color.White;
            this.btnAbout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAbout.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnAbout.UseVisualStyleBackColor = false;
            this.btnAbout.Cursor = System.Windows.Forms.Cursors.Hand;
            
            // Cancel button (bottom right)
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Location = new System.Drawing.Point(this.ClientSize.Width - buttonWidth - padding, buttonY);
            this.btnCancel.Size = new System.Drawing.Size(buttonWidth, buttonHeight);
            this.btnCancel.BackColor = System.Drawing.Color.White;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            
            // Save button (before Cancel)
            this.btnSave.Text = "Save";
            this.btnSave.Location = new System.Drawing.Point(this.ClientSize.Width - (buttonWidth * 2) - padding - 10, buttonY);
            this.btnSave.Size = new System.Drawing.Size(buttonWidth, buttonHeight);
            this.btnSave.BackColor = System.Drawing.Color.White;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            
            // Add controls to form
            this.Controls.Add(this.lblLanguage);
            this.Controls.Add(this.cmbLanguage);
            this.Controls.Add(this.lblDefaultLocation);
            this.Controls.Add(this.txtDefaultLocation);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.btnAbout);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            
            // Set accept/cancel buttons
            this.AcceptButton = this.btnSave;
            this.CancelButton = this.btnCancel;
        }

        private void SetupEventHandlers()
        {
            this.btnBrowse.Click += BtnBrowse_Click;
            this.btnAbout.Click += BtnAbout_Click;
            this.btnSave.Click += BtnSave_Click;
        }

        #endregion
    }
}