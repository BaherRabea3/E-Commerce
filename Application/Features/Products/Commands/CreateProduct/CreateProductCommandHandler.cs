using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities.Categories;
using Domain.Entities.Products;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Products.Commands.CreateProduct
{
    public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<int>>
    {
        private readonly IAppDbContext _context;
        private readonly IFileService _fileService;

        public CreateProductCommandHandler(IFileService fileService , IAppDbContext context)
        {
            _fileService = fileService;
            _context = context;
        }

        public async Task<Result<int>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var isCategoryExist = await _context.Categories.AnyAsync(x => x.Id == request.categoryId , cancellationToken);

            if (!isCategoryExist)
                return Result<int>.Failure<int>(CategoryErrors.NotFound(request.categoryId));

            string? imagePath = null;
            try
            {
                imagePath = await _fileService.UploadFileAsync(request.Image, "Products");

                var product = new Product()
                {
                    Name = request.name,
                    Description = request.description,
                    Image = imagePath,
                    Quantity = request.quantity,
                    UnitPrice = request.unitPrice,
                    CategoryId = request.categoryId
                };

                _context.Products.Add(product);

                await _context.SaveChangesAsync(cancellationToken);

                return Result<int>.Success(product.Id);
            }
            catch
            {
                _fileService.DeleteFile(imagePath);

                throw;
            }
           
        }
    }
}
