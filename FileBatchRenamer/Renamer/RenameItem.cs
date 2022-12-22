/*
File Batch Renamer by Ville Ojala
GNU General Public License, version 3.0
https://github.com/VilleOjala/File-Batch-Renamer
*/

namespace FileBatchRenamer
{
    public class RenameItem 
    {       
        public string FilePath { get; set; }
        public string MatchName { get; set; }
        public string NewName { get; set; }
        
        public RenameItem()
        {
            FilePath = string.Empty;
            MatchName = string.Empty;
            NewName = string.Empty;
        }

        public RenameItem(string filePath)
        {
            FilePath = filePath;
            MatchName = string.Empty;
            NewName = string.Empty;
        }

        public RenameItem(string filePath, string searchName, string newName)
        {
            FilePath = filePath;
            MatchName = searchName;
            NewName = newName;
        }
    }
}