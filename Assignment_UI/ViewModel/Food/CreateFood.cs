using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Assignment_UI.ViewModel.Food
{
    public class CreateFood
    {
        [Required, MaxLength(50)]
        public string Name { get; set; }
        [Required, Range(1000, 1000000), Column(TypeName = "decimal(18,2)"),Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        public string? mainImage { get; set; } = string.Empty;

        [Required, Display(Name = "Category Id")]
        public int CategoryID { get; set; }
    }
}
