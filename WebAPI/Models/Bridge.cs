#nullable enable
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class Bridge : BaseEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên cầu không được bỏ trống")]
        public string Name { get; set; }

        [DataType(DataType.DateTime)]
        // [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public string StartTime { get; set; }

        [DataType(DataType.DateTime)]
        // [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public string EndTime { get; set; }

        // Chủ đầu tư
        [Required(ErrorMessage = "Chủ đầu tư không được bỏ trống")]
        public string Investor { get; set; }

        // Tổng mức đầu tư
        [Range(0.001, float.MaxValue, ErrorMessage = "Tổng mức đầu tư phải ở dạng số")]
        [Required(ErrorMessage = "Tổng mức đầu tư không được bỏ trống")]
        public float TotalInvestment { get; set; }

        // Tải trọng thiết kế
        [Range(0, float.MaxValue, ErrorMessage = "Tải trọng thiết kế phải ở dạng số")]
        [Required(ErrorMessage = "Tải trọng thiết kế không được bỏ trống")]
        public float DesignLoad { get; set; }

        // Đơn vị thiết kế
        [Required(ErrorMessage = "Đơn vị thiết kế không được bỏ trống")]
        public string Designer { get; set; }

        // Đơn vị thi công
        [Required(ErrorMessage = "Đơn vị thi công không được bỏ trống")]
        public string Builder { get; set; }

        // Đơn vị giám sát
        [Required(ErrorMessage = "Đơn vị giám sát không được bỏ trống")]
        public string Supervisor { get; set; }

        // Đơn vị quản lý giám sát
        [Required(ErrorMessage = "Đơn vị quản lý giám sát không được bỏ trống")]
        public string Manager { get; set; }

        // Quốc lộ
        // [Required(ErrorMessage = "Địa điểm không được bỏ trống")]
        public string Location { get; set; }
        
        public float Latitude { get; set; }
        
        public float Longitude { get; set; }

        // Trạng thái
        [Required(ErrorMessage = "Trạng thái không được bỏ trống")]
        public string Status { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }
    }
}