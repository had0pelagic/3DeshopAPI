using _3DeshopAPI.Models.Product;
using _3DeshopAPI.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _3DeshopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public ProductController(IProductService productService, IUserService userService, IMapper mapper)
        {
            _productService = productService;
            _userService = userService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all products
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<ProductDisplayModel>>> GetAllProducts()
        {
            var response = await _productService.GetAllProducts();

            return Ok(response);
        }

        /// <summary>
        /// Get product by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductModel>> GetProduct(Guid id)
        {
            var response = await _productService.GetProduct(id);

            return Ok(response);
        }

        /// <summary>
        /// Upload product
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<ProductModel>> UploadProduct(ProductUploadModel model)
        {
            var response = await _productService.UploadProduct(model);

            return CreatedAtAction(nameof(GetProduct), new { id = response.Id }, await _productService.ToProductModel(response));
        }

        /// <summary>
        /// Returns all user bought products
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/purchases")]
        public async Task<ActionResult<List<ProductDisplayModel>>> GetAllPurchases(Guid id)
        {
            ///get purchased product ids
            var productIds = await _userService.GetPurchasedIds(id);
            var response = await _productService.GetAllPurchases(id, productIds);

            return Ok(response);
        }
    }
}
