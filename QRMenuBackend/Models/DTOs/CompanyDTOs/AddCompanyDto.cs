public class AddCompanyDto
{
    public required string Name { get; set; }  // Şirketin adı
    public required string Domain { get; set; }  // Şirketin domain adresi ya da subdomain'ii
    public  string? ImageUrl { get; set; }  // Şirketin domain adresi ya da subdomain'ii
}
