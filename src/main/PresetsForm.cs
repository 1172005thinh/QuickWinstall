using System;
using System.IO;
using System.Windows.Forms;
using QuickWinstall.Lib;

namespace QuickWinstall
{
    public partial class PresetsForm : Form, ILangRefreshable
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

        // State variables

        public string SelectedPresetName { get; private set; }
        public PresetsManager.PresetData SelectedPreset { get; private set; }

        #region PresetsForm
        public PresetsForm()
        {
            InitializeComponent();
            LoadPresetsList();

            LangHelper.RegisterForm(this);
            RefreshLang();

            ThemeManager.SetForm(this);
        }

        #region LoadPresetsList
        private void LoadPresetsList()
        {
            try
            {
                presetsListBox.Items.Clear();
                var presets = PresetsManager.GetAvailablePresets();

                foreach (var preset in presets)
                {
                    // Try to get the preset info to display the Name
                    var presetInfo = PresetsManager.GetPresetInfo(preset);
                    
                    var listItem = new PresetListItem
                    {
                        FileName = preset
                    };

                    if (presetInfo != null && (!string.IsNullOrWhiteSpace(presetInfo.Name)))
                    {
                        // Use Name from metadata
                        listItem.DisplayName = presetInfo.Name;
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
                    string.Format(LangManager.GetString("PresetsForm_Error_LoadingPresets", "Error loading presets: {0}"), ex.Message),
                    LangManager.GetString("PresetsForm_Error_Title", "Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
        #endregion

        #region PresetsListBox_SelectedIndexChanged
        private void PresetsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (presetsListBox.SelectedItem == null)
            {
                applyBtn.Enabled = false;
                removeBtn.Enabled = false;
                exportBtn.Enabled = false;
                presetInfoTextBox.Text = "";
                return;
            }

            var selectedItem = presetsListBox.SelectedItem as PresetListItem;
            if (selectedItem == null)
                return;

            string selectedPresetFileName = selectedItem.FileName;
            applyBtn.Enabled = true;
            removeBtn.Enabled = true;
            exportBtn.Enabled = true;

            // Load preset info
            var presetInfo = PresetsManager.GetPresetInfo(selectedPresetFileName);
            if (presetInfo != null)
            {
                presetInfoTextBox.Text =
                    $"{LangManager.GetString("PresetsForm_Info_Name", "Name")}: {presetInfo.Name}\r\n" +
                    $"{LangManager.GetString("PresetsForm_Info_Version", "Version")}: {presetInfo.Version}\r\n" +
                    $"{LangManager.GetString("PresetsForm_Info_Author", "Author")}: {presetInfo.Author}\r\n\r\n" +
                    $"{LangManager.GetString("PresetsForm_Info_Description", "Description")}: {presetInfo.Description}";
            }
            else
            {
                presetInfoTextBox.Text = LangManager.GetString("PresetsForm_Info_NoData", "");
            }
        }
        #endregion

        #region AddBtn_Click
        private void AddBtn_Click(object sender, EventArgs e)
        {
            // Prompt for preset name
            using (var inputForm = new Form())
            {
                inputForm.Text = LangManager.GetString("PresetsForm_Title_AddPreset", "Add Preset");
                inputForm.Size = new Size(400, 150);
                inputForm.StartPosition = FormStartPosition.CenterParent;
                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                inputForm.MaximizeBox = false;
                inputForm.MinimizeBox = false;

                Label label = new Label()
                {
                    Text = LangManager.GetString("PresetsForm_Label_PresetName", "Preset Name:"),
                    Location = new Point(20, 20),
                    AutoSize = true
                };

                TextBox textBox = new TextBox()
                {
                    Location = new Point(20, 45),
                    Size = new Size(340, 20)
                };

                Button okButton = new Button()
                {
                    Text = LangManager.GetString("PresetsForm_Button_OK", "OK"),
                    DialogResult = DialogResult.OK,
                    Location = new Point(205, 75),
                    Size = new Size(75, 25)
                };

                Button cancelButton = new Button()
                {
                    Text = LangManager.GetString("PresetsForm_Button_Cancel", "Cancel"),
                    DialogResult = DialogResult.Cancel,
                    Location = new Point(285, 75),
                    Size = new Size(75, 25)
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
                            LangManager.GetString("PresetsForm_Error_InvalidPresetName", "Please enter a valid preset name."),
                            LangManager.GetString("PresetsForm_Error_Title", "Error"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return;
                    }

                    // Check if preset already exists
                    if (PresetsManager.PresetExists(presetName))
                    {
                        var result = MessageBox.Show(
                            LangManager.GetString("PresetsForm_Warning_Overwrite", "Preset already exists. Overwrite?"),
                            LangManager.GetString("PresetsForm_Warning_Title", "Warning"),
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning
                        );

                        if (result != DialogResult.Yes)
                            return;
                    }

                    // This would need to collect current configuration from MainForm
                    // For now, show a message that this feature requires MainForm integration
                    MessageBox.Show(
                        LangManager.GetString("PresetsForm_Info_NotImplemented", "Save preset functionality requires integration with MainForm to collect current configuration."),
                        LangManager.GetString("PresetsForm_Info_Title", "Not Implemented"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
        }
        #endregion

        #region RemoveBtn_Click
        private void RemoveBtn_Click(object sender, EventArgs e)
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
                    LangManager.GetString("PresetsForm_Error_CannotRemoveDefault", "Cannot remove default preset."),
                    LangManager.GetString("PresetsForm_Error_Title", "Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            var result = MessageBox.Show(
                string.Format(LangManager.GetString("PresetsForm_Warning_RemovePreset", "Remove preset '{0}'?"), selectedItem.DisplayName),
                LangManager.GetString("PresetsForm_Warning_Title", "Warning"),
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
                        LangManager.GetString("PresetsForm_Error_FailedToRemove", "Failed to remove preset."),
                        LangManager.GetString("PresetsForm_Error_Title", "Error"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }
        #endregion

        #region ExportBtn_Click
        private void ExportBtn_Click(object sender, EventArgs e)
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
                saveDialog.Title = LangManager.GetString("PresetsForm_Dialog_ExportPreset", "Export Preset");

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    if (PresetsManager.ExportPreset(selectedPresetFileName, saveDialog.FileName))
                    {
                        MessageBox.Show(
                            LangManager.GetString("PresetsForm_Success_ExportedPreset", "Preset exported successfully."),
                            LangManager.GetString("PresetsForm_Success_Title", "Success"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                    else
                    {
                        MessageBox.Show(
                            LangManager.GetString("PresetsForm_Error_FailedToExport", "Failed to export preset."),
                            LangManager.GetString("PresetsForm_Error_Title", "Error"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
        }
        #endregion

        #region ImportBtn_Click
        private void ImportBtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openDialog = new OpenFileDialog())
            {
                openDialog.Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*";
                openDialog.Title = LangManager.GetString("PresetsForm_Dialog_ImportPreset", "Import Preset");

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    string fileName = Path.GetFileNameWithoutExtension(openDialog.FileName);

                    if (PresetsManager.ImportPreset(openDialog.FileName, fileName))
                    {
                        LoadPresetsList();
                        MessageBox.Show(
                            LangManager.GetString("PresetsForm_Success_ImportedPreset", "Preset imported successfully."),
                            LangManager.GetString("PresetsForm_Success_Title", "Success"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                    else
                    {
                        MessageBox.Show(
                            LangManager.GetString("PresetsForm_Error_FailedToImport", "Failed to import preset."),
                            LangManager.GetString("PresetsForm_Error_Title", "Error"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
        }
        #endregion

        #region ApplyBtn_Click
        private void ApplyBtn_Click(object sender, EventArgs e)
        {
            if (presetsListBox.SelectedItem == null)
                return;

            var selectedItem = presetsListBox.SelectedItem as PresetListItem;
            if (selectedItem == null)
                return;

            string selectedPresetFileName = selectedItem.FileName;
            var preset = PresetsManager.LoadPresetData(selectedPresetFileName);

            if (preset == null)
            {
                MessageBox.Show(
                    string.Format(LangManager.GetString("PresetsForm_Error_FailedToApply", "Failed to apply preset '{0}'."), selectedItem.DisplayName),
                    LangManager.GetString("PresetsForm_Error_Title", "Error"),
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
        #endregion

        #region CancelBtn_Click
        private void CancelBtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        #endregion

        #region RefreshLang
        public void RefreshLang()
        {
            try
            {

                this.Text = LangManager.GetString("PresetsForm_Title", "Presets");

                if (presetsListLabel != null) presetsListLabel.Text = LangManager.GetString("PresetsForm_Label_PresetsList", "Available Presets:");
                if (presetInfoLabel != null) presetInfoLabel.Text = LangManager.GetString("PresetsForm_Label_PresetInfo", "Preset Information:");
                if (addBtn != null) addBtn.Text = LangManager.GetString("PresetsForm_Button_Add", "Add");
                if (removeBtn != null) removeBtn.Text = LangManager.GetString("PresetsForm_RemoveButton", "Remove");
                if (exportBtn != null) exportBtn.Text = LangManager.GetString("PresetsForm_ExportButton", "Export");
                if (importBtn != null) importBtn.Text = LangManager.GetString("PresetsForm_ImportButton", "Import");
                if (applyBtn != null) applyBtn.Text = LangManager.GetString("PresetsForm_ApplyButton", "Apply");
                if (cancelBtn != null) cancelBtn.Text = LangManager.GetString("PresetsForm_CancelButton", "Cancel");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PresetsForm: Error refreshing language: {ex.Message}");
            }
        }
        #endregion
    }
    #endregion
}
