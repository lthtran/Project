using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebsiteThuCungBento.Models;

namespace WebsiteThuCungBento.App_Start
{
    public class AdminPhanQuyen : AuthorizeAttribute
    {
        DataClassesDataContext data = new DataClassesDataContext();
        public int MACN { set; get; }
        public string MACHUCNANG { set; get; }
        public override void OnAuthorization(AuthorizationContext context)
        {
            ADMIN quantri = (ADMIN)HttpContext.Current.Session["Taikhoanadmin"];
            if(quantri != null)
            {
                #region Check quyền  quản trị viên
                var count = data.PHANQUYENs.Count(m => m.MAADMIN == quantri.MAADMIN & m.MACHUCNANG == MACHUCNANG);
                if (count != 0)
                {
                    return;
                }
                else
                {
                    var returnUrl = context.RequestContext.HttpContext.Request.RawUrl;
                    context.Result = new RedirectToRouteResult(new
                        RouteValueDictionary
                        (
                            new
                            {
                                controller = "BaoLoi",
                                action = "KhongCoQuyen",
                                area = "Admin",
                                returnUrl = returnUrl.ToString()
                            }
                        ));
                }
                #endregion
                return;
            }
            else
            {
                var returnUrl = context.RequestContext.HttpContext.Request.RawUrl;
                context.Result = new RedirectToRouteResult(new 
                    RouteValueDictionary
                    (
                        new
                        {
                          controller = "Admin",
                          action = "Index",
                          area = "Admin",
                          returnUrl = returnUrl.ToString()
                        }
                    ));
            }
        }

    }
}