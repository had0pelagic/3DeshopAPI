using _3DeshopAPI.Models.Product;
using _3DeshopAPI.Services.Interfaces;
using AutoMapper;
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

            return Ok(_mapper.Map<ProductModel>(product));
        }

        /// <summary>
        /// Upload product
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(Roles = "User")]
        public async Task<ActionResult<ProductUploadModel>> UploadProduct(ProductModel model)
        {
            var newProduct = _mapper.Map<Domain.Product.Product>(model);

            await _productService.UploadProduct(newProduct);

            return CreatedAtAction(nameof(GetProduct), new { id = newProduct.Id }, _mapper.Map<ProductModel>(newProduct));
        }

        /// <summary>
        /// Sets product category
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("set-product-category")]
        //[Authorize(Roles = "User")]
        public async Task<IActionResult> SetProductCategory(Guid productId, Guid categoryId)
        {
            return await _productService.SetProductCategory(productId, categoryId);
        }

        /// <summary>
        /// Sets product format
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="formatId"></param>
        /// <returns></returns>
        [HttpPost("set-product-format")]
        //[Authorize(Roles = "User")]
        public async Task<IActionResult> SetProductFormat(Guid productId, Guid formatId)
        {
            return await _productService.SetProductFormat(productId, formatId);
        }

        /// <summary>
        /// Adds and sets product image
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("set-product-image")]
        //[Authorize(Roles = "User")]
        public async Task<IActionResult> SetProductImage(Guid productId, ProductImageModel model)
        {
            var image = _mapper.Map<Domain.Product.Image>(model);

            return await _productService.SetProductImage(productId, image);
        }

        /// <summary>
        /// Adds and sets product comment
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("set-product-comment")]
        //[Authorize(Roles = "User")]
        public async Task<IActionResult> AddProductComment(Guid productId, Guid userId, ProductCommentModel model)
        {
            var comment = _mapper.Map<Domain.Product.Comment>(model);

            return await _productService.AddProductComment(productId, userId, comment);
        }
    }
}
