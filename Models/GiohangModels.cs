//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace WebsiteThuCungBento.Models
//{
//    public class Giohang
//    {
//        DataClassesDataContext data = new DataClassesDataContext();
//        /*FormCollection f;*/
//        public int iMASP { set; get; }
//        public int CONLAI { set; get; }
//        public string gTENSP { set; get; }
//        public string gHINHANH { set; get; }
//        public Double dDONGIA { set; get; }
//        public int iSOLUONG { set; get; }
//        public Double dTHANHTIEN
//        {
//            get { return iSOLUONG * dDONGIA; }

//        }
//        public Double iTONGTIEN { set; get; }

//        public Giohang(int MASP)
//        {

//            iMASP = MASP;
//            SANPHAM sanpham = data.SANPHAMs.Single(n => n.MASP == iMASP);
//            CONLAI = sanpham.SOLUONG.GetValueOrDefault();
//            gTENSP = sanpham.TENSP;
//            gHINHANH = sanpham.HINHANH;
//            dDONGIA = double.Parse(sanpham.DONGIABAN.ToString());
//            iSOLUONG = 1;
//            /*iSOLUONG = int.Parse(f["SoLuong"].ToString());*/
//        }

//    }
//}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebsiteThuCungBento.Models
{
    public class GiohangModels
    {
        DataClassesDataContext data = new DataClassesDataContext();

        public int iMASP { set; get; }
        public int CONLAI { set; get; }
        public string gTENSP { set; get; }
        public string gHINHANH { set; get; }
        public Double dDONGIA { set; get; }
        public int iSOLUONG { set; get; }
        public Double dTHANHTIEN
        {
            get { return iSOLUONG * dDONGIA; }
        }
        public Double iTONGTIEN { set; get; }

        // Parameterless constructor for deserialization
        public GiohangModels()
        {
        }

        // Constructor with parameters to load data when creating a new cart item
        public GiohangModels(int MASP)
        {
            iMASP = MASP;
            SANPHAM sanpham = data.SANPHAMs.Single(n => n.MASP == iMASP);
            CONLAI = sanpham.SOLUONG.GetValueOrDefault();
            gTENSP = sanpham.TENSP;
            gHINHANH = sanpham.HINHANH;
            dDONGIA = double.Parse(sanpham.DONGIABAN.ToString());
            iSOLUONG = 1;
        }
    }
}
