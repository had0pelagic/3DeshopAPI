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
        private readonly IMapper _mapper;

        public ProductController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all products
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<ProductPageModel>>> GetAllProducts()
        {
            var products = await _productService.GetAllProducts();

            return Ok(products.Select(x => _mapper.Map<ProductPageModel>(x)));
        }

        /// <summary>
        /// Get product by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductModel>> GetProduct(Guid id)
        {
            var product = await _productService.GetProduct(id);

            return Ok(product);
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
            var product = await _productService.UploadProduct(model);

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, await _productService.ToProductModel(product));
        }
    }
}
