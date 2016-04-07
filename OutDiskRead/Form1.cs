using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace OutDiskRead
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //BuildTree();
            Thread tShow = new Thread(SavePicFile);
            tShow.Start(@"F:\");
        }

        private void kk()
        {
        }

        /// <summary>
        /// 生成目录树
        /// </summary>
        private void BuildTree()
        {
            //设置图像列表，并暂停重绘
            treeView1.ImageList = imageList1;
            treeView1.BeginUpdate();
            //存放树节点的栈
            Stack<TreeNode> skNode = new Stack<TreeNode>();
            int imageIndex = 0;
            //添加磁盘列表
            string[] drives = new string[] { @"F:\" };//Directory.GetLogicalDrives();
            for (int i = 0; i < drives.Length; i++)
            {
                //每个节点的Text存放目录名，Name存放全路径
                TreeNode node = new TreeNode(drives[i], 0, 0);
                node.Name = drives[i];
                treeView1.Nodes.Add(node);
                skNode.Push(node);
            }
            while (skNode.Count > 0)
            {
                //弹出栈顶目录，并获取路径
                TreeNode curNode = skNode.Pop();
                string path = curNode.Name;

                FileInfo fInfo = new FileInfo(path);
                try
                {
                    if ((fInfo.Attributes & FileAttributes.Directory) != 0)
                    {
                        string[] subDirs = null;
                        string[] subFiles = null;
                        try
                        {
                            //获取当前目录下的所有子目录和文件
                            subDirs = Directory.GetDirectories(path);
                            subFiles = Directory.GetFiles(path);
                        }
                        catch
                        { }
                        if (subDirs != null && subFiles != null)
                        {
                            //目录入栈
                            imageIndex = 1;
                            for (int i = 0; i < subDirs.Length; i++)
                            {
                                string dirName = Path.GetFileName(subDirs[i]);
                                TreeNode dirNode = new TreeNode(dirName, 1, 1);
                                dirNode.Name = subDirs[i];
                                curNode.Nodes.Add(dirNode);
                                skNode.Push(dirNode);
                            }
                            //文件无需入栈
                            imageIndex = 2;
                            for (int i = 0; i < subFiles.Length; i++)
                            {
                                string fileName = Path.GetFileName(subFiles[i]);
                                curNode.Nodes.Add(subFiles[i], fileName, 2);
                            }
                        }
                    }
                }
                catch { }
            }
            treeView1.EndUpdate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] arr = Environment.GetLogicalDrives();
            List<string> list = GetRemovableDeviceID();
            Stack<string> stackList = new Stack<string>();
            
        }

        public const int WM_DEVICECHANGE = 0x219;
        public const int DBT_DEVICEARRIVAL = 0x8000;
        public const int DBT_CONFIGCHANGECANCELED = 0x0019;
        public const int DBT_CONFIGCHANGED = 0x0018;
        public const int DBT_CUSTOMEVENT = 0x8006;
        public const int DBT_DEVICEQUERYREMOVE = 0x8001;
        public const int DBT_DEVICEQUERYREMOVEFAILED = 0x8002;
        public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        public const int DBT_DEVICEREMOVEPENDING = 0x8003;
        public const int DBT_DEVICETYPESPECIFIC = 0x8005;
        public const int DBT_DEVNODES_CHANGED = 0x0007;
        public const int DBT_QUERYCHANGECONFIG = 0x0017;
        public const int DBT_USERDEFINED = 0xFFFF;
        protected override void WndProc(ref Message m)
        {
            try
            {
                if (m.Msg == WM_DEVICECHANGE)
                {
                    switch (m.WParam.ToInt32())
                    {
                        case WM_DEVICECHANGE:
                            break;
                        case DBT_DEVICEARRIVAL://插入移动存储设备
                            DriveInfo[] s = DriveInfo.GetDrives();
                            foreach (DriveInfo drive in s)
                            {
                                if(drive.DriveType == DriveType.Removable)//可移动磁盘
                                {
                                }
                                //if (drive.DriveType == DriveType.CDRom)
                                //你可以判断插入的是什么类型的移动存储设备,并且知道他的盘符,这样就可以在后台遍历所有文件夹跟文件了,后面怎么搞就你自己来搞了,复制文件到你指定的地方.
                            }
                            break;
                        case DBT_CONFIGCHANGECANCELED:
                            break;
                        case DBT_CONFIGCHANGED:
                            break;
                        case DBT_CUSTOMEVENT:
                            break;
                        case DBT_DEVICEQUERYREMOVE:
                            break;
                        case DBT_DEVICEQUERYREMOVEFAILED:
                            break;
                        case DBT_DEVICEREMOVECOMPLETE://卸载移动存储设备
                            break;
                        case DBT_DEVICEREMOVEPENDING:
                            break;
                        case DBT_DEVICETYPESPECIFIC:
                            break;
                        case DBT_DEVNODES_CHANGED:
                            break;
                        case DBT_QUERYCHANGECONFIG:
                            break;
                        case DBT_USERDEFINED:
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            base.WndProc(ref m);
        }

        //获取磁盘列表
        public List<string> GetRemovableDeviceID()
        {
            List<string> deviceIDs = new List<string>();
            ManagementObjectSearcher query = new ManagementObjectSearcher("SELECT   *   From   Win32_LogicalDisk   ");
            ManagementObjectCollection queryCollection = query.Get();
            foreach (ManagementObject mo in queryCollection)
            {
                switch (int.Parse(mo["DriveType"].ToString()))
                {
                    case (int)DriveType.Removable:   //可以移动磁盘     
                        {
                            //MessageBox.Show("可以移动磁盘");
                            deviceIDs.Add(mo["DeviceID"].ToString());
                            break;
                        }
                    case (int)DriveType.Fixed:   //本地磁盘     
                        {
                            //MessageBox.Show("本地磁盘");
                            break;
                        }
                    case (int)DriveType.CDRom:   //CD   rom   drives     
                        {
                            MessageBox.Show("CD   rom   drives ");
                            break;
                        }
                    case (int)DriveType.Network:   //网络驱动   
                        {
                            MessageBox.Show("网络驱动器 ");
                            break;
                        }
                    case (int)DriveType.Ram:
                        {
                            MessageBox.Show("驱动器是一个 RAM 磁盘 ");
                            break;
                        }
                    case (int)DriveType.NoRootDirectory:
                        {
                            MessageBox.Show("驱动器没有根目录 ");
                            break;
                        }
                    default:   //defalut   to   folder     
                        {
                            //MessageBox.Show("驱动器类型未知 ");
                            break;
                        }
                }

            }
            return deviceIDs;
        }

        public void SavePicFile(object fileUrl)
        {
            string kFileUrl = Convert.ToString(fileUrl);
            string FileDir = DateTime.Now.ToString("yyyyMMdd");
            string SaveDir = @"C:\SavePic\" + FileDir + "\\" + kFileUrl;
            if(!Directory.Exists(SaveDir))
            {
                Directory.CreateDirectory(SaveDir);
            }
            if (string.IsNullOrEmpty(kFileUrl))
            {
                return;
            }
            Queue<string> queueList = new Queue<string>();
            Stack<string> stackList = new Stack<string>();
            stackList.Push(kFileUrl);
            while(stackList.Count > 0)
            {
                string path = stackList.Pop();
                FileInfo fInfo = new FileInfo(path);
                if((fInfo.Attributes & FileAttributes.Directory) != 0)
                {
                    string[] fDirs = null;//目录
                    string[] fFiles = null;//文件
                    if (fInfo.Name == "System Volume Information")//无权限访问
                    {
                        continue;
                    }
                    fDirs = Directory.GetDirectories(path);
                    fFiles = Directory.GetFiles(path);
                    if(fDirs != null && fFiles != null)
                    {
                        //目录入栈
                        for (int i = 0; i < fDirs.Length; i++ )
                        {
                            //string fDirName = Path.GetFileName(fDirs[i]);
                            stackList.Push(fDirs[i]);
                        }
                        //文件不入栈
                        for (int i = 0; i < fFiles.Length;i++ )
                        {
                            string fFileExt = Path.GetExtension(fFiles[i]);
                            if(!string.IsNullOrEmpty(fFileExt))
                            {
                                fFileExt = fFileExt.Substring(1);
                            }
                            if (fFileExt.ToUpper() == "BMP" || fFileExt.ToUpper() == "JPEG" || fFileExt.ToUpper() == "JPG" || fFileExt.ToUpper() == "GIF" || fFileExt.ToUpper() == "PNG"
                                || fFileExt.ToUpper() == "PSD" || fFileExt.ToUpper() == "PCX" || fFileExt.ToUpper() == "DXF" || fFileExt.ToUpper() == "CDR" || fFileExt.ToUpper() == "ICO")
                            {
                                queueList.Enqueue(fFiles[i]);
                            }
                            string fFilName = Path.GetFileName(fFiles[i]);
                        }
                    }
                }
            }
            ////保存图片
            //while(queueList.Count > 0)
            //{
            //    string picPath = queueList.Dequeue();
            //    string destPath = Path.Combine(@"D:\A", Guid.NewGuid().ToString() + "_" + Path.GetFileName(picPath));
            //    File.Copy(picPath, destPath);
            //}
        }
    }
}
