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
        //[HttpGet]
        //public ActionResult Create()
        //{
        //    if (Session["Taikhoanadmin"] == null)
        //        return RedirectToAction("dangnhap", "Admin");
        //    else
        //    {
        //        ViewBag.MASP = new SelectList(data.SANPHAMs.ToList().OrderBy(n => n.TENSP), "MASP", "TENSP");
        //        return View();
        //    }
        //}
        //[HttpPost]
        //public ActionResult Create(PHIEUNHAPKHO kho)
        //{
        //    if (Session["Taikhoanadmin"] == null)
        //        return RedirectToAction("dangnhap", "Admin");
        //    else
        //    {
        //        ViewBag.MASP = new SelectList(data.SANPHAMs.ToList().OrderBy(n => n.TENSP), "MASP", "TENSP");
        //        data.PHIEUNHAPKHOs.InsertOnSubmit(kho);
        //        SANPHAM sanpham = data.SANPHAMs.Single(n => n.MASP == kho.MASP);
        //        sanpham.SOLUONG = sanpham.SOLUONG + kho.SOLUONG;
        //        data.SubmitChanges();
        //        return RedirectToAction("Index", "KhoHang");
        //    }
        //}
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                // Lấy danh sách sản phẩm kèm số lượng tồn kho
                ViewBag.MASP = data.SANPHAMs
                    .ToList()
                    .OrderBy(n => n.TENSP)
                    .Select(sp => new SelectListItem
                    {
                        Value = sp.MASP.ToString(),
                        Text = $"{sp.TENSP} (Còn lại: {sp.SOLUONG})"
                    });

                return View();
            }
        }

        //[HttpPost]
        //public ActionResult Create(PHIEUNHAPKHO kho)
        //{
        //    if (Session["Taikhoanadmin"] == null)
        //        return RedirectToAction("dangnhap", "Admin");
        //    else
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            data.PHIEUNHAPKHOs.InsertOnSubmit(kho);

        //            // Cập nhật số lượng sản phẩm trong kho
        //            SANPHAM sanpham = data.SANPHAMs.Single(n => n.MASP == kho.MASP);
        //            sanpham.SOLUONG = sanpham.SOLUONG + kho.SOLUONG;

        //            data.SubmitChanges();
        //            return RedirectToAction("Index", "KhoHang");
        //        }

        //        // Nếu có lỗi, tái tạo ViewBag.MASP
        //        ViewBag.MASP = data.SANPHAMs
        //            .ToList()
        //            .OrderBy(n => n.TENSP)
        //            .Select(sp => new SelectListItem
        //            {
        //                Value = sp.MASP.ToString(),
        //                Text = $"{sp.TENSP} (Còn lại: {sp.SOLUONG})"
        //            });

        //        return View(kho);
        //    }
        //}
        [HttpPost]
        public ActionResult Create(PHIEUNHAPKHO kho)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                // Kiểm tra số lượng phải lớn hơn 0
                if (kho.SOLUONG <= 0)
                {
                    ModelState.AddModelError("SOLUONG", "Số lượng phải lớn hơn 0.");
                }

                if (ModelState.IsValid)
                {
                    // Thêm phiếu nhập kho vào cơ sở dữ liệu
                    data.PHIEUNHAPKHOs.InsertOnSubmit(kho);

                    // Cập nhật số lượng sản phẩm trong kho
                    SANPHAM sanpham = data.SANPHAMs.Single(n => n.MASP == kho.MASP);
                    sanpham.SOLUONG = sanpham.SOLUONG + kho.SOLUONG;

                    data.SubmitChanges();
                    return RedirectToAction("Index", "KhoHang");
                }

                // Nếu có lỗi, tái tạo ViewBag.MASP để hiển thị danh sách
                ViewBag.MASP = data.SANPHAMs
                    .ToList()
                    .OrderBy(n => n.TENSP)
                    .Select(sp => new SelectListItem
                    {
                        Value = sp.MASP.ToString(),
                        Text = $"{sp.TENSP} (Còn lại: {sp.SOLUONG})"
                    });

                return View(kho);
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
                // Find the item to delete
                PHIEUNHAPKHO phieunhapkho = data.PHIEUNHAPKHOs.SingleOrDefault(p => p.MAPHIEUNK == id);

                if (phieunhapkho != null)
                {
                    // Delete the item
                    data.PHIEUNHAPKHOs.DeleteOnSubmit(phieunhapkho);
                    data.SubmitChanges();

                    // Return success response
                    return Json(new { success = true, message = "Xóa thành công!" });
                }

                // If not found, return failure response
                return Json(new { success = false, message = "Không tìm thấy phiếu nhập kho cần xóa." });
            }
        }
        public ActionResult ThongBao()
        {
            return View();
        }
    }
}