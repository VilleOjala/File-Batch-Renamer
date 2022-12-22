/*
File Batch Renamer by Ville Ojala
GNU General Public License, version 3.0
https://github.com/VilleOjala/File-Batch-Renamer
*/

using System.ComponentModel;

namespace FileBatchRenamer
{
    public enum MatchingMethod
    {
        [Description("File Name")]
        FileName,
        [Description("File Name Without Extension")]
        FileNameWithoutExtension,
        [Description("File Path")]
        FilePath
    }
}
