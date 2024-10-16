using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace QRMenuBackend.Entities
{
   public class FoodGroupEntity
    {
        [Key]
        public int FoodGroupId { get; set; }

        // Foreign Key tanımlandı
        public int CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]

    [JsonIgnore]
    public CompanyEntity Company { get; set; } = null!;  // Non-nullable, ancak başlangıçta null atanıyor

        public string GroupName { get; set; } = string.Empty;  // Varsayılan değer atandı
        public string Description { get; set; } = string.Empty;  // Varsayılan değer atandı
        public string? ImageUrl { get; set; }  // Görsel URL'si (nullable)

        // Metadata alanları
        public DateTime CreatedAt { get; set; }  // Kayıt oluşturulma tarihi
        public DateTime? UpdatedAt { get; set; }  // Kayıt güncellenme tarihi

        // İlişkiler
        public ICollection<FoodEntity> Foods { get; set; } = new List<FoodEntity>();  // Varsayılan değer atandı

        public required int Invalidated { get; set; }  // Kayıt güncellenme tarihi
    }
}
