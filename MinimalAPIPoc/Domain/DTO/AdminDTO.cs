using MinimalAPIPoc.Domain.Enums;

namespace MinimalAPIPoc.Domain.DTO
{
    public class AdminDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = Enums.Role.None.ToString();
    }
}
