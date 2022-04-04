using _3DeshopAPI.Exceptions;
using _3DeshopAPI.Services.Interfaces;
using AutoMapper;
using Domain.Order;
using Domain.Product;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Services.Interfaces;

namespace _3DeshopAPI.Services
{
    public class FileService : IFileService
    {
        private readonly ILogger<ProductService> _logger;
        private readonly IUserService _userService;
        private readonly IPaymentService _paymentService;
        private readonly Context _context;

        public FileService(ILogger<ProductService> logger, IUserService userService, IPaymentService paymentService, Context context)
        {
            _logger = logger;
            _userService = userService;
            _paymentService = paymentService;
            _context = context;
        }

        /// <summary>
        /// Returns file content result list, which contains bytes, filenames
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidClientOperationException"></exception>
        public async Task<List<FileContentResult>> GetOrderFiles(Guid orderId, Guid userId)
        {
            var user = await _userService.GetUser(userId);

            if (user == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.UserNotFound);
            }

            var orderFiles = await GetOrderFiles();
            var files = GetOrderFileData(orderFiles, orderId);

            return GetFileContents(files);
        }

        /// <summary>
        /// Returns file content result list, which contains bytes, filenames
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidClientOperationException"></exception>
        public async Task<List<FileContentResult>> GetProductFiles(Guid productId, Guid userId)
        {
            var user = await _userService.GetUser(userId);

            if (user == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.UserNotFound);
            }

            var payment = await _paymentService.UserHasPaid(productId, userId);

            if (payment == null)
            {
                throw new InvalidClientOperationException(ErrorCodes.NotPaid);
            }

            var productFiles = await GetProductFiles();
            var files = GetProductFileData(productFiles, productId);

            return GetFileContents(files);
        }

        /// <summary>
        /// Returns FileContentResult list, which is constructed by using database files
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        private List<FileContentResult> GetFileContents(List<Domain.Product.File> files)
        {
            var fileContents = new List<FileContentResult>();

            foreach (var file in files)
            {
                var format = FormatParser(file.Format);
                var fileContent = new FileContentResult(file.Data, format);
                fileContent.FileDownloadName = file.Name;
                fileContents.Add(fileContent);
            }

            return fileContents;
        }

        /// <summary>
        /// Returns ProductFiles table from database
        /// </summary>
        /// <returns></returns>
        private async Task<List<ProductFiles>> GetProductFiles()
        {
            var files = await _context.ProductFiles
                .Include(x => x.Product)
                .Include(x => x.File)
                .ToListAsync();

            return files;
        }

        /// <summary>
        /// Returns OrderFiles table from database
        /// </summary>
        /// <returns></returns>
        private async Task<List<OrderFiles>> GetOrderFiles()
        {
            var files = await _context.OrderFiles
                .Include(x => x.Order)
                .Include(x => x.File)
                .ToListAsync();

            return files;
        }

        /// <summary>
        /// Returns all files associated with product
        /// </summary>
        /// <param name="productFiles"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        private List<Domain.Product.File> GetProductFileData(List<ProductFiles> productFiles, Guid productId)
        {
            var files = productFiles
                .Where(x => x.Product.Id == productId)
                .Select(x => x.File)
                .ToList();

            return files;
        }

        /// <summary>
        /// Returns all files associated with order
        /// </summary>
        /// <param name="orderFiles"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        private List<Domain.Product.File> GetOrderFileData(List<OrderFiles> orderFiles, Guid orderId)
        {
            var files = orderFiles
                .Where(x => x.Order.Id == orderId)
                .Select(x => x.File)
                .ToList();

            return files;
        }

        /// <summary>
        /// Removes unnecessary parts of given format
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        private string FormatParser(string format)
        {
            var from = format.IndexOf(':') + 1;
            var to = format.LastIndexOf(';');

            return format.Substring(from, to - from);
        }
    }
}
