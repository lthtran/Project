using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteThuCungBento.Models
{
    public class DangKy
    {
        DataClassesDataContext data = new DataClassesDataContext();
        /*FormCollection f;*/
        public int iMACT { set; get; }
        public string iTENDV { set; get; }
        public int iTONGTIEN { set; get; }

        public DangKy(int MACT)
        {
            iMACT = MACT;
            ChiTietDichVu ctdv = data.ChiTietDichVus.Single(n => n.MaCT == MACT);
            iTENDV = ctdv.DichVu.TenDV;
            iTONGTIEN = (int)ctdv.DonGia;
        }
    }
}