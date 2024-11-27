using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteThuCungBento.Models;
using WebsiteThuCungBento.ViewModels;

namespace WebsiteThuCungBento.Controllers
{
    public class DanhGiaDVController : Controller
    {
        DataClassesDataContext data = new DataClassesDataContext();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Danhgiadichvu(int id)
        {
            var model = new DanhGiaDVModels
            {
                SoDK = id
            };
            return View(model);
        }

        // POST: User/Danhgiadichvu
        //[HttpPost]
        //public ActionResult Danhgiadichvu(DanhGiaDVModels model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var danhGia = new DanhGiaDichVu
        //        {
        //            SoDK = model.SoDK,
        //            DanhGia = model.DanhGia,
        //            BinhLuan = model.BinhLuan,
        //            NgayDanhGia = DateTime.Now
        //        };
        //        data.DanhGiaDichVus.InsertOnSubmit(danhGia);
        //        data.SubmitChanges();
        //        return RedirectToAction("ListServiceRegis"); // Chuyển hướng về trang chính hoặc trang khác tùy bạn
        //    }
        //    return View(model);
        //}
        //[HttpPost]
        //public ActionResult Danhgiadichvu(DanhGiaDVModels model)
        //{
        //    // Lấy giá trị đánh giá sao
        //    int rating = model.DanhGia;  // Lấy giá trị điểm từ input hidden

        //    // Tiến hành lưu dữ liệu vào cơ sở dữ liệu (ví dụ: trong bảng Đánh Giá)
        //    var newDanhGia = new DanhGiaDichVu
        //    {
        //        SoDK = model.SoDK,  // Sử dụng mã đăng ký
        //        DanhGia = rating,
        //        BinhLuan = model.BinhLuan,  // Nếu có bình luận
        //        NgayDanhGia = DateTime.Now
        //    };

        //    // Lưu vào cơ sở dữ liệu (Giả sử bạn có context DB)
        //    data.DanhGiaDichVus.InsertOnSubmit(newDanhGia);
        //    data.SubmitChanges();

        //    // Quay lại trang hoặc trả về kết quả
        //    return RedirectToAction("ListServiceRegis");
        //}
        [HttpPost]
        public JsonResult Danhgiadichvu(DanhGiaDVModels model)
        {
            if (ModelState.IsValid && model.SoDK != null)
            {
                using (var db = new DataClassesDataContext())
                {
                    // Kiểm tra xem người dùng đã đánh giá chưa
                    var existingRating = db.DanhGiaDichVus.FirstOrDefault(d => d.SoDK == model.SoDK);
                    if (existingRating == null)
                    {
                        var danhGia = new DanhGiaDichVu
                        {
                            SoDK = model.SoDK,
                            DanhGia = model.DanhGia,
                            BinhLuan = model.BinhLuan,
                            NgayDanhGia = DateTime.Now
                        };
                        db.DanhGiaDichVus.InsertOnSubmit(danhGia);

                        // Cập nhật trạng thái của dịch vụ đăng ký
                        var dangKyDichVu = db.DangKyDichVus.FirstOrDefault(d => d.SoDK == model.SoDK);
                        if (dangKyDichVu != null)
                        {
                            dangKyDichVu.TinhTrang = 3; // Đã Đánh Giá
                        }

                        db.SubmitChanges();
                        return Json(new { Success = true });
                    }
                    else
                    {
                        return Json(new { Success = false, Message = "Bạn đã đánh giá dịch vụ này rồi!" });
                    }
                }
            }
            return Json(new { Success = false, Message = "Dữ liệu không hợp lệ!" });
        }

        [HttpGet]
        public ActionResult XemDanhGia(int id)
        {
            using (var db = new DataClassesDataContext())
            {
                var danhGia = db.DanhGiaDichVus.FirstOrDefault(d => d.SoDK == id);
                if (danhGia != null)
                {
                    var model = new DanhGiaDVModels
                    {
                        SoDK = danhGia.SoDK,
                        DanhGia = danhGia.DanhGia,
                        BinhLuan = danhGia.BinhLuan,
                        NgayDanhGia = danhGia.NgayDanhGia,
                        DaDanhGia = true
                    };
                    return Json(new { Success = true, Data = model }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { Success = false, Message = "Không tìm thấy đánh giá!" }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
