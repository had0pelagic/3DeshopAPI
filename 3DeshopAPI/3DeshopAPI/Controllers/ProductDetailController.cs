using _3DeshopAPI.Models.ProductDetail;
using _3DeshopAPI.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace _3DeshopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductDetailController : ControllerBase
    {
        private readonly IProductDetailService _productDetailService;
        private readonly IMapper _mapper;

        public ProductDetailController(IProductDetailService productDetailService, IMapper mapper)
        {
            _productDetailService = productDetailService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-categories")]
        public async Task<ActionResult<List<CategoryModel>>> GetAllCategories()
        {
            var categories = await _productDetailService.GetAllCategories();

            return Ok(categories.Select(x => _mapper.Map<CategoryModel>(x)));
        }

        /// <summary>
        /// Get all formats
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-formats")]
        public async Task<ActionResult<List<FormatModel>>> GetAllFormats()
        {
            var formats = await _productDetailService.GetAllFormats();

            return Ok(formats.Select(x => _mapper.Map<FormatModel>(x)));
        }
    }
}
