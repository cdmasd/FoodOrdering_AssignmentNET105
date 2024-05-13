using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment_Server.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }
        [MaxLength(450),ForeignKey("User")]
        public string UserId { get; set; }

        // Navigation
        public virtual User User { get; set; }
        public virtual ICollection<CartDetail> CartDetails { get; set; }
    }
}
