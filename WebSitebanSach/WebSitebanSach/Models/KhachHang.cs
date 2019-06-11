namespace WebSiteBanSach.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("KhachHang")]
    public partial class KhachHang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public KhachHang()
        {
            DonHang = new HashSet<DonHang>();
        }

        [Key]
        public int MaKH { get; set; }

        [StringLength(50)]
		[Display(Name ="Họ tên")]
		[Required(ErrorMessage = "Mời nhập Họ tên")]
		public string HoTen { get; set; }

		[Display(Name = "Ngày sinh")]
		[Required(ErrorMessage = "Mời nhập Ngày sinh")]
		public DateTime? NgaySinh { get; set; }

		[Display(Name = "Giới tính")]
		[Required(ErrorMessage = "Mời chọn giới tính")]
		[StringLength(3)]
        public string GioiTinh { get; set; }

		[Display(Name = "Số điện thoại")]
		[Required(ErrorMessage = "Mời nhập Số điện thoại")]
		[StringLength(50)]
        public string DienThoai { get; set; }

		[Display(Name = "Tên tài khoản")]
		[Required(ErrorMessage = "Mời nhập Tên tài khoản")]
		[StringLength(50)]
        public string TaiKhoan { get; set; }

		[Display(Name = "Mật khẩu")]
		[Required(ErrorMessage = "Mời nhập Họ tên")]
		[StringLength(50)]
        public string MatKhau { get; set; }

		[Display(Name = "Email")]
		[Required(ErrorMessage = "Mời nhập Email")]
		[StringLength(50)]
        public string Email { get; set; }

		[Display(Name = "Địa chỉ")]
		[Required(ErrorMessage = "Mời nhập Địa chỉ")]
		[StringLength(10)]
        public string DiaChi { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DonHang> DonHang { get; set; }
    }
}
