using BTD_Backend.Persistence;
using BTD_Backend.Web;
using System;
using System.Collections.Generic;
using System.IO;

namespace BTD_Backend.Game.Jet_Files
{
    /// <summary>
    /// Manages methods related to obtaining the password for jet files
    /// </summary>
    public class JetPassword
    {
        /// <summary>
        /// Where to save the jet passwords retrieved from github
        /// </summary>
        public string passwordsFilePath = UserData.MainSettingsDir + "\\passwords.txt";
        
        /// <summary>
        /// The github url to the passwords list
        /// </summary>
        public string passwordFileURL = "https://raw.githubusercontent.com/TDToolbox/BTDToolbox-2019_LiveFIles/master/BTD%20Battles%20Passwords";


        /// <summary>
        /// Get Password List from file or from github. If downloaded from github it will save that new list to file
        /// </summary>
        /// <param name="redownload">Wheter or not to redownload the list from github</param>
        /// <returns></returns>
        public static List<string> GetPasswords(bool redownload = false)
        {
            Log.Output("Getting password list...");
            JetPassword jet = new JetPassword();
            

            List<string> passwords = new List<string>();
            if (!jet.PasswordsFileExist() || redownload)
                passwords = jet.CreatePasswordsList();
            else
                passwords = jet.CreatePasswordsList(File.ReadAllText(jet.passwordsFilePath));

            JetPasswordEventArgs args = new JetPasswordEventArgs();
            args.PasswordList = passwords;
            if (passwords != null && passwords.Count > 0)
                jet.OnPasswordListAquired(args);
            else
                jet.OnFailedToAquirePasswordList(args);

            return passwords;
        }

        /// <summary>
        /// Check if Passwords File exists locally on PC. Passwords file is a list of passwords saved to PC
        /// </summary>
        /// <returns>True or false, whether or not passwords file exists</returns>
        private bool PasswordsFileExist() => File.Exists(passwordsFilePath);

        /// <summary>
        /// Gets the list of passwords from Github as a string
        /// </summary>
        /// <returns>raw text read from url</returns>
        private string ReadGitPasswords() => WebHandler.ReadText_FromURL(passwordFileURL);

        /// <summary>
        /// Gets the list of passwords from Github as a string, removes all unnecessary words/characters, and
        /// returns a string of the "cleaned" passwords list
        /// </summary>
        /// <returns>A string containing all of the passwords from github, minus unnecessary characters and words</returns>
        private string CleanGitPasswords()
        {
            string result = "";
            string text = ReadGitPasswords();
            string[] split = text.Split('\n');

            for (int i = split.Length - 1; i > 0; i--)
            {
                if (!split[i].Contains("["))
                    continue;                

                string[] line = split[i].Split('-');
                string cleanedLine = line[line.Length - 1].Replace(" ", "");

                result += cleanedLine + "\n";
            }

            return result;
        }

        /// <summary>
        /// Takes raw text from github and turns it into a List<string> of passwords
        /// </summary>
        /// <returns>A List<string> containing all of the passwords</returns>
        private List<string> CreatePasswordsList() => CreatePasswordsList(CleanGitPasswords(), true);

        /// <summary>
        /// Takes string of passwords and turns it into a List<string> of passwords
        /// </summary>
        /// <returns>A List<string> containing all of the passwords</returns>
        private List<string> CreatePasswordsList(string text, bool saveList = false)
        {
            List<string> passwords = new List<string>();
            string cleanedGitText = text;

            string[] split = cleanedGitText.Split('\n');
            foreach (string line in split)
            {
                if (line.Length == 0)
                    continue;

                passwords.Add(line);
            }

            if (saveList)
                SavePasswordFile(passwords);

            return passwords;
        }

        /// <summary>
        /// Creates a list of the passwords and stores it on disk
        /// </summary>
        /// <param name="passwords">the list of passwords gotten from github</param>
        private void SavePasswordFile(List<string> passwords)
        {
            if (File.Exists(passwordsFilePath))
                File.Delete(passwordsFilePath);

            using (StreamWriter writetext = new StreamWriter(passwordsFilePath))
            {
                foreach (string password in passwords)
                    writetext.WriteLine(password);
            }
        }



        #region Events

        /// <summary>
        /// Event is fired when the password list is successfully aquired
        /// </summary>
        public static event EventHandler<JetPasswordEventArgs> AquiredPasswordList;
        
        /// <summary>
        /// Event is fired when the password list failed to be aquired
        /// </summary>
        public static event EventHandler<JetPasswordEventArgs> FailedToAquirePasswordList;


        /// <summary>
        /// Events related to JetPasswords
        /// </summary>
        public class JetPasswordEventArgs : EventArgs
        {
            /// <summary>
            /// The password list that was aquired during GetPasswords()
            /// </summary>
            public List<string> PasswordList { get; set; }
        }

        /// <summary>
        /// Fired when the password list was successfully aquired. Passes password list as arg
        /// </summary>
        /// <param name="e">JetPasswordEvetnArgs takes the aquired password list as an argument</param>
        public void OnPasswordListAquired(JetPasswordEventArgs e)
        {
            EventHandler<JetPasswordEventArgs> handler = AquiredPasswordList;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Fired when the password list failed to be aquired. Passes password list as arg
        /// </summary>
        /// <param name="e">JetPasswordEvetnArgs takes the failed password list as an argument</param>
        public void OnFailedToAquirePasswordList(JetPasswordEventArgs e)
        {
            EventHandler<JetPasswordEventArgs> handler = FailedToAquirePasswordList;
            if (handler != null)
                handler(this, e);
        }

        #endregion
    }
}
