using WebsiteThuCungBento.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteThuCungBento.App_Start;

namespace WebsiteThuCungBento.Controllers
{
    [AdminPhanQuyen(MACHUCNANG = "QL_LOAISANPHAM")]
    public class LoaiController : Controller
    {
        DataClassesDataContext data = new DataClassesDataContext();
        // GET: Loai
        public ActionResult Index()
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                var loai = from l in data.LOAIs select l;
                return View(loai);
            }
        }
        public ActionResult Details(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                var loai = from l in data.LOAIs where l.MALOAI == id select l;
                return View(loai.Single());
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
        public ActionResult Create(LOAI loai)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                data.LOAIs.InsertOnSubmit(loai);
                data.SubmitChanges();
                return RedirectToAction("Index", "Loai");
            }
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                var loai = from l in data.LOAIs where l.MALOAI == id select l;
                return View(loai.Single());
            }
        }
        [HttpPost, ActionName("Edit")]
        public ActionResult Capnhat(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                LOAI loai = data.LOAIs.SingleOrDefault(n => n.MALOAI == id);
                UpdateModel(loai);
                data.SubmitChanges();
                return RedirectToAction("Index", "Loai");
            }
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                var loai = from nxb in data.LOAIs where nxb.MALOAI == id select nxb;
                return View(loai.Single());
            }
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult Xoa(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                LOAI nhaxuatban = data.LOAIs.SingleOrDefault(n => n.MALOAI == id);
                data.LOAIs.DeleteOnSubmit(nhaxuatban);
                data.SubmitChanges();
                return RedirectToAction("Index", "Loai");
            }
        }
    }
}