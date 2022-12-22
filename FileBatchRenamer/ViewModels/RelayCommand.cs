/*
File Batch Renamer by Ville Ojala
Apache License, Version 2.0
https://github.com/VilleOjala/File-Batch-Renamer
*/

using System;
using System.Windows.Input;

namespace FileBatchRenamer
{
    public class RelayCommand : ICommand
    {
        private Action mAction;
        public event EventHandler CanExecuteChanged = (sender, e) => { }; 

        public RelayCommand(Action action)
        {
            mAction = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            mAction?.Invoke();
        }
    }
}