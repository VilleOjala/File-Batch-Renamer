
namespace FileBatchRenamer
{
    public class RenameItem
    {
        public string FilePath { get; set; }
        public string SearchName { get; set; }
        public string NewName { get; set; }

        public RenameItem()
        {
            FilePath = string.Empty;
            SearchName = string.Empty;
            NewName = string.Empty;
        }

        public RenameItem(string filePath)
        {
            FilePath = filePath;
            SearchName = string.Empty;
            NewName = string.Empty;
        }

        public RenameItem(string filePath, string searchName, string newName)
        {
            FilePath = filePath;
            SearchName = searchName;
            NewName = newName;
        }
    }
}