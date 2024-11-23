public class UpdateFoodDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int? IsGroupPrice { get; set; }  // 1, 2, 3 gibi değerler
    public string? PriceDesc { get; set; }
    public string? PriceDesc2 { get; set; }
    public string? PriceDesc3 { get; set; }
    public decimal? Price { get; set; }
    public decimal? Price2 { get; set; }
    public decimal? Price3 { get; set; }
    public int? Invalidated { get; set; }  // 1 veya 0
}
