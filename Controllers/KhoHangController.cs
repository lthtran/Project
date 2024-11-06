using WebsiteThuCungBento.Models;
using System.Linq;
using System.Web.Mvc;
using WebsiteThuCungBento.App_Start;

namespace WebsiteThuCungBento.Controllers
{
    [AdminPhanQuyen(MACHUCNANG = "QL_KHOSANPHAM")]
    public class KhoHangController : Controller
    {
        DataClassesDataContext data = new DataClassesDataContext();
        // GET: Kho Hang
        [AdminPhanQuyen(MACHUCNANG = "QL_KHACHHANG")]
        public ActionResult Index()
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                var kho = from k in data.PHIEUNHAPKHOs select k;
                return View(kho);
            }
        }
        public ActionResult Details(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                var kho = from k in data.PHIEUNHAPKHOs where k.MAPHIEUNK == id select k;
                return View(kho.Single());
            }
        }
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                ViewBag.MASP = new SelectList(data.SANPHAMs.ToList().OrderBy(n => n.TENSP), "MASP", "TENSP");
                return View();
            }
        }
        [HttpPost]
        public ActionResult Create(PHIEUNHAPKHO kho)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                ViewBag.MASP = new SelectList(data.SANPHAMs.ToList().OrderBy(n => n.TENSP), "MASP", "TENSP");
                data.PHIEUNHAPKHOs.InsertOnSubmit(kho);
                SANPHAM sanpham = data.SANPHAMs.Single(n => n.MASP == kho.MASP);
                sanpham.SOLUONG = sanpham.SOLUONG + kho.SOLUONG;
                data.SubmitChanges();
                return RedirectToAction("Index", "KhoHang");
            }
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                var kho = from k in data.PHIEUNHAPKHOs where k.MAPHIEUNK == id select k;
                ViewBag.MASP = new SelectList(data.SANPHAMs.ToList().OrderBy(n => n.TENSP), "MASP", "TENSP");
                return View(kho.Single());
            }
        }
        [HttpPost, ActionName("Edit")]
        public ActionResult Capnhat(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                ViewBag.MASP = new SelectList(data.SANPHAMs.ToList().OrderBy(n => n.TENSP), "MASP", "TENSP");
                PHIEUNHAPKHO kho = data.PHIEUNHAPKHOs.SingleOrDefault(n => n.MAPHIEUNK == id);
                SANPHAM sanpham = data.SANPHAMs.SingleOrDefault(n => n.MASP == kho.MASP);
                var lstkho = from k in data.PHIEUNHAPKHOs where k.MASP == sanpham.MASP select k;
                UpdateModel(kho);
                sanpham.SOLUONG = 0;
                foreach (var item in lstkho)
                {
                    sanpham.SOLUONG = sanpham.SOLUONG + item.SOLUONG;
                }
                data.SubmitChanges();
                return RedirectToAction("Index", "KhoHang");
            }
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                var kho = from k in data.PHIEUNHAPKHOs where k.MAPHIEUNK == id select k;
                return View(kho.Single());
            }
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult Xoa(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                PHIEUNHAPKHO kho = data.PHIEUNHAPKHOs.SingleOrDefault(n => n.MAPHIEUNK == id);
                data.PHIEUNHAPKHOs.DeleteOnSubmit(kho);
                SANPHAM sanpham = data.SANPHAMs.Single(n => n.MASP == kho.MASP);
                if (sanpham.SOLUONG > kho.SOLUONG)
                {
                    sanpham.SOLUONG = sanpham.SOLUONG - kho.SOLUONG;
                }
                else
                {
                    return RedirectToAction("ThongBao", "KhoHang");
                }
                data.SubmitChanges();
                return RedirectToAction("Index", "KhoHang");
            }
        }
        public ActionResult ThongBao()
        {
            return View();
        }
    }
}