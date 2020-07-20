using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace BTD_Backend.Web
{
    public class UpdateHandler
    {
        #region Properties
        /// <summary>
        /// The url to the githubApi releases page, which contains info about the releases
        /// </summary>
        public string GitApiReleasesURL { get; set; }

        /// <summary>
        /// The name of the project. Example: "BTDToolbox". Used for logging messages like: "Updating BTDToolbox"
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// The path to the project's exe. Example: "Environment.CurrentDirectory + "\\BTDToolbox.exe".
        /// Used to get the version number for the project
        /// </summary>
        public string ProjectExePath { get; set; }

        /// <summary>
        /// The EXE name of the updater. Example: "BTDToolbox Updater.exe". Used to launch the updater
        /// </summary>
        public string UpdaterExeName { get; set; }

        /// <summary>
        /// The name of the zip file that's downloaded from github releases if there is an update.
        /// Example: "BTDToolbox.zip". Used to delete the updater after updates
        /// </summary>
        public string UpdatedZipName { get; set; }

        /// <summary>
        /// The text read from GitApiReleasesURL
        /// </summary>
        private string AquiredGitApiText;

        #endregion

        #region Constructors
        /// <summary>
        /// Check github releases for the latest release and install it if there is an update. 
        /// Need to manaully set properties with this constructor
        /// </summary>
        public UpdateHandler() { }

        /// <summary>
        /// Check github releases for the latest release and install it if there is an update.
        /// </summary>
        public UpdateHandler(string gitApiReleaseURL, string projectName, string projectExePath, string updaterExeName, string updatedZipName)
        {
            GitApiReleasesURL = gitApiReleaseURL;
            ProjectName = projectName;
            ProjectExePath = projectExePath;
            UpdaterExeName = updaterExeName;
            UpdatedZipName = updatedZipName;
        }
        #endregion

        /// <summary>
        /// Main updater method. Handles all update related functions for ease of use.
        /// </summary>
        public void HandleUpdates()
        {
            DeleteUpdater();    //delete updater if found to keep directory clean and prevent using old updater

            GetGitApiText();
            if (!Guard.IsStringValid(AquiredGitApiText))
            {
                Log.Output("Failed to read release info for " + ProjectName);
                return;
            }

            if (!IsUpdate())
            {
                Log.Output(ProjectName + " is up to date!");
                return;
            }

            Log.Output("An update is available for " + ProjectName + ". Downloading latest version...");
            DownloadUpdates();
            ExtractUpdater();

            Log.Output("Closing " + ProjectName + "...");
            LaunchUpdater();
        }

        /// <summary>
        /// Check if there is an update
        /// </summary>
        /// <returns>true or false, whether or not there is an update</returns>
        private bool IsUpdate()
        {
            string latestVersion = GetLatestVersion();
            string currentVersion = FileVersionInfo.GetVersionInfo(ProjectExePath).FileVersion;

            return Version.Parse(latestVersion) > Version.Parse(currentVersion);
        }

        /// <summary>
        /// Get the text from the GitApiReleasesUrl
        /// </summary>
        /// <returns>The text on the page from the url</returns>
        private string GetGitApiText()
        {
            AquiredGitApiText = WebHandler.ReadText_FromURL(GitApiReleasesURL);
            if (!Guard.IsStringValid(AquiredGitApiText))
                return null;

            return AquiredGitApiText;
        }

        /// <summary>
        /// Reads gitApi text and gets the latest release version
        /// </summary>
        /// <returns>an int of the latest release version, as a whole number without decimals</returns>
        private string GetLatestVersion()
        {
            var gitApi = GitApi.FromJson(AquiredGitApiText);
            string latestRelease = gitApi[0].TagName;

            return latestRelease;
        }

        /// <summary>
        /// Reads gitApi text and gets all of the download urls associated with the latest release
        /// </summary>
        /// <returns>a list of download url strings</returns>
        private List<string> GetDownloadURLs()
        {
            List<string> downloads = new List<string>();
            var gitApi = GitApi.FromJson(AquiredGitApiText);
            foreach (var a in gitApi[0].Assets)
                downloads.Add(a.BrowserDownloadUrl.ToString());

            return downloads;
        }

        /// <summary>
        /// Download all files aquired from the GetDownloadURLs method
        /// </summary>
        private void DownloadUpdates()
        {
            List<string> downloads = GetDownloadURLs();
            foreach (string file in downloads)
                WebHandler.DownloadFile(file, Environment.CurrentDirectory);
        }

        /// <summary>
        /// Extract the updater from the downloaded zip
        /// </summary>
        private void ExtractUpdater()
        {
            var files = Directory.GetFiles(Environment.CurrentDirectory);
            foreach (var file in files)
            {
                if (!file.EndsWith(".zip") && !file.EndsWith(".rar") && !file.EndsWith(".7z"))
                    continue;

                using (ZipArchive archive = System.IO.Compression.ZipFile.OpenRead(file))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (!entry.FullName.ToLower().Contains("update"))
                            continue;

                        string destinationPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, entry.FullName));
                        if (destinationPath.StartsWith(Environment.CurrentDirectory, StringComparison.Ordinal))
                        {
                            if (File.Exists(destinationPath))
                                File.Delete(destinationPath);

                            entry.ExtractToFile(destinationPath);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Launch the updater exe so the update can continue.
        /// </summary>
        private void LaunchUpdater()
        {
            string updater = Environment.CurrentDirectory + "\\" + UpdaterExeName;

            if (!File.Exists(updater))
            {
                Log.Output("ERROR! Unable to find updater. You will need to close " + ProjectName +
                    " and manually extract " + UpdatedZipName);
                return;
            }

            Process.Start(updater);
        }

        /// <summary>
        /// Delete all files related to updater. Used to keep program directory clean
        /// </summary>
        private void DeleteUpdater()
        {
            if (File.Exists(Environment.CurrentDirectory + "\\" + UpdaterExeName))
                File.Delete(Environment.CurrentDirectory + "\\" + UpdaterExeName);

            var files = Directory.GetFiles(Environment.CurrentDirectory);
            foreach (var file in files)
            {
                FileInfo f = new FileInfo(file);
                if (f.Name.ToLower().Replace(".", "").Replace(" ", "") == UpdatedZipName.ToLower().Replace(".", "").Replace(" ", ""))
                {
                    File.Delete(file);
                    break;
                }
            }
        }
    }
}
