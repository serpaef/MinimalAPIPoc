using MinimalAPIPoc.Domain.Enums;

namespace MinimalAPIPoc.Domain.DTO
{
    public class AdminDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Role Role { get; set; } = Role.None;
    }
}
