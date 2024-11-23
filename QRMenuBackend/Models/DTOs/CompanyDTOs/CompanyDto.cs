public class CompanyDto
{
    public int CompanyId { get; set; }
    public string? Name { get; set; }
    public string? Domain { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActiveCompanyImage { get; set; } = false;  // Şirketin QR URL'si (nullable)
    public string? CompanyUrl { get; set; }  // Şirketin görsel URL'si (nullable)
    public string? Phone { get; set; }  // Şirketin Telefon Numarası (nullable)
    public string? Adress { get; set; }   // Şirketin Adresi GoogleMaps (nullable)
    public int SelectedMenu { get; set; } = 1;
}
