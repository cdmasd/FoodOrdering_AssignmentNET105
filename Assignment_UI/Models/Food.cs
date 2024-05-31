using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Assignment_UI.Models
{
    public class Food
    {
        public int FoodId { get; set; }
        [Required,DisplayName("Name")]
        public string Name { get; set; }
        [Required,DisplayName("Unit Price")]
        public decimal UnitPrice { get; set; }
        public int View { get; set; }
        public string mainImage { get; set; }
        public int CategoryId { get; set; }
    }
}
