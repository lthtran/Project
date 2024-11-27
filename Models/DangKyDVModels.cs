using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteThuCungBento.Models
{
    public class DangKyDVModels
    {
        DataClassesDataContext data = new DataClassesDataContext();
        /*FormCollection f;*/
        public int iMACT { set; get; }
        public string iTENDV { set; get; }
        public int iTONGTIEN { set; get; }
        public string iKHUNGGIO { set; get; }

        public int? iMAADMIN { set; get; }

        public string iTENNV {  set; get; }
        public DangKyDVModels(int MACT)
        {
            iMACT = MACT;
            ChiTietDichVu ctdv = data.ChiTietDichVus.Single(n => n.MaCT == MACT);
            iTENDV = ctdv.DichVu.TenDV;
            iKHUNGGIO = ctdv.KhungGio;
            iTONGTIEN = (int)ctdv.DonGia;
            iMAADMIN = ctdv.MAADMIN;
            iTENNV = ctdv.TENNV;
        }
    }
}