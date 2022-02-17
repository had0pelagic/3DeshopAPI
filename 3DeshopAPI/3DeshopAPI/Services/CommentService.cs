﻿using _3DeshopAPI.Exceptions;
using _3DeshopAPI.Services.Interfaces;
using Domain.Product;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace _3DeshopAPI.Services
{
    public class CommentService : ICommentService
    {
        private readonly ILogger<ProductService> _logger;
        private readonly IUserService _userService;
        private readonly Context _context;

        public CommentService(ILogger<ProductService> logger, IUserService userService, Context context)
        {
            _logger = logger;
            _userService = userService;
            _context = context;
        }

        /// <summary>
        /// Adds and sets product comment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> AddProductComment(Guid productId, Guid userId, Comment model)
        {
            var product = await _context.Products.FindAsync(productId);
            var user = await _userService.GetUser(userId);

            if (product == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.ProductNotFound);
            }
            if (user == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.UserNotFound);
            }

            var comment = new Comment()
            {
                Product = product,
                User = user,
                Description = model.Description,
                Created = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return new NoContentResult();
        }
    }
}