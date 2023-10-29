using maFileTool.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace maFileTool.Services
{
    internal class Utils
    {
        public void SaveSettings() 
        {
            Settings settings = new Settings();
            using (StreamWriter file = new StreamWriter(String.Format("{0}\\Settings.json", Environment.CurrentDirectory), false))
            {
                string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
                file.WriteLine(json);
            }
        }
    }
}
