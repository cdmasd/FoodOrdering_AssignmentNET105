using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Assignment_UI.Models
{
    public class Images
    {
        public int ImageId { get; set; }
        public string ImageUrl { get; set; }
        public int FoodId { get; set; }
    }
}
