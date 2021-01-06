using HtmlAgilityPack;
using Member.Models;
using Member.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Member.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        UsersEntities db = new UsersEntities();
        //抓取個股公司資料 有問題
        public string comp(int stid)
        {
            WebClient url = new WebClient();
            MemoryStream ms = new MemoryStream(url.DownloadData("https://tw.stock.yahoo.com/d/s/company_" + stid + ".html"));
            HtmlDocument doc = new HtmlDocument();
            doc.Load(ms, Encoding.Default);
            doc.LoadHtml(doc.DocumentNode.SelectSingleNode("/html/body/table[1]").InnerHtml);
            string[] t1 = doc.DocumentNode.SelectSingleNode(".//table[1]").InnerText.Trim().Split('\n');
            string[] t2 = doc.DocumentNode.SelectSingleNode(".//table[2]").InnerText.Trim().Split('\n');
            string[] t3 = doc.DocumentNode.SelectSingleNode(".//table[3]").InnerText.Trim().Split('\n');
            string[] p1 = new string[t1.Length];
            string[] p2 = new string[t2.Length];
            string[] p3 = new string[t3.Length];
            for (int i = 0; i < t1.Length; i++)
            {
                p1[i] = Regex.Replace(t1[i], @"\s", "");
            }
            for (int i = 0; i < t2.Length; i++)
            {
                p2[i] = Regex.Replace(t2[i], @"\s", "");
            }
            for (int i = 0; i < t3.Length; i++)
            {
                p3[i] = Regex.Replace(t3[i], @"\s", "");
            }
            List<string> list1 = new List<string>();
            foreach (string s in p1)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    list1.Add(s);
                }
            }
            p1 = list1.ToArray();
            List<string> list2 = new List<string>();
            foreach (string s in p2)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    list2.Add(s);
                }
            }
            p2 = list2.ToArray();
            List<string> list3 = new List<string>();
            foreach (string s in p3)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    list3.Add(s);
                }
            }
            p3 = list3.ToArray();

            string dd =
            "<table class='table table-bordered table-hover '>" +
            "<tr><td colspan='2'>" + p1[0] + "</td><td colspan='2'>" +
            p1[1] + p1[2] + "</td></tr>";
            // 輸出資料
            for (int i = 3; i < p1.Length - 1; i++)
            {
                if (i < 20)
                {
                    dd += "<tr><td>" + p1[i] + "</td><td>" + p1[i + 1] + "</td><td>" + p1[i + 2] + "</td><td>" + p1[i + 3] + "</td></tr>";
                    i = i + 3;
                }
                if (i > 22)
                {
                    dd += "<tr><td>" + p1[i] + "</td><td colspan='3'>" + p1[i + 1] + "</td><tr>";
                    i++;
                }
            }
            dd += "<table class='table table-bordered table-hover '>" +
                "<tr><td colspan='2'>" + p2[0] + "</td><td colspan='2'>" +
            p2[1] + "<td colspan='2'>" + p2[2] + "</td></tr>";
            for (int i = 3; i < p2.Length - 1; i++)
            {
                if (i < 22)
                {
                    dd += "<tr><td>" + p2[i] + "</td><td>" + p2[i + 1] + "</td><td>" + p2[i + 2] + "</td><td>" + p2[i + 3] + "</td><td>" + p2[i + 4] + "</td><td>" + p2[i + 5] + "</td></tr>";
                    i = i + 5;
                }
                if (i > 26)
                {
                    dd += "<tr><td>" + p2[i] + "</td><td>" + p2[i + 1] + "</td><td colspan='4'>" + p2[i + 2] + "</td><tr>";
                    i = i + 3;
                }
            }
            dd += "<table class='table table-bordered table-hover '>" +
            "<tr><td colspan='2'>" + p3[0] + "</td><td colspan='2'>" +
            p3[1] + "</td></tr>";
            for (int i = 2; i < p3.Length - 1; i++)
            {
                if (i < 23)
                {
                    dd += "<tr><td>" + p3[i] + "</td><td>" + p3[i + 1] + "</td><td>" + p3[i + 2] + "</td><td>" + p3[i + 3] + "</td></tr>";
                    i = i + 3;
                }

            }

            dd += "</table>";


            return dd;
        }
        //抓取個股新聞 前4筆
        public string news(int stid)
        {
            WebClient url = new WebClient();
            MemoryStream ms = new MemoryStream(url.DownloadData("https://tw.stock.yahoo.com/q/q?s=" + stid));
            HtmlDocument doc = new HtmlDocument();
            doc.Load(ms, Encoding.Default);
            doc.LoadHtml(doc.DocumentNode.SelectSingleNode("//*[@id='researchNews']/div[2]").InnerHtml);
            string[] t1 = doc.DocumentNode.SelectSingleNode("./ul").InnerText.Trim().Split('\n');
            HtmlNodeCollection t2 = doc.DocumentNode.SelectNodes("./ul/li/a");
            string[] p1 = new string[t1.Length];
            List<string> list1 = new List<string>();
            for (int j = 0; j < p1.Length; j++)
            {
                p1[j] = Regex.Replace(t1[j], @"\s", "");
            }
            foreach (string s in p1)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    list1.Add(s);
                }
            }
            p1 = list1.ToArray();

            string dd = "<table class='table table-bordered table-hover '>";
            int i = 0;
            foreach (HtmlNode t in t2)
            {

                dd += "<tr><td><a target='_blank' href='https://tw.stock.yahoo.com/" + t.Attributes["href"].Value + "'>";
                dd += p1[i] + "</a></td></tr>";

                i = i + 2;
            }
            dd += "</table>";


            return dd;
        }
        //抓取個股資訊
        public string stock(int stid)
        {
            WebClient url = new WebClient();
            MemoryStream ms = new MemoryStream(url.DownloadData("https://tw.stock.yahoo.com/q/q?s=" + stid));
            HtmlDocument doc = new HtmlDocument();
            doc.Load(ms, Encoding.Default);
            doc.LoadHtml(doc.DocumentNode.SelectSingleNode("/html[1]/body[1]/center[1]/table[2]/tr[1]/td[1]/table[1]").InnerHtml);
            string[] txt = doc.DocumentNode.SelectSingleNode("./tr[2]").InnerText.Trim().Split('\n');
           
            string dd = "<div class='form-row mt-4 d-flex'>" +
                "<button type='button'id='col'class='btn btn-primary a1 mb-1'>" +
                "<i class='fas fa-heart'></i>" +
                "</button>"
                + "<div class='col-md-10 mb-1 '><h2 class='mb-0'>" + txt[0].Trim().Replace("加到投資組合", "")+"</h2>" +
                "</div>" +
                "<div class='col-md-1 mb-3'>"+txt[1].Trim()+ "</div>"+"</div>";
            dd += "<table class='table table-borderless text-secondary mt-2'>" +
                "<tr>" +
                "<td rowspan ='3'width ='15%' class='p-0'>" +
                "<div class='col-auto border rounded border-left-0 p-3 mr-2'>" +
                "<h1 class='text-center'>" + txt[2].Trim() + "</h1>" +
                "<div class='text-center mt-3'>" + txt[5].Trim() + "</div>" +
                "</div>"+"</td>" + "</tr>" +
                   "<tr class='border-bottom'><td>開盤</td><td class='text-dark' colspan='3'>" + txt[8].Trim() + "</td><td>成交量(張)</td><td class='text-dark' colspan='3'>" + txt[6].Trim() + "</td>" +
                    "<td>買進</td><td class='text-dark' colspan='2'>" + txt[3].Trim() + "</td></tr>" +
                    "<tr><td>昨收</td><td class='text-dark' colspan='3'>" + txt[7].Trim() + "</td>" +
                    "<td>最高</td><td class='text-dark' colspan='3'>" + txt[9].Trim() + "</td><td>賣出</td><td class='text-dark' colspan='2'>" + txt[4].Trim() + "</td></tr>" +
                    "</table>";



            return dd;
        }
        //抓取個股60分鐘內成交紀錄
        public string detil(int stid)
        {
            WebClient url = new WebClient();
            MemoryStream ms = new MemoryStream(url.DownloadData("https://tw.stock.yahoo.com/q/ts?&t=50&s=" + stid));
            HtmlDocument doc = new HtmlDocument();
            doc.Load(ms, Encoding.Default);
            doc.LoadHtml(doc.DocumentNode.SelectSingleNode("/html/body/center/table[1]/tbody/tr/td[1]/table[2]/tbody/tr[2]/td/table").InnerHtml);

            HtmlNodeCollection htnode = doc.DocumentNode.SelectNodes(".//td");
            string dd = "<table class='table table-bordered table-hover '>";
            int i = 0;
            // 輸出資料
            foreach (HtmlNode nodeHeader in htnode)
            {
                if (i < 7)
                {
                    if (i == 0)
                    {
                        dd += "<tr>";
                    }
                    dd += "<td>" + nodeHeader.InnerText + "</td>";
                    i++;
                }

                if (i == 7)
                {
                    dd += "</tr>";
                    i = 0;
                }



            }
            dd += "</table>";


            return dd;
        }
        //抓取個股評估資料
        public string evaluate(int stid)
        {
            WebClient url = new WebClient();
            MemoryStream ms = new MemoryStream(url.DownloadData("https://news.money-link.com.tw/yahoo/0061_" + stid + ".html"));
            HtmlDocument doc = new HtmlDocument();
            doc.Load(ms, Encoding.Default);
            doc.LoadHtml(doc.DocumentNode.SelectSingleNode("/html/body/table").InnerHtml);
            HtmlNodeCollection htnode = doc.DocumentNode.SelectNodes(".//td");
            string dd = "<table class='table table-bordered table-hover '>";

            for (int i = 0; i < 26; i++)
            {
                if (i < 14)
                {
                    dd += "<tr><td colspan='2'>" + htnode[i].InnerText + "</td><td>" + htnode[i + 1].InnerText + "</td><td>" + htnode[i + 2].InnerText + "</td></tr>";
                    i = i + 2;
                }
                if (i > 14)
                {
                    dd += "<tr><td>" + htnode[i].InnerText + "</td><td>" + htnode[i + 1].InnerText + "</td><td>" + htnode[i + 2].InnerText + "</td><td>" + htnode[i + 3].InnerText + "</td></tr>";
                    i = i + 3;
                }


            }

            dd += "</table>";

            return dd;
        }
        //抓取個股歷年股利資料
        public string value(int stid)
        {
            //處理C# 連線 HTTPS 網站發生驗證失敗導致基礎連接已關閉            
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            WebClient url = new WebClient();
            MemoryStream ms = new MemoryStream(url.DownloadData("https://tw.stock.yahoo.com/d/s/dividend_" + stid + ".html"));
            HtmlDocument doc = new HtmlDocument();
            doc.Load(ms, Encoding.Default);
            HtmlNodeCollection htnode = doc.DocumentNode.SelectNodes("/html/body/table[1]/tr/td/table[2]//td");
            string dd = "<table class='table table-bordered table-hover '>";
            int i = 0;
            // 輸出資料
            foreach (HtmlNode nodeHeader in htnode)
            {
                if (i < 7)
                {
                    if (i == 0)
                    {
                        dd += "<tr>";
                    }
                    dd += "<td>" + nodeHeader.InnerText + "</td>";
                    i++;
                }

                if (i == 7)
                {
                    dd += "</tr>";
                    i = 0;
                }

            }
            dd += "</table>";



            return dd;
        }
        //抓取個股當日主力進出
        public string main(int stid)
        {
            //處理C# 連線 HTTPS 網站發生驗證失敗導致基礎連接已關閉            
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            WebClient url = new WebClient();
            MemoryStream ms = new MemoryStream(url.DownloadData("https://tw.stock.yahoo.com/d/s/major_" + stid + ".html"));
            HtmlDocument doc = new HtmlDocument();
            doc.Load(ms, Encoding.Default);
            HtmlNodeCollection htnode1 = doc.DocumentNode.SelectNodes("/html/body/table[1]/tr/td/table[1]//td");
            HtmlNodeCollection htnode2 = doc.DocumentNode.SelectNodes("/html/body/table[1]/tr/td/table[2]//td");

            string dd = "<table class='table table-bordered table-hover '>";
            int i = 0;

            dd += "<tr><td colspan='3'>" + htnode1[0].InnerText + "</td>" + "<td colspan='3'> " + htnode1[1].InnerText + "</td>" + "<td colspan='2'>" + htnode1[2].InnerText + "</td></tr>";
            foreach (HtmlNode nodeHeader in htnode2)
            {
                if (i < 8)
                {
                    if (i == 0)
                    {
                        dd += "<tr>";
                    }
                    dd += "<td>" + nodeHeader.InnerText + "</td>";
                    i++;
                }

                if (i == 8)
                {
                    dd += "</tr>";
                    i = 0;
                }

            }
            dd += "</table>";



            return dd;
        }
        //抓取個股當日資卷變化
        public string Voucher(int stid)
        {
            //處理C# 連線 HTTPS 網站發生驗證失敗導致基礎連接已關閉            
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            WebClient url = new WebClient();
            MemoryStream ms = new MemoryStream(url.DownloadData("https://tw.stock.yahoo.com/d/s/credit_" + stid + ".html"));
            HtmlDocument doc = new HtmlDocument();
            doc.Load(ms, Encoding.Default);
            HtmlNodeCollection htnode = doc.DocumentNode.SelectNodes("/html/body/table[1]/tr[2]/td/table//td");
            string dd = "<table class='table table-bordered table-hover '><tr>";
            int j = 0;
            for (int i = 0; i < 26; i++)
            {
                if (i < 7)
                {
                    if (i == 0)
                    {
                        dd += "<tr>";
                    }
                    dd += "<td>" + htnode[i].InnerText + "</td>";
                }
                if (i == 7)
                {
                    dd += "</tr><tr><td rowspan='4'>" + htnode[i].InnerText + "</td></tr>";
                }
                if (i > 7)
                {
                    if (j == 0)
                    {
                        dd += "<tr>";
                    }
                    dd += "<td>" + htnode[i].InnerText + "</td>";
                    j++;
                }
                if (j == 6)
                {
                    dd += "</tr>";
                    j = 0;
                }

            }
            dd += "</table>";



            return dd;
        }
        //抓取個股每月變化
        public string Revenue(int stid)
        {
            //處理C# 連線 HTTPS 網站發生驗證失敗導致基礎連接已關閉            
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            WebClient url = new WebClient();
            MemoryStream ms = new MemoryStream(url.DownloadData("https://tw.stock.yahoo.com/d/s/earning_" + stid + ".html"));
            HtmlDocument doc = new HtmlDocument();
            doc.Load(ms, Encoding.Default);
            HtmlNodeCollection htnode = doc.DocumentNode.SelectNodes("/html/body/center[2]/table[2]/tr/td/table[3]/tr/td/table[2]//td");
            string dd = "<table class='table table-bordered table-hover '>";

            dd += "<tr><td colspan='3'>" + htnode[0].InnerText + "</td>" + "<td colspan='6'> " + htnode[1].InnerText + "</td></tr>";
            int j = 0;
            for (int i = 2; i < 119; i++)
            {
                if (i < 11)
                {
                    if (i == 0)
                    {
                        dd += "<tr>";
                    }
                    dd += "<td>" + htnode[i].InnerText + "</td>";
                }
                if (i == 11)
                {
                    dd += "</tr>";
                }
                if (i > 10)
                {
                    if (j == 0)
                    {
                        dd += "<tr>";
                    }
                    dd += "<td>" + htnode[i].InnerText + "</td>";
                    j++;
                }
                if (j == 9)
                {
                    dd += "</tr>";
                    j = 0;
                }

            }


            return dd;
        }
        //抓取個股每季稅後盈餘變化
        public string Asurplus(int stid)
        {
            //處理C# 連線 HTTPS 網站發生驗證失敗導致基礎連接已關閉            
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            WebClient url = new WebClient();
            MemoryStream ms = new MemoryStream(url.DownloadData("https://tw.stock.yahoo.com/d/s/earning_" + stid + ".html"));
            HtmlDocument doc = new HtmlDocument();
            doc.Load(ms, Encoding.Default);
            HtmlNodeCollection htnode = doc.DocumentNode.SelectNodes("/html/body/table[1]/tr/td/table[3]//td");
            string dd = "<table class='table table-bordered table-hover '>";

            dd += "<tr><td colspan='3'>" + htnode[0].InnerText + "</td>" + "<td colspan='4'> " + htnode[1].InnerText + "</td></tr>";
            int j = 0;
            for (int i = 2; i < 37; i++)
            {
                if (i < 9)
                {
                    if (i == 0)
                    {
                        dd += "<tr>";
                    }
                    dd += "<td>" + htnode[i].InnerText + "</td>";
                }
                if (i == 9)
                {
                    dd += "</tr>";
                }
                if (i > 8)
                {
                    if (j == 0)
                    {
                        dd += "<tr>";
                    }
                    dd += "<td>" + htnode[i].InnerText + "</td>";
                    j++;
                }
                if (j == 7)
                {
                    dd += "</tr>";
                    j = 0;
                }
            }
            dd += "</table>";
            return dd;
        }
        //抓取個股每季稅前盈餘變化
        public string Bsurplus(int stid)
        {
            //處理C# 連線 HTTPS 網站發生驗證失敗導致基礎連接已關閉            
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            WebClient url = new WebClient();
            MemoryStream ms = new MemoryStream(url.DownloadData("https://tw.stock.yahoo.com/d/s/earning_" + stid + ".html"));
            HtmlDocument doc = new HtmlDocument();
            doc.Load(ms, Encoding.Default);
            HtmlNodeCollection htnode = doc.DocumentNode.SelectNodes("/html/body/table[2]/tr[1]/td/table[2]//td");
            string dd = "<table class='table table-bordered table-hover '>";

            dd += "<tr><td colspan='3'>" + htnode[0].InnerText + "</td>" + "<td colspan='4'> " + htnode[1].InnerText + "</td></tr>";
            int j = 0;
            for (int i = 2; i < 37; i++)
            {
                if (i < 9)
                {
                    if (i == 0)
                    {
                        dd += "<tr>";
                    }
                    dd += "<td>" + htnode[i].InnerText + "</td>";
                }
                if (i == 9)
                {
                    dd += "</tr>";
                }
                if (i > 8)
                {
                    if (j == 0)
                    {
                        dd += "<tr>";
                    }
                    dd += "<td>" + htnode[i].InnerText + "</td>";
                    j++;
                }
                if (j == 7)
                {
                    dd += "</tr>";
                    j = 0;
                }
            }
            dd += "</table>";

            return dd;
        }
        //抓取個股大股東申報轉讓
        public string Transfer(int stid)
        {
            //處理C# 連線 HTTPS 網站發生驗證失敗導致基礎連接已關閉            
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            WebClient url = new WebClient();
            MemoryStream ms = new MemoryStream(url.DownloadData("https://tw.stock.yahoo.com/d/s/insider_" + stid + ".html"));
            HtmlDocument doc = new HtmlDocument();
            doc.Load(ms, Encoding.Default);
            HtmlNodeCollection htnode = doc.DocumentNode.SelectNodes("/html/body/table[1]/tr[2]/td/table//td");
            string dd = "<table class='table table-bordered table-hover '>";
            int i = 0;
            foreach (HtmlNode nodeHeader in htnode)
            {
                if (i < 4)
                {
                    if (i == 0)
                    {
                        dd += "<tr>";
                    }
                    dd += "<td>" + nodeHeader.InnerText + "</td>";
                    i++;
                }

                if (i == 4)
                {
                    dd += "</tr>";
                    i = 0;
                }

            }
            dd += "</table>";


            return dd;
        }
        //抓取個股價量比
        public string Pricevolume(int stid)
        {
            //處理C# 連線 HTTPS 網站發生驗證失敗導致基礎連接已關閉            
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            WebClient url = new WebClient();
            MemoryStream ms = new MemoryStream(url.DownloadData("https://tw.stock.yahoo.com/q/ts?s=" + stid));
            HtmlDocument doc = new HtmlDocument();
            doc.Load(ms, Encoding.Default);
            HtmlNodeCollection htnode1 = doc.DocumentNode.SelectNodes("/html/body/center/table[1]/tbody/tr/td[1]/table[2]/tbody/tr[2]/td/table[2]/tbody/tr[2]/td/table/tr[1]/td/b");
            HtmlNodeCollection htnode2 = doc.DocumentNode.SelectNodes("/html/body/center/table[1]/tbody/tr/td[1]/table[2]/tbody/tr[2]/td/table[2]/tbody/tr[2]/td/table//table//td");
            string dd = "<table class='table table-bordered table-hover '>";
            int i = 0;
            dd += "<tr><td colspan='2'>" + htnode1[0].InnerText + "</td></tr>";
            foreach (HtmlNode nodeHeader in htnode2)
            {
                if (i < 3)
                {
                    if (i == 0)
                    {
                        dd += "<tr>";
                    }
                    if (i != 1)
                    {
                        dd += "<td>" + nodeHeader.InnerText + "</td>";
                    }

                    i++;
                }

                if (i == 3)
                {
                    dd += "</tr>";
                    i = 0;
                }

            }
            dd += "</table>";


            return dd;
        }
        //個股顯示頁面
        public ActionResult ShowStock(int stid = 1101)
        {
            
            var p = 0;
            if (Session["result"] != null)
            {
                p = int.Parse(Session["result"].ToString());
            }


            VM vme = new VM()
            {
                MesS = db.MesS.OrderBy(m => m.MsSId).Where(m => m.StId == stid & m.UsId == p).ToList(),
                MesR = db.MesR.OrderBy(m => m.MsRId).Where(m => m.StId == stid & m.UsId == p).ToList(),
                Mes = db.Mes.OrderBy(m => m.MsP).Where(m => m.StId == stid).ToList()
            };
            Session["stid"] = stid;
            ViewBag.st = stock(stid);
            ViewBag.stnews = news(stid);
            ViewBag.stcomp = comp(stid);
            ViewBag.stdetil = detil(stid);
            ViewBag.stevaluate = evaluate(stid);
            ViewBag.stvalue = value(stid);
            ViewBag.stmain = main(stid);
            ViewBag.stVoucher = Voucher(stid);
            ViewBag.stRevenue = Revenue(stid);
            ViewBag.stAsurplus = Asurplus(stid);
            ViewBag.stBsurplus = Bsurplus(stid);
            ViewBag.stTransfer = Transfer(stid);
            ViewBag.Pricevolume = Pricevolume(stid);

            return View(vme);

        }
        //行業類股頁面
        public ActionResult st(int inid = 1)
        {

            VM vme = new VM()
            {
                Industry = db.Industry.ToList(),
                IndustryD = db.IndustryD.Where(m => m.InId == inid).ToList()

            };
            return View(vme);
        }       
        //收藏個股功能
        public void Col(Collection col, int stid)
        {
            int usid = (int)Session["result"];
            var result = db.Collection.Where(m => m.StId == stid && m.UsId == usid).FirstOrDefault();
            if(result == null) { 
                col.UsId = usid;
                col.StId = stid;
                db.Collection.Add(col);
                db.SaveChanges();
            }
        }
        //刪除收藏個股功能
        public ActionResult ColDelete(int stid)
        {
            int usid = (int)Session["result"];
            var emp = db.Collection.Where(m => m.StId == stid && m.UsId == usid).FirstOrDefault();
            db.Collection.Remove(emp);
            db.SaveChanges();
            return RedirectToAction("Index","Account");
        }
        //建立留言
        public JsonResult MeCreateP(MesR qq, Mes mm, string mscont, int stid)
        {
            int usid = (int)Session["result"];
            try
            {
                var p = db.Mes.Where(m => m.StId == stid).Max(m => m.MsP);
                mm.MsP = p + 1;
            }
            catch
            {
                mm.MsP = 0;
            }
            mm.MsR = 0;            
            mm.UsId = usid;
            mm.StId = stid;
            mm.MsDate = DateTime.Now;
            mm.MsCont = mscont;
            mm.MsRpt = MeCheck(mscont);

            db.Mes.Add(mm);
            db.SaveChanges();
            if (MeCheck(mscont) == true) {
                var p = db.Mes.Max(m => m.MsId);
                MeReport(qq, p, stid,"系統判斷異常發言");
            }

            var p2 = db.Mes.Where(m => m.StId == stid ).OrderByDescending(m =>m.MsId).Take(1).FirstOrDefault();
            var p3 = db.Memb.Where(m => m.UsId == p2.UsId).FirstOrDefault();

            return Json(new { msid = p2.MsId,emaile = p3.UsEmail.ToString(), cont = p2.MsCont, date = p2.MsDate.ToString(), msgc =p2.MsGC,msbc =p2.MsBC }, JsonRequestBehavior.AllowGet);
        }
        //建立回復
        public JsonResult MeCreateR(MesR qq, Mes mm, string mscont, int msp, int stid)
        {
            int usid = (int)Session["result"];
            try
            {
                var p = db.Mes.Where(m => m.StId == stid && m.MsP == msp).Max(m => m.MsR);
                mm.MsR = p + 1;
            }
            catch
            {
                mm.MsR = 0;
            }
            mm.MsRpt = MeCheck(mscont);
            mm.MsP = msp;
            mm.UsId = usid;
            mm.StId = stid;
            mm.MsDate = DateTime.Now;
            mm.MsCont = mscont;

            db.Mes.Add(mm);
            db.SaveChanges();
            if (MeCheck(mscont) == true)
            {
                var p = db.Mes.Max(m => m.MsId);
                MeReport(qq, p, stid, "系統判斷異常發言");
            }
            var p2 = db.Mes.Where(m => m.StId == stid).OrderByDescending(m => m.MsId).FirstOrDefault();
            var p3 = db.Memb.Where(m => m.UsId == p2.UsId).FirstOrDefault();

            return Json(new { msid = p2.MsId, emaile = p3.UsEmail.ToString(), cont = p2.MsCont, date = p2.MsDate.ToString(), msgc = p2.MsGC, msbc = p2.MsBC }, JsonRequestBehavior.AllowGet);
        }
        //評價功能
        public JsonResult MeCreateS(MesS mm, int msid, int msgd, int msbd, int stid)
        {
            int usid = (int)Session["result"];
            var p = db.MesS.Where(m => m.StId == stid && m.MsId == msid && m.UsId == usid).FirstOrDefault();
            var pg = db.MesS.Where(m => m.StId == stid && m.MsId == msid && m.UsId == usid && m.MsGd == msgd).FirstOrDefault();
            var pb = db.MesS.Where(m => m.StId == stid && m.MsId == msid && m.UsId == usid && m.MsBd == msbd).FirstOrDefault();

            if (p == null)
            {
                mm.MsId = msid;
                mm.UsId = usid;
                mm.StId = stid;
                mm.MsGd = msgd;
                mm.MsBd = msbd;
                mm.MsSDate = DateTime.Now;
                db.MesS.Add(mm);
            }
            else
            {
                p.MsBd = msbd;
                p.MsGd = msgd;
                if (pg != null) {
                    if (pg.MsGd == msgd) {
                        p.MsGd = 0;

                    }
                }
                if (pb != null) {
                    if (pb.MsBd == msbd)
                    {
                        p.MsBd = 0;

                    }
                }
            }
            db.SaveChanges();

            var gc = db.MesS.Where(m => m.StId == stid && m.MsId == msid && m.MsGd == 1).Count();
            var bc = db.MesS.Where(m => m.StId == stid && m.MsId == msid && m.MsBd == 1).Count();
            var ps = db.Mes.Where(m => m.StId == stid && m.MsId == msid).FirstOrDefault();
            ps.MsGC = gc;
            ps.MsBC = bc;
            db.SaveChanges();

            var pl = db.MesS.Where(m => m.StId == stid && m.MsId == msid && m.UsId == usid).FirstOrDefault();
            List<int> result = new List<int>();
            result.Add(pl.MsGd);
            result.Add(pl.MsBd);
            result.Add(gc);
            result.Add(bc);
            return Json(result,JsonRequestBehavior.AllowGet);
        }
        //檢舉功能
        public void MeReport(MesR mm, int msid, int stid, string mscont)
        {
            int usid = (int)Session["result"];
            var p = db.Mes.Where(m => m.StId == stid && m.MsId == msid).FirstOrDefault();

            mm.MsId = msid;
            mm.UsId = usid;
            mm.StId = stid;
            mm.MsRCont = mscont;
            mm.MsRDate = DateTime.Now;
            p.MsRpt = true;
            db.MesR.Add(mm);
            db.SaveChanges();
        }    
        //檢查留言是否正常
        public bool MeCheck(string mscont)
        {
            string[] check = { "你好", "不要臉", "去死吧", "幹" };
            for (int i = 0; i < check.Length; i++)
            {
                if (mscont.IndexOf(check[i]) != -1)
                    return true;
            }

            return false;
        }
        //選股區
        public ActionResult ChStock() {
            return View();
        }
        //選股區抓值範圍
        public JArray Ch(string p) {
            string uri = "https://tw.screener.finance.yahoo.net/screener/ws?f=j&657&PickCount=20&PickID="+p;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri); //request請求
            req.Timeout = 10000; //request逾時時間
            req.Method = "GET"; //request方式
            HttpWebResponse respone = (HttpWebResponse)req.GetResponse(); //接收respone
            StreamReader streamReader = new StreamReader(respone.GetResponseStream(), Encoding.UTF8); //讀取respone資料
            string result = streamReader.ReadToEnd(); //讀取到最後一行
            respone.Close();
            streamReader.Close();
            JObject jsondata = JsonConvert.DeserializeObject<JObject>(result); //將資料轉為json物件
            JArray jarray = (JArray)jsondata["items"]; //回傳json陣列
            return jarray;
        }       
        

        //影音文章
        public ActionResult Arti(int page = 1)
        {
            int pageSize = 10;
            int currentPage = page < 1 ? 1 : page;
            var arti = db.Arti.OrderByDescending(m => m.ArPublishTime).ToList();
            var result = arti.ToPagedList(currentPage, pageSize);


            return View(result);

        }
        //新手條件
        public ActionResult NewQuestion()
        {

            return View();
        }
        [HttpPost]
        public ActionResult NewQuestion(newer p)
        {
            int usid = (int)Session["result"];
            var q = db.Memb.Where(m => m.UsId == usid).FirstOrDefault();
            var pp = 0;
            int result = p.gridRadios0 + p.gridRadios1 + p.gridRadios2 + p.gridRadios3 + p.gridRadios4 + p.gridRadios5 + p.gridRadios6 + p.gridRadios7 + p.gridRadios8 + p.gridRadios9;

            if (result > 25)
                pp = 3;
            else if (result > 17 && result < 24)
                pp = 2;
            else if (result >= 1)
                pp = 1;
            else if (result < 1)
                pp = 0;

            q.ITId = pp;

            db.SaveChanges();

            return RedirectToAction("NewAns", new { itid = pp });
        }

        public ActionResult NewAns(int itid)
        {
            ViewBag.yy = itid;

            switch (itid)
            {
                case 1:
                    ViewBag.result = "101101";
                    break;
                case 2:
                    ViewBag.result = "101101";
                    break;
                case 3:
                    ViewBag.result = "101101";
                    break;

            }

            return View();
        }

    }
}