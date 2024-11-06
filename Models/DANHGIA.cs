using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebsiteThuCungBento.Models
{
    [Table("DANHGIA")]
    public class DanhGia
    {
        DataClassesDataContext data = new DataClassesDataContext();
        public int DANHGIAID { get; set; } // Khóa chính

        [ForeignKey("SANPHAM")]

        public int? MASP { get; set; } // Mã sản phẩm
        public string TENDNKH { get; set; }

        [ForeignKey("KHACHHANG")]
        public int? MAKH { get; set; }
        public string HINHANH { get; set; }
        public string EMAIL { get; set; }
        public int? Rate { get; set; } // Số sao (từ 1 đến 5
        public string NHANXET { get; set; } // Nhận xét của khách hàng
        public DateTime NGAYDG { get; set; } // Ngày đánh gi

        public virtual SANPHAM SANPHAM { get; set; }
        public virtual KHACHHANG KHACHHANG { get; set; }
    }
}
