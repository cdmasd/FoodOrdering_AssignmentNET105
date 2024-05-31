using System.ComponentModel.DataAnnotations;

namespace Assignment_UI.ViewModel.Food
{
    public class UploadCreateFood
    {
        public CreateFood Food { get; set; }
        [Display(Name = "Food Image")]
        public IFormFile ImageFile { get; set; }
    }
}
