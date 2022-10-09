using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace FileBatchRenamer
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public ObservableCollection<RenameItem> RenameItems { get; set; }

        private MatchingMethod matchingMethod = MatchingMethod.FileNameWithoutExtension;
        public MatchingMethod MatchingMethod
        {
            get
            {
                return matchingMethod;
            }
            set
            {
                if (value == matchingMethod)
                    return;

                matchingMethod = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(MatchingMethod)));
            }
        }

        public ICommand ImportFilesCommand { get; set; }
        public ICommand ImportFolderCommand { get; set; }
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
            ImportFolderCommand = new RelayCommand(ImportFolder);
            RemoveFileCommand = new RelayCommand(RemoveFiles);
            ClearFilesCommand = new RelayCommand(ClearFiles);
            ImportCSVCommand = new RelayCommand(ImportCSV);
            ClearCSVCommand = new RelayCommand(ClearCSV);
            MatchingMethodChangedCommand = new RelayCommand(MatchingMethodChanged);
            RenameCommand = new RelayCommand(Rename);

            //ImportFolder();
           // ImportCSV();
        }

        private void ImportFiles()
        {
            var dialogue = new OpenFileDialog() { Multiselect = true };      
            var result = dialogue.ShowDialog();

            if (result == true)
            {
                FileHandler.ImportFiles(dialogue.FileNames);

                //TODO: Update the GUI-shown list of imported files

                Renamer.UpdateRenameItems(MatchingMethod);
                this.RenameItems = new ObservableCollection<RenameItem>(Renamer.RenameItems);
            }
        }

        private void ImportFolder()
        {
            var dialogue = new System.Windows.Forms.FolderBrowserDialog();
            var result = dialogue.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                FileHandler.ImportFolder(dialogue.SelectedPath);

                //TODO: Update the GUI-shown list of imported files

                Renamer.UpdateRenameItems(MatchingMethod);
                this.RenameItems = new ObservableCollection<RenameItem>(Renamer.RenameItems);
            }
        }

        //TODO: Needs a system to  track which file is currently selected
        private void RemoveFiles()
        {

        }

        private void ClearFiles()
        {
            FileHandler.ClearFiles();
            Renamer.UpdateRenameItems(MatchingMethod);
            RenameItems = new ObservableCollection<RenameItem>(Renamer.RenameItems);
        }

        private void ImportCSV()
        {
            if (CSVParser.IsImported)
            {
                string messageBoxText = "Placeholder"; //TODO
                string caption = "Placeholder"; //TODO
                var button = MessageBoxButton.OKCancel;
                var icon = MessageBoxImage.Exclamation;
                var defaultResult = MessageBoxResult.Cancel;
                var messageBoxResult = MessageBox.Show(messageBoxText, caption, button, icon, defaultResult);

                if (messageBoxResult == MessageBoxResult.Cancel)
                    return;

                CSVParser.ClearParseData();
            }

            string filter = "CSV files (*.csv)|*.csv";
            var dialogue = new OpenFileDialog { Filter = filter };
            var dialogueResult = dialogue.ShowDialog();

            if (dialogueResult == true)
            {
                if (!CSVParser.ParseCSV(dialogue.FileName, out string errorMessage))
                {
                    string messageBoxText = errorMessage;
                    string caption = "Placeholder"; //TODO
                    var button = MessageBoxButton.OK;
                    var icon = MessageBoxImage.Error;
                    var defaultResult = MessageBoxResult.OK;
                    MessageBox.Show(messageBoxText, caption, button, icon, defaultResult);
                }
            }

            Renamer.UpdateRenameItems(MatchingMethod);
            RenameItems = new ObservableCollection<RenameItem>(Renamer.RenameItems);
        }

        private void ClearCSV()
        {
            CSVParser.ClearParseData();
            Renamer.UpdateRenameItems(MatchingMethod);
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
