using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.Windows.Forms;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using System.Text.RegularExpressions;
using mshtml;

namespace crawlerTaskIR
{
    public partial class Form1 : Form
    {
        static string connString = @"Data Source=(LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Marwan\source\repos\crawlerTaskIR\crawlerTaskIR\simpleDatabase.mdf; Integrated Security = True";
        SqlConnection conn = new SqlConnection(connString);
        SqlCommand sc;
        Queue<string> seeds;
        string url;
        string path;
        int fileCount = 1;
        Dictionary<string, string> visited = new Dictionary<string, string>();
       // Dictionary<string, string> content = new Dictionary<string, string>();
        public Form1()
        {
            InitializeComponent();
            seeds = new Queue<string>();
            seeds.Enqueue("https://www.bbc.com/");

        }

        private void crawl_Click(object sender, EventArgs e)
        {
            crawling(seeds);

        }


        public void crawling(Queue<string> seeds)
        {
            while (fileCount != 3200 && seeds.Count != 0)
            {

                url = seeds.Dequeue();

                if (checkDuplication(url, visited))
                {
                    //break loop to avoid adding these url
                    continue;
                }
                try
                {
                    WebRequest myWebRequest = WebRequest.Create(url);

                    WebResponse myWebResponse = myWebRequest.GetResponse();

                    Stream streamResponse = myWebResponse.GetResponseStream();

                    StreamReader sReader = new StreamReader(streamResponse);
                    string rString = sReader.ReadToEnd();

                    if (string.IsNullOrEmpty(rString))
                    {
                        //break loop to avoid adding these url (empty string)
                        continue;

                    }

                    if (!checkLanguage(rString))
                    {
                        //break loop to avoid adding these url (Not English)
                        continue;
                    }


                    path = @"E:/IRCrawler" + '/' + "file" + fileCount + ".txt";

                    FileStream fileStream = File.Open(path, FileMode.OpenOrCreate);
                    StreamWriter file = new StreamWriter(fileStream);

                    //StreamWriter sr = new StreamWriter(path);
                    file.WriteLine(url);
                    file.Write(rString);

                    visited.Add(url, path);
                    addURLtoDatabase(url, path);
                    
                    parsingContent(rString, seeds);
                    fileCount++;

                   
                    file.Close();
                    streamResponse.Close();
                    sReader.Close();
                    myWebResponse.Close();
                
                } catch (Exception ex)
                {
                    continue;
                }
            
                
                
            }




           


        }

        public void addURLtoDatabase(string url,string path)
        {

            try
            {
                conn.Open();
                // MessageBox.Show("Connection is active");
                sc = new SqlCommand("insert into Websites(URL,path) values(@url,@path)", conn);
                sc.Parameters.AddWithValue("URL", url);
                sc.Parameters.AddWithValue("path", path);
                //  sc = new SqlCommand("DELETE FROM Websites;", conn);
                //  sc = new SqlCommand("ALTER TABLE Websites AUTO_INCREMENT = 1", conn);
                sc.ExecuteNonQuery();
                sc.Dispose();
                conn.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }


        }
        public bool checkDuplication(string url, Dictionary<string, string> visited)
        {
            if (visited.ContainsKey(url))
            {
                return true;
            }


            return false;
        }

        public bool checkLanguage(string text)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(text);
            try
            {
                HtmlNode lang = doc.DocumentNode.SelectSingleNode("(//html[@lang])[0]");

               
                if (lang==null || !lang.HasAttributes)
                    {
                    return true;

                   }
            
                    if (lang.Attributes["lang"].Value.ToLower().Contains("en"))
                {
                    return true;
                }

                return false;

            } catch (Exception ex)
            {
                
                MessageBox.Show(ex.ToString());
                return true;

            }




        }
        public void parsingContent(string rString,Queue<string>seeds)
        {

            IHTMLDocument2 myDoc = new HTMLDocumentClass();
            myDoc.write(rString);
            IHTMLElementCollection elements = myDoc.links;
            foreach (IHTMLElement el in elements)
            {
                string link = (string)el.getAttribute("href",0);
                if (link != null && ! seeds.Contains(link))
                {
                    seeds.Enqueue(link);
                }
            }


        }




     }

}
