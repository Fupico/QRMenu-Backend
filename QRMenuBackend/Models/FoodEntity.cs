using System;
using System.ComponentModel.DataAnnotations;

namespace QRMenuBackend.Entities
{
public class FoodEntity
{[Key]
    public int FoodId { get; set; }
    
    // FoodGroupId alanı eklendi (Foreign Key)
    public int FoodGroupId { get; set; }  
    
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public decimal Price { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // İlişkiler
    public FoodGroupEntity FoodGroup { get; set; } = null!;
}

}
