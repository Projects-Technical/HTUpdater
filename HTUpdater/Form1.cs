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

        private void getManifest(string solutionName)
        {
            WebClient wc = new WebClient();

            string manifest = wc.DownloadString("http://dev.htapplications.com/Updater/BlazeMusic/Manifest.xml");



            XmlReader xreader = XmlReader.Create(new StringReader(manifest));

            string nodename = "";
            string nodeid = "";
            xreader.MoveToContent();
            while (xreader.Read())
            {
                try
                {

                    if (xreader.HasAttributes)
                    {
                        xreader.MoveToNextAttribute();
                        nodeid = xreader.GetAttribute(0);

                    }
                    if (xreader.NodeType == XmlNodeType.Element)
                    {
                        nodename = xreader.Name;
                    }

                    if (xreader.NodeType == XmlNodeType.Text)
                    {

                        switch (nodename.ToLower())
                        {
                            case "version":


                                dlversion = Convert.ToInt32(xreader.ReadInnerXml());
                                listBox1.Items.Add("Version:" + dlversion);

                                break;
                            case "file":
                                listBox1.Items.Add("File:" + xreader.ReadInnerXml());
                                if (runtimeversion < dlversion)
                                {
                                    wc.DownloadFile(xreader.ReadInnerXml(), nodeid);

                                }
                                break;



                        }

                    }

                    File.WriteAllText(Application.StartupPath + "\\version.cfg", dlversion.ToString());
                }
                catch (Exception ex)
                {

                }
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
                foreach(ConfigInfo ci2 in ci.Config_Entries())
                {
                    MessageBox.Show(ci2.SolutionName[0] + " - " + ci2.SolutionPath[0]);
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
                    for(i = 0;i<SolutionName.Count;i++)
                    {
                        ConfigInfo ci = new ConfigInfo(SolutionName[i], SolutionPath[i]);

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
