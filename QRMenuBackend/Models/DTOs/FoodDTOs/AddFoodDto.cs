public class AddFoodDto
{
    public int FoodGroupId { get; set; }  // Hangi gıda grubuna ait olduğu
    public string Name { get; set; } = string.Empty;  // Yemek adı
    public string Description { get; set; } = string.Empty;  // Yemek açıklaması
    public string? ImageUrl { get; set; }  // Opsiyonel görsel URL'si
    public decimal Price { get; set; }  // Yemek fiyatı
}
