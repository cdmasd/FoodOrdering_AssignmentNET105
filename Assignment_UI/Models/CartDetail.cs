using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Assignment_UI.Models
{
    public class CartDetail
    {
        public Food Food { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }
}
