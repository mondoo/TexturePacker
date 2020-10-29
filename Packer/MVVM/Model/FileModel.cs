using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Packer.MVVM.Model
{
    public class FileModel : INotifyPropertyChanged
    {
        private int index;
        private string filePath;

        public int Index
        {
            get => index;

            set
            {
                if (index != value)
                {
                    index = value;
                    NotifyChange("Index");
                }
            }
        }

        public string FilePath
        {
            get => filePath;

            set
            {
                if (filePath != value)
                {
                    filePath = value;
                    NotifyChange("FilePath");
                }
            }
        }

        public string FileName
        {
            get
            {
                string[] split = filePath.Split(Path.DirectorySeparatorChar);
                return split[split.Length - 1];
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyChange(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
