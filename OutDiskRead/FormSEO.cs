using HtmlAgilityPack;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace OutDiskRead
{
    public partial class FormSEO : Form
    {
        private ILog log = log4net.LogManager.GetLogger(typeof(FormSEO));
        public FormSEO()
        {
            InitializeComponent();
        }

        private void FormSEO_Load(object sender, EventArgs e)
        {
            //HtmlWeb web = new HtmlWeb();
            //HtmlAgilityPack.HtmlDocument doc = web.Load("http://www.cnblogs.com/pick/");
            //HtmlNode node = doc.GetElementbyId("post_list");
            //StreamWriter sw = File.CreateText("log.txt");
            //foreach (HtmlNode child in node.ChildNodes)
            //{
            //    if (child.Attributes["class"] == null || child.Attributes["class"].Value != "post_item")
            //        continue;
            //    HtmlNode hn = HtmlNode.CreateNode(child.OuterHtml);

            //    ///如果用child.SelectSingleNode("//*[@class=\"titlelnk\"]").InnerText这样的方式查询，是永远以整个document为基准来查询，
            //    ///这点就不好，理应以当前child节点的html为基准才对。
            //    string k = String.Format("推荐：{0}", hn.SelectSingleNode("//*[@class=\"diggnum\"]").InnerText);
            //    string k1 = String.Format("标题：{0}", hn.SelectSingleNode("//*[@class=\"titlelnk\"]").InnerText);
            //    string k2 = String.Format("介绍：{0}", hn.SelectSingleNode("//*[@class=\"post_item_summary\"]").InnerText);
            //    string k3 = String.Format("信息：{0}", hn.SelectSingleNode("//*[@class=\"post_item_foot\"]").InnerText);

            //    string ff = hn.SelectSingleNode("//*[@id='digg_count_5814610']").InnerText;
            //}
            //sw.Close();
            //Console.ReadLine();

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            string html = GetHtmlStr(@"https://www.hao123.com");
            doc.LoadHtml(html);

            HtmlNode rootNode = doc.DocumentNode;
            HtmlNodeCollection categoryNodeList = rootNode.SelectNodes("//*/div[@id='site']/div/ul/li");
            HtmlNode temp = null;
            foreach (HtmlNode categoryNode in categoryNodeList)
            {
                temp = HtmlNode.CreateNode(categoryNode.OuterHtml);
                //if (temp.SelectSingleNode(CategoryNameXPath).InnerText != "全部文章")
                //{
                //    category = new Category();
                //    category.Subject = temp.SelectSingleNode(CategoryNameXPath).InnerText;
                //    Uri.TryCreate(UriBase, temp.SelectSingleNode(CategoryNameXPath).Attributes["href"].Value, out uriCategory);
                //    category.IndexUrl = uriCategory.ToString();
                //    category.PageUrlFormat = category.IndexUrl + "/page/{0}";
                //    list.Add(category);
                //    Category.CategoryDetails.Add(category.IndexUrl, category);
                //}
            }
            //return list;  

            //log.Info("1122");
            //HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            //string html = GetHtmlStr(@"https://www.hao123.com");
            //doc.LoadHtml(html);
            //HtmlAgilityPack.HtmlNode htmlnode = doc.DocumentNode.SelectSingleNode("//*[@id='site']/div/ul/li[1]");
            //this.textBox1.Text = htmlnode.InnerText + "___________" + htmlnode.InnerHtml;
        }

        //下载页面数据
        private string ShowWebClient(string url)
        {
            string strHtml = string.Empty;
            try
            {
                WebClient wc = new WebClient();
                Stream myStream = wc.OpenRead(url);
                StreamReader sr = new StreamReader(myStream, Encoding.Default);
                strHtml = sr.ReadToEnd();
                myStream.Close();
            }
            catch (Exception ex)
            {
                log.Error("下载沪港通页面出错，错误信息：" + ex.Message + ";错误详情：" + ex);
            }
            return strHtml;
        }

        private string GetHtmlStr(string url)
        {
            try
            {
                WebRequest rGet = WebRequest.Create(url);
                WebResponse rSet = rGet.GetResponse();
                Stream s = rSet.GetResponseStream();
                StreamReader reader = new StreamReader(s, Encoding.UTF8);
                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                log.Error("下载沪港通页面出错，错误信息：" + ex.Message + ";错误详情：" + ex);
            }
            return null;
        }
    }
}
