namespace MinimalAPIPoc.Domain.ModelViews
{
    public record AdminModelView
    {
        public int Id { get; set; }
        public string Username { get; set; } = default!;
        public string Role { get; set; } = default!;
    }
}
