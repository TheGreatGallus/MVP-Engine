//using MVP_Core.Global;
//using MVP_Core.Managers;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using System.Xml.Linq;

//namespace MVP_.Managers
//{
//    class ProjectManager
//    {
//        private static readonly Lazy<ProjectManager> lazy =
//            new Lazy<ProjectManager>(() => new ProjectManager());

//        public static ProjectManager Instance { get { return lazy.Value; } }

//        public void OpenProject()
//        {
//            var fileContent = string.Empty;
//            var filePath = string.Empty;
//            using (var ofd = new OpenFileDialog())
//            {
//                ofd.InitialDirectory = "c:\\";
//                ofd.Filter = "XML files|*.xml";
//                ofd.FilterIndex = 0;
//                ofd.RestoreDirectory = false;

//                if (ofd.ShowDialog() == DialogResult.OK)
//                {
//                    filePath = ofd.FileName;
//                    GameValues.Path = Path.GetDirectoryName(filePath);
//                    GameValues.Path = GameValues.Path.Replace('\\', '/');
//                    var fileStream = ofd.OpenFile();
//                    try
//                    {
//                        XDocument doc = XDocument.Load(fileStream);
//                        XElement settings = doc.Descendants("settings").Single();
//                        GameValues.Initialize(settings.Descendants("engineSettings").Single());
//                        InputManager.Initialize(settings.Descendants("keyFunctions").Single());
//                    }
//                    catch (Exception e)
//                    {

//                    }
//                }
//            }
//        }

//        public void SaveProject()
//        {

//        }
//    }
//}

