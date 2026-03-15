using RDBExplorer.Services;

namespace RDBExplorer.Forms
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            rdbNamesDatabaseTb.Text = SettingsService.Instance.Config.RDBNamesDatabasePath;
            modelAndTexturesDBTb.Text = SettingsService.Instance.Config.ModelsAndTextutesDatabasePath;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SettingsService.Instance.Config.RDBNamesDatabasePath = rdbNamesDatabaseTb.Text;
            SettingsService.Instance.Config.ModelsAndTextutesDatabasePath = modelAndTexturesDBTb.Text;
            SettingsService.Instance.Save();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void selectPathDbBtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select RDB Names Database (CSV)";
                ofd.Filter = "CSV Files|*.csv|All Files|*.*";
                if (File.Exists(rdbNamesDatabaseTb.Text))
                {
                    ofd.InitialDirectory = Path.GetDirectoryName(rdbNamesDatabaseTb.Text);
                    ofd.FileName = Path.GetFileName(rdbNamesDatabaseTb.Text);
                }

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    rdbNamesDatabaseTb.Text = ofd.FileName;
                }
            }
        }

        private void selectModelAndTexturesDBBtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select Models and Textures Database (JSON)";
                ofd.Filter = "JSON Files|*.json|All Files|*.*";
                if (File.Exists(modelAndTexturesDBTb.Text))
                {
                    ofd.InitialDirectory = Path.GetDirectoryName(modelAndTexturesDBTb.Text);
                    ofd.FileName = Path.GetFileName(modelAndTexturesDBTb.Text);
                }

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    modelAndTexturesDBTb.Text = ofd.FileName;
                }
            }
        }
    }
}
