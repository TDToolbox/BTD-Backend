using BTD_Backend.Persistence;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BTD_Backend.IO
{
    public class Wbp : Zip
    {

        #region Constructors
        public Wbp() : base()
        {

        }
        public Wbp(string path) : base(path)
        {

        }
        public Wbp(string path, string password) : base(path, password)
        {

        }
        #endregion

        public ProjectData getProjectData()
        {
            try
            {
                string metaJson = this.ReadFileInZip("meta.json");
                return JsonConvert.DeserializeObject<ProjectData>(metaJson);
            }catch(Exception)
            {
                return null;
            }
        }
        public void setProjectData(ProjectData metaData)
        {
            this.Archive.RemoveEntry("meta.json");
            string json = JsonConvert.SerializeObject(metaData);
            //MessageBox.Show(json);
            this.Archive.AddEntry("meta.json", json);
        }
    }
}
