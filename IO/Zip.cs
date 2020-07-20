using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;

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
        public string Path { get; private set; }
        
        /// <summary>
        /// The password to the existing zip file, if there is one
        /// </summary>
        public string Password { get; private set; }
        
        /// <summary>
        /// The ionicZip ZipFile instance created and managed by this class
        /// </summary>
        public ZipFile Archive { get; private set; }
        #endregion


        #region Constructors
        /// <summary>
        /// Custom Zip class wrapped around an ionicZip ZipFile. Contains useful methods related to Zip stuff
        /// </summary>
        public Zip()
        {
            this.Archive = new ZipFile();
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


        //might remove this
        public void Extract(ZipEntry entry, string dest, ExtractExistingFileAction overwriteExistingFile = ExtractExistingFileAction.OverwriteSilently)
        {
            if (IsPasswordCorrect())
                entry.Extract(dest, overwriteExistingFile);
            else
                Log.Output("ERROR! Tried to extract \"" + entry.FileName + "\" and failed. Can't extract the file because the password is incorrect");
        }

        #region IsPasswordCorrect methods

        /// <summary>
        /// Check if the password associated with this object is correct
        /// </summary>
        /// <returns>Whether or not the password is correct for the zip</returns>
        public bool IsPasswordCorrect() => IsPasswordCorrect(Archive, Password);

        /// <summary>
        /// Check if string is correct password
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


        //might move this somewhere else
        public List<string> GetAllModdedFiles(Zip original, Zip modded)
        {
            return null;
        }

        /// <summary>
        /// Use ToString to get the name(path) of the zip object
        /// </summary>
        /// <returns>The name (path) of the current zip object</returns>
        public override string ToString() =>  Archive.Name;
    }
}
