public class AddFoodGroupDto
{
    public int CompanyId { get; set; }  // Hangi şirkete ait olduğu
    public string GroupName { get; set; } = string.Empty;  // Gıda grubu adı
    public string Description { get; set; } = string.Empty;  // Gıda grubu açıklaması
    public string? ImageUrl { get; set; }  // Opsiyonel görsel URL'si
}
