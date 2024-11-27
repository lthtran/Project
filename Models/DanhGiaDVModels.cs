using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebsiteThuCungBento.Models
{
    [Table("DanhGiaDichVu")]
    public class DanhGiaDVModels
    {
        DataClassesDataContext data = new DataClassesDataContext();

        public int MaDanhGia { get; set; }

        [ForeignKey("DangKyDichVu")]
        public int? SoDK { get; set; }

        public int DanhGia { get; set; }

        public string BinhLuan { get; set; }
        public DateTime NgayDanhGia { get; set; }

        public bool DaDanhGia { get; set; }  // Thêm thuộc tính để theo dõi trạng thái đánh giá

        public virtual DangKyDichVu DangKyDichVu { get; set; }
    }
}