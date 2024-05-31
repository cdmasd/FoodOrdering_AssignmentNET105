using Assignment_UI.Models;

namespace Assignment_UI.ViewModel
{
    public class ProductVM
    {
        public List<Models.Food> Foods { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public int totalItem { get; set; }
        public int pageCount { get; set; }
    }
}
