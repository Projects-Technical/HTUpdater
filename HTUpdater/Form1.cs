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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml;
using Microsoft.VisualBasic;


namespace HTUpdater
{
    
    public partial class Form1 : Form
    {

        static DownloadList dllist = new DownloadList();
        static DownloadList deleteList = new DownloadList();
        public Form1()
        {
            InitializeComponent();
        }
        int runtimeversion = 0;
        int dlversion = 0;
        config nconfig;


        Boolean update = false;
        private static Label FileLabel;
        private static Label ProgressLabel;
        private static ProgressBar DownloadProgress;
        private void Form1_Load(object sender, EventArgs e)
        {
            this.BringToFront();
            FileLabel = lblFilename;
            ProgressLabel = lblprogress;
            DownloadProgress = pbdownload;
            config nc = new config();

            nc.Read_Config();



        }
        static string baseaddr = "http://dev.htapplications.com/Updater/";
        static List<Application_File> aps = new List<Application_File>();
      
        
        public static void getManifest(string solutionName, string solutionpath)
        {
           


            FileLabel.Text = "Checking Solution " + solutionName;
            try
            {
                WebClient wc = new WebClient();

                string manifest = wc.DownloadString(baseaddr + solutionName + "/Manifest.xml");

                aps = new List<Application_File>();

                XmlReaderSettings xrs = new XmlReaderSettings();
                XmlReader xr = XmlReader.Create(new StringReader(manifest), xrs);
                xr.MoveToContent();
                while (xr.Read())
                {
                    if (xr.NodeType == XmlNodeType.Element)
                    {
                        switch (xr.Name.ToLower())
                        {
                                case "file":
                                string path = "";
                                int version = 0;
                                int active = 0;
                                if (xr.HasAttributes == true)
                                {
                                    switch (xr.AttributeCount)
                                    {
                                        case 1:
                                            active = Convert.ToInt32(xr.GetAttribute("Active"));
                                        break;
                                        case 2:
                                            version = Convert.ToInt32(xr.GetAttribute("Version"));
                                            active = Convert.ToInt32(xr.GetAttribute("Active"));
                                       break;
                                    }


                                    
                                }

                                DateTime modified = new DateTime();
                                string localfile = xr.ReadInnerXml();
                                string nuri = new Uri(baseaddr + solutionName + "/Package/" + localfile).AbsoluteUri;

                                
                                HttpWebRequest wr = (HttpWebRequest) HttpWebRequest.Create(nuri);
                                HttpWebResponse wresp = (HttpWebResponse) wr.GetResponse();
                                modified = wresp.LastModified;
                                long length = wresp.ContentLength;

                                

                                FileLabel.Text = "Checking File " + localfile + " in Package " + solutionName;
                                if (active == 1)
                                {
                                    aps.Add(new Application_File(solutionName, solutionpath, localfile, modified, length));
                                }
                                else
                                {
                                    deleteList.Add("", solutionpath + "\\" + localfile);
                                }
                                break;



                        }

                    }
                }



                xr.Close();
                xr.Dispose();

                foreach(var(value,index) in aps.Select((v,i)=>(v,i)))
                {
                    
                    string name = value.Name;
                    string path = value.Path;
                    string filename = value.LocalPath;
                    DateTime modified = value.Modified;

                    if (!Directory.Exists(Path.GetDirectoryName(path + "\\" + filename)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(path + "\\" + filename));

                    }
                    long filesize = -200;
                    try
                    {
                       filesize = new FileInfo(path + "\\" + filename).Length;
                    }catch(Exception ex)
                    {

                    }

                    if (!File.Exists(path + "\\" + filename) || File.GetLastWriteTimeUtc(path + "\\" + filename) < modified.ToUniversalTime() || filesize != value.FileSize)
                    {
                       
                        string dllink = baseaddr + name + "/Package/" + filename;

                        dllist.Add(dllink, path + "\\" + filename);
                       


                    }

                    if(dllist.DownList.Count == 0)
                    {
                        Form1.ActiveForm.Show();
                        
                        FileLabel.Text = "All Files Up to Date";
                        Form1.ActiveForm.Update();
                        System.Windows.Forms.Timer ntimer = new System.Windows.Forms.Timer();
                        ntimer.Interval = 5000;
                        ntimer.Enabled = true;
                        ntimer.Tick += closeapp;

                    }
                }

                foreach(DownloadFile f in deleteList.DownList)
                {
                    if(File.Exists(f.Dir))
                    {
                        File.Delete(f.Dir);
                        if(Directory.GetFiles(Path.GetDirectoryName(f.Dir)).Length == 0)
                        {
                            Directory.Delete(Path.GetDirectoryName(f.Dir));
                        }
                    }
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show("Couldnt Connect to Server");
            }
        }

        private static void closeapp(object sender, EventArgs args)
        {
            Application.Exit();
        }
        public class DownloadList
        {
            public List<DownloadFile> DownList { get; set; }


            public DownloadList()
            {
                DownList = new List<DownloadFile>();
            }

            public void Add(string url, string dir)
            {
                DownList.Add(new DownloadFile(url, dir));
            }
        }

        public class DownloadFile
        {
            public string URL { get; set; }
            public string Dir { get; set; }

            public DownloadFile(string url, string dir)
            {
                URL = url;
                Dir = dir;
            }
        }
  
        public class Application_File
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public string LocalPath { get; set; }
            public DateTime Modified { get; set; }
            public long FileSize { get; set; }

            public Application_File Get_File()
            {
                Application_File af = new Application_File(Name,Path,LocalPath,Modified,FileSize);

                return af;
                
            }

            public Application_File(string name, string path,string localpath, DateTime modified, long filesize)
            {
                Name = name;
                Path = path;
                Modified = modified;
                LocalPath = localpath;
                FileSize = filesize;
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
                sb.AppendLine("<Updater>");
                sb.AppendLine("<Version>" + Version + "</Version>");
                sb.AppendLine("<Solution Name=\"Updater\">" + "C:\\Updater\\" + "</Solution>");

                sb.AppendLine("</Updater>");

                File.WriteAllText(Config_File, sb.ToString());
            }


            string configloc = Application.StartupPath + "\\Config\\Config.xml";
            public void Read_Config()
            {
                XmlReaderSettings xrs = new XmlReaderSettings();
                if(!Directory.Exists(Path.GetDirectoryName(configloc)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(configloc));
                }
                if (!File.Exists(configloc))
                {
                    Write_Config();
                }
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
                   
                    Form1.getManifest(value.SolutionName[index], value.SolutionPath[index]);
                    if(dllist.DownList.Count > 0)
                    {
                       
                        Thread ntrd = new Thread(Downloader);
                        ntrd.IsBackground = true;
                        ntrd.Start();

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

        static void Downloader()
        {
            try
            {
                foreach (var (value, index) in dllist.DownList.Select((v, i) => (v, i)))
                {
                    WebClient webClient = new WebClient();
                    webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DLProg);
                    webClient.DownloadFileCompleted += HandleDownloadComplete;
                    FileLabel.Invoke(new Action(()=> FileLabel.Text = "Downloading File: " + (index + 1) + "/" + dllist.DownList.Count()));

                    var syncObject = new Object();
                    lock (syncObject)
                    {
                        webClient.DownloadFileAsync(new Uri(value.URL) , value.Dir, syncObject);
                        //This would block the thread until download completes
                        Monitor.Wait(syncObject);
                    }
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            FileLabel.Invoke(new Action(() => FileLabel.Text = "Downloading Completed"));
            Thread.Sleep(5000);
            Application.Exit();
        }

        static void DLProg(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgress.Invoke(new Action(()=> DownloadProgress.Value = e.ProgressPercentage));
            ProgressLabel.Text = (e.BytesReceived / 1024 / 1024) + "MB /" + (e.TotalBytesToReceive / 1024 / 1024) + "MB";
        }

        static void HandleDownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            lock (e.UserState)
            {
                //releases blocked thread
                Monitor.Pulse(e.UserState);
            }
        }
    }
}
