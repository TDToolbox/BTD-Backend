using BTD_Backend.IO;
using Ionic.Zip;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BTD_Backend.Persistence
{
    public class ProjectData
    {
        #region Properties
        private static ProjectData instance;

        public static ProjectData Instance
        {
            get 
            {
                if (instance == null)
                    instance = new ProjectData();
                return instance;
            }
            set { instance = value; }
        }
        public bool IsJet { get; set; }
        public bool IsSave { get; set; }
        public bool IsNKH { get; set; }
        public string ProjectName { get; set; }
        public string WBP_Path { get; set; }
        public DateTime LastOpened { get; set; } = DateTime.Now;
        public string TargetGame { get; set; }
        public string TargetVersion { get; set; }
        public string JetPassword { get; set; } = "Q%_{6#Px]]";

        #endregion


        #region Constructors
        public ProjectData()
        {

        }
        public ProjectData(string wbp_path)
        {
            WBP_Path = wbp_path;
        }
        public ProjectData(string projName, string wbp_path)
        {
            ProjectName = projName;
            WBP_Path = wbp_path;
        }
        public ProjectData(string projName, string wbp_path, DateTime lastOpened)
        {
            ProjectName = projName;
            WBP_Path = wbp_path;
            LastOpened = lastOpened;
        }
        public ProjectData(string projName, string wbp_path, DateTime lastOpened, bool IsJet)
        {
            ProjectName = projName;
            WBP_Path = wbp_path;
            LastOpened = lastOpened;
            this.IsJet = IsJet;
        }
        public ProjectData(string projName, string wbp_path, DateTime lastOpened, bool IsJet, bool IsSave)
        {
            ProjectName = projName;
            WBP_Path = wbp_path;
            LastOpened = lastOpened;
            this.IsJet = IsJet;
            this.IsSave = IsSave;
        }
        public ProjectData(string projName, string wbp_path, DateTime lastOpened, bool IsJet, bool IsSave, bool IsNKH)
        {
            ProjectName = projName;
            WBP_Path = wbp_path;
            LastOpened = lastOpened;
            this.IsJet = IsJet;
            this.IsSave = IsSave;
            this.IsNKH = IsNKH;
        }
        public ProjectData(string projName, string wbp_path, DateTime lastOpened, bool IsJet, bool IsSave, bool IsNKH, string TargetGame)
        {
            ProjectName = projName;
            WBP_Path = wbp_path;
            LastOpened = lastOpened;
            this.IsJet = IsJet;
            this.IsSave = IsSave;
            this.IsNKH = IsNKH;
            this.TargetGame = TargetGame;
        }
        public ProjectData(string projName, string wbp_path, DateTime lastOpened, bool IsJet, bool IsSave, bool IsNKH, string TargetGame, string TargetVersion)
        {
            ProjectName = projName;
            WBP_Path = wbp_path;
            LastOpened = lastOpened;
            this.IsJet = IsJet;
            this.IsSave = IsSave;
            this.IsNKH = IsNKH;
            this.TargetGame = TargetGame;
            this.TargetVersion = TargetVersion;
        }
        public ProjectData(string projName, string wbp_path, DateTime lastOpened, bool IsJet, bool IsSave, bool IsNKH, string TargetGame, string TargetVersion, string JetPassword)
        {
            ProjectName = projName;
            WBP_Path = wbp_path;
            LastOpened = lastOpened;
            this.IsJet = IsJet;
            this.IsSave = IsSave;
            this.IsNKH = IsNKH;
            this.TargetGame = TargetGame;
            this.TargetVersion = TargetVersion;
            this.JetPassword = JetPassword;
        }
        #endregion


        public void SaveProject() => SaveProject(Instance, WBP_Path);

        public static void SaveProject(ProjectData project, string WBP_Path)
        {
            Zip wbp = new Zip(WBP_Path);
            if (project.WBP_Path == null)
                project.WBP_Path = WBP_Path;

            if (!wbp.Archive.EntryFileNames.Contains("project data.json"))
            {
                MessageBox.Show(project.WBP_Path);
                string output = JsonConvert.SerializeObject(Instance, Formatting.Indented);
                StreamWriter serialize = new StreamWriter(Environment.CurrentDirectory + "\\project data.json", false);
                serialize.Write(output);
                serialize.Close();

                wbp.Archive.AddEntry("project data.json", output);
                MessageBox.Show(output);
            }

            
        }

        public ProjectData LoadProject() => LoadProject(WBP_Path);
        public static ProjectData LoadProject(string WBP_Path)
        {
            Zip wbp = new Zip(WBP_Path);
            foreach (var item in wbp.Archive.Entries)
            {
                if (!item.FileName.Contains("project data.json"))
                    continue;

                string json = wbp.ReadFileInZip(item.FileName);
                if (json == "null" || !Guard.IsJsonValid(json))
                {
                    Log.Output("ProjectData has invalid json, generating a new one.");
                    return Instance = new ProjectData();
                }

                return Instance = JsonConvert.DeserializeObject<ProjectData>(json);
            }

            return new ProjectData();
        }
    }
}
