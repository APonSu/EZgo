using Member.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using Member.ViewModels;
using System.Web;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Member.Controllers
{
    [HandleError]
    public class BackController : Controller
    {
        UsersEntities db = new UsersEntities();
        //登入
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string MaAct, string MaPwd)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string pwd = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(MaPwd)), 4, 8);
            MaPwd = pwd.Replace("-", "");
            var p = db.Mager.Where(m => m.MaAct == MaAct & m.MaPwd == MaPwd).FirstOrDefault();

            if (p == null)
            {
                ViewBag.Message = "信箱密碼錯誤，請重新輸入";
                return View();
            }
            Session["MaAct"] = p.MaAct;
            Session["Authority"] = p.Authority;
            if (p.Authority.Contains("B")) {
                var q = p.Authority;
                var count = q.IndexOf("B")+1;
                var number = int.Parse(q.Substring(count, 1));
                Session["AutB"] = Two(number);
            }
            if (p.Authority.Contains("C"))
            {
                var q = p.Authority;
                var count = q.IndexOf("C") + 1;
                var number = int.Parse(q.Substring(count, 1));
                Session["AutC"] = Two(number);
            }
            if (p.Authority.Contains("D"))
            {
                var q = p.Authority;
                var count = q.IndexOf("D") + 1;
                var number = int.Parse(q.Substring(count, 1));
                Session["AutD"] = Two(number);
            }
            return RedirectToAction("Violmember");
        }
        //登出
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
        //查詢會員列表
        public ActionResult Violmember(Membinquiry p, int page = 1, int ChJnD = 0)
        {
            if (Session["Authority"] == null || Session["Authority"].ToString().Contains("B") != true)
            {
                ViewBag.error = "您無此權限";
            }
            if (p.UsEmail != null)
            {
                ViewBag.UsEmail = p.UsEmail;
            }
            ViewBag.ITId = p.ITId;

            ViewBag.UsLkC = p.UsLkC;

            ViewBag.UsULkT = p.UsULkT;

            if (p.UsLks != null)
            {
                ViewBag.UsLks = p.UsLks;
            }

            ViewBag.UsSex = p.UsSex;

            ViewBag.ChJnD = ChJnD;

            var UsJnD = p.UsJnD.ToString("yyyy-MM-dd");
            ViewBag.UsJnD = UsJnD;

            ViewBag.UsLks = p.UsLks;

            ViewBag.UsULkT = p.UsULkT;
            int pageSize = 3;
            int currentPage = page < 1 ? 1 : page;
            List<Memb> member = new List<Memb>();
            if (page == 1)
            {
                if (p.UsEmail != null)
                    member = db.Memb.Where(m => m.UsEmail == p.UsEmail).ToList();
                else
                {
                    member = db.Memb.ToList();
                    if (p.UsLkC > 0)
                        member = member.Where(m => m.UsLkc == p.UsLkC).OrderBy(m => m.UsId).ToList();
                    if (p.ITId > 0)
                        member = member.Where(m => m.ITId == p.ITId).OrderBy(m => m.UsId).ToList();
                    if (ChJnD > 0)
                        if (ChJnD == 1)
                            member = member.Where(m => m.UsJnD <= p.UsJnD.AddDays(1)).OrderBy(m => m.UsId).ToList();
                        else
                            member = member.Where(m => m.UsJnD >= p.UsJnD.AddDays(1)).OrderBy(m => m.UsId).ToList();
                    if (p.UsULkT > 0)
                        member = member.Where(m => m.UsJnD <= DateTime.Now.AddDays(p.UsULkT)).OrderBy(m => m.UsId).ToList();
                    if (p.UsLks == "on")
                        member = member.Where(m => m.UsLks == true).OrderBy(m => m.UsId).ToList();
                    if (p.UsSex == "true")
                        member = member.Where(m => m.UsSex == true).OrderBy(m => m.UsId).ToList();
                    if (p.UsSex == "false")
                        member = member.Where(m => m.UsSex == false).OrderBy(m => m.UsId).ToList();
                }
                var count = member.Count();
                if (count > pageSize)
                {
                    Session["ser"] = member;
                }
            }
            else
                member = (List<Memb>)Session["ser"];
            var result = member.ToPagedList(currentPage, pageSize);
            return View(result);
        }
        // 舉報列表
        public ActionResult Violmes(int Maincurrent = 1, int Contcurrent = 1, int msid = 0)
        {
            if (Session["Authority"] == null || Session["Authority"].ToString().Contains("C") != true)
            {
                ViewBag.error = "您無此權限";
            }
            //被舉報內容
            var Maintotal = db.Mes.Where(m => m.MsRpt == true).Count();
            int MainpageSize = 5;
            int Mainskipcount = (Maincurrent - 1) * MainpageSize;
            ViewBag.total1 = Maintotal;
            ViewBag.pageSize1 = MainpageSize;
            ViewBag.current1 = Maincurrent;
            //舉報原因
            var Conttotal = db.MesR.Where(m => m.MsId == msid).Count();
            int ContpageSize = 10;
            int Contskipcount = (Contcurrent - 1) * ContpageSize;
            ViewBag.total2 = Conttotal;
            ViewBag.pageSize2 = ContpageSize;
            ViewBag.current2 = Contcurrent;


            ViewBag.msid = msid;
            MesRpt vme = new MesRpt()
            {

                Mes = db.Mes.Where(m => m.MsRpt == true).OrderBy(m => m.UsId).Skip(Mainskipcount).Take(MainpageSize),
                MesR = db.MesR.Where(m => m.MsId == msid && m.MsCheck == false).OrderBy(m => m.UsId).Skip(Contskipcount).Take(ContpageSize)
            };


            return View(vme);
        }
        //舉報列表 確認異常
        public ActionResult VioMesOK(int msid) 
        {
            var p = db.Mes.Where(m => m.MsId == msid).FirstOrDefault();
            var q = db.MesR.Where(m => m.MsId == msid).ToList();
            p.MsRpt = false;
            foreach (var item in q) {
                item.MsCheck = true;
            }
            db.SaveChanges();
            return RedirectToAction("Violmes");
        }
        //舉報列表 確認無異
        public ActionResult VioMesNo(int msid)
        {
            var p = db.Mes.Where(m => m.MsId == msid).FirstOrDefault();
            var q = db.MesR.Where(m => m.MsId == msid).ToList();
            p.MsRpt = false;
            p.MsE = true;
            p.Memb.UsLkc = p.Memb.UsLkc + 1;
            p.Memb.UsLks = true;
            p.Memb.UsLkT = DateTime.Now;
            switch (p.Memb.UsLkc)
            {
                case 3:
                    p.Memb.UsULkT = DateTime.Now.AddYears(1);
                    break;
                case 2:
                    p.Memb.UsULkT = DateTime.Now.AddMonths(1);
                    break;
                case 1:
                    p.Memb.UsULkT = DateTime.Now.AddDays(3);
                    break;

            }          

            foreach (var item in q)
            {
                item.MsCheck = true;
            }
            db.SaveChanges();
            return RedirectToAction("Violmes");
        }
        //影音文章列表
        public ActionResult ArtiIndex(int page = 1)
        {
            if (Session["Authority"] == null || Session["Authority"].ToString().Contains("D") != true)
            {
                ViewBag.error = "您無此權限";
            }
            int pageSize = 10;
            int currentPage = page < 1 ? 1 : page;
            var arti = db.Arti.OrderByDescending(m => m.ArPublishTime).ToList();
            var result = arti.ToPagedList(currentPage, pageSize);
            ViewBag.ArtiCreateResult = TempData["ArtiCreateResult"]; //顯示新增文章成功視窗用
            ViewBag.ArtiEditResult = TempData["ArtiEditResult"]; //顯示編輯文章成功視窗用    
            return View(result);
        }
        //新增影音文章
        public ActionResult ArtiCreate()
        {
            if (Session["Authority"] == null || Session["Authority"].ToString().Contains("D") != true)
            {
                ViewBag.error = "您無此權限";
            }
            return View();
        }
        [HttpPost]
        public ActionResult ArtiCreate(Arti article, HttpPostedFileBase ArFile, string hd)
        {
            //上傳檔案
            string fileName = "";
            if (ArFile != null)
            {
                if (ArFile.ContentLength > 0)
                {
                    fileName = Path.GetFileName(ArFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/EZgoPDF"), fileName);
                    ArFile.SaveAs(path);
                    article.ArFromWeb = hd;

                }
            }
            var result = db.Arti.Where(m => m.ArFromWeb == article.ArFromWeb).FirstOrDefault();
            if (result != null)
            {
                ViewBag.Message = "此文章已上架，新增失敗";
                return View(article);
            }
            else
            {
                article.ArPublishTime = DateTime.Now;
                db.Arti.Add(article);
                db.SaveChanges();
                TempData["ArtiCreateResult"] = true;  //顯示新增文章成功視窗用
                return RedirectToAction("ArtiIndex");
            }
        }
        //影音文章編輯
        public ActionResult ArtiEdit(int id)
        {
            if (Session["Authority"] == null || Session["Authority"].ToString().Contains("D") != true)
            {
                ViewBag.error = "您無此權限";
            }
            Arti p = db.Arti.Find(id);
            return View(p);
        }
        [HttpPost]
        public ActionResult ArtiEdit(Arti article)
        {
            var arti = db.Arti.Where(m => m.ArId == article.ArId).FirstOrDefault();
            arti.ArTitle = article.ArTitle;
            arti.ArFromWeb = article.ArFromWeb;
            arti.ArType = article.ArType;
            arti.ArCont = article.ArCont;
            arti.ArPublishTime = DateTime.Now;
            db.SaveChanges();
            TempData["ArtiEditResult"] = true;  //顯示編輯文章成功視窗用
            return RedirectToAction("ArtiIndex");
        }
        //刪除影音文章
        public ActionResult ArtiDelete(int? id)
        {
            var article = db.Arti.Find(id);
            db.Arti.Remove(article);
            db.SaveChanges();
            return RedirectToAction("ArtiIndex");

        }
        //封鎖會員
        public ActionResult Lock(int usid, int msid)
        {
            var p = db.Memb.Where(m => m.UsId == usid).FirstOrDefault();
            p.UsLks = true;
            p.UsLkT = DateTime.Now;
            p.UsULkT = DateTime.Now.AddYears(100);
            var x = db.Mes.Where(m => m.MsId == msid).FirstOrDefault();
            var q = db.MesR.Where(m => m.MsId == msid).ToList();
            x.MsRpt = false;
            foreach (var item in q)
            {
                item.MsCheck = true;
            }
            db.SaveChanges();
            return RedirectToAction("Violmes");
        }
        //判斷權限
        public string Two(int v1)
        {
            string s1 = Ten(v1);
            string t1 = "";
            for (int i = 0; i < s1.Length; i++)
            {
                if (s1.Substring(s1.Length-1-i, 1) == "1")
                {
                    if (i == 0)
                        t1 += "新增";
                    if (i == 1)
                        t1 += "修改";
                    if (i == 2)
                        t1 += "刪除";

                }
            }
            return t1;
        }
        //計算某數的10進位轉2進位
        public string Ten(int n)
        {
            
            string result = "";
            while (true)
            {
                int m = n % 2;    //取餘數
                int k = (n / 2); //取商數(成為下一輪的被除數)
                result += m.ToString();
                n = k;
                if (n == 0)
                    break;
            }
            var result2 = "";
            for (int i = result.Length - 1; i >= 0; i--)
            {
                result2 += result[i];
            }
            return result2;
        }
        //管理員權限列表
        public ActionResult AuthIndex(int page = 1)
        {
            if (Session["Authority"] == null || Session["Authority"].ToString().Contains("A") != true)
            {
                ViewBag.error = "您無此權限";
            }
            int pageSize = 10;
            int currentPage = page < 1 ? 1 : page;
            var manager = db.Mager.Where(m => m.Authority.Contains("A") == false).OrderBy(m => m.MaId).ToList();
            var result = manager.ToPagedList(currentPage, pageSize);
            ViewBag.AuthCreateResult = TempData["AuthCreateResult"]; //顯示新增管理員成功視窗用
            ViewBag.AuthEditResult = TempData["AuthEditResult"]; //顯示編輯管理員成功視窗用
            return View(result);
        }
        //新增管理員
        public ActionResult AuthCreate()
        {
            if (Session["Authority"] == null || Session["Authority"].ToString().Contains("A") != true)
            {
                ViewBag.error = "您無此權限";
            }
            return View();
        }
        //接收處理新增管理員表單資料
        /// MD5 16位加密 加密後密碼為大寫
        /// </summary>
        /// <param name="ConvertString"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AuthCreate(Mager mager, string bc, string cc, string dc)
        {
            var result = db.Mager.Where(m => m.MaAct == mager.MaAct).FirstOrDefault();
            if (result != null)
            {
                ViewBag.Message = "此管理員帳號已被註冊";
                return View();
            }
            else if (ModelState.IsValid)
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                string pwd = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(mager.MaPwd)), 4, 8);
                mager.MaPwd = pwd.Replace("-", "");
                mager.Authority = bc + cc + dc;
                db.Mager.Add(mager);
                db.SaveChanges();
                TempData["AuthCreateResult"] = true;
                return RedirectToAction("AuthIndex");
            }
            return View();
        }
        //編輯管理員權限
        public ActionResult AuthEdit(int? id)
        {
            if (Session["Authority"] == null || Session["Authority"].ToString().Contains("A") != true)
            {
                ViewBag.error = "您無此權限";
            }
            Mager mager = db.Mager.Find(id);

            return View(mager);
        }
        //接收處理編輯管理員表單資料
        [HttpPost]
        public ActionResult AuthEdit(Mager mager, string bc, string cc, string dc)
        {
            var mag = db.Mager.Where(m => m.MaId == mager.MaId).FirstOrDefault();
            if (ModelState.IsValid)
            {
                mag.Authority = bc + cc + dc;
                db.SaveChanges();
                TempData["AuthEditResult"] = true;
                return RedirectToAction("AuthIndex");
            }
            return View();
        }
        //刪除管理員
        public ActionResult AuthDelete(int? id)
        {
            Mager mager = db.Mager.Find(id);
            db.Mager.Remove(mager);
            db.SaveChanges();
            return RedirectToAction("AuthIndex");
        }
        public ActionResult AD()
        {
            return View();
        }
        //廣告上傳修改頁面
        public ActionResult ADEdit()
        {
            return View();
        }

        //廣告上傳修改頁面資料
        [HttpPost]
        public ActionResult ADEdit(HttpPostedFileBase ADimg)
        {
            string fileName = "";
            if (ADimg != null)
            {
                if (ADimg.ContentLength > 0)
                {
                    fileName = Path.GetFileName(ADimg.FileName);
                    var path = Path.Combine(Server.MapPath("~/img_AD"), fileName);
                    ADimg.SaveAs(path);
                }
            }
            return RedirectToAction("AD");
        }
    }
}