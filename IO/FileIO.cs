using Microsoft.Win32;
using System.Diagnostics;
using System.IO;

namespace BTD_Backend.IO
{
    /// <summary>
    /// Contains methods relating to files, such as getting file info, adding and exporting files, etc. 
    /// </summary>
    public class FileIO
    {
        /// <summary>
        /// Browse for a file
        /// </summary>
        /// <param name="title">Title of dialog window. Example: "Open game exe"</param>
        /// <param name="defaultExt">Default extension for dialog window. Example: "exe"</param>
        /// <param name="filter">File extension filter for dialog window. Example: "Exe files (*.exe)|*.exe|All files (*.*)|*.*"</param>
        /// <param name="startDir">Starting directory for dialog window. Example: ""   or   "Environment.CurrentDirectory"</param>
        /// <returns></returns>
        public static string BrowseForFile(string title, string defaultExt, string filter, string startDir, bool multiSelect = false)
        {
            OpenFileDialog fileDiag = new OpenFileDialog();
            fileDiag.Title = title;
            fileDiag.DefaultExt = defaultExt;
            fileDiag.Filter = filter;
            fileDiag.Multiselect = multiSelect;
            fileDiag.InitialDirectory = startDir;

            fileDiag.ShowDialog();
            return fileDiag.FileName;
        }

        /// <summary>
        /// Get file version number for exe
        /// </summary>
        /// <param name="file">Fileinfo you want the version number for</param>
        /// <returns></returns>
        public static string GetFileVersion(FileInfo file) => FileVersionInfo.GetVersionInfo(file.FullName).FileVersion;

        /// <summary>
        /// Get file version number for exe
        /// </summary>
        /// <param name="path">the full path to the file</param>
        /// <returns></returns>
        public static string GetFileVersion(string path)
        {
            if (!File.Exists(path))
            {
                Log.Output("EXE not found! unable to get version for: " + path);
                return "";
            }

            return FileVersionInfo.GetVersionInfo(path).FileVersion;
        }
    }
}
