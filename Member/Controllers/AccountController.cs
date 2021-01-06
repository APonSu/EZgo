using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Member.Models;
using Member.ViewModels;

namespace Member.Controllers
{
    [HandleError]
    public class AccountController : Controller
    {
        // GET: Account
        UsersEntities db = new UsersEntities();
        //註冊功能
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public string Register(string ReEmail, string RePassword)
        {
            var result = db.Memb.Where(m => m.UsEmail == ReEmail).FirstOrDefault();
            string vailerror = "";
            if (result == null)
            {
                string s1 = Random();
                Session["email"] = ReEmail;
                //加密 密碼
                Session["pwd"] = AesEncryptBase64(RePassword);
                Session["s1"] = s1;
                //製作驗證碼
                Session["verification"] = AesEncryptBase64(s1);
                Session["time"] = DateTime.Now;
                //信件發送 內容
                vailerror = "成功";
                return vailerror;
            }
            else { 
                vailerror = "此帳號己有人使用，註冊失敗";
                return vailerror;
            }
        }
        //確認註冊資料
        public ActionResult CheckRegister(Memb p) {
            string s1 = (string)Session["s1"];
            string verification = (string)Session["verification"];
            p.UsEmail = (string)Session["email"];
            p.UsPassword = (string)Session["pwd"];
            p.UsJnD = DateTime.Now;
            if (s1 == AesDecryptBase64(verification))
            {
                db.Memb.Add(p);
                db.SaveChanges();             
            }
            Session.Clear();
            return RedirectToAction("Login");
        }
        //登入
        public ActionResult Login() {
            return View();
        }
        //登入
        [HttpPost]
        public string Login(string UsLOEmail, string UsLOPassword,string code)
        {
            string forpwd = (string)Session["FP"];
            string vailerror = "";
            if (code == Session["code"].ToString())
            {
                if (forpwd == null)
                {
                    var p = db.Memb.Where(m => m.UsEmail == UsLOEmail).FirstOrDefault();
                    if (p != null)
                    {
                        //解密 密碼
                        string pwd = AesDecryptBase64(p.UsPassword);
                        var result = db.Memb.Where(m => m.UsEmail == UsLOEmail && pwd == UsLOPassword).FirstOrDefault();
                        if (result.UsLks == false)
                        {
                            if (result == null)
                            {
                                vailerror = "信箱密碼錯誤，請重新輸入";
                                return vailerror;
                            }
                            else
                            {
                                Session["WelCome"] = result.UsEmail + "歡迎光臨";
                                Session["result"] = result.UsId;
                                vailerror = "成功";
                                return vailerror;
                            }
                        }
                        else
                        {
                            if (DateTime.Now > result.UsULkT)
                            {
                                result.UsLks = false;
                                result.UsLkT = null;
                                result.UsULkT = null;
                                db.SaveChanges();
                                Session["WelCome"] = result.UsEmail + "歡迎光臨";
                                Session["result"] = result.UsId;
                                vailerror = "成功";
                                return vailerror;
                            }
                            else
                            {
                                vailerror = "您在" + result.UsLkT + "封鎖了，將於" + result.UsULkT + "解鎖";
                                return vailerror;
                            }

                        }

                    }
                    else
                    {
                        vailerror = "信箱密碼錯誤，請重新輸入";
                        return vailerror;
                    }
                }
                else
                {
                    var p = db.Memb.Where(m => m.UsEmail == UsLOEmail & forpwd == UsLOPassword).FirstOrDefault();
                    if (p != null)
                    {
                        Session.Clear();
                        Session["WelCome"] = p.UsEmail + "歡迎光臨";
                        Session["result"] = p.UsId;
                        vailerror = "成功";
                        return vailerror;
                    }
                    else
                    {
                        vailerror = "信箱密碼錯誤，請重新輸入";
                        return vailerror;
                    }
                }
            }
            else {
                return "驗證碼錯誤";
            }
        }        //登出
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("ShowStock", "Home");
        }
        //忘記密碼
        public ActionResult ForgotPwd()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPwd(string UsEmail) {
            var p = db.Memb.Where(m => m.UsEmail == UsEmail).FirstOrDefault();
            if (p != null) {
                Session["FP"] = Random();
                Session["time"] = DateTime.Now;
                ViewBag.allow = 0;
                //信件發送 內容
                return View();
            }
            ViewBag.Message = "信箱錯誤，請重新輸入";
            return View();
        }       
        //驗證碼亂數產生
        public string Random()
        {
            string range = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-";
            char[] chars = new char[2];
            Random rd = new Random();

            for (int i = 0; i < 2; i++)
            {
                chars[i] = range[rd.Next(0, range.Length)];
            }

            string code = new string(chars);
           return code;
        }
        //發信驗證
        public void SendGmail(string content)
        {
            MailMessage mail = new MailMessage();
            //前面是發信email後面是顯示的名稱
            mail.From = new MailAddress("xxx@gmail.com", "信件名稱");

            //收信者email
            mail.To.Add("xxx@gmail.com");

            //設定優先權
            mail.Priority = MailPriority.Normal;

            //標題
            mail.Subject = "AutoEmail";

            //內容
            mail.Body = content;

            //內容使用html
            mail.IsBodyHtml = true;

            //設定gmail的smtp (這是google的)
            SmtpClient MySmtp = new SmtpClient("smtp.gmail.com", 587);

            //您在gmail的帳號密碼
            MySmtp.Credentials = new System.Net.NetworkCredential("account", "pw");

            //開啟ssl
            MySmtp.EnableSsl = true;

            //發送郵件
            MySmtp.Send(mail);

            //放掉宣告出來的MySmtp
            MySmtp = null;

            //放掉宣告出來的mail
            mail.Dispose();
        }
        /// <summary>
        /// 字串加密(非對稱式)
        /// </summary>
        /// <param name="Source">加密前字串</param>
        /// <param name="CryptoKey">加密金鑰</param>
        /// <returns>加密後字串</returns>
        public  string AesEncryptBase64(string SourceStr)
        {
            string encrypt = "";
            try
            {
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
                byte[] key = sha256.ComputeHash(Encoding.UTF8.GetBytes("我是傳奇"));
                byte[] iv = md5.ComputeHash(Encoding.UTF8.GetBytes("我是傳奇"));
                aes.Key = key;
                aes.IV = iv;

                byte[] dataByteArray = Encoding.UTF8.GetBytes(SourceStr);
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(dataByteArray, 0, dataByteArray.Length);
                    cs.FlushFinalBlock();
                    encrypt = Convert.ToBase64String(ms.ToArray());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
            return encrypt;
        }
        /// <summary>
        /// 字串解密(非對稱式)
        /// </summary>
        /// <param name="Source">解密前字串</param>
        /// <param name="CryptoKey">解密金鑰</param>
        /// <returns>解密後字串</returns>
        public  string AesDecryptBase64(string SourceStr)
        {
            string decrypt = "";
            try
            {
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
                byte[] key = sha256.ComputeHash(Encoding.UTF8.GetBytes("我是傳奇"));
                byte[] iv = md5.ComputeHash(Encoding.UTF8.GetBytes("我是傳奇"));
                aes.Key = key;
                aes.IV = iv;

                byte[] dataByteArray = Convert.FromBase64String(SourceStr);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(dataByteArray, 0, dataByteArray.Length);
                        cs.FlushFinalBlock();
                        decrypt = Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
            catch 
            {
                decrypt = "錯誤";
            }
            return decrypt;
        }
        //會員專區
        public ActionResult Index()
        {

            int usid = (int)Session["result"];
            VM vme = new VM()
            {
                Memb = db.Memb.Where(m => m.UsId == usid).ToList(),
                Collection = db.Collection.Where(m => m.UsId == usid).ToList()

            };
            vme.Memb[0].UsPassword = AesDecryptBase64(vme.Memb[0].UsPassword);
            return View(vme);
        }
        [HttpPost]
        public ActionResult Index(VM p)
        {
            int usid = (int)Session["result"];
            var q = db.Memb.Where(m => m.UsId == usid).FirstOrDefault();

            q.UsPassword = p.Memb[0].UsPassword;
            q.UsName = p.Memb[0].UsName;
            q.UsSex = p.Memb[0].UsSex;
            q.UsPh = p.Memb[0].UsPh;
            q.UsPassword = AesEncryptBase64(p.Memb[0].UsPassword);
            db.SaveChanges();
            VM vme = new VM()
            {
                Memb = db.Memb.Where(m => m.UsId == usid).ToList(),
                Collection = db.Collection.Where(m => m.UsId == usid).ToList()

            };
            return View(vme);
        }

        public JsonResult MesShow()
        {
            int usid = (int)Session["result"];
            List<string> eva = new List<string>();
            var e = db.Mes.Where(m => m.UsId == usid).ToList();
             
            for (int q = 0; q < e.Count(); q++) {
                var p = e[q].MsId;
                var evaluation = db.MesS.Where(m => m.MsId == p).OrderByDescending(m => m.MsSDate).ToList();
                for (int i = 0; i < evaluation.Count(); i++)
                {
                    string text = "";
                    if (evaluation[i].MsGd == 1)
                        text = "<a href='/Home/ShowStock?stid=" + evaluation[i].StId + "#" + evaluation[i].MsId + "'><div class='dropdown-item text-white bg-secondary mb-1 mshh'><div>" + evaluation[i].Memb.UsEmail + "對你的發言按讚</div><div><small>" + evaluation[i].MsSDate + "</small></div></div></a>";
                    if (evaluation[i].MsBd == 1)
                        text = "<a href='/Home/ShowStock?stid=" + evaluation[i].StId + "#" + evaluation[i].MsId + "'><div class='dropdown-item text-white bg-secondary mb-1 mshh'><div>" + evaluation[i].Memb.UsEmail + "對你的發言按爛</div><div><small>" + evaluation[i].MsSDate + "</small></div></div></a>";
                    eva.Add(text);
                }

            }

            return Json(eva, JsonRequestBehavior.AllowGet);
        }

        public string aa(string z) {

            
            int q = z.IndexOf("按");
            q = q + 13;
            string p = z.Substring(q, z.Length-q);
            
            int q1 = p.IndexOf("<");
            q1 = q1 + 12;
            string p1 = p.Substring(0,q1);
            return p1;
        }
        
        private void PaintInterLine(Graphics g, int num, int width, int height)
        {
            Random r = new Random();
            int startX, startY, endX, endY;
            for (int i = 0; i < num; i++)
            {
                startX = r.Next(0, width);
                startY = r.Next(0, height);
                endX = r.Next(0, width);
                endY = r.Next(0, height);
                g.DrawLine(new Pen(Brushes.Red), startX, startY, endX, endY);
            }
        }
        public ActionResult GetValidateCode()
        {
            byte[] data = null;
            string code = Random();
            Session["code"] = code;
            //定義一個畫板
            MemoryStream ms = new MemoryStream();
            using (Bitmap map = new Bitmap(200, 40))
            {
                //畫筆,在指定畫板畫板上畫圖
                //g.Dispose();
                using (Graphics g = Graphics.FromImage(map))
                {
                    g.Clear(Color.White);
                    g.DrawString(code, new Font("黑體", 18.0F), Brushes.Blue, new Point(10, 8));
                    //繪製干擾線(數字代表幾條)
                    PaintInterLine(g, 10, map.Width, map.Height);
                }
                map.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            data = ms.GetBuffer();
            return File(data, "image/jpeg");
        }
    }

}
