using System.ComponentModel.DataAnnotations;

namespace MinimalAPIPoc.Domain.DTO
{
    public class VehicleDTO
    {
        public string Name { get; set; } = default!;
        public string Make { get; set; } = default!;
        public int Year { get; set; }
    }
}
