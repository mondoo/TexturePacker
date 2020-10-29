using System;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using Packer.Classes;

using Packer.MVVM.ViewModel;

namespace Packer
{
    public partial class MainWindow : Window
    {
        //private ImageProcessor imageProcessor = new ImageProcessor();
        private ImagePacker imagePacker = new ImagePacker();
        private OpenFileDialog addFileDialog;
        private SaveFileDialog saveFileDialog;
        private FileListViewModel FileListVM = new FileListViewModel();

        private string currentPaddingText;

        public MainWindow()
        {
            InitializeComponent();

            addFileDialog = new OpenFileDialog();
            addFileDialog.RestoreDirectory = true;
            addFileDialog.Multiselect = true;
            addFileDialog.Filter = "Images (*.png;*.bmp;*.jpg;*.gif)|*.png;*.bmp;*.jpg;*.gif";
            addFileDialog.Title = "Add Image";

            saveFileDialog = new SaveFileDialog();
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.Filter = "PNG (*.png)|*.png";

            AddFile.Click += AddFile_Click;
            UpdatePreview.Click += UpdatePreview_Click;
            Save.Click += Save_Click;
            Padding.TextChanged += Padding_TextChanged;

            Sizes.Items.Add(128);
            Sizes.Items.Add(256);
            Sizes.Items.Add(512);
            Sizes.Items.Add(1024);
            Sizes.Items.Add(2048);
            Sizes.Items.Add(4096);
            Sizes.SelectedIndex = 0;

            FileListControl.DataContext = FileListVM;
        }

        private void AddFile_Click(object sender, EventArgs e)
        {
            if (addFileDialog.ShowDialog() == true)
            {
                foreach (string file in addFileDialog.FileNames)
                {
                    FileListVM.AddFile(file);
                }
            }
        }

        private void UpdatePreview_Click(object sender, EventArgs e)
        {
            imagePacker.Reset();
            imagePacker.SetSize(Int32.Parse(Sizes.SelectedItem.ToString()));
            imagePacker.SetPadding(currentPaddingText != null ? Int32.Parse(currentPaddingText) : 0);
            if (FileListVM.Files.Count > 0)
            {
                FileListVM.Files.ToList().ForEach(x =>
                {
                    imagePacker.AddImagePath(x.FilePath);
                });

                imagePacker.PreviewOutput(PreviewImage);
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (FileListVM.Files.Count > 0)
            {
                if (saveFileDialog.ShowDialog() == true)
                {
                    imagePacker.Reset();
                    imagePacker.SetSize(Int32.Parse(Sizes.SelectedItem.ToString()));
                    imagePacker.SetPadding(currentPaddingText != null ? Int32.Parse(currentPaddingText) : 0);
                    FileListVM.Files.ToList().ForEach(x =>
                    {
                        imagePacker.AddImagePath(x.FilePath);
                    });

                    imagePacker.SaveFile(saveFileDialog.FileName);
                }
            }
        }

        private void Padding_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(Padding.Text, "[^0-9]"))
            {
                Padding.Text = currentPaddingText;
            }

            currentPaddingText = Padding.Text;
        }
    }
}
