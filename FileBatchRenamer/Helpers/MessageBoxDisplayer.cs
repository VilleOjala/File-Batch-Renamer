using System.Windows;

namespace FileBatchRenamer
{
    public static class MessageBoxDisplayer
    {
        public static MessageBoxResult DisplayInfoMessageBox(string messageBoxText, string caption = "Info")
        {
            var button = MessageBoxButton.OK;
            var icon = MessageBoxImage.Information;
            var defaultResult = MessageBoxResult.OK;
            return MessageBox.Show(messageBoxText, caption, button, icon, defaultResult);
        }

        public static MessageBoxResult DisplayConfirmationMessageBox(string messageBoxText, string caption = "Confirm")
        {
            var button = MessageBoxButton.YesNo;
            var icon = MessageBoxImage.Exclamation;
            var defaultResult = MessageBoxResult.No;
            return MessageBox.Show(messageBoxText, caption, button, icon, defaultResult);
        }

        public static MessageBoxResult DisplayErrorMessageBox(string messageBoxText, string caption = "Error")
        { 
            var button = MessageBoxButton.OK;
            var icon = MessageBoxImage.Error;
            var defaultResult = MessageBoxResult.OK;
            return MessageBox.Show(messageBoxText, caption, button, icon, defaultResult);
        }
    }
}
