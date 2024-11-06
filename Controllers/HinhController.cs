using PagedList;
using WebsiteThuCungBento.Models;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteThuCungBento.App_Start;

namespace WebsiteThuCungBento.Controllers
{
    [AdminPhanQuyen(MACHUCNANG = "QL_HINHMOTA")]
    public class HinhController : Controller
    {
        DataClassesDataContext data = new DataClassesDataContext();
        // GET: Hinh
        public ActionResult Index(int? page)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {

                if (page == null) page = 1;
                int pageSize = 9;
                int pageNumber = (page ?? 1);
                var hinh = from h in data.HINHs select h;
                return View(hinh.ToPagedList(pageNumber, pageSize));
            }
        }
        public ActionResult Details(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                var hinh = from h in data.HINHs where h.MAHINH == id select h;
                if (hinh == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                return View(hinh.Single());
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
        public ActionResult Create(HINH hinh, HttpPostedFileBase fileUpload)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                ViewBag.MASP = new SelectList(data.SANPHAMs.ToList().OrderBy(n => n.TENSP), "MASP", "TENSP");
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
                    hinh.HINH1 = fileName;
                    data.HINHs.InsertOnSubmit(hinh);
                    data.SubmitChanges();
                    return RedirectToAction("Index", "Hinh");
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
                var hinh = from h in data.HINHs where h.MAHINH == id select h;

                ViewBag.MASP = new SelectList(data.SANPHAMs.ToList().OrderBy(n => n.TENSP), "MASP", "TENSP");
                return View(hinh.Single());
            }
        }
        [HttpPost, ActionName("Edit")]
        public ActionResult Capnhat(int id, HttpPostedFileBase fileUpload)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                HINH hinh = data.HINHs.SingleOrDefault(g => g.MAHINH == id);
                ViewBag.MASP = new SelectList(data.SANPHAMs.ToList().OrderBy(n => n.TENSP), "MASP", "TENSP");
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
                    hinh.HINH1 = fileName;
                    UpdateModel(hinh);
                    data.SubmitChanges();
                    return RedirectToAction("Index", "Hinh");
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
                var hinh = from h in data.HINHs where h.MAHINH == id select h;
                return View(hinh.Single());
            }
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult Xoa(int id)
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else
            {
                HINH hinh = data.HINHs.SingleOrDefault(g => g.MAHINH == id);
                data.HINHs.DeleteOnSubmit(hinh);
                data.SubmitChanges();
                return RedirectToAction("Index", "Hinh");
            }
        }
    }
}