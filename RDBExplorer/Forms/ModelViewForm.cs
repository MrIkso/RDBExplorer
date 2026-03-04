using Metanoia.Rendering;
using RDBExplorer.Core;
using RDBExplorer.Core.Formats.G1M;
using RDBExplorer.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RDBExplorer.Forms
{
    public partial class ModelViewForm : Form
    {
        public ModelViewer ModelViewer;
        private string _currentFileName;
        private byte[] _rawData;

        public ModelViewForm()
        {
            InitializeComponent();
            ModelViewer = new ModelViewer();
            ModelViewer.Dock = DockStyle.Fill;
            this.Controls.Add(ModelViewer);
        }

        public ModelViewForm(RDBEntry entry, byte[] data) : this()
        {
            _currentFileName = entry.Name;
            _rawData = data;

            ShowModel(data);
            UpdateTitle();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "Model files (*.g1m)|*.g1m";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string seletedFile = ofd.FileName;
                _currentFileName = seletedFile;
                G1MImporter g1MImporter = new G1MImporter();
                g1MImporter.Open(seletedFile);

                ModelViewer.SetModel(g1MImporter.ToGenericModel());
                UpdateTitle();
            }
        }

        private void ShowModel(byte[] data)
        {
            G1MImporter g1MImporter = new G1MImporter();
            g1MImporter.Open(data);

            ModelViewer.SetModel(g1MImporter.ToGenericModel());
        }

        private void UpdateTitle()
        {
            if (_currentFileName != null)
            {
                this.Text = $"Model View - {_currentFileName}";
            }
            else
            {
                this.Text = "Model View";
            }
        }
    }
}
