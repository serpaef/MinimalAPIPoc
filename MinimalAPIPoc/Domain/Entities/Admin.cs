namespace MinimalAPIPoc.Domain.Entities
{
    public class Admin
    {
        public int Id { get; set; }
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string Role { get; set; } = default!;
    }
}
