public class FoodGroupDto
{
    public int FoodGroupId { get; set; }  // Gıda grubunun benzersiz kimliği
    public string GroupName { get; set; } = string.Empty;  // Gıda grubu adı
    public string Description { get; set; } = string.Empty;  // Gıda grubu açıklaması
    public string? ImageUrl { get; set; }  // Opsiyonel görsel URL'si
    public DateTime CreatedAt { get; set; }  // Oluşturulma tarihi
    public DateTime? UpdatedAt { get; set; }  // Güncellenme tarihi
}
