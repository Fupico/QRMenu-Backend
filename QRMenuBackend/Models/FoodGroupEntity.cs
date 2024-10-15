using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QRMenuBackend.Entities
{
    public class FoodGroupEntity
    {
        [Key]
      
    public int FoodGroupId { get; set; }
    public int CompanyId { get; set; }
    public string GroupName { get; set; } = string.Empty;  // Varsayılan değer atandı
    public string Description { get; set; } = string.Empty;  // Varsayılan değer atandı
    public CompanyEntity Company { get; set; } = null!;  // Non-nullable, ancak başlangıçta null atanıyor
    public ICollection<FoodEntity> Foods { get; set; } = new List<FoodEntity>();  // Default değer atandı

        // Metadata alanları
        public DateTime CreatedAt { get; set; }  // Kayıt oluşturulma tarihi
        public DateTime? UpdatedAt { get; set; }  // Kayıt güncellenme tarihi

        // İlişkiler
    }
}
