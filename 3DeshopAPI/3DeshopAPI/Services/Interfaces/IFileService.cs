﻿using Microsoft.AspNetCore.Mvc;

namespace _3DeshopAPI.Services.Interfaces
{
    public interface IFileService
    {
        public Task<List<FileContentResult>> GetProductFiles(Guid productId, Guid userId);
    }
}