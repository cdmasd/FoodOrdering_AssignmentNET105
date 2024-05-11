using Assignment_Server.Models;
using Microsoft.AspNetCore.Mvc;

namespace Assignment_Server.Interfaces
{
    public interface ICategoryRepo
    {
        bool CreateCategory(Category category);
        IEnumerable<Category> GetAll();
        Category GetById(int id);
        bool UpdateCategory(Category category);
        bool DeleteCategory(int id);
    }
}
