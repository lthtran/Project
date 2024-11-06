using WebsiteThuCungBento.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteThuCungBento.App_Start;

namespace WebsiteThuCungBento.Controllers
{
    public class AdminController : Controller
    {
        DataClassesDataContext data = new DataClassesDataContext();
        // GET: Admin
        public ActionResult Index()
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
                return View();
        }

        [HttpGet]
        public ActionResult dangnhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult dangnhap(DangNhapModel model)
        {
            if (ModelState.IsValid)
            {
                ADMIN ad = data.ADMINs.SingleOrDefault(n => n.TENDN == model.tendn && n.MATKHAU == model.matkhau);
                if (ad != null)
                {
                    ViewBag.Thongbao = "Đăng nhập thành công";
                    Session["Taikhoanadmin"] = ad;
                    return RedirectToAction("Index", "Admin");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            else
            {

            }
            return View(model);
        }

        public ActionResult thongtinadmin()
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("dangnhap", "Admin");

            }
            return View();
        }

        public ActionResult dangxuat()
        {
            Session.Clear();
            return RedirectToAction("Index", "Admin");
        }

        [AdminPhanQuyen(MACHUCNANG = "QL_QUANTRIVIEN")]
        public ActionResult listadmin()
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("dangnhap", "Admin");

            }
            else
            {
                var ad = from admin in data.ADMINs select admin;
                return View(ad) ;
            }
        }

        [HttpGet]
        [AdminPhanQuyen(MACHUCNANG = "QL_QUANTRIVIEN")]
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
        [ValidateInput(false)]
        public ActionResult Create(ADMIN admin, HttpPostedFileBase fileUpload)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                    if (ModelState.IsValid)
                    {
                        var fileName = Path.GetFileName(fileUpload.FileName);
                        var path = Path.Combine(Server.MapPath("~/img/"), fileName);
                        if (System.IO.File.Exists(path))
                            ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                        else
                        {
                            fileUpload.SaveAs(path);
                        }
                        admin.AVATAR = fileName;

                        data.ADMINs.InsertOnSubmit(admin);
                        data.SubmitChanges();
                    }
           
                return RedirectToAction("listadmin", "Admin");
            }
        }

        [HttpGet]
        public ActionResult Edit(int id )
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                var admin = from ad in data.ADMINs where ad.MAADMIN == id select ad;
                return View(admin.Single());
            }
        }
        [HttpPost, ActionName("Edit")]
        [ValidateInput(false)]
        public ActionResult Capnhat(int id, HttpPostedFileBase fileUpload)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                ADMIN ad = data.ADMINs.SingleOrDefault(n => n.MAADMIN == id);
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
                    ad.AVATAR = fileName;
                    UpdateModel(ad);
                    data.SubmitChanges();
                    return RedirectToAction("listadmin", "Admin");
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
                var ad = from adm in data.ADMINs where adm.MAADMIN == id select adm;
                return View(ad.Single());
            }
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult Xoa(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                ADMIN ad = data.ADMINs.SingleOrDefault(n => n.MAADMIN == id);
                data.ADMINs.DeleteOnSubmit(ad);
                data.SubmitChanges();
                return RedirectToAction("listadmin", "Admin");
            }
        }
        [HttpGet]
        public ActionResult DoiMK(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                
                DoiMKadmin model = new DoiMKadmin();               
                return View(model);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DoiMK(int id, DoiMKadmin reset)
        {
            
            var message = "";
            if (ModelState.IsValid)
            {
                ADMIN ad = data.ADMINs.SingleOrDefault(n => n.MAADMIN == id && n.MATKHAU == reset.CheckPass); 
                if(ad != null)
                {
                    ad.MATKHAU = reset.NewPassword;
                    UpdateModel(ad);
                    Session["Taikhoanadmin"] = ad;
                    data.SubmitChanges();
                    message = "Cập nhật mật khẩu mới thành công ";
                    return RedirectToAction("thongtinadmin", "Admin");
                }
                else
                {
                    ViewBag.Thongbao = "Mật khẩu cũ không đúng ";
                }
                      
            }
            else
            {
                message = "Điều gì đó không hợp lệ";

            }
            ViewBag.Message = message;
            return View(reset);
        }
    }
}