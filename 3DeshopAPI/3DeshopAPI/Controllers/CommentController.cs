using _3DeshopAPI.Models.Product;
using _3DeshopAPI.Services.Interfaces;
using AutoMapper;
using Domain.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _3DeshopAPI.Controllers;

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
    [HttpGet("{id}")]
    public async Task<ActionResult<List<ProductCommentDisplayModel>>> GetProductComments(Guid id)
    {
        var response = await _commentService.GetProductComments(id);

        return Ok(response.Select(x => _mapper.Map<ProductCommentDisplayModel>(x)));
    }

    /// <summary>
    /// Adds and sets product comment
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="userId"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("post-comment/{productId}/{userId}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> AddProductComment(Guid productId, Guid userId, [FromBody] ProductCommentModel model)
    {
        var comment = _mapper.Map<Comment>(model);

        return await _commentService.AddProductComment(productId, userId, comment);
    }
}