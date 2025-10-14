using System;
using System.IO;
using System.Windows.Forms;
using QuickWinstall.Lib;

namespace QuickWinstall
{
    public partial class PresetForm : Form
    {
        // Helper class to store preset display name and filename
        private class PresetListItem
        {
            public string DisplayName { get; set; }
            public string FileName { get; set; }

            public override string ToString()
            {
                return DisplayName;
            }
        }

        public string SelectedPresetName { get; private set; }
        public PresetsManager.PresetData SelectedPreset { get; private set; }

        public PresetForm()
        {
            Initialize();
            LoadPresetsList();
        }

        private void LoadPresetsList()
        {
            try
            {
                presetsListBox.Items.Clear();
                var presets = PresetsManager.GetAvailablePresets();

                foreach (var preset in presets)
                {
                    // Try to get the preset info to display the PresetName
                    var presetInfo = PresetsManager.GetPresetInfo(preset);
                    
                    var listItem = new PresetListItem
                    {
                        FileName = preset
                    };

                    if (presetInfo != null && (!string.IsNullOrWhiteSpace(presetInfo.PresetName)))
                    {
                        // Use PresetName from metadata
                        listItem.DisplayName = presetInfo.PresetName;
                    }
                    else
                    {
                        // Fallback to filename with .json extension
                        listItem.DisplayName = $"res/presets/{preset}.json";
                    }

                    presetsListBox.Items.Add(listItem);
                }

                if (presetsListBox.Items.Count > 0)
                {
                    presetsListBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading presets: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void PresetsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (presetsListBox.SelectedItem == null)
            {
                loadButton.Enabled = false;
                deleteButton.Enabled = false;
                exportButton.Enabled = false;
                presetDescriptionTextBox.Text = "";
                return;
            }

            var selectedItem = presetsListBox.SelectedItem as PresetListItem;
            if (selectedItem == null)
                return;

            string selectedPresetFileName = selectedItem.FileName;
            loadButton.Enabled = true;
            deleteButton.Enabled = true;
            exportButton.Enabled = true;

            // Load preset info
            var presetInfo = PresetsManager.GetPresetInfo(selectedPresetFileName);
            if (presetInfo != null)
            {
                presetDescriptionTextBox.Text =
                    $"{LangManager.GetString("PresetFormTextBoxDescriptionTextBoxName")}: {presetInfo.PresetName}\r\n" +
                    $"{LangManager.GetString("PresetFormTextBoxDescriptionTextBoxVersion")}: {presetInfo.PresetVersion}\r\n" +
                    $"{LangManager.GetString("PresetFormTextBoxDescriptionTextBoxAuthor")}: {presetInfo.PresetAuthor}\r\n\r\n" +
                    $"{LangManager.GetString("PresetFormTextBoxDescriptionTextBoxDescription")}: {presetInfo.PresetDescription}";
            }
            else
            {
                presetDescriptionTextBox.Text = LangManager.GetString("PresetFormTextboxDescriptionTextBoxNoDescription");
            }
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            if (presetsListBox.SelectedItem == null)
                return;

            var selectedItem = presetsListBox.SelectedItem as PresetListItem;
            if (selectedItem == null)
                return;

            string selectedPresetFileName = selectedItem.FileName;
            var preset = PresetsManager.LoadPreset(selectedPresetFileName);

            if (preset == null)
            {
                MessageBox.Show(
                    $"Failed to load preset '{selectedItem.DisplayName}'.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            SelectedPresetName = selectedPresetFileName;
            SelectedPreset = preset;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            // Prompt for preset name
            using (var inputForm = new Form())
            {
                inputForm.Text = LangManager.GetString("PresetFormDialogSavePreset");
                inputForm.Size = new System.Drawing.Size(400, 150);
                inputForm.StartPosition = FormStartPosition.CenterParent;
                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                inputForm.MaximizeBox = false;
                inputForm.MinimizeBox = false;

                Label label = new Label()
                {
                    Text = LangManager.GetString("PresetFormLabelPresetName"),
                    Location = new System.Drawing.Point(20, 20),
                    AutoSize = true
                };

                TextBox textBox = new TextBox()
                {
                    Location = new System.Drawing.Point(20, 45),
                    Size = new System.Drawing.Size(340, 20)
                };

                Button okButton = new Button()
                {
                    Text = LangManager.GetString("PresetFormButtonOK"),
                    DialogResult = DialogResult.OK,
                    Location = new System.Drawing.Point(205, 75),
                    Size = new System.Drawing.Size(75, 25)
                };

                Button cancelButton = new Button()
                {
                    Text = LangManager.GetString("PresetFormButtonCancel"),
                    DialogResult = DialogResult.Cancel,
                    Location = new System.Drawing.Point(285, 75),
                    Size = new System.Drawing.Size(75, 25)
                };

                inputForm.Controls.AddRange(new Control[] { label, textBox, okButton, cancelButton });
                inputForm.AcceptButton = okButton;
                inputForm.CancelButton = cancelButton;

                if (inputForm.ShowDialog() == DialogResult.OK)
                {
                    string presetName = textBox.Text.Trim();

                    if (string.IsNullOrWhiteSpace(presetName))
                    {
                        MessageBox.Show(
                            LangManager.GetString("PresetFormErrorInvalidPresetName"),
                            LangManager.GetString("PresetFormErrorTitle"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return;
                    }

                    // Check if preset already exists
                    if (PresetsManager.PresetExists(presetName))
                    {
                        var result = MessageBox.Show(
                            LangManager.GetString("PresetFormWarningOverwrite"),
                            LangManager.GetString("PresetFormWarningTitle"),
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning
                        );

                        if (result != DialogResult.Yes)
                            return;
                    }

                    // This would need to collect current configuration from MainForm
                    // For now, show a message that this feature requires MainForm integration
                    MessageBox.Show(
                        "Save preset functionality requires integration with MainForm to collect current configuration.",
                        "Not Implemented",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (presetsListBox.SelectedItem == null)
                return;

            var selectedItem = presetsListBox.SelectedItem as PresetListItem;
            if (selectedItem == null)
                return;

            string selectedPresetFileName = selectedItem.FileName;

            // Prevent deletion of default presets
            if (selectedPresetFileName.Equals("default", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(
                    LangManager.GetString("PresetFormErrorCannotDeleteDefault"),
                    LangManager.GetString("PresetFormErrorTitle"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            var result = MessageBox.Show(
                string.Format(LangManager.GetString("PresetFormWarningDeletePreset"), selectedItem.DisplayName),
                LangManager.GetString("PresetFormWarningTitle"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                if (PresetsManager.DeletePreset(selectedPresetFileName))
                {
                    LoadPresetsList();
                }
                else
                {
                    MessageBox.Show(
                        LangManager.GetString("PresetFormErrorFailedToDelete"),
                        LangManager.GetString("PresetFormErrorTitle"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            if (presetsListBox.SelectedItem == null)
                return;

            var selectedItem = presetsListBox.SelectedItem as PresetListItem;
            if (selectedItem == null)
                return;

            string selectedPresetFileName = selectedItem.FileName;

            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*";
                saveDialog.FileName = $"{selectedPresetFileName}.json";
                saveDialog.Title = LangManager.GetString("PresetFormDialogExportPreset");

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    if (PresetsManager.ExportPreset(selectedPresetFileName, saveDialog.FileName))
                    {
                        // Do nothing
                    }
                    else
                    {
                        MessageBox.Show(
                            LangManager.GetString("PresetFormErrorFailedToExportPreset"),
                            LangManager.GetString("PresetFormErrorTitle"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
        }

        private void ImportButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openDialog = new OpenFileDialog())
            {
                openDialog.Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*";
                openDialog.Title = LangManager.GetString("PresetFormDialogImportPreset");

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    string fileName = Path.GetFileNameWithoutExtension(openDialog.FileName);

                    if (PresetsManager.ImportPreset(openDialog.FileName, fileName))
                    {
                        LoadPresetsList();
                    }
                    else
                    {
                        MessageBox.Show(
                            LangManager.GetString("PresetFormErrorFailedToImportPreset"),
                            LangManager.GetString("PresetFormErrorTitle"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
