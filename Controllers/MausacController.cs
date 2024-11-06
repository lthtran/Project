using WebsiteThuCungBento.Models;
using System.Linq;
using System.Web.Mvc;
using WebsiteThuCungBento.App_Start;

namespace WebsiteThuCungBento.Controllers
{
    [AdminPhanQuyen(MACHUCNANG = "QL_MAUSAC")]
    public class MausacController : Controller
    {
        // GET: Mausac
        DataClassesDataContext data = new DataClassesDataContext();
        // GET: Loai
        public ActionResult Index()
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                var mau = from m in data.MAUSACs select m;
                return View(mau);
            }
        }
        public ActionResult Details(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                var mau = from m in data.MAUSACs where m.MAMAUSAC == id select m;
                return View(mau.Single());
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Create(MAUSAC mau)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                data.MAUSACs.InsertOnSubmit(mau);
                data.SubmitChanges();
                return RedirectToAction("Index", "Mausac");
            }
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                var mau = from m in data.MAUSACs where m.MAMAUSAC == id select m;
                return View(mau.Single());
            }
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult Capnhat(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                MAUSAC mau = data.MAUSACs.SingleOrDefault(n => n.MAMAUSAC == id);
                UpdateModel(mau);
                data.SubmitChanges();
                return RedirectToAction("Index", "Mausac");
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                var mau = from m in data.MAUSACs where m.MAMAUSAC == id select m;
                return View(mau.Single());
            }
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult Xoa(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                MAUSAC mau = data.MAUSACs.SingleOrDefault(n => n.MAMAUSAC == id);
                data.MAUSACs.DeleteOnSubmit(mau);
                data.SubmitChanges();
                return RedirectToAction("Index", "Mausac");
            }
        }
    }
}