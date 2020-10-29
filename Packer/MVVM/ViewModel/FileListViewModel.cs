using Packer.MVVM.Model;
using System.Collections.ObjectModel;
using Packer.Classes;
using System.Diagnostics;
using System.Windows.Input;

namespace Packer.MVVM.ViewModel
{
    class FileListViewModel
    {
        private ICommand removeItem;
        private ICommand moveUp;
        private ICommand moveDown;
        public ObservableCollection<FileModel> Files { get; set; }

        public FileListViewModel()
        {
            Files = new ObservableCollection<FileModel>();
        }

        public void AddFile(string filePath)
        {
            Files.Add(new FileModel { 
                Index = Files.Count,
                FilePath = filePath
            });
        }

        public void RemoveAll()
        {
            for (int i = Files.Count - 1; i >= 0; i--)
            {
                if (Files[i] != null)
                {
                    Files.RemoveAt(i);
                }
            }
        }
        
        public ICommand RemoveItem
        {
            get { return removeItem ?? (removeItem = new RelayCommand(param => RemoveItemCommand((int)param))); }
        }

        public ICommand MoveUp
        {
            get { return moveUp ?? (moveUp = new RelayCommand(param => MoveUpCommand((int)param))); }
        }

        public ICommand MoveDown
        {
            get { return moveDown ?? (moveDown = new RelayCommand(param => MoveDownCommand((int)param))); }
        }

        private void RemoveItemCommand(int item)
        {
            Files.RemoveAt(item);
        }

        private void MoveUpCommand(int item)
        {
            if (item > 0)
            {
                Files.Move(item, item - 1);
            }
        }

        private void MoveDownCommand(int item)
        {
            if (Files.Count < item)
            {
                Files.Move(item, item - 1);
            }
        }
    }
}
