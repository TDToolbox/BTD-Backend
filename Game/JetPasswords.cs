using BTD_Backend.Persistence;
using BTD_Backend.Web;
using System.Collections.Generic;
using System.IO;

namespace BTD_Backend.Game
{
    /// <summary>
    /// Manages methods related to obtaining the password for jet files
    /// </summary>
    public class JetPassword
    {
        /// <summary>
        /// Where to save the jet passwords retrieved from github
        /// </summary>
        string passwordsFilePath = UserData.MainSettingsDir + "\\passwords.txt";

        /// <summary>
        /// Get Password List from file or from github. If downloaded from github it will save that new list to file
        /// </summary>
        /// <param name="redownload">Wheter or not to redownload the list from github</param>
        /// <returns></returns>
        public List<string> GetPasswords(bool redownload = false)
        {
            if (!PasswordsFileExist() || redownload)
                return CreatePasswordsList();

            string passwordText = File.ReadAllText(passwordsFilePath);
            return CreatePasswordsList(passwordText);
        }

        /// <summary>
        /// Check if Passwords File exists locally on PC. Passwords file is a list of passwords saved to PC
        /// </summary>
        /// <returns>True or false, whether or not passwords file exists</returns>
        private bool PasswordsFileExist()
        {
            if (!File.Exists(passwordsFilePath))
                return false;

            return true;
        }

        /// <summary>
        /// Gets the list of passwords from Github as a string
        /// </summary>
        /// <returns>raw text read from url</returns>
        private string ReadGitPasswords()
        {
            string url = "https://raw.githubusercontent.com/TDToolbox/BTDToolbox-2019_LiveFIles/master/BTD%20Battles%20Passwords";
            return WebHandler.ReadText_FromURL(url);
        }

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
                passwords.Add(line);

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
    }
}
