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
        /// Changes product activity status
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("change-product-status/{id}")]
        public async Task<IActionResult> ChangeProductStatus(Guid id)
        {
            return await _productService.ChangeProductStatus(id);
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
        /// Gets all products by given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("get-products-by-name/{name}")]
        public async Task<ActionResult<List<ProductDisplayModel>>> GetProductsByName(string name)
        {
            var response = await _productService.GetProductsByName(name);

            return Ok(response);
        }

        /// <summary>
        /// Gets all products by given criteria
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("get-products-by-criteria")]
        public async Task<ActionResult<List<ProductDisplayModel>>> GetProductsByCriteria([FromBody] ProductFindByCriteriaModel model)
        {
            var response = await _productService.GetProductsByCriteria(model);

            return Ok(response);
        }

        /// <summary>
        /// Gets all products by given ids and sorts by price
        /// </summary>
        /// <param name="model"></param> 
        /// <returns></returns>
        [HttpPost("get-products-by-ids-order-by-price")]
        public async Task<ActionResult<List<ProductDisplayModel>>> GetProductsByGivenIdsAndOrderByPrice(ProductGetByIdAndOrderModel model)
        {
            var response = await _productService.GetProductsByGivenIdsAndOrderByPrice(model);

            return Ok(response);
        }

        /// <summary>
        /// Get all products by given ids and sorts by date
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("get-products-by-ids-order-by-date")]
        public async Task<ActionResult<List<ProductDisplayModel>>> GetProductsByGivenIdsAndOrderByDate(ProductGetByIdAndOrderModel model)
        {
            var response = await _productService.GetProductsByGivenIdsAndOrderByDate(model);

            return Ok(response);
        }

        /// <summary>
        /// Gets all products ordered by price
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-products-order-by-price/{ascending}")]
        public async Task<ActionResult<List<ProductDisplayModel>>> GetProductsOrderByPrice(bool ascending)
        {
            var response = await _productService.GetProductsOrderByPrice(ascending);

            return Ok(response);
        }

        /// <summary>
        /// Gets all products ordered by upload date
        /// </summary>
        /// <param name="ascending"></param>
        /// <returns></returns>
        [HttpGet("get-products-order-by-date/{ascending}")]
        public async Task<ActionResult<List<ProductDisplayModel>>> GetProductsOrderByUploadDate(bool ascending)
        {
            var response = await _productService.GetProductsOrderByUploadDate(ascending);

            return Ok(response);
        }

        /// <summary>
        /// Returns all user uploaded products
        /// </summary>
        /// <returns></returns>
        [HttpGet("user-products/{id}")]
        public async Task<ActionResult<List<ProductTableDisplayModel>>> GetUserProducts(Guid id)
        {
            var response = await _productService.GetUserProducts(id);

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
        [HttpPost("upload-product")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<ProductModel>> UploadProduct([FromBody] ProductUploadModel model)
        {
            var response = await _productService.UploadProduct(model);

            return CreatedAtAction(nameof(GetProduct), new { id = response.Id }, await _productService.ProductToProductModel(response));
        }

        /// <summary>
        /// Returns all user bought products
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/purchases")]
        public async Task<ActionResult<List<ProductDisplayModel>>> GetAllPurchases(Guid id)
        {
            var productIds = await _userService.GetPurchasedIds(id);
            var response = await _productService.GetAllPurchases(id, productIds);

            return Ok(response);
        }
    }
}
