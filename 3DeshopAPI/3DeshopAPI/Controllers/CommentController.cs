using _3DeshopAPI.Models.Product;
using _3DeshopAPI.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _3DeshopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly IMapper _mapper;

        public CommentController(ICommentService commentService, IMapper mapper)
        {
            _commentService = commentService;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets product comments
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<ProductCommentDisplayModel>>> GetProductComments(Guid productId)
        {
            var response = await _commentService.GetProductComments(productId);

            return Ok(response.Select(x => _mapper.Map<ProductCommentDisplayModel>(x)));
        }

        /// <summary>
        /// Adds and sets product comment
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> AddProductComment(Guid productId, Guid userId, ProductCommentModel model)
        {
            var comment = _mapper.Map<Domain.Product.Comment>(model);

            return await _commentService.AddProductComment(productId, userId, comment);
        }
    }
}
