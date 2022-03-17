using _3DeshopAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _3DeshopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        /// <summary>
        /// Returns all files for download associated with given product
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("get-files/{productId}/{userId}")]
        [Authorize(Roles = "User")]
        public async Task<List<FileContentResult>> GetProductFiles(Guid productId, Guid userId)
        {
            var response = await _fileService.GetProductFiles(productId, userId);

            return response;
        }
    }
}
