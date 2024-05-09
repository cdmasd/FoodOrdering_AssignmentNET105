using Assignment_Server.Data;
using Assignment_Server.Mapper;
using Assignment_Server.Models;
using Assignment_Server.Models.DTO.Category;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(FoodDbContext db) : ControllerBase
    {
        private readonly FoodDbContext _db = db;

        // Tạo category mới
        [HttpPost()]
        public IActionResult CreateCategory([FromBody] CreateCategoryDTO category)
        {
            if (ModelState.IsValid)
            {
                Category addCate = new Category()
                {
                    Name = category.Name,
                    ImageUrl = category.ImageUrl
                };
                _db.Categories.Add(addCate);
                _db.SaveChanges();
                return Ok(new { message = "Created!", obj = addCate.toCategoryDTO() });
            }
            return BadRequest(ModelState);
        }



        // Hiển thị tất cả category
        [HttpGet]
        public IActionResult GetAll()
        {
            var categories = _db.Categories.ToList().Select(x => x.toCategoryDTO());
            return Ok(categories);
        }




        // Tìm kiếm category theo id
        [HttpGet("{id:int}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var category =  _db.Categories.Find(id);
            if(category != null)
            {
                return Ok(category.toCategoryDTO());
            }
            return NotFound();
        }




        // Cập nhật category
        [HttpPut]
        public IActionResult UpdateCategory([FromBody] CategoryDTO category)
        {
            var Updatecate = _db.Categories.Find(category.CategoryId);
            if (Updatecate != null)
            {
                if (ModelState.IsValid)
                {
                    Updatecate.Name = category.Name;
                    Updatecate.ImageUrl = category.ImageUrl;
                    _db.Categories.Update(Updatecate);
                    _db.SaveChanges();
                    return Ok(new { message = "Updated!" , obj = Updatecate.toCategoryDTO()});
                }
                return BadRequest(ModelState);
            }
            return NotFound();
        }




        // Xoá category
        [HttpDelete("{id:int}")]
        public IActionResult DeleteCategory([FromRoute] int id)
        {
            var delete = _db.Categories.Find(id);
            if (delete != null)
            {
                _db.Categories.Remove(delete);
                _db.SaveChanges();
                return Ok(new { message = "Deleted!", obj = delete.toCategoryDTO() });
            }
            return NotFound();
        }
    }
}
