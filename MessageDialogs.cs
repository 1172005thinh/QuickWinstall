using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuickWinstall
{
    public static class MessageDialogs
    {
        public enum DialogType
        {
            Warning,
            Error,
            Info
        }

        public enum DialogButtons
        {
            YesNoCancel,
            YesCancel,
            OK
        }

        public static DialogResult ShowDialog(string message, DialogType type, DialogButtons buttons, string title = null)
        {
            using (var dialog = new CustomMessageDialog())
            {
                dialog.SetupDialog(message, type, buttons, title);
                return dialog.ShowDialog();
            }
        }
    }

    internal class CustomMessageDialog : Form
    {
        private Label lblMessage;
        private Button btnYes;
        private Button btnNo;
        private Button btnCancel;
        private Button btnOK;
        
        public CustomMessageDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.lblMessage = new Label();
            this.btnYes = new Button();
            this.btnNo = new Button();
            this.btnCancel = new Button();
            this.btnOK = new Button();
            
            this.SuspendLayout();
            
            // Form setup
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.ClientSize = new Size(400, 150);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            
            // Message label
            this.lblMessage.Location = new Point(20, 20);
            this.lblMessage.Size = new Size(360, 80);
            this.lblMessage.TextAlign = ContentAlignment.TopLeft;
            this.lblMessage.Font = new Font("Segoe UI", 9F);
            
            this.Controls.Add(this.lblMessage);
            
            this.ResumeLayout(false);
        }

        public void SetupDialog(string message, MessageDialogs.DialogType type, MessageDialogs.DialogButtons buttons, string title = null)
        {
            // Set title
            this.Text = title ?? GetDefaultTitle(type);
            
            // Set message
            this.lblMessage.Text = message;
            
            // Adjust size based on message length
            using (Graphics g = this.CreateGraphics())
            {
                SizeF textSize = g.MeasureString(message, this.lblMessage.Font, 360);
                int requiredHeight = (int)textSize.Height + 100; // Add space for buttons
                this.ClientSize = new Size(400, Math.Max(150, requiredHeight));
                this.lblMessage.Size = new Size(360, (int)textSize.Height + 10);
            }
            
            // Setup buttons
            SetupButtons(buttons);
        }

        private string GetDefaultTitle(MessageDialogs.DialogType type)
        {
            switch (type)
            {
                case MessageDialogs.DialogType.Warning:
                    return LanguageManager.Instance.GetString("Warning_Title", "Warning");
                case MessageDialogs.DialogType.Error:
                    return LanguageManager.Instance.GetString("Error_Title", "Error");
                case MessageDialogs.DialogType.Info:
                    return LanguageManager.Instance.GetString("Info_Title", "Information");
                default:
                    return "Dialog";
            }
        }

        private void SetupButtons(MessageDialogs.DialogButtons buttons)
        {
            const int buttonWidth = 80;
            const int buttonHeight = 30;
            const int padding = 20;
            const int spacing = 10;
            
            int buttonY = this.ClientSize.Height - buttonHeight - padding;
            
            switch (buttons)
            {
                case MessageDialogs.DialogButtons.YesNoCancel:
                    SetupYesNoCancelButtons(buttonY, buttonWidth, buttonHeight, padding, spacing);
                    break;
                case MessageDialogs.DialogButtons.YesCancel:
                    SetupYesCancelButtons(buttonY, buttonWidth, buttonHeight, padding, spacing);
                    break;
                case MessageDialogs.DialogButtons.OK:
                    SetupOKButton(buttonY, buttonWidth, buttonHeight, padding);
                    break;
            }
        }

        private void SetupYesNoCancelButtons(int buttonY, int buttonWidth, int buttonHeight, int padding, int spacing)
        {
            // Cancel button (rightmost)
            this.btnCancel.Text = LanguageManager.Instance.GetString("Common_Cancel", "Cancel");
            this.btnCancel.Location = new Point(this.ClientSize.Width - buttonWidth - padding, buttonY);
            this.btnCancel.Size = new Size(buttonWidth, buttonHeight);
            this.btnCancel.DialogResult = DialogResult.Cancel;
            SetupButtonStyle(this.btnCancel);
            
            // No button
            this.btnNo.Text = LanguageManager.Instance.GetString("Common_No", "No");
            this.btnNo.Location = new Point(this.ClientSize.Width - (buttonWidth * 2) - padding - spacing, buttonY);
            this.btnNo.Size = new Size(buttonWidth, buttonHeight);
            this.btnNo.DialogResult = DialogResult.No;
            SetupButtonStyle(this.btnNo);
            
            // Yes button
            this.btnYes.Text = LanguageManager.Instance.GetString("Common_Yes", "Yes");
            this.btnYes.Location = new Point(this.ClientSize.Width - (buttonWidth * 3) - padding - (spacing * 2), buttonY);
            this.btnYes.Size = new Size(buttonWidth, buttonHeight);
            this.btnYes.DialogResult = DialogResult.Yes;
            SetupButtonStyle(this.btnYes);
            
            this.Controls.Add(this.btnYes);
            this.Controls.Add(this.btnNo);
            this.Controls.Add(this.btnCancel);
            
            this.CancelButton = this.btnCancel;
        }

        private void SetupYesCancelButtons(int buttonY, int buttonWidth, int buttonHeight, int padding, int spacing)
        {
            // Cancel button (rightmost)
            this.btnCancel.Text = LanguageManager.Instance.GetString("Common_Cancel", "Cancel");
            this.btnCancel.Location = new Point(this.ClientSize.Width - buttonWidth - padding, buttonY);
            this.btnCancel.Size = new Size(buttonWidth, buttonHeight);
            this.btnCancel.DialogResult = DialogResult.Cancel;
            SetupButtonStyle(this.btnCancel);
            
            // Yes button
            this.btnYes.Text = LanguageManager.Instance.GetString("Common_Yes", "Yes");
            this.btnYes.Location = new Point(this.ClientSize.Width - (buttonWidth * 2) - padding - spacing, buttonY);
            this.btnYes.Size = new Size(buttonWidth, buttonHeight);
            this.btnYes.DialogResult = DialogResult.Yes;
            SetupButtonStyle(this.btnYes);
            
            this.Controls.Add(this.btnYes);
            this.Controls.Add(this.btnCancel);
            
            this.CancelButton = this.btnCancel;
        }

        private void SetupOKButton(int buttonY, int buttonWidth, int buttonHeight, int padding)
        {
            // OK button (centered right)
            this.btnOK.Text = LanguageManager.Instance.GetString("Common_OK", "OK");
            this.btnOK.Location = new Point(this.ClientSize.Width - buttonWidth - padding, buttonY);
            this.btnOK.Size = new Size(buttonWidth, buttonHeight);
            this.btnOK.DialogResult = DialogResult.OK;
            SetupButtonStyle(this.btnOK);
            
            this.Controls.Add(this.btnOK);
            
            this.AcceptButton = this.btnOK;
            this.CancelButton = this.btnOK;
        }

        private void SetupButtonStyle(Button button)
        {
            button.BackColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderColor = Color.Black;
            button.UseVisualStyleBackColor = false;
            button.Cursor = Cursors.Hand;
        }
    }
}