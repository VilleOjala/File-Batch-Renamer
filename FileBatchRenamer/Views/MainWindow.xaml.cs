using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FileBatchRenamer
{
    public partial class MainWindow : Window
    {
        public MainWindow(ICSVParser parser, IFileHandler fileHandler, IRenamer renamer)
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel(parser, fileHandler, renamer);
        }

        public void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            var dataGrid = sender as DataGrid;
            var selectedItems = dataGrid.SelectedItems.OfType<RenameItem>().ToList();
            var viewModel = this.DataContext as MainWindowViewModel;
            viewModel.SelectedRenameItems = new List<RenameItem>(selectedItems);
        }
    }
}