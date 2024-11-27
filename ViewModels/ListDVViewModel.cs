using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteThuCungBento.Models;

namespace WebsiteThuCungBento.ViewModels
{
    public class ListDVViewModel
    {
        public IEnumerable<DichVu> listDV { get; set; }
        public IEnumerable<ChiTietDichVu> listCTDV { get; set; }
        public IEnumerable<ADMIN> ADMINs { get; set; } // Thêm thuộc tính này

    }
}