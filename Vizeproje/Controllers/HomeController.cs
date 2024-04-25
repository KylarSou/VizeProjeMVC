using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vizeproje.Models;
using System.Data.SqlClient;
using System.Data;

namespace Vizeproje.Controllers
{

    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult idari()
        {
            ViewBag.Message = "İdari Sayfası";
            return View();
        }

        public ActionResult Raporlar()
        {
            ViewBag.Message = "Rapor Sayfası";
            return View();
        }

        public ActionResult Yonetici()
        {
            ViewBag.Message = "Yonetici sayfası";
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            // Kullanıcı adı ve şifre kontrolü
            using (var db = new UniversiteEntities())
            {
                // Kullanıcı adı ve şifre kontrolü
                var user = db.LoginTb.FirstOrDefault(u => u.Ad == username && u.Sifre == password);
                if (user != null)
                {
                    // Giriş başarılıysa oturum değişkeni ayarla
                    Session["LoggedInUser"] = user;

                    // Giriş başarılı mesajını göster
                    ViewBag.SuccessMessage = "Giriş başarılı!";

                    // Ana sayfaya yönlendir
                    return RedirectToAction("Index");
                }
                else
                {
                    // Giriş başarısızsa hata mesajını göster
                    ViewBag.ErrorMessage = "Geçersiz kullanıcı adı veya şifre";
                }
            }

            // Index sayfasına geri dön
            return View("Index");
        }
        public ActionResult Logout()
        {
            // Oturumu sil
            Session["LoggedInUser"] = null;

            // Çıkış yapıldı mesajını göster
            ViewBag.SuccessMessage = "Çıkış yapıldı.";

            // Index sayfasına yönlendir
            return RedirectToAction("Index");
        }
        private UniversiteEntities db = new UniversiteEntities();

        public SqlDbType Birim { get; private set; }
        public SqlDbType Ad { get; private set; }

        [HttpPost]
        public ActionResult AddStudent(string id, string ad, string sifre)
        {
            if (Session["LoggedInUser"] == null)
            {
                return RedirectToAction("Index");
            }

            string query = "INSERT INTO LoginTb (id, Ad, Sifre) VALUES (@Id, @Ad, @Sifre)";
            db.Database.ExecuteSqlCommand(query, new SqlParameter("@Id", id), new SqlParameter("@Ad", ad), new SqlParameter("@Sifre", sifre));

            ViewBag.SuccessMessage = "Öğrenci başarıyla eklendi.";
            return RedirectToAction("Yonetici");
        }

        [HttpPost]
        public ActionResult AddBolum(string Ogrencid, string Ders, int Kredi)
        {
            // Oturum kontrolü
            if (Session["LoggedInUser"] == null)
            {
                return RedirectToAction("Index");
            }

            // Veritabanına ekleme işlemi
            using (var db = new UniversiteEntities())
            {
                string query = "INSERT INTO BolumTb (Ogrencid, Ders, Kredi) VALUES (@Ogrencid, @Ders, @Kredi)";
                db.Database.ExecuteSqlCommand(query,
                    new SqlParameter("@Ogrencid", Ogrencid),
                    new SqlParameter("@Ders", Ders),
                    new SqlParameter("@Kredi", Kredi));
            }

            ViewBag.SuccessMessage = "Bölüm başarıyla eklendi.";
            return RedirectToAction("Yonetici");
        }

        public ActionResult ShowBolum()
        {
            using (var db = new UniversiteEntities())
            {
                var bolumler = db.BolumTb.ToList(); // Bütün bölümleri getir

                return View(bolumler);
            }
        }
        

        [HttpPost]
        public ActionResult AddAkademik(int id, string Ogretmenad, string BolumDers, DateTime Baslangic, DateTime? Ayrilis = null)
        {
            if (Session["LoggedInUser"] == null)
            {
                return RedirectToAction("Index");
            }

            using (var db = new UniversiteEntities())
            {
                string query;
                if (Ayrilis.HasValue)
                {
                    query = "INSERT INTO Akademik (id, Ogretmenad, BolumDers, Baslangic, Ayrilis) VALUES (@id, @Ogretmenad, @BolumDers, @Baslangic, @Ayrilis)";
                }
                else
                {
                    query = "INSERT INTO Akademik (id, Ogretmenad, BolumDers, Baslangic) VALUES (@id, @Ogretmenad, @BolumDers, @Baslangic)";
                }

                var parameters = new List<SqlParameter>
        {
            new SqlParameter("@id", id),
            new SqlParameter("@Ogretmenad", Ogretmenad),
            new SqlParameter("@BolumDers", BolumDers),
            new SqlParameter("@Baslangic", Baslangic)
        };

                if (Ayrilis.HasValue)
                {
                    parameters.Add(new SqlParameter("@Ayrilis", Ayrilis.Value));
                }

                db.Database.ExecuteSqlCommand(query, parameters.ToArray());
            }

            ViewBag.SuccessMessage = "Akademik bilgiler başarıyla eklendi.";
            return RedirectToAction("Contact");
        }

        public ActionResult ShowAkademik()
        {
            using (var db = new UniversiteEntities())
            {
                var akademikBilgiler = db.Akademik.ToList(); // Akademik tablosundaki tüm verileri getir

                ViewBag.AkademikBilgiler = akademikBilgiler;
            }

            return View();
        }

        [HttpPost]
        public ActionResult AddIdari(int id, string birim, string ad)
        {
            if (Session["LoggedInUser"] == null)
            {
                return RedirectToAction("Index");
            }

            using (var db = new UniversiteEntities())
            {
                string query = "INSERT INTO Idari (id, Birim, Ad) VALUES (@Id, @Birim, @Ad)";
                db.Database.ExecuteSqlCommand(query, new SqlParameter("@Id", id), new SqlParameter("@Birim", birim), new SqlParameter("@Ad", ad));
            }

            ViewBag.SuccessMessage = "İdari bilgi başarıyla eklendi.";
            return RedirectToAction("idari");
        }

        [HttpPost]
        public ActionResult AddReport(int id, string mezunlar, string aktif)
        {
            if (Session["LoggedInUser"] == null)
            {
                return RedirectToAction("Index");
            }

            using (var db = new UniversiteEntities())
            {
                string query = "INSERT INTO Raporlar (id, Mezunlar, Aktif) VALUES (@Id, @Mezunlar, @Aktif)";
                db.Database.ExecuteSqlCommand(query, new SqlParameter("@Id", id), new SqlParameter("@Mezunlar", mezunlar), new SqlParameter("@Aktif", aktif));
            }

            ViewBag.SuccessMessage = "Rapor başarıyla eklendi.";
            return RedirectToAction("Raporlar");
        }





    }
}