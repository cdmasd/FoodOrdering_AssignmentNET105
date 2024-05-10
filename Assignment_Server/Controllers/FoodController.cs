using Assignment_Server.Data;
using Assignment_Server.Mapper;
using Assignment_Server.Models;
using Assignment_Server.Models.DTO.Food;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Assignment_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodController(FoodDbContext db) : ControllerBase
    {
        private readonly FoodDbContext _db = db;

        #region For Food
        // Tạo sản phẩm mới
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
                    View = 0
                };
                _db.Foods.Add(food);
                _db.SaveChanges();
                return Ok(new { message = "Created!", obj = food.toFoodDTO() });
            }
            return BadRequest(ModelState);
        }




        // Hiển thị tất cả sản phẩm
        [HttpGet]
        public IActionResult GetAll()
        {
            var foods = _db.Foods.ToList().Select(x => x.toFoodDTO());
            return Ok(foods);
        }




        // Tìm sản phẩm theo id
        [HttpGet("{id:int}")]
        public IActionResult GetById([FromRoute] int id)
        {

            var getFood = _db.Foods.Find(id);
            if (getFood != null)
            {
                getFood.View++;
                _db.Foods.Update(getFood);
                _db.SaveChanges();
                return Ok(getFood.toFoodDTO());
            }
            return NotFound();
        }




        // Tìm kiếm theo tên
        [HttpGet("{searchName}")]
        public IActionResult SearchByName([FromRoute] string searchName)
        {
            var search = _db.Foods.Where(x => x.Name.Contains(searchName)).ToList();
            if (search.Count > 0)
            {
                return Ok(search.Select(x => x.toFoodDTO()));
            }
            return NotFound();
        }




        // Tìm kiếm theo filter
        [HttpGet("getByFilter")]
        public IActionResult getByFilter([FromQuery]decimal? priceRange, [FromQuery] int? categoryId)
        {
            var foods = _db.Foods.AsQueryable();
            if (priceRange.HasValue)
            {
                foods = foods.Where(x => x.UnitPrice <  priceRange.Value);
            }
            if(categoryId.HasValue)
            {
                foods = foods.Where(x => x.CategoryID ==  categoryId.Value);
            }
            var filteredFoods = foods.ToList();
            return Ok(filteredFoods.Select(x => x.toFoodDTO()));
        }




        // Cập nhật sản phẩm
        [HttpPut]
        public IActionResult UpdateFood([FromBody]FoodDTO foodDTO)
        {
            var upFood = _db.Foods.Find(foodDTO.FoodId);
            if (upFood != null)
            {
                if (ModelState.IsValid)
                {
                    upFood.Name = foodDTO.Name;
                    upFood.UnitPrice = foodDTO.UnitPrice;
                    upFood.View = foodDTO.View;
                    upFood.CategoryID = foodDTO.CategoryID;

                    _db.Foods.Update(upFood);
                    _db.SaveChanges();
                    return Ok(new {message = "Updated!" , obj = upFood.toFoodDTO()});
                }
                return BadRequest(ModelState);
            }
            return NotFound();
        }




        // Xoá sản phẩm
        [HttpDelete("{id:int}")]
        public IActionResult DeleteFood([FromRoute]int id)
        {
            var delete = _db.Foods.Find(id);
            if (delete != null)
            {
                _db.Foods.Remove(delete);
                _db.SaveChanges();
                return Ok(new { message = "Deleted!" , obj = delete.toFoodDTO()});
            }
            return NotFound();
        }
        #endregion

        #region For FoodImage
        // Thêm mới hình ảnh
        [HttpPost("AddImage")]
        public IActionResult createImage([FromBody] CreateImageFoodDTO image)
        {
            if (ModelState.IsValid)
            {
                var foodimg = new FoodImage()
                {
                    ImageUrl = image.ImageUrl,
                    FoodId = image.FoodId
                };
                _db.FoodImages.Add(foodimg);
                _db.SaveChanges();
                return Ok(new {msg = "Add successfully!", foodimg });
            }
            return BadRequest(ModelState);
        }
        [HttpGet("SearchImageWithFoodID{foodid:int}")]
        public IActionResult GetImage([FromRoute]int foodid)
        {
            var imgs = _db.FoodImages.Where(x=> x.FoodId == foodid).ToList();
            if (imgs.Any())
            {
                return Ok(imgs);
            }
            return NotFound();
        }
        #endregion
    }
}
