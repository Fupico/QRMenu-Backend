using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QRMenuBackend.Entities
{
  public class FoodEntity
    {
        [Key]
        public int FoodId { get; set; }

        // Foreign Key tanımlandı
        public int FoodGroupId { get; set; }

        [ForeignKey(nameof(FoodGroupId))]
        public FoodGroupEntity FoodGroup { get; set; } = null!;  // Foreign key ile ilişki

        public string Name { get; set; } = string.Empty;  // Yemek adı
        public string Description { get; set; } = string.Empty;  // Yemek açıklaması
        public string ImageUrl { get; set; } = string.Empty;  // Görsel URL'si
        public decimal Price { get; set; }  // Fiyat

        // Metadata alanları
        public DateTime CreatedAt { get; set; }  // Kayıt oluşturulma tarihi
        public DateTime? UpdatedAt { get; set; }  // Kayıt güncellenme tarihi
        public required int Invalidated { get; set; }  // Kayıt güncellenme tarihi
    }
}
