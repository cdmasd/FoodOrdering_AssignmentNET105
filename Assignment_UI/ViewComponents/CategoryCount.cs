
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
namespace Assignment_UI.ViewComponents
{
    public class CategoryCount : ViewComponent
    {
        Uri baseAddress = new Uri("https://localhost:7294/api");
        readonly HttpClient _client;
        public CategoryCount()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }
        public IViewComponentResult Invoke()
        {
            var categories = new List<ViewModel.CategoryCount>();
            var response = _client.GetAsync(baseAddress + "/Category/CategoryMenu").Result;
            if(response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                categories = JsonConvert.DeserializeObject<List<ViewModel.CategoryCount>>(data);
            }
            return View(categories);
        }
    }
}
