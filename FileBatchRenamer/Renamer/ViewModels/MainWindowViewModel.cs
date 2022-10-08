using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileBatchRenamer
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public ObservableCollection<RenameItem> RenameItems { get; set; }

        public ICommand ImportFilesCommand { get; set; }
        public ICommand RemoveFileCommand { get; set; }
        public ICommand ClearFilesCommand { get; set; }
        public ICommand ImportCSVCommand { get; set; }
        public ICommand ClearCSVCommand { get; set; }
        public ICommand MatchingMethodChangedCommand { get; set; }
        public ICommand RenameCommand { get; set; }

        public MainWindowViewModel()
        {
            RenameItems = new ObservableCollection<RenameItem>();
            ImportFilesCommand = new RelayCommand(ImportFiles);
            RemoveFileCommand = new RelayCommand(RemoveFiles);
            ClearFilesCommand = new RelayCommand(ClearFiles);
            ImportCSVCommand = new RelayCommand(ImportCSV);
            ClearCSVCommand = new RelayCommand(ClearCSV);
            MatchingMethodChangedCommand = new RelayCommand(MatchingMethodChanged);
            RenameCommand = new RelayCommand(Rename);
        }

        private void ImportFiles()
        {

        }

        //TODO: Needs a system to  track which file is currently selected
        private void RemoveFiles()
        {

        }

        private void ClearFiles()
        {
            FileHandler.ClearFiles();
            Renamer.UpdateRenameItems();
            RenameItems = new ObservableCollection<RenameItem>(Renamer.RenameItems);
        }

        private void ImportCSV()
        {

        }

        private void ClearCSV()
        {
            CSVParser.ClearParseData();
            Renamer.UpdateRenameItems();
            RenameItems = new ObservableCollection<RenameItem>(Renamer.RenameItems);
        }

        private void MatchingMethodChanged()
        {

        }

        private void Rename()
        {

        }
    }
}
