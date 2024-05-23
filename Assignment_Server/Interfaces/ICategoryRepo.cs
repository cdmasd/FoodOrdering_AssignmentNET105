using Assignment_Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace Assignment_Server.Interfaces
{
    public interface ICategoryRepo
    {
        IEnumerable<Category> Categories { get; }
        Category GetById(int id);
        Category AddCategory(Category category);
        Category UpdateCategory(Category category);
        void DeleteCategory(int id);
    }
}
