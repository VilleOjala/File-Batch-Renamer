using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Printing;
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
        public List<RenameItem> SelectedRenameItems { get; set; }

        private readonly ICSVParser parser;
        private readonly IFileHandler fileHandler;
        private readonly IRenamer renamer;

        private bool hasNoImportedCSV = true;
        public bool HasNoImportedCSV
        {
            get
            {
                return hasNoImportedCSV;
            }
            set
            {
                if (hasNoImportedCSV == value)
                    return;

                hasNoImportedCSV = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(HasNoImportedCSV)));
            }
        }

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
        public ICommand ClearSelectedFilesCommand { get; set; }
        public ICommand ClearAllFilesCommand { get; set; }
        public ICommand ImportCSVCommand { get; set; }
        public ICommand ClearCSVCommand { get; set; }
        public ICommand MatchingMethodChangedCommand { get; set; }
        public ICommand RenameCommand { get; set; }

        private bool canExecuteCommands = false;

        public MainWindowViewModel(ICSVParser parser, IFileHandler fileHandler, IRenamer renamer)
        {
            this.parser = parser;
            this.parser.ImportStatusChanged += CSVParser_ImportStatusChanged; // unsubscribe?
            this.fileHandler = fileHandler;
            this.renamer = renamer;
            RenameItems = new ObservableCollection<RenameItem>();
            ImportFilesCommand = new RelayCommand(ImportFiles);
            ImportFolderCommand = new RelayCommand(ImportFolder);
            ClearSelectedFilesCommand = new RelayCommand(ClearSelectedFiles);
            ClearAllFilesCommand = new RelayCommand(ClearAllFiles);
            ImportCSVCommand = new RelayCommand(ImportCSV);
            ClearCSVCommand = new RelayCommand(ClearCSV);
            MatchingMethodChangedCommand = new RelayCommand(MatchingMethodChanged);
            RenameCommand = new RelayCommand(Rename);
            canExecuteCommands = true;
        }

        ~MainWindowViewModel()
        {
            this.parser.ImportStatusChanged -= CSVParser_ImportStatusChanged;
        }

        private void ImportFiles()
        {
            if (!canExecuteCommands)
                return;
            
            canExecuteCommands = false;
            var dialogue = new OpenFileDialog() { Multiselect = true };      
            var result = dialogue.ShowDialog();

            if (result == true)
            {
                fileHandler.ImportFiles(dialogue.FileNames);
                renamer.UpdateRenameItems(MatchingMethod, parser.ParseData, fileHandler.ImportedFiles);
                UpdateRenameItems();
            }

            canExecuteCommands = true;
        }

        private void ImportFolder()
        {
            if (!canExecuteCommands)
                return;

            canExecuteCommands = false;
            var dialogue = new System.Windows.Forms.FolderBrowserDialog();
            var result = dialogue.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                fileHandler.ImportFolder(dialogue.SelectedPath);
                UpdateRenameItems();
            }

            canExecuteCommands = true;
        }

        public void ClearSelectedFiles()
        {
            if (SelectedRenameItems == null || SelectedRenameItems.Count == 0 || !canExecuteCommands)
                return;

            canExecuteCommands = false;
            string messageBoxText = "Are you sure you want to clear the selected imported files?"; 

            if (MessageBoxDisplayer.DisplayConfirmationMessageBox(messageBoxText) == MessageBoxResult.Yes)
            {
                var filesToClear = SelectedRenameItems.Select(x => x.FilePath).ToArray();
                fileHandler.ClearFiles(filesToClear);
                UpdateRenameItems();
            }

            canExecuteCommands = true;
        }

        private void ClearAllFiles()
        {
            if (!canExecuteCommands || fileHandler.ImportedFiles.Count == 0)
                return;

            canExecuteCommands = false;
            string messageBoxText = "Are you sure you want to clear all imported files?";
            
            if (MessageBoxDisplayer.DisplayConfirmationMessageBox(messageBoxText) == MessageBoxResult.Yes)
            {
                fileHandler.ClearAllFiles();
                UpdateRenameItems();
            }

            canExecuteCommands = true;
        }

        private void ImportCSV()
        {
            if (!canExecuteCommands)
                return;

            canExecuteCommands = false;

            if (!HasNoImportedCSV)
            {
                string messageBoxText = "Are you sure you want to import a new CSV file? Previously imported CSV file will be cleared."; 
                
                if (MessageBoxDisplayer.DisplayConfirmationMessageBox(messageBoxText) == MessageBoxResult.Yes)
                {
                    parser.ClearParseData();
                }
                else
                {
                    canExecuteCommands = true;
                    return;
                }
            }

            string filter = "CSV files (*.csv)|*.csv";
            var dialogue = new OpenFileDialog { Filter = filter };
            var dialogueResult = dialogue.ShowDialog();

            if (dialogueResult == true)
            {
                if (!parser.ParseCSV(dialogue.FileName, out string errorMessage))
                {
                    MessageBoxDisplayer.DisplayErrorMessageBox(errorMessage);
                }
            }

            UpdateRenameItems();
            canExecuteCommands = true;
        }

        private void ClearCSV()
        {
            if (!canExecuteCommands)
                return;

            if (hasNoImportedCSV)
            {
                string infoMessage = "No CSV file has been imported.";
                MessageBoxDisplayer.DisplayInfoMessageBox(infoMessage);
                return;
            }

            canExecuteCommands = false;
            string messageBoxText = "Are you sure you want to clear the imported CSV file?"; 
            
            if (MessageBoxDisplayer.DisplayConfirmationMessageBox(messageBoxText) == MessageBoxResult.Yes)
            {
                parser.ClearParseData();
                UpdateRenameItems();
            }

            canExecuteCommands = true;
        }

        //TODO: Button needs to be disabled since there is a state?
        private void MatchingMethodChanged()
        {
            if (!canExecuteCommands)
                return;

            canExecuteCommands = false;
            UpdateRenameItems();
            canExecuteCommands = true;
        }

        private void Rename()
        {
            if (!canExecuteCommands)
                return;

            canExecuteCommands = false;
            string messageBoxText = "Are you sure you want to rename the imported files? " +
                                    "This action cannot be undone and both the imported files and the CSV file " +
                                    "will be automatically cleared once the operation has finished.";

            if (MessageBoxDisplayer.DisplayConfirmationMessageBox(messageBoxText) == MessageBoxResult.Yes)
            {
                renamer.Rename();
                ClearAllFiles();
                ClearCSV();
                UpdateRenameItems();
            }

            canExecuteCommands = true;
        }

        private void UpdateRenameItems()
        {
            renamer.UpdateRenameItems(MatchingMethod, parser.ParseData, fileHandler.ImportedFiles);
            RenameItems.Clear();

            foreach (var renameItem in renamer.RenameItems)
            {
                RenameItems.Add(renameItem);
            }
        }

        private void CSVParser_ImportStatusChanged(bool status)
        {
            HasNoImportedCSV = !status;
        }
    }
}