using WebsiteThuCungBento.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.Net.Mail;
using System.Net;
using System.IO;

using WebsiteThuCungBento.ViewModels;
using System.Configuration;
using System.Data.Linq;
using System.Web.Helpers;
using System.Web.Services.Description;
public class ReviewController : Controller
{
    DataClassesDataContext data = new DataClassesDataContext();

    public ActionResult Index()
    {
        return View();
    }
    public ActionResult _Review(int MASP)
    {
        ViewBag.MASP = MASP;

        // Check if the user is logged in
        if (Session["Taikhoan"] != null)

        {
            // Instead of redirecting, return a view or partial view with a message

        // Assuming Session["Taikhoan"] contains the user object, cast it to KHACHHANG
        var userStore = Session["Taikhoan"] as KHACHHANG;

            // Create a new instance of ReviewProduct
            var reviewProduct = new DANHGIA
            {
                TENDNKH = userStore.TENDNKH, // Assuming TENDNKH is a property in your KHACHHANG model
                EMAIL = userStore.EMAIL,
                HINHANH = userStore.HINHANH// Assuming EMAIL is a property in your KHACHHANG model
                                           // Add any other necessary properties
            };
            return PartialView(reviewProduct);

        }
        return PartialView();
    }


    public ActionResult _load_Review(int MASP)
    {
        var item = data.DANHGIAs.Where(x => x.MASP == MASP).OrderByDescending(x => x.MASP).ToList();
        ViewBag.Count = item.Count;
        if (!item.Any())
        {
            return PartialView("NoReview");
        }
        return PartialView(item);
    }
    [HttpPost]
    public ActionResult PostReview(DANHGIA req)
    {
       
        if (ModelState.IsValid)
        {
            if (Session["Taikhoan"] == null)
            {
                return Json(new { Failure = true });
            }
            req.NGAYDG = DateTime.Now;

            // Retrieve user from session
           
            KHACHHANG user = (KHACHHANG)Session["Taikhoan"];
            req.HINHANH = user.HINHANH;
            req.MAKH = user.MAKH;
            data.DANHGIAs.InsertOnSubmit(req);
            data.SubmitChanges();
            return Json(new { Success = true });
        }
        return Json(new {Success = false});
    }
}
