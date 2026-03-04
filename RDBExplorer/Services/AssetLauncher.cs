using RDBExplorer.Core;
using RDBExplorer.Core.Models;
using RDBExplorer.Forms;
using RDBExplorer.Utils;

namespace RDBExplorer.Services
{
    public static class AssetLauncher
    {
        public static async Task OpenEntry(RDBEntry item, ArchiveExploler explorer)
        {
            if (item == null || explorer == null)
            {
                return;
            }
            Form? existingForm = Application.OpenForms.Cast<Form>()
                .FirstOrDefault(f => f.Tag is uint openedKtId && openedKtId == item.FileKtid);

            if (existingForm != null)
            {
                if (existingForm.WindowState == FormWindowState.Minimized)
                {
                    existingForm.WindowState = FormWindowState.Normal;
                }

                existingForm.BringToFront();
                existingForm.Focus();
                return;
            }

            Form? activeForm = Form.ActiveForm;
            try
            {
                if (activeForm != null)
                {
                    activeForm.Cursor = Cursors.WaitCursor;
                }

                byte[]? entryData = await Task.Run(() => explorer.GetEntryData(item));

                if (entryData == null)
                {
                    MessageBox.Show("Failed to load entry data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                KTFileType fileType = (KTFileType)(item.TypeInfoKtid);
                Form? newForm = null;
                switch (fileType)
                {
                    case KTFileType.TexContext:
                    case KTFileType.StreamingTexContext:
                        newForm = new G1ToolForm(item.Name, entryData, item);
                        break;
                    case KTFileType.BinaryFile:
                        // a some binary file should be a texture, need check it
                        string extention = FileTypeDetector.DetectExtension(entryData);
                        if (extention.StartsWith(".g1t"))
                        {
                            newForm = new G1ToolForm(item.Name, entryData, item);
                        }
                        else
                        {
                            newForm = new AssetViewForm(item, entryData, explorer);
                        }
                        break;
                    case KTFileType.ModelData:
                        string modelExtention = FileTypeDetector.DetectExtension(entryData);
                        if (modelExtention.StartsWith(".g1m"))
                        {
                            newForm = new ModelViewForm(item, entryData);
                        }
                        else
                        {
                            newForm = new AssetViewForm(item, entryData, explorer);
                        }
                        break;
                    default:
                        newForm = new AssetViewForm(item, entryData, explorer);
                        break;
                }

                if (newForm != null)
                {
                    newForm.Tag = item.FileKtid;
                    newForm.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading entry: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (activeForm != null)
                {
                    activeForm.Cursor = Cursors.Default;
                }
            }
        }
    }
}
