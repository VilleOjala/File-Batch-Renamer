﻿using Microsoft.Win32;
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
        public List<RenameItem> SelectedRenameItems { get; set; }

        private readonly ICSVParser parser;
        private readonly IFileHandler fileHandler;
        private readonly IRenamer renamer;

        //TODO: Maybe replace with a boolean flipping value converter?
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
        }

        ~MainWindowViewModel()
        {
            this.parser.ImportStatusChanged -= CSVParser_ImportStatusChanged;
        }

        private void ImportFiles()
        {
            Trace.WriteLine(matchingMethod); //!!!!!
            var dialogue = new OpenFileDialog() { Multiselect = true };      
            var result = dialogue.ShowDialog();

            if (result == true)
            {
                fileHandler.ImportFiles(dialogue.FileNames);
                renamer.UpdateRenameItems(MatchingMethod, parser.ParseData, fileHandler.ImportedFiles);
                UpdateRenameItems();
            }
        }

        private void ImportFolder()
        {
            var dialogue = new System.Windows.Forms.FolderBrowserDialog();
            var result = dialogue.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                fileHandler.ImportFolder(dialogue.SelectedPath);
                UpdateRenameItems();
            }
        }

        public void ClearSelectedFiles()
        {
            //TODO: 'Are you sure' -prompt
            var filesToClear = SelectedRenameItems.Select(x => x.FilePath).ToArray();
            fileHandler.ClearFiles(filesToClear);
            UpdateRenameItems();
        }

        private void ClearAllFiles()
        {
            //TODO: 'Are you sure' -prompt
            fileHandler.ClearAllFiles();
            UpdateRenameItems();
        }

        private void ImportCSV()
        {
            if (!HasNoImportedCSV)
            {
                string messageBoxText = "Placeholder"; //TODO
                string caption = "Placeholder"; //TODO
                var button = MessageBoxButton.OKCancel;
                var icon = MessageBoxImage.Exclamation;
                var defaultResult = MessageBoxResult.Cancel;
                var messageBoxResult = MessageBox.Show(messageBoxText, caption, button, icon, defaultResult);

                if (messageBoxResult == MessageBoxResult.Cancel)
                    return;

                parser.ClearParseData();
            }

            string filter = "CSV files (*.csv)|*.csv";
            var dialogue = new OpenFileDialog { Filter = filter };
            var dialogueResult = dialogue.ShowDialog();

            if (dialogueResult == true)
            {
                if (!parser.ParseCSV(dialogue.FileName, out string errorMessage))
                {
                    string messageBoxText = errorMessage;
                    string caption = "Placeholder"; //TODO
                    var button = MessageBoxButton.OK;
                    var icon = MessageBoxImage.Error;
                    var defaultResult = MessageBoxResult.OK;
                    MessageBox.Show(messageBoxText, caption, button, icon, defaultResult);
                }
            }

            UpdateRenameItems();
        }

        private void ClearCSV()
        {
            //TODO: 'Are you sure' -prompt
            parser.ClearParseData();
            UpdateRenameItems();
        }

        private void MatchingMethodChanged()
        {
            UpdateRenameItems();
        }

        private void Rename()
        {
            //TODO: 'Are you sure' -prompt
            renamer.Rename();
            ClearAllFiles();
            ClearCSV();
            UpdateRenameItems();
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