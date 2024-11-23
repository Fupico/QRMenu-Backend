public class AddFoodDto
{
    public int FoodGroupId { get; set; }  // Hangi gıda grubuna ait olduğu
    public string Name { get; set; } = string.Empty;  // Yemek adı
    public string Description { get; set; } = string.Empty;  // Yemek açıklaması
    public string? ImageUrl { get; set; }  // Opsiyonel görsel URL'si
    public decimal Price { get; set; }  // Yemek fiyatı
    public int IsGroupPrice { get; set; } = 1;  // 1, 2, 3 gibi değerler
    public string? PriceDesc { get; set; }
    public string? PriceDesc2 { get; set; }
    public string? PriceDesc3 { get; set; }
    public decimal? Price2 { get; set; }
    public decimal? Price3 { get; set; }
  

}
