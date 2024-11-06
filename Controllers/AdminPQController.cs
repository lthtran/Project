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
    [AdminPhanQuyen(MACHUCNANG = "QL_PHANQUYEN")]
    public class AdminPQController : Controller
    {
        // GET: AdminPQ

        DataClassesDataContext data = new DataClassesDataContext();
        #region PHÂN QUYỀN
        public ActionResult DSPhanQuyen()
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("dangnhap", "Admin");

            }
            else
            {
                var ad = from admin in data.ADMINs select admin;
                return View(ad);
            }
        }

        public ActionResult ChiTietDSPhanQuyen(int id)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("dangnhap", "Admin");

            }
            else
            {
                var ad = from admin in data.PHANQUYENs where admin.MAADMIN == id select admin;
                return View(ad);
            }
        }

        public ActionResult DSQuyen()
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("dangnhap", "Admin");

            }
            else
            {
                var ad = from admin in data.PHANQUYENs select admin;
                return View(ad);
            }
        }

        public ActionResult Create()
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                ViewBag.MACHUCNANG = new SelectList(data.CHUCNANG_QUYENs.ToList().OrderBy(n => n.TENCN), "MACHUCNANG", "TENCN");
                ViewBag.MAADMIN = new SelectList(data.ADMINs.ToList().OrderBy(n => n.HOTEN), "MAADMIN", "HOTEN");
                return View();
            }
        }

        [HttpPost]
        public ActionResult Create(PHANQUYEN kichthuoc)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                ViewBag.MACHUCNANG = new SelectList(data.CHUCNANG_QUYENs.ToList().OrderBy(n => n.TENCN), "MACHUCNANG", "TENCN");
                ViewBag.MAADMIN = new SelectList(data.ADMINs.ToList().OrderBy(n => n.HOTEN), "MAADMIN", "HOTEN");
                data.PHANQUYENs.InsertOnSubmit(kichthuoc);
                data.SubmitChanges();
                return RedirectToAction("DSPhanQuyen", "AdminPQ");

            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                ViewBag.MACHUCNANG = new SelectList(data.CHUCNANG_QUYENs.ToList().OrderBy(n => n.TENCN), "MACHUCNANG", "TENCN");

                var mau = from m in data.PHANQUYENs where m.MAPQ == id select m;
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
                ViewBag.MACHUCNANG = new SelectList(data.CHUCNANG_QUYENs.ToList().OrderBy(n => n.TENCN), "MACHUCNANG", "TENCN");
                PHANQUYEN mau = data.PHANQUYENs.SingleOrDefault(n => n.MAPQ == id);
                UpdateModel(mau);
                data.SubmitChanges();
                return RedirectToAction("DSPhanQuyen", "AdminPQ");
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                var mau = from m in data.PHANQUYENs where m.MAPQ == id select m;
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
                PHANQUYEN mau = data.PHANQUYENs.SingleOrDefault(n => n.MAPQ == id);
                data.PHANQUYENs.DeleteOnSubmit(mau);
                data.SubmitChanges();
                return RedirectToAction("DSPhanQuyen", "AdminPQ");
            }
        }
        #endregion

        #region CHỨC NĂNG QUYỀN
        public ActionResult DSChucNang()
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("dangnhap", "Admin");

            }
            else
            {
                var ad = from admin in data.CHUCNANG_QUYENs select admin;
                return View(ad);
            }
        }

        public ActionResult CreateCN()
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult CreateCN(CHUCNANG_QUYEN chucnang)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                data.CHUCNANG_QUYENs.InsertOnSubmit(chucnang);
                data.SubmitChanges();
                return RedirectToAction("DSChucNang", "AdminPQ");
            }
        }

        [HttpGet]
        public ActionResult EditCN(string id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                var mau = from m in data.CHUCNANG_QUYENs where m.MACHUCNANG == id select m;
                return View(mau.Single());
            }
        }

        [HttpPost, ActionName("EditCN")]
        public ActionResult CapnhatCN(string id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                CHUCNANG_QUYEN mau = data.CHUCNANG_QUYENs.SingleOrDefault(n => n.MACHUCNANG == id);
                UpdateModel(mau);
                data.SubmitChanges();
                return RedirectToAction("DSChucNang", "AdminPQ");
            }
        }

        [HttpGet]
        public ActionResult DeleteCN(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                var mau = from m in data.PHANQUYENs where m.MAPQ == id select m;
                return View(mau.Single());
            }
        }

        [HttpPost, ActionName("DeleteCN")]
        public ActionResult XoaCN(string id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                PHANQUYEN mau = data.PHANQUYENs.SingleOrDefault(n => n.MACHUCNANG == id);
                data.PHANQUYENs.DeleteOnSubmit(mau);
                data.SubmitChanges();
                return RedirectToAction("DSPhanQuyen", "AdminPQ");
            }
        }
        #endregion
    }
}