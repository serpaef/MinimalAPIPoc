using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalAPIPoc.Domain.Entities
{
    public class Admin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string Username { get; set; } = default!;

        [Required]
        [StringLength(25)]
        public string Password { get; set; } = default!;

        [StringLength(10)]
        public string Role { get; set; } = default!;
    }
}
