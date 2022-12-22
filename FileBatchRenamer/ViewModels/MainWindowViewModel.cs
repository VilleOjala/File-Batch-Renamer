/*
File Batch Renamer by Ville Ojala
GNU General Public License, version 3.0
https://github.com/VilleOjala/File-Batch-Renamer
*/

using System;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
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

        //TODO: Update the link once the project has a GitHub page.
        private const string HelpURL = "https://github.com/VilleOjala/File-Batch-Renamer";

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
                MatchingMethodChanged();
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(MatchingMethod)));
            }
        }

        public ICommand HelpCommand { get; set; }
        public ICommand AboutCommand { get; set; }
        public ICommand ImportFilesCommand { get; set; }
        public ICommand ImportFolderCommand { get; set; }
        public ICommand ClearSelectedFilesCommand { get; set; }
        public ICommand ClearAllFilesCommand { get; set; }
        public ICommand ImportCSVCommand { get; set; }
        public ICommand ClearCSVCommand { get; set; }
        public ICommand RenameCommand { get; set; }

        private bool canExecuteCommands = false;
        public bool CanExecuteCommands
        {
            get
            {
                return canExecuteCommands;
            }
            set
            {
                if (value == canExecuteCommands)
                    return;

                canExecuteCommands = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(CanExecuteCommands)));
            }
        }

        public MainWindowViewModel(ICSVParser parser, IFileHandler fileHandler, IRenamer renamer)
        {
            this.parser = parser;
            this.parser.ImportStatusChanged += CSVParser_ImportStatusChanged; 
            this.fileHandler = fileHandler;
            this.renamer = renamer;
            RenameItems = new ObservableCollection<RenameItem>();
            HelpCommand = new RelayCommand(OpenHelpURL);
            AboutCommand = new RelayCommand(DisplayApplicationInfo);
            ImportFilesCommand = new RelayCommand(ImportFiles);
            ImportFolderCommand = new RelayCommand(ImportFolder);
            ClearSelectedFilesCommand = new RelayCommand(ClearSelectedFiles);
            ClearAllFilesCommand = new RelayCommand(ClearAllFiles);
            ImportCSVCommand = new RelayCommand(ImportCSV);
            ClearCSVCommand = new RelayCommand(ClearCSV);
            RenameCommand = new RelayCommand(Rename);
            CanExecuteCommands = true;
        }

        ~MainWindowViewModel()
        {
            this.parser.ImportStatusChanged -= CSVParser_ImportStatusChanged;
        }

        private void OpenHelpURL()
        {
            try
            {
                var processStartInfo = new ProcessStartInfo(HelpURL) { UseShellExecute = true };
                Process.Start(processStartInfo);
            }
            catch (Exception e)
            {
                string errorMessage = "An exception (" + e.GetType().ToString() + ") occured when trying to open " + HelpURL + ". " + e.Message;
                MessageBoxDisplayer.DisplayErrorMessageBox(errorMessage);
            }
        }

        private void DisplayApplicationInfo()
        {
            string message = "File Batch Renamer\n\nCopyright 2022, Ville Ojala.";
            MessageBoxDisplayer.DisplayInfoMessageBox(message, "About File Batch Renamer");
        }

        private void ImportFiles()
        {
            if (!CanExecuteCommands)
                return;
            
            CanExecuteCommands = false;
            var dialogue = new OpenFileDialog() { Multiselect = true };      
            var result = dialogue.ShowDialog();

            if (result == true)
            {
                fileHandler.ImportFiles(dialogue.FileNames);
                renamer.UpdateRenameItems(MatchingMethod, parser.ParseData, fileHandler.ImportedFiles);
                UpdateRenameItems();
            }

            CanExecuteCommands = true;
        }

        private void ImportFolder()
        {
            if (!CanExecuteCommands)
                return;

            CanExecuteCommands = false;
            var dialogue = new System.Windows.Forms.FolderBrowserDialog();
            var result = dialogue.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                fileHandler.ImportFolder(dialogue.SelectedPath);
                UpdateRenameItems();
            }

            CanExecuteCommands = true;
        }

        public void ClearSelectedFiles()
        {
            if (!CanExecuteCommands)
                return;

            if (SelectedRenameItems == null || SelectedRenameItems.Count == 0)
            {
                string message = "No files are selected.";
                MessageBoxDisplayer.DisplayInfoMessageBox(message);
                return;
            }

            CanExecuteCommands = false;
            string messageBoxText = "Are you sure you want to clear the selected imported files?"; 

            if (MessageBoxDisplayer.DisplayConfirmationMessageBox(messageBoxText) == MessageBoxResult.Yes)
            {
                var filesToClear = SelectedRenameItems.Select(x => x.FilePath).ToArray();
                fileHandler.ClearFiles(filesToClear);
                UpdateRenameItems();
            }

            CanExecuteCommands = true;
        }

        private void ClearAllFiles()
        {
            if (!CanExecuteCommands)
                return;

            if (fileHandler.ImportedFiles.Count == 0)
            {
                string message = "No files have been imported.";
                MessageBoxDisplayer.DisplayInfoMessageBox(message);
                return;
            }

            CanExecuteCommands = false;
            string messageBoxText = "Are you sure you want to clear all imported files?";
            
            if (MessageBoxDisplayer.DisplayConfirmationMessageBox(messageBoxText) == MessageBoxResult.Yes)
            {
                fileHandler.ClearAllFiles();
                UpdateRenameItems();
            }

            CanExecuteCommands = true;
        }

        private void ImportCSV()
        {
            if (!CanExecuteCommands)
                return;

            CanExecuteCommands = false;

            if (!HasNoImportedCSV)
            {
                string messageBoxText = "Are you sure you want to import a new CSV file? Previously imported CSV file will be cleared."; 
                
                if (MessageBoxDisplayer.DisplayConfirmationMessageBox(messageBoxText) == MessageBoxResult.Yes)
                {
                    parser.ClearParseData();
                }
                else
                {
                    CanExecuteCommands = true;
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
            CanExecuteCommands = true;
        }

        private void ClearCSV()
        {
            if (!CanExecuteCommands)
                return;

            if (HasNoImportedCSV)
            {
                string infoMessage = "No CSV file has been imported.";
                MessageBoxDisplayer.DisplayInfoMessageBox(infoMessage);
                return;
            }

            CanExecuteCommands = false;
            string messageBoxText = "Are you sure you want to clear the imported CSV file?"; 
            
            if (MessageBoxDisplayer.DisplayConfirmationMessageBox(messageBoxText) == MessageBoxResult.Yes)
            {
                parser.ClearParseData();
                UpdateRenameItems();
            }

            CanExecuteCommands = true;
        }

        private void MatchingMethodChanged()
        {
            if (!CanExecuteCommands)
                return;

            CanExecuteCommands = false;
            UpdateRenameItems();
            CanExecuteCommands = true;
        }

        private void Rename()
        {
            if (!CanExecuteCommands)
                return;

            CanExecuteCommands = false;
            string messageBoxText = "Are you sure you want to rename the imported files? " +
                                    "This action cannot be undone and both the imported files and the CSV file " +
                                    "will be automatically cleared once the operation has finished.";

            if (MessageBoxDisplayer.DisplayConfirmationMessageBox(messageBoxText) == MessageBoxResult.Yes)
            {
                renamer.Rename();
                fileHandler.ClearAllFiles();
                parser.ClearParseData();
                UpdateRenameItems();
            }

            CanExecuteCommands = true;
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