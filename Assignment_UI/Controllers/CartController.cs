using Assignment_UI.Models;
using Microsoft.AspNetCore.Mvc;

namespace Assignment_UI.Controllers
{
    public class CartController : Controller
    {
        List<CartDetail> cartDetail;
        public CartController()
        {
            //cartDetail = new List<CartDetail>
            //{
            //    new CartDetail
            //    {
            //        Food = new FoodImages { 
            //            Food = new Food{Name = "Food1", UnitPrice = 99},
            //            images = new List<string> {"https://placehold.co/50x50"}
            //        },
            //        Quantity = 3,
            //        Total = 99 * 3
            //    },
            //    new CartDetail
            //    {
            //        Food = new FoodImages {
            //            Food = new Food{Name = "Food2", UnitPrice = 99},
            //            images = new List<string> { "https://placehold.co/50x50" }
            //        },
            //        Quantity = 3,
            //        Total = 99 * 3
            //    },
            //    new CartDetail
            //    {
            //        Food = new FoodImages {
            //            Food = new Food{Name = "Food3", UnitPrice = 99},
            //            images = new List<string> { "https://placehold.co/50x50" }
            //        },
            //        Quantity = 3,
            //        Total = 99 * 3
            //    },
            //};
        }
        public IActionResult Index()
        {
            return View(cartDetail);
        }
    }
}
