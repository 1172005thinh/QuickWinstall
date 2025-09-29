namespace QuickWinstall
{
    partial class AboutForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // Controls
        private System.Windows.Forms.Label lblProjectTitle;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label lblAuthor;
        private System.Windows.Forms.LinkLabel lnkGitHub;
        private System.Windows.Forms.LinkLabel lnkFacebook;
        private System.Windows.Forms.Label lblContributors;
        private System.Windows.Forms.Label lblContributorsContent;
        private System.Windows.Forms.Label lblReferences;
        private System.Windows.Forms.LinkLabel lnkSchneegans;
        private System.Windows.Forms.Label lblLicense;
        private System.Windows.Forms.Label lblLicenseContent;
        private System.Windows.Forms.Button btnOK;

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
            this.lblProjectTitle = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblAuthor = new System.Windows.Forms.Label();
            this.lnkGitHub = new System.Windows.Forms.LinkLabel();
            this.lnkFacebook = new System.Windows.Forms.LinkLabel();
            this.lblContributors = new System.Windows.Forms.Label();
            this.lblContributorsContent = new System.Windows.Forms.Label();
            this.lblReferences = new System.Windows.Forms.Label();
            this.lnkSchneegans = new System.Windows.Forms.LinkLabel();
            this.lblLicense = new System.Windows.Forms.Label();
            this.lblLicenseContent = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            
            this.SuspendLayout();
            
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
            this.ClientSize = new System.Drawing.Size(400, 500);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
        }

        private void SetupControls()
        {
            const int padding = 20;
            const int lineHeight = 25;
            const int spacing = 10;
            
            int yPos = padding;
            
            // Project Title
            this.lblProjectTitle.Text = "Project: QuickWinstall - Autounattend Generator";
            this.lblProjectTitle.Location = new System.Drawing.Point(padding, yPos);
            this.lblProjectTitle.Size = new System.Drawing.Size(360, lineHeight);
            this.lblProjectTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            
            yPos += lineHeight + spacing;
            
            // Version
            this.lblVersion.Text = "Version: 0.1";
            this.lblVersion.Location = new System.Drawing.Point(padding, yPos);
            this.lblVersion.Size = new System.Drawing.Size(360, lineHeight);
            
            yPos += lineHeight + spacing;
            
            // Description
            this.lblDescription.Text = "For LAZYY~ Windows 11 Installation";
            this.lblDescription.Location = new System.Drawing.Point(padding, yPos);
            this.lblDescription.Size = new System.Drawing.Size(360, lineHeight);
            this.lblDescription.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            
            yPos += lineHeight + spacing * 2;
            
            // Author
            this.lblAuthor.Text = "Original author: 1172005thinh (QuickComp.)";
            this.lblAuthor.Location = new System.Drawing.Point(padding, yPos);
            this.lblAuthor.Size = new System.Drawing.Size(360, lineHeight);
            
            yPos += lineHeight + 5;
            
            // GitHub Link
            this.lnkGitHub.Text = "     GitHub";
            this.lnkGitHub.Location = new System.Drawing.Point(padding, yPos);
            this.lnkGitHub.Size = new System.Drawing.Size(360, lineHeight);
            this.lnkGitHub.LinkColor = System.Drawing.Color.Blue;
            this.lnkGitHub.VisitedLinkColor = System.Drawing.Color.Purple;
            this.lnkGitHub.Tag = "https://github.com/1172005thinh/QuickWinstall";
            
            yPos += lineHeight + 5;
            
            // Facebook Link
            this.lnkFacebook.Text = "     Facebook";
            this.lnkFacebook.Location = new System.Drawing.Point(padding, yPos);
            this.lnkFacebook.Size = new System.Drawing.Size(360, lineHeight);
            this.lnkFacebook.LinkColor = System.Drawing.Color.Blue;
            this.lnkFacebook.VisitedLinkColor = System.Drawing.Color.Purple;
            this.lnkFacebook.Tag = "https://facebook.com/quickcomp.hungthinhnguyen";
            
            yPos += lineHeight + spacing * 2;
            
            // Contributors
            this.lblContributors.Text = "Contributors:";
            this.lblContributors.Location = new System.Drawing.Point(padding, yPos);
            this.lblContributors.Size = new System.Drawing.Size(360, lineHeight);
            this.lblContributors.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            
            yPos += lineHeight + 5;
            
            this.lblContributorsContent.Text = "     ";
            this.lblContributorsContent.Location = new System.Drawing.Point(padding, yPos);
            this.lblContributorsContent.Size = new System.Drawing.Size(360, lineHeight);
            
            yPos += lineHeight + spacing;
            
            // References
            this.lblReferences.Text = "References:";
            this.lblReferences.Location = new System.Drawing.Point(padding, yPos);
            this.lblReferences.Size = new System.Drawing.Size(360, lineHeight);
            this.lblReferences.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            
            yPos += lineHeight + 5;
            
            // Schneegans Link
            this.lnkSchneegans.Text = "     Schneegans Autounattend Generator";
            this.lnkSchneegans.Location = new System.Drawing.Point(padding, yPos);
            this.lnkSchneegans.Size = new System.Drawing.Size(360, lineHeight);
            this.lnkSchneegans.LinkColor = System.Drawing.Color.Blue;
            this.lnkSchneegans.VisitedLinkColor = System.Drawing.Color.Purple;
            this.lnkSchneegans.Tag = "https://schneegans.de/windows/unattend-generator";
            
            yPos += lineHeight + spacing * 2;
            
            // License
            this.lblLicense.Text = "License:";
            this.lblLicense.Location = new System.Drawing.Point(padding, yPos);
            this.lblLicense.Size = new System.Drawing.Size(360, lineHeight);
            this.lblLicense.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            
            yPos += lineHeight + 5;
            
            this.lblLicenseContent.Text = "     This is a free, open-source project.";
            this.lblLicenseContent.Location = new System.Drawing.Point(padding, yPos);
            this.lblLicenseContent.Size = new System.Drawing.Size(360, lineHeight);
            
            // OK Button
            const int buttonWidth = 80;
            const int buttonHeight = 30;
            
            this.btnOK.Text = "OK";
            this.btnOK.Location = new System.Drawing.Point(this.ClientSize.Width - buttonWidth - padding, 
                                                           this.ClientSize.Height - buttonHeight - padding);
            this.btnOK.Size = new System.Drawing.Size(buttonWidth, buttonHeight);
            this.btnOK.BackColor = System.Drawing.Color.White;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            
            // Add all controls to form
            this.Controls.Add(this.lblProjectTitle);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.lblAuthor);
            this.Controls.Add(this.lnkGitHub);
            this.Controls.Add(this.lnkFacebook);
            this.Controls.Add(this.lblContributors);
            this.Controls.Add(this.lblContributorsContent);
            this.Controls.Add(this.lblReferences);
            this.Controls.Add(this.lnkSchneegans);
            this.Controls.Add(this.lblLicense);
            this.Controls.Add(this.lblLicenseContent);
            this.Controls.Add(this.btnOK);
            
            // Set accept button
            this.AcceptButton = this.btnOK;
        }

        private void SetupEventHandlers()
        {
            this.lnkGitHub.LinkClicked += LinkLabel_LinkClicked;
            this.lnkFacebook.LinkClicked += LinkLabel_LinkClicked;
            this.lnkSchneegans.LinkClicked += LinkLabel_LinkClicked;
        }

        #endregion
    }
}