using Anotar.NLog;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace ConsoleApplication2
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static Queue<string> que = new Queue<string>();
        public static List<string> butunLinkler = new List<string>();
        public static string aranacak;
        public static string baslangic;
        public static int countsonuc = 100;
        [LogToErrorOnException]
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Aranacak Kelimeyi Yaziniz / 0 cikis icin");
                aranacak = Console.ReadLine();
                if(aranacak=="0")
                {
                    break;
                }
                baslangic = "https://www.youtube.com/results?search_query="  + aranacak + "";
                AllProces(countsonuc);
                QueWillBeEmpty();
                QuedenListe();
                butunLinkler = DuplicateEliminate(butunLinkler);
                Console.WriteLine("----------------");
                Print(butunLinkler);
                //PrinttoTxt(butunLinkler);
            }
        }
        [LogToErrorOnException]
        public static ISet<string> GetNewLinks(string content)
        {
            Regex regexLink = new Regex("(?<=<a\\s*?href=(?:'|\"))[^'\"]*?(?=(?:'|\"))");

            ISet<string> newLinks = new HashSet<string>();
            foreach (var match in regexLink.Matches(content))
            {
                if (!newLinks.Contains(match.ToString()))
                    newLinks.Add(match.ToString());
            }

            return newLinks;
        }
        [LogToErrorOnException]
        public static void QueWillBeEmpty()
        {
            AllProces(que.Count);
        }
        [LogToErrorOnException]
        public static List<string> GetJustLinks(List<string> content)
        {
            List<string> differentLinks = new List<string>();
            foreach (var allLinks in content)
            {
                if (allLinks.StartsWith("http"))
                {
                    if (!allLinks.Contains("google"))
                    {
                        differentLinks.Add(allLinks);
                    }
                }
            }
            return differentLinks;
        }
        [LogToErrorOnException]
        public static List<string> DuplicateEliminate(List<string> ButunLinkler)
        {
            return ButunLinkler.Distinct().ToList(); ;
        }
        [LogToErrorOnException]
        public static void Print(List<string> butunLinkler)
        {
            foreach (var a in butunLinkler)
            {
                Console.WriteLine(a);
            }
            Console.WriteLine(butunLinkler.Count);
        }
        [LogToErrorOnException]
        public static void QuedenListe()
        {
            while (que.Count > 0)
            {
                butunLinkler.Add(que.Dequeue());
            }
        }
        [LogToErrorOnException]
        public static void PrinttoTxt(List<string> butunLinkler)
        {
            foreach (var kisaltilmis in butunLinkler)
            {
                int wantedlenght = 50;
                if (kisaltilmis.Length < 51)
                {
                    wantedlenght = kisaltilmis.Length;
                }
                File.AppendAllText("~\text1", kisaltilmis.Substring(0, wantedlenght));
            }
        }
        [LogToErrorOnException]
        public static void AllProces(int sayim)
        {
            for (int count = 0; count < sayim; count++)
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(baslangic);
                    WebResponse response = request.GetResponse();
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream);
                    string htmlText = reader.ReadToEnd();
                    htmlText = htmlText.ToLower();
                    if (htmlText.Contains(aranacak.ToLower()))
                    {
                        string Sentence = FromHtmlTofindSimpleText(htmlText);
                        System.Diagnostics.Process.Start(baslangic);
                        List<string> Links = GetNewLinks(htmlText).ToList();
                        List<string> DifferentLinks = GetJustLinks(Links);
                        foreach (var dl in DifferentLinks)
                        {
                            que.Enqueue(dl);
                        }
                    }
                    baslangic = que.Dequeue();
                    Console.WriteLine(sayim - count);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    if (que.Count > 0)
                    {
                        baslangic = que.Dequeue();
                    }
                    if (que.Count == 0)
                    {
                        break;
                    }
                }
            }
        }
        [LogToErrorOnException]
        public static string FromHtmlTofindSimpleText(string html)
        {
            Boolean donecek = false;
            string returnum = "";
            string temp = "";
            List<string> parcala = new List<string>();
            parcala = html.Split(' ').ToList();
            foreach (var parca in parcala)
            {
                if (donecek)
                {
                    return returnum + " " + parca;
                }
                if (parca.Contains(aranacak))
                {
                    returnum = temp + "  " + parca;
                    donecek = true;
                }
                temp = parca;
            }
            return " ";
        }
    }
}
