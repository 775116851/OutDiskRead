using log4net;
using OutDiskReadService.ebThread;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace OutDiskReadService.APP
{
    public class DiskFileRead : EBThread
    {
        private static readonly ILog _log = log4net.LogManager.GetLogger(typeof(DiskFileRead));
        public override void run()
        {
            while(true)
            {
                try
                {
                    if (Convert.ToInt32(ConfigurationManager.AppSettings["EnableService"]) == 1)
                    {
                        SavePicFile();
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(string.Format("读取出现异常,异常时间:{0};异常信息:{1};异常详情:{2}", DateTime.Now, ex.Message, ex));
                }
                Thread.Sleep(Convert.ToInt32(ConfigurationManager.AppSettings["ServiceTimeInterval"]));
            }
        }

        public void SavePicFile()
        {
            DriveInfo OutDrive = null;
            DriveInfo[] s = DriveInfo.GetDrives();
            foreach (DriveInfo drive in s)
            {
                if (drive.DriveType == DriveType.Removable)//可移动磁盘
                {
                    OutDrive = drive;
                }
                //if (drive.DriveType == DriveType.CDRom)
                //你可以判断插入的是什么类型的移动存储设备,并且知道他的盘符,这样就可以在后台遍历所有文件夹跟文件了,后面怎么搞就你自己来搞了,复制文件到你指定的地方.
            }
            if(OutDrive == null)
            {
                return;
            }
            string kFileUrl = OutDrive.ToString();//Convert.ToString(fileUrl);
            string FileDir = DateTime.Now.ToString("yyyyMMdd");
            string SaveDir = ConfigurationManager.AppSettings["SaveDir"] + FileDir + "\\" + OutDrive.VolumeLabel + "_" + OutDrive.AvailableFreeSpace + "_" + OutDrive.DriveFormat + "\\ImgList";
            if (!Directory.Exists(SaveDir))
            {
                Directory.CreateDirectory(SaveDir);
            }
            else
            {
                return;
            }
            _log.Info("读取到新移动磁盘并保存在:" + SaveDir);
            if (string.IsNullOrEmpty(kFileUrl))
            {
                return;
            }
            Queue<string> queueList = new Queue<string>();
            Stack<string> stackList = new Stack<string>();
            stackList.Push(kFileUrl);
            while (stackList.Count > 0)
            {
                string path = stackList.Pop();
                FileInfo fInfo = new FileInfo(path);
                if ((fInfo.Attributes & FileAttributes.Directory) != 0)
                {
                    string[] fDirs = null;//目录
                    string[] fFiles = null;//文件
                    if (fInfo.Name == "System Volume Information")//无权限访问
                    {
                        continue;
                    }
                    fDirs = Directory.GetDirectories(path);
                    fFiles = Directory.GetFiles(path);
                    if (fDirs != null && fFiles != null)
                    {
                        //目录入栈
                        for (int i = 0; i < fDirs.Length; i++)
                        {
                            //string fDirName = Path.GetFileName(fDirs[i]);
                            stackList.Push(fDirs[i]);
                        }
                        //文件不入栈
                        for (int i = 0; i < fFiles.Length; i++)
                        {
                            string fFileExt = Path.GetExtension(fFiles[i]);
                            if (!string.IsNullOrEmpty(fFileExt))
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
            StringBuilder sbImgList = new StringBuilder();
            //保存图片
            while (queueList.Count > 0)
            {
                string picPath = queueList.Dequeue();
                string destPath = Path.Combine(SaveDir, Guid.NewGuid().ToString() + "_" + Path.GetFileName(picPath));
                File.Copy(picPath, destPath);
                sbImgList.Append(picPath).Append(Environment.NewLine);
            }
            File.AppendAllText(SaveDir + "\\ImgList.txt", sbImgList.ToString(), Encoding.UTF8);
        }
    }
}
