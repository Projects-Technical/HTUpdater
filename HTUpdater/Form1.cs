using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Internal;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Microsoft.VisualBasic;


namespace HTUpdater
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int runtimeversion = 0;
        int dlversion = 0;
        config nconfig;


        Boolean update = false;
        private void Form1_Load(object sender, EventArgs e)
        {

            config nc = new config();

            nc.Read_Config();



        }

        private void getManifest(string solutionName, string solutionpath)
        {
            try
            {
                WebClient wc = new WebClient();

                string manifest = wc.DownloadString("http://dev.htapplications.com/Updater/" + solutionName + "/Manifest.xml");

                Applications aps = new Applications();

                XmlReaderSettings xrs = new XmlReaderSettings();
                XmlReader xr = XmlReader.Create(new StringReader(manifest), xrs);
                xr.MoveToContent();
                while (xr.Read())
                {
                    if (xr.NodeType == XmlNodeType.Element)
                    {
                        switch (xr.Name.ToLower())
                        {
                                case "solution":
                                string path = "";
                                int version = 0;
                                if (xr.HasAttributes == true)
                                {
                                    path = xr.GetAttribute(0);
                                    version = Convert.ToInt32(xr.GetAttribute(1));
                                }

                                DateTime modified = new DateTime();

                                WebRequest wr = WebRequest.Create("http//www.google.com");
                                

                                aps.Add_Package(version,xr.ReadInnerXml(),path,)
                                break;



                        }

                    }
                }



                xr.Close();
                xr.Dispose();

            }
            catch(Exception ex)
            {

            }
        }
        
        public class Applications
        {
            public List<Package> Packages = new List<Package>();
            
            public Applications()

            {
                Packages = new List<Package>();

            }

            public void Add_Package(int version, string Name, string Path, DateTime Modified)
            {
                Packages.Add(new Package(version,Name,Path,Modified));
                
            }


        }
        public class Package
        {
            public static int Version;
            public List<Application_File> Manifest = new List<Application_File>();

            public Package()
            {
                Version = 0;
                Manifest = new List<Application_File>();
            }

            public Package(int version, string name, string path, DateTime Modified)
            {
                Version = version;
                Manifest.Add(new Application_File(name, path, Modified));
            }

            public void Set_Version(int version)
                
            {
                Version = version;
            }


            public void Add_Entry(string Name, string Path, DateTime Modified)
            {
                Manifest.Add(new Application_File(Name, Path, Modified));
            }
        }
        public class Application_File
        {
            string Name;
            string Path;
            DateTime Modified;

            public Application_File Get_File()
            {
                Application_File af = new Application_File(Name,Path,Modified);

                return af;
                
            }

            public Application_File(string name, string path, DateTime modified)
            {
                Name = name;
                Path = path;
                Modified = modified;
            }
        }
      
        public class config
        {
            private static int Version;
            ConfigInfo ci = new ConfigInfo();
            private static string Product;
            private static string Config_File = Application.StartupPath + "\\Config\\Config.xml";
            public int Get_Version()
            {
                return Version;
            }

            public static void Set_Version(int version)
            {
                Version = version;
            }

       

            public void Write_Config()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sb.AppendLine("<Config>");
                sb.AppendLine("<Version>" + Version + "</Version>");
                sb.AppendLine("<Product>" + "</Product>");

                int incrementer = 0;
                foreach (ConfigInfo cfg in ci.Config_Entries())
                {
                    sb.AppendLine("<Application Name=\"" + cfg.SolutionName + "\">" + cfg.SolutionPath + "</Application>"); ;
                    incrementer++;
                    
                }
                sb.AppendLine("</Config>");

                File.WriteAllText(Config_File, sb.ToString());
            }


            string configloc = Application.StartupPath + "\\Config\\Config.xml";
            public void Read_Config()
            {
                XmlReaderSettings xrs = new XmlReaderSettings();
                XmlReader xr = XmlReader.Create(File.OpenRead(configloc), xrs);
                xr.MoveToContent();
                while (xr.Read())
                {
                    if (xr.NodeType == XmlNodeType.Element)
                    {
                        switch (xr.Name.ToLower())
                        {
                            case "version":

                                Version = Convert.ToInt32(xr.ReadInnerXml().ToString());
                                break;
                            case "solution":
                                string attr = "";
                                if(xr.HasAttributes == true)
                                {
                                    attr = xr.GetAttribute(0);
                                }
                                
                               ci.Add_Entry(attr, xr.ReadInnerXml().ToString());
                            break;
                            


                        }

                    }
                }


                
                xr.Close();
                xr.Dispose();
                foreach(var(value,index) in ci.Config_Entries().Select((v,i)=>(v,i)))
                {
                    if(!Directory.Exists(value.SolutionPath[index]))
                    {
                        Directory.CreateDirectory(value.SolutionPath[index]);

                    }
                }
            }


            public class ConfigInfo
            {
                public  List<string> SolutionName = new List<string>();
                public  List<string> SolutionPath = new List<string>();


                public ConfigInfo()
                {
                    List<string> solutionName = SolutionName;
                    List<string> solutionPath = SolutionPath;
                }

                public ConfigInfo(string Name, String Path)
                {
                    SolutionName.Add(Name);
                    SolutionPath.Add(Path);

                }

                public void Add_Entry(string Name, string Path)
                {
                    SolutionName.Add(Name);
                    SolutionPath.Add(Path);
                }

                public IEnumerable<ConfigInfo> Config_Entries()
                {
                    int i = 0;
                    ConfigInfo ci = new ConfigInfo();
                    for(i = 0;i<SolutionName.Count;i++)
                    {
                        ci.Add_Entry(SolutionName[i], SolutionPath[i]);

                        yield return ci;
                    }
                    
                }
            }


            public class getConfig
            {
                public static string Name;

                public static string Path;

                public getConfig(string name, string path)
                {
                    Name = name;
                    Path = path;
                }
            }

            private void loadcfg()
            {

            }
        }
    }
}
