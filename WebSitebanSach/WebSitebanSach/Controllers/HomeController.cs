using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteBanSach.Models;
using PagedList;
using PagedList.Mvc;

namespace WebSiteBanSach.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        QuanLyBanSachModel db = new QuanLyBanSachModel();
        public ActionResult Index(int? page)
        {
            // Tao bien so sp/1trang
            int pageSize = 9;
            // Tao bien so trang
            int pageNumber = (page ?? 1);

            return View(db.Sach.Where(n => n.Moi == 1).OrderBy(n => n.GiaBan).ToPagedList(pageNumber, pageSize));
        }
        
    }
}