using Assignment_Server.Data;
using Assignment_Server.Interfaces;
using Assignment_Server.Mapper;
using Assignment_Server.Models;
using Assignment_Server.Models.DTO;
using Assignment_Server.Models.DTO.Food;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace Assignment_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodController(IFoodRepo foodService, IFoodImage imageService) : ControllerBase
    {
        private readonly IFoodRepo _foodService = foodService;
        private readonly IFoodImage _imgService = imageService;

        #region For Food
        // Tạo sản phẩm mới
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult CreateFood([FromBody] CreateFoodDTO dto)
        {
            if (ModelState.IsValid)
            {
                var food = new Food()
                {
                    Name = dto.Name,
                    UnitPrice = dto.UnitPrice,
                    CategoryID = dto.CategoryID,
                    mainImage = dto.mainImage,
                    View = 0
                };
                return StatusCode(201, _foodService.AddFood(food).toFoodDTO());
            }
            return BadRequest(ModelState);
        }




        // Hiển thị tất cả sản phẩm
        [HttpGet]
        public IActionResult GetAll(int? page, int pageSize = 9)
        {
            var foods = _foodService.Foods.Select(x=> x.toFoodDTO());
            int pageNumber = (page ?? 1);
            return Ok(foods.ToPagedList(pageNumber,pageSize));
        }


 
        // Tìm sản phẩm theo id
        [HttpGet("{id:int}")]
        public IActionResult GetById([FromRoute] int id)
        {

            var getFood = _foodService.GetById(id);
            if (getFood != null)
            {
                getFood.View++;
                return Ok(_foodService.UpdateFood(getFood).toFoodDTO());
            }
            return NotFound("Food not found");
        }




        // Tìm kiếm theo tên
        [HttpGet("name={searchName}")]
        public IActionResult SearchByName([FromRoute] string searchName)
        {
            var search = _foodService.SearchByName(searchName);
            if (search.Any())
            {
                return Ok(search.Select(x => x.toFoodDTO()));
            }
            return NotFound();
        }




        // Tìm kiếm theo filter
        [HttpGet("filter")]
        public IActionResult GetByFilter([FromQuery]decimal? minrange, [FromQuery]decimal? maxrange)
        {
            var filteredFoods = _foodService.getByFilter(minrange,maxrange);
            return Ok(filteredFoods.Select(x => x.toFoodDTO()));
        }




        // Lấy sản phẩm theo mã danh mục
        [HttpGet("categoryid={categoryId:int}")]
        public IActionResult GetByCategoryID([FromRoute]int categoryId)
        {
            var foods = _foodService.getByCategoryID(categoryId);
            if (foods.Any())
                return Ok(foods.Select(x => x.toFoodDTO()));
            return NotFound();
        }




        // Sắp xếp giá giảm dần
        [HttpGet("sort={sort}")]
        public IActionResult Sort(int? page, string sort)
        {
            var foods = _foodService.Sort(sort).Select(x => x.toFoodDTO());
            var pageSize = 9;
            int pageNumber = (page ?? 1);
            return Ok(foods.ToPagedList(pageNumber, pageSize));
        }



 
        // Cập nhật sản phẩm
        [HttpPut, Authorize(Roles = "admin")]
        public IActionResult UpdateFood([FromBody]FoodDTO foodDTO)
        {
            var upFood = _foodService.GetById(foodDTO.FoodId);
            if (upFood != null)
            {
                if (ModelState.IsValid)
                {
                    upFood.Name = foodDTO.Name;
                    upFood.UnitPrice = foodDTO.UnitPrice;
                    upFood.View = foodDTO.View;
                    upFood.CategoryID = foodDTO.CategoryID;

                    return Ok(_foodService.UpdateFood(upFood));
                }
                return BadRequest(ModelState);
            }
            return NotFound("Food is not existed!");
        }




        // Xoá sản phẩm
        [HttpDelete("{id:int}")]
        public void DeleteFood([FromRoute]int id) => _foodService.DeleteFood(id);
        #endregion
    }
}
