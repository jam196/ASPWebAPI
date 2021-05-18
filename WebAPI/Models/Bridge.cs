using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class Bridge : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [DataType(DataType.DateTime)]
        // [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public string StartTime { get; set; }
        [DataType(DataType.DateTime)]
        // [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public string EndTime { get; set; }
        
        // Tổng mức đầu tư
        public int TotalInvestment { get; set; }
        
        // Tải trọng thiết kế
        public int DesignLoad { get; set; }
        
        // Đơn vị thiết kế
        public string Designer { get; set; }
        
        // Đơn vị thi công
        public string Builder { get; set; }
        
        // Đơn vị giám sát
        public string Supervisor { get; set; }
        
        // Đơn vị quản lý giám sát
        public string Manager { get; set; }
        
        // Quốc lộ
        public string NationalHighway { get; set; }
        
        // Trạng thái
        public string Status { get; set; }
    }
}