﻿using Assignment_Server.Data;
using Assignment_Server.Interfaces;
using Assignment_Server.Mapper;
using Assignment_Server.Models;
using Assignment_Server.Models.DTO;
using Assignment_Server.Models.DTO.Food;
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
                if(_foodService.CreateFood(food))
                {
                    return StatusCode(201, food.toFoodDTO());
                }
                return BadRequest("Somethings went wrong ( FoodController line : 31 )");
            }
            return BadRequest(ModelState);
        }




        // Hiển thị tất cả sản phẩm
        [HttpGet]
        public IActionResult GetAll(int? page, int pageSize)
        {
            var foods = _foodService.GetAllFood().Select(x=> x.toFoodDTO());
            int pageNumber = (page ?? 1);
            return Ok(foods.ToPagedList(pageNumber,pageSize));
        }


 
        // Tìm sản phẩm theo id
        [HttpGet("searchId{id:int}")]
        public IActionResult GetById(int?page,[FromRoute] int id)
        {

            var getFood = _foodService.GetById(id);
            if (getFood != null)
            {
                getFood.View++;
                if (_foodService.UpdateFood(getFood))
                {
                    return Ok(getFood.toFoodDTO());
                }
            }
            return BadRequest("Food not found");
        }




        // Tìm kiếm theo tên
        [HttpGet("SearchName={searchName}")]
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
        [HttpGet("getByFilter")]
        public IActionResult GetByFilter([FromQuery]decimal? minrange, [FromQuery]decimal? maxrange)
        {
            var filteredFoods = _foodService.getByFilter(minrange,maxrange);
            return Ok(filteredFoods.Select(x => x.toFoodDTO()));
        }




        // Lấy sản phẩm theo mã danh mục
        [HttpGet("SearchCategoryId{categoryId:int}")]
        public IActionResult GetByCategoryID([FromRoute]int categoryId)
        {
            var foods = _foodService.getByCategoryID(categoryId);
            if (foods.Any())
                return Ok(foods.Select(x => x.toFoodDTO()));
            return NotFound();
        }




        // Sắp xếp giá giảm dần
        [HttpGet("sort{sort}")]
        public IActionResult Sort(int? page, string sort)
        {
            var foods = _foodService.Sort(sort).Select(x => x.toFoodDTO());
            var pageSize = 9;
            int pageNumber = (page ?? 1);
            return Ok(foods.ToPagedList(pageNumber, pageSize));
        }



 
        // Cập nhật sản phẩm
        [HttpPut]
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

                    if (_foodService.UpdateFood(upFood))
                    {
                        return Ok(new { message = "Updated!", obj = upFood.toFoodDTO() });
                    }
                }
                return BadRequest(ModelState);
            }
            return NotFound();
        }




        // Xoá sản phẩm
        [HttpDelete("{id:int}")]
        public IActionResult DeleteFood([FromRoute]int id)
        {
            if (_foodService.DeleteFood(id))
            {
                return Ok(new { message = "Deleted!"});
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
                if (_imgService.CreateFoodImage(foodimg))
                {
                    return StatusCode(201, foodimg);
                }        
            }
            return BadRequest(ModelState);
        }
        [HttpGet("GetImages{foodid:int}")]
        public IActionResult GetImage([FromRoute]int foodid)
        {
            var imgs = _imgService.GetFoodImages(foodid);
            if (imgs.Any())
            {
                return Ok(imgs);
            }
            return NotFound();
        }
        #endregion
    }
}
