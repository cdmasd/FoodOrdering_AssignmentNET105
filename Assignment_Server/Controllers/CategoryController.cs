using Assignment_Server.Data;
using Assignment_Server.Interfaces;
using Assignment_Server.Mapper;
using Assignment_Server.Models;
using Assignment_Server.Models.DTO;
using Assignment_Server.Models.DTO.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Assignment_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(ICategoryRepo db, IFoodRepo foodService) : ControllerBase
    {
        private readonly ICategoryRepo _cateService = db;
        private readonly IFoodRepo _foodService = foodService;

        // Tạo category mới
        [Authorize(Roles = "admin")]
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
                if(_cateService.CreateCategory(addCate))
                {
                    return StatusCode(201, addCate.toCategoryDTO());
                }
                return BadRequest("Somethings went wrong");
            }
            return BadRequest(ModelState);
        }



        // Hiển thị tất cả category
        [HttpGet]
        public IActionResult GetAll()
        {
            var categories = _cateService.GetAll().Select(x => x.toCategoryDTO());
            return Ok(categories);
        }



        
        // Tìm kiếm category theo id
        [HttpGet("{id:int}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var category =  _cateService.GetById(id);
            if(category != null)
            {
                return Ok(category.toCategoryDTO());
            }
            return NotFound();
        }




        // Cập nhật category
        [Authorize(Roles = "admin")]
        [HttpPut]
        public IActionResult UpdateCategory([FromBody] CategoryDTO category)
        {
            var Updatecate = _cateService.GetById(category.CategoryId);
            if (Updatecate != null)
            {
                if (ModelState.IsValid)
                {
                    Updatecate.Name = category.Name;
                    Updatecate.ImageUrl = category.ImageUrl;
                    if(_cateService.UpdateCategory(Updatecate))
                    {
                        return Ok(new { message = "Updated!", obj = Updatecate.toCategoryDTO() });
                    }
                    return BadRequest("Somethings went wrong");
                }
                return BadRequest(ModelState);
            }
            return NotFound();
        }




        // Xoá category
        [Authorize(Roles ="admin")]
        [HttpDelete("{id:int}")]
        public IActionResult DeleteCategory([FromRoute] int id)
        {
            if (_cateService.DeleteCategory(id))
            {
                return Ok(new { message = "Deleted!" });
            }
            return NotFound();
        }



        // Số lượng sản phẩm của mỗi category
        [HttpGet("CategoryMenu")]
        public IActionResult GetFoods()
        {
            var foods = _foodService.GetAllFood().GroupBy(x => x.CategoryID).Select(c => new CategoryCount()
            {
                CategoryId = c.Key,
                Name = _cateService.GetById(c.Key).Name,
                Quantity = c.Count()
            });
            return Ok(foods);
        }
    }
}
