using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QRMenuBackend.Entities
{
       public class CompanyEntity
    {
        [Key]
        public int CompanyId { get; set; }  // Şirketin benzersiz ID'si
        public required string Name { get; set; }  // Şirketin adı
        public required string Domain { get; set; }  // Şirketin domain adresi ya da subdomain'i
        public string? ImageUrl { get; set; }  // Şirketin görsel URL'si (nullable)

        // Metadata alanları
        public DateTime CreatedAt { get; set; }  // Kayıt oluşturulma tarihi
        public required string CreatedBy { get; set; }  // Kayıt oluşturan kullanıcı
        public DateTime? UpdatedAt { get; set; }  // Kayıt güncellenme tarihi
        public string? UpdatedBy { get; set; }  // Kayıt güncelleyen kullanıcı

        // İlişkiler
        public ICollection<FoodGroupEntity> FoodGroups { get; set; } = new List<FoodGroupEntity>();  // Şirketin yemek grupları
    }
}
