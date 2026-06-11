
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities.Categories;
using Domain.Entities.Products;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Products.Commands.UpdateProduct
{
    public sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand , Result>
    {
        private readonly IAppDbContext _context;
        private readonly IFileService _fileService;

        public UpdateProductCommandHandler(IAppDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync([request.id], cancellationToken);

            if (product is null)
                return Result.Failure(ProductErrors.NotFound(request.id));

            var isCategoryExist = await _context.Categories
                .AnyAsync(x => x.Id == request.categoryId, cancellationToken);

            if (!isCategoryExist)
                return Result.Failure(CategoryErrors.NotFound(request.categoryId));

            string? olderImagePath = product.Image;
            string? newImagePath = null;
            try
            {
                product.Name = request.name;
                product.Description = request.description;
                product.UnitPrice = request.unitPrice;
                product.Quantity = request.quantity;
                product.CategoryId = request.categoryId;

                newImagePath = await _fileService.UploadFileAsync(request.Image, "Products");

                product.Image = newImagePath;


                await _context.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                _fileService.DeleteFile(newImagePath);
                throw;
            }

            _fileService.DeleteFile(olderImagePath);

            return Result.Success();
        }
    }
}
