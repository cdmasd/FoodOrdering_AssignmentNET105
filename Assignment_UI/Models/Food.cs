using System.ComponentModel;

namespace Assignment_UI.Models
{
    public class Food
    {
        public int FoodId { get; set; }
        [DisplayName("Name")]
        public string Name { get; set; }
        [DisplayName("Unit Price")]
        public decimal UnitPrice { get; set; }
        public int View { get; set; }
        public string mainImage { get; set; }
        public int CategoryId { get; set; }
    }
}
