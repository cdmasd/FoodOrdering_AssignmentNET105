using System.ComponentModel.DataAnnotations;

namespace Assignment_UI.ViewModel.Category
{
    public class CreateCategoryVM
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }
        public string? ImageUrl { get; set; } = string.Empty;
    }
}
