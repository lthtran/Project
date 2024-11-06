using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteThuCungBento.App_Start;
using WebsiteThuCungBento.Models;

namespace WebsiteThuCungBento.Controllers
{
    [AdminPhanQuyen(MACHUCNANG = "QL_SANPHAM")]
    public class SanPhamController : Controller
    {
        DataClassesDataContext data = new DataClassesDataContext();
        // GET: SanPham
        public ActionResult Index(int? page)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {

                if (page == null) page = 1;
                int pageSize = 9;
                int pageNumber = (page ?? 1);
                var sanpham = from g in data.SANPHAMs select g;
                return View(sanpham.ToPagedList(pageNumber, pageSize));
            }
        }
        public ActionResult Details(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                var sanpham = from g in data.SANPHAMs where g.MASP == id select g;
                if (sanpham == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                return View(sanpham.Single());
            }
        }
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                ViewBag.MALOAI = new SelectList(data.LOAIs.ToList().OrderBy(n => n.TENLOAI), "MALOAI", "TENLOAI");
                ViewBag.MATH = new SelectList(data.THUONGHIEUs.ToList().OrderBy(n => n.TENTH), "MATH", "TENTH");
                ViewBag.MAMAUSAC = new SelectList(data.MAUSACs.ToList().OrderBy(n => n.TENMAUSAC), "MAMAUSAC", "TENMAUSAC");
                return View();
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(FormCollection collection, SANPHAM sanpham, HttpPostedFileBase fileUpload)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                ViewBag.MALOAI = new SelectList(data.LOAIs.ToList().OrderBy(n => n.TENLOAI), "MALOAI", "TENLOAI");
                ViewBag.MATH = new SelectList(data.THUONGHIEUs.ToList().OrderBy(n => n.TENTH), "MATH", "TENTH");
                ViewBag.MAMAUSAC = new SelectList(data.MAUSACs.ToList().OrderBy(n => n.TENMAUSAC), "MAMAUSAC", "TENMAUSAC");
                if (fileUpload == null)
                {
                    ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                    return View();
                }
                else
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/img/"), fileName);
                    if (System.IO.File.Exists(path))
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    else
                    {
                        fileUpload.SaveAs(path);
                    }
                    sanpham.HINHANH = fileName;
                    data.SANPHAMs.InsertOnSubmit(sanpham);
                    data.SubmitChanges();
                    return RedirectToAction("Index", "SanPham");
                }

            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                var sanpham = from g in data.SANPHAMs where g.MASP == id select g;

                ViewBag.MALOAI = new SelectList(data.LOAIs.ToList().OrderBy(n => n.TENLOAI), "MALOAI", "TENLOAI");
                ViewBag.MATH = new SelectList(data.THUONGHIEUs.ToList().OrderBy(n => n.TENTH), "MATH", "TENTH");
                ViewBag.MAMAUSAC = new SelectList(data.MAUSACs.ToList().OrderBy(n => n.TENMAUSAC), "MAMAUSAC", "TENMAUSAC");
                return View(sanpham.Single());
            }
        }
        [HttpPost, ActionName("Edit")]
        public ActionResult Capnhat(int id, HttpPostedFileBase fileUpload)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                SANPHAM sanpham = data.SANPHAMs.SingleOrDefault(g => g.MASP == id);
                ViewBag.MALOAI = new SelectList(data.LOAIs.ToList().OrderBy(n => n.TENLOAI), "MALOAI", "TENLOAI");
                ViewBag.MATH = new SelectList(data.THUONGHIEUs.ToList().OrderBy(n => n.TENTH), "MATH", "TENTH");
                ViewBag.MAMAUSAC = new SelectList(data.MAUSACs.ToList().OrderBy(n => n.TENMAUSAC), "MAMAUSAC", "TENMAUSAC");
                if (fileUpload == null)
                {
                    ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                    return View();
                }
                else
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/img/"), fileName);
                    fileUpload.SaveAs(path);
                    sanpham.HINHANH = fileName;
                    UpdateModel(sanpham);
                    data.SubmitChanges();
                    return RedirectToAction("Index", "SanPham");
                }
            }
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                var sanpham = from g in data.SANPHAMs where g.MASP == id select g;
                return View(sanpham.Single());
            }
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult Xoa(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                SANPHAM sanpham = data.SANPHAMs.SingleOrDefault(g => g.MASP == id);
                var kichthuoc = from KICHTHUOC in data.KICHTHUOCs where KICHTHUOC.MASP == id select KICHTHUOC;
                var hinh = from HINH in data.HINHs where HINH.MASP == id select HINH;
                var kho = from PHIEUNHAPKHO in data.PHIEUNHAPKHOs where PHIEUNHAPKHO.MASP == id select PHIEUNHAPKHO;
                var dathang = from CTDONDATHANG in data.CTDONDATHANGs where CTDONDATHANG.MASP == id select CTDONDATHANG;
                var dondathang = from DONDATHANG in data.DONDATHANGs select DONDATHANG;
                foreach (var item in dathang)
                {
                    data.CTDONDATHANGs.DeleteOnSubmit(item);
                }
                /*foreach (var item in dathang)
                {
                    foreach (var itam in dondathang)
                    {
                        if (itam.MADH != item.MADH)
                        {
                            data.DONDATHANGs.DeleteOnSubmit(itam);
                        }
                    }
                }*/
                foreach (var item in hinh)
                {
                    data.HINHs.DeleteOnSubmit(item);
                }
                foreach (var item in kho)
                {
                    data.PHIEUNHAPKHOs.DeleteOnSubmit(item);
                }
                foreach (var item in kichthuoc)
                {
                    data.KICHTHUOCs.DeleteOnSubmit(item);
                }
                data.SANPHAMs.DeleteOnSubmit(sanpham);
                data.SubmitChanges();
                return RedirectToAction("Index", "SanPham");
            }
        }

        #region UpdateProduct
        //Hàm Ẩn hoặc Hiện Thú Cưng (ở đây sử dụng hàm void để Response.Write hình update lại)
        [HttpPost]
        public void UpdateProduct(int id)
        {
            //Lấy ra Thú Cưng cần Update Ẩn Hiện
            var _sp = (from s in data.SANPHAMs where s.MASP == id select s).SingleOrDefault();

            //Tạo chuỗi _Hinh để chưa đường dẫn hình Ẩn Hiện khi Update lại
            string _Hinh = "";

            //Ẩn thì cập nhật lại thành hiện và ngược lại
            if (_sp.ANHIEN == true)
            {
                _sp.ANHIEN = false;
                _Hinh = "/Content/Images/icon_An.png";
            }
            else
            {
                _sp.ANHIEN = true;
                _Hinh = "/Content/Images/icon_Hien.png";
            }

            //Lưu chỉnh sửa
            UpdateModel(_sp);
            data.SubmitChanges();

            //Xuất ra (Trả về) đường dẫn hình để Update lại trên Form
            Response.Write(_Hinh);
        }

      
        #endregion
    }
}