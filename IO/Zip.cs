using BTD_Backend.Game.Jet_Files;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace BTD_Backend.IO
{
    /// <summary>
    /// Custom zip class used like a wrapper around ionicZip's ZipFile. Contains useful methods for managing zip files
    /// </summary>
    public class Zip
    {
        #region Properties
        /// <summary>
        /// The path to the existing zip file
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The password to the existing zip file, if there is one
        /// </summary>
        private string password;

        public string Password
        {
            get { return password; }
            set 
            { 
                password = value;
                if (Archive != null)
                    Archive.Password = value;
            }
        }



        /// <summary>
        /// The ionicZip ZipFile instance created and managed by this class
        /// </summary>
        public ZipFile Archive { get; set; }
        #endregion


        #region Constructors
        /// <summary>
        /// Custom Zip class wrapped around an ionicZip ZipFile. Contains useful methods related to Zip stuff
        /// </summary>
        public Zip()
        {
            this.Archive = new ZipFile();
            this.Archive.Encryption = EncryptionAlgorithm.PkzipWeak;
            this.Archive.CompressionLevel = Ionic.Zlib.CompressionLevel.BestSpeed;
        }

        /// <summary>
        /// Custom Zip class wrapped around an ionicZip ZipFile. Contains useful methods related to Zip stuff
        /// </summary>
        /// <param name="path">The path to the existing zip file</param>
        /// <param name="password">The password to the zip file, if there is one</param>
        public Zip(string path, string password = "")
        {
            this.Path = path;
            this.Password = password;

            this.Archive = new ZipFile(path);
            this.Archive.Encryption = EncryptionAlgorithm.PkzipWeak;
            this.Archive.CompressionLevel = Ionic.Zlib.CompressionLevel.BestSpeed;
            this.Archive.Password = password;
        }
        #endregion


        /// <summary>
        /// Extract the Archive associated with this class.
        /// </summary>
        /// <param name="dest">The extract destination</param>
        /// <param name="overwrite">How to handle existing files</param>
        public void Extract(string dest, ExtractExistingFileAction overwrite = ExtractExistingFileAction.OverwriteSilently)
        {
            if (IsPasswordCorrect())
                this.Archive.ExtractAll(dest, overwrite);
            else
                Log.Output("ERROR! Tried to extract \"" + ToString() + "\" and failed. Can't extract the file because the password is incorrect");
        }

        public enum EntryType
        {
            All,
            Files,
            Directories
        }

        /// <summary>
        /// Get all of the files in a ZipFile while preserving their original file structure. 
        /// Uses the Archive assosiated with the custom Zip object. Needs to be called on seperate thread
        /// </summary>
        /// <returns>A list of files contained within the ZipFile, with their original path preserved</returns>
        public List<string> GetEntries(EntryType type, string baseDirectory = "", SearchOption searchOption = SearchOption.TopDirectoryOnly) => GetEntries(Archive, type, baseDirectory, searchOption);

        /// <summary>
        /// Get all of the files in a ZipFile while preserving their original file structure. 
        /// Needs to be called on seperate thread
        /// </summary>
        /// <param name="zipFile">The ZipFile you want to get files for</param>
        /// <returns>A list of files contained within the ZipFile, with their original path preserved</returns>
        public static List<string> GetEntries(ZipFile zipFile, EntryType entryType, string baseDirectory = "", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            if (String.IsNullOrEmpty(baseDirectory))
                baseDirectory = "Assets/";

            List<string> entries = new List<string>();
            foreach (var file in zipFile.Entries)
            {
                if (entryType == EntryType.Directories)
                    if (!file.IsDirectory)
                        continue;

                if (entryType == EntryType.Files)
                    if (file.IsDirectory)
                        continue;

                if (!file.FileName.Contains(baseDirectory))
                    continue;

                string[] folderNameSplit = file.FileName.Split('/');
                if (folderNameSplit[folderNameSplit.Length - 2].ToLower().Contains(baseDirectory.ToLower()))
                    baseDirectory = file.FileName;

                if (searchOption == SearchOption.TopDirectoryOnly)
                {
                    string[] baseSplit = baseDirectory.Split('/');
                    string[] currentSplit = file.FileName.Split('/');

                    if ((baseSplit.Length + 1) < currentSplit.Length)
                        continue;
                }

                entries.Add(file.FileName);
            }
            return entries;
        }

        /// <summary>
        /// Get all directories in the ZipFile
        /// </summary>
        /// <param name="directory">an optional base directory that all returned directories need to share</param>
        /// <returns>A list of directories in the Archive of the Zip</returns>
        //public List<string> GetDirectories(string baseDirectory = "", SearchOption searchOption = SearchOption.TopDirectoryOnly) => GetDirectories(Archive, baseDirectory, searchOption);

        /// <summary>
        /// Get all directories in the ZipFile
        /// </summary>
        /// <param name="zipFile">The ZipFile you want to get directories from</param>
        /// <param name="directory">an optional base directory that all returned directories need to share</param>
        /// <returns>A list of directories in the ZipFile</returns>
        /*public static List<string> GetDirectories(ZipFile zipFile, string baseDirectory = "", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            if (String.IsNullOrEmpty(baseDirectory))
                baseDirectory = "Assets/";

            List<string> directories = new List<string>();
            foreach (var file in zipFile.Entries)
            {
                if (!file.IsDirectory)
                    continue;

                if (!file.FileName.Contains(baseDirectory))
                    continue;

                string[] folderNameSplit = file.FileName.Split('/');
                if (folderNameSplit[folderNameSplit.Length - 2].ToLower().Contains(baseDirectory.ToLower()))
                    baseDirectory = file.FileName;

                if (searchOption == SearchOption.TopDirectoryOnly)
                {
                    string[] baseSplit = baseDirectory.Split('/');
                    string[] currentSplit = file.FileName.Split('/');

                    if ((baseSplit.Length + 1) < currentSplit.Length)
                        continue;
                }
                
                directories.Add(file.FileName);
            }
            return directories;
        }*/
        

        /// <summary>
        /// Attempt to find the correct password for the ZipFile associated with this custom Zip object's Archive.
        /// Returns null if not found
        /// </summary>
        /// <returns>The correct password or null</returns>
        public string TryGetPassword() => TryGetPassword(Archive);

        /// <summary>
        /// Attempt to find the correct password for the ZipFile associated with the custom Zip object's Archive.
        /// Returns null if not found
        /// </summary>
        /// <param name="zip">The Zip object you want the password for</param>
        /// <returns>The correct password or null</returns>
        public static string TryGetPassword(Zip zip) => TryGetPassword(zip.Archive);

        /// <summary>
        /// Attempt to find the correct password for a ZipFile. Returns null if not found
        /// </summary>
        /// <param name="zipFile">The ZipFile you want the password for</param>
        /// <returns>The correct password or null</returns>
        public static string TryGetPassword(ZipFile zipFile)
        {
            if (IsPasswordCorrect(zipFile, ""))
                return "";

            var passList = JetPassword.GetPasswords();
            foreach (var pass in passList)
            {
                if (IsPasswordCorrect(zipFile, pass.Trim('\n','\r')))
                    return pass.Trim('\n', '\r');
            }

            return null;
        }


        #region IsPasswordCorrect methods

        /// <summary>
        /// Check if the password associated with this object is correct. 
        /// Uses the Password property of this class to check the Archive of this class
        /// </summary>
        /// <returns>Whether or not the password is correct for the zip</returns>
        public bool IsPasswordCorrect() => IsPasswordCorrect(Archive, Password);

        /// <summary>
        /// Check if string is correct password for the Archive of this class
        /// </summary>
        /// <param name="password">The text to check if valid password</param>
        /// <returns>Whether or not the password is correct for the zip</returns>
        public bool IsPasswordCorrect(string password) => IsPasswordCorrect(Archive, password);

        /// <summary>
        /// Check if string is correct password for the archive associated with the Zip object
        /// </summary>
        /// <param name="zip">The Zip object whose archive you want to check the password for</param>
        /// <param name="password">The text to check if valid password</param>
        /// <returns>Whether or not the password is correct for the zip</returns>
        public static bool IsPasswordCorrect(Zip zip, string password) => IsPasswordCorrect(zip.Archive, password);

        /// <summary>
        /// Check if string is correct password for the Zipfile. This is not using a Zip object
        /// </summary>
        /// /// <param name="zip">The ZipFile you want to check the password for</param>
        /// <param name="password">The text to check if valid password</param>
        /// <returns>Whether or not the password is correct for the zip</returns>
        public static bool IsPasswordCorrect(ZipFile zip, string password)
        {
            if (!Guard.IsStringValid(password))
                return false;

            string tempDir = Environment.CurrentDirectory + "\\Temp";
            if (!Directory.Exists(tempDir))
                Directory.CreateDirectory(tempDir);

            zip.Password = password;

            int i = 0;
            bool result = false;
            foreach (var file in zip)
            {
                try
                {
                    file.Extract(tempDir, ExtractExistingFileAction.OverwriteSilently);
                    if (i > 1)
                    {
                        result = true;
                        break;
                    }
                    i++;
                }
                catch (BadPasswordException)
                {
                    result = false;
                    break;
                }
            }
            Directory.Delete(tempDir, true);
            return result;
        }

        #endregion



        #region JetReading
        
        /// <summary>
        /// Reads the text from a file within the ZipFile "Archive" that the class creates by default
        /// </summary>
        /// <param name="filePathInZip">The path inside the zip to the file you want to read from</param>
        /// <returns>The text contained within the file at filePathInZip</returns>
        public string ReadFileInZip(string filePathInZip) => ReadFileInZip(this.Archive, filePathInZip, this.Password);

        /// <summary>
        /// Reads the text from a file in the ZipFile "Archive" that the class creates by default
        /// </summary>
        /// <param name="path">The path to the zip file you want to read from</param>
        /// <param name="filePathInZip">The path inside the zip to the file you want to read from</param>
        /// <param name="password">The password to the zip</param>
        /// <returns>The text contained within the file at filePathInZip</returns>
        public static string ReadFileInZip(string path, string filePathInZip, string password)
        {
            if (!File.Exists(path))
            {
                Log.Output("Error! Can't get Jet password for \"" + path + "\" . That file does not exist!");
                return null;
            }
            return ReadFileInZip(new ZipFile(path), filePathInZip, password);
        }

        /// <summary>
        /// Reads the text from a file in the ZipFile "Archive" that the class creates by default
        /// </summary>
        /// <param name="zipFile">The ZipFile you want to read from</param>
        /// <param name="filePathInZip">The path inside the zip to the file you want to read from</param>
        /// <param name="password">The password to the zip</param>
        /// <returns>The text contained within the file at filePathInZip</returns>
        public static string ReadFileInZip(ZipFile zipFile, string filePathInZip, string password)
        {
            if (!zipFile.ContainsEntry(filePathInZip))
            {
                Log.Output("Not found! Failed to read file in zip\n\nZip: " + zipFile.Name + "\n\nFailed to read file at: " + filePathInZip);
                return null;
            }

            string returnText = "";
            foreach (var entry in zipFile.Entries)
            {
                if (!entry.FileName.Replace("\\", "/").Contains(filePathInZip))
                    continue;

                Stream s;
                if (!Guard.IsStringValid(password))
                    s = entry.OpenReader();
                else
                {
                    if (!IsPasswordCorrect(zipFile, password))
                    {
                        Log.Output("Error! Password is not correct for Jet file. Trying to find password");
                        var result = JetHandler.FindJetPassword(zipFile);
                        if (String.IsNullOrEmpty(result))
                        {
                            Log.Output("Failed to find password. Unable to continue...");
                            return null;
                        }
                        password = result;
                    }
                    entry.Password = password;
                    s = entry.OpenReader(password);
                }

                StreamReader sr = new StreamReader(s);
                returnText = sr.ReadToEnd();
            }
            return returnText;
        }

        #endregion


        /// <summary>
        /// Get all of the modded files in the jet file. Compares it to the unmodded jet
        /// </summary>
        /// <param name="original">Original unmodded jet</param>
        /// <param name="modded">The modded jet you want to check</param>
        /// <returns>A list of filepaths to all of the modded files</returns>
        public List<string> GetAllModdedFiles(Zip original, Zip modded)
        {
            List<string> moddedFiles = new List<string>();
            return moddedFiles;
        }

        /// <summary>
        /// Use ToString to get the name(path) of the zip object
        /// </summary>
        /// <returns>The name (path) of the current zip object</returns>
        public override string ToString() =>  Archive.Name;
    }
}
