namespace core_strength_yoga_products_api.Models;

public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public IEnumerable<string>?  Roles { get; set; }
}