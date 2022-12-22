# File-Batch-Renamer

A batch renaming tool with a basic GUI (WPF, .NET 6.0). The algorithm operates with a user-provided two-column CSV file in which the data of the first column is used for searching against the files imported into the tool and the second column provides the new name for a file for which a match was found.  

The intended use case for this tool is in the context of a commonly occuring game audio voiceover pipeline in which each dialogue line in the game’s dialogue system has an (often compex) unique ID which can be used to retrieve the corresponding voiceover file by having the file name match the ID exactly. However, during voiceover recording sessions each recorded file is usually given a much simpler temporary unique name by the recordist, since manual typing of long and complex IDs would obviously be way too time-consuming and error-prone. Simplified names are also added to a spreadsheet containing the actual unique IDs in an adjacent column. Consequently, a robust and time-efficient method is needed for replacing the temporary file names with the correct technical IDs using the spreadsheet data, as games routinely contain tens of thousands of lines of dialogue.

[Download build](https://drive.google.com/file/d/1fZIX4lORc2bFONvnGHx9fyHifZUyQSxu/view?usp=share_link)

# How to use

![ExampleCSV](https://user-images.githubusercontent.com/69209034/209230354-2a8d1ecf-d876-4ff1-892b-c70822c97ffb.png)

![BatchRenamerExample](https://user-images.githubusercontent.com/69209034/209230183-27efec48-e55c-4cb8-a806-1a6b2ba003a1.png)

1. Use **Import Files** or **Import Folder** to add the files to be renamed. Use **Clear Selected Files** and **Clear All Files** to remove individual or all imported files, respectively.

2. Use **Import CSV** to provide the data needed in the renaming process. Only one CSV file can be imported at a time; use **Clear CSV** to remove the currently imported file. The **Import CSV**-button is inactive when a CSV file has already been imported. Only the rows of the first two columns of the imported CSV file are parsed and used for file matching (1st column) and renaming (2nd column). The first column cannot contain any duplicate elements (encountered duplicates will fail the CSV import process). The second column can contain duplicates but one should then be wary of having the original files about to be identically renamed located in the same folder; the renaming algorithm will not overwrite existing files, nor will it do any automatic file name appending when identical file paths are encountered. Furthermore, renaming will always preserve the original file format, and thus file extensions should not be included in the provided new names.

3. Use the **Matching Method** dropdown menu to determine how the match names (1st column of the CSV) will be compared against the imported files. The options are *File Name*, *File Name Without Extension* and *Full Path*.

4. After importing the files to be renamed, adding a valid CSV file and selecting the appropriate matching method, observe the data window to make sure that each imported file has both a "Match Name" and a "New Name" assigned for it. Any empty fields will produce errors during the renaming process.  

5. Use **Rename** to start the renaming process. Be aware that this action cannot be undone and all the imported files and the CSV file will be automatically cleared once the operation has finished. 

6. See the generated log file for info regarding possible errors encountered during the renaming proces. A separate log file (.txt) is created for each overall renaming operation and the starting DateTime info is included in the log file name. The folder containing the logs (*C:\Users\<username>\AppData\Local\FileBatchRenamer*) is opened automatically after the renaming has finished.        
