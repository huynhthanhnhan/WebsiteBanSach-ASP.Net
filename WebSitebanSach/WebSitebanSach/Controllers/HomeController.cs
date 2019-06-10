using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteBanSach.Models;

namespace WebSiteBanSach.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        QuanLyBanSachModel db = new QuanLyBanSachModel();
        public ActionResult Index()
        {
            return View(db.Sach.OrderByDescending(s=>s.NgayCapNhat).Take(9).ToList());
        }
        
    }
}