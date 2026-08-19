
using Application.Features.Carts.Commands.AddToCart;
using Domain.Entities.CartItems;
using Domain.Entities.Carts;
using Domain.Entities.Customers;
using Domain.Entities.Products;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace E_Commerce.Tests.Carts
{
    public class AddToCartCommandTest
    {

        private readonly AddToCartCommandHnadler _handler;
        private readonly DbContextMock<AppDbContext> _dbContext;

        public AddToCartCommandTest()
        {
            var customers = new List<Customer>()
            {
                new Customer() {Id = Guid.NewGuid(), UserId = 1},
                new Customer() {Id = Guid.Parse("A4694554-BB96-47D7-B0E6-7A41F5DB40D7"), UserId = 2},
                new Customer() {Id = Guid.Parse("952C5524-9E2C-4D36-B2B9-909B232C8D89"), UserId = 3},
                new Customer() {Id = Guid.Parse("42C89F86-E449-4C65-AF10-72FC0B2E9ED5"), UserId = 4},
                new Customer() {Id = Guid.Parse("BF4DF798-A2BF-4B9C-8F99-21C0D6CFDD05"), UserId = 5},
                new Customer() {Id = Guid.Parse("38CCCDB9-27AA-43D1-B33B-AE6C56788FB0"), UserId = 6}

            };

            var products = new List<Domain.Entities.Products.Product>
                {
                    new Domain.Entities.Products.Product { Id = 1, Name = "Laptop", CategoryId = 1, UnitPrice = 1000, Quantity = 10, Description = "kgifggeorigi", RowVersion = new byte[8] },
                    new Domain.Entities.Products.Product { Id = 2, Name = "Mobile", CategoryId = 1, UnitPrice = 500, Quantity = 20 , Description = "kgifggeorigic", RowVersion = new byte[8], IsDeleted = true},
                    new Domain.Entities.Products.Product { Id = 3, Name = "Airpods", CategoryId = 2, UnitPrice = 200, Quantity = 15 , Description = "kgifggeorigis", RowVersion = new byte[8]},
                    new Domain.Entities.Products.Product { Id = 4, Name = "Smart Watch", CategoryId = 2, UnitPrice = 300, Quantity = 5 , Description = "kgifggeorigih", RowVersion = new byte[8], IsDeleted = true},
                    new Domain.Entities.Products.Product { Id = 5, Name = "Boot", CategoryId = 3, UnitPrice = 150, Quantity = 8 , Description = "kgifggeorigij", RowVersion = new byte[8]}
                };

            var carts = new List<Cart>()
            {
                new Cart
                {
                    Id = 1,
                    CustomerId =  Guid.Parse("A4694554-BB96-47D7-B0E6-7A41F5DB40D7"),
                },
                new Cart
                {
                    Id = 2,
                    CustomerId =  Guid.Parse("952C5524-9E2C-4D36-B2B9-909B232C8D89"),
                    CartItems = new List<CartItem>()
                    {
                        new CartItem {Id = 1, ProductId = 1},
                        new CartItem {Id = 2, ProductId = 3},
                        new CartItem {Id = 3, ProductId = 7}
                    }
                },
                new Cart
                {
                    Id = 3,
                    CustomerId =  Guid.Parse("42C89F86-E449-4C65-AF10-72FC0B2E9ED5"),
                    CartItems = new List<CartItem>()
                    {
                        new CartItem {Id = 1, ProductId = 1},
                        new CartItem {Id = 2, ProductId = 3},
                        new CartItem {Id = 3, ProductId = 4}
                    }
                },
                new Cart
                {
                    Id = 4,
                    CustomerId =  Guid.Parse("BF4DF798-A2BF-4B9C-8F99-21C0D6CFDD05"),
                    CartItems = new List<CartItem>()
                    {
                        new CartItem {Id = 1, ProductId = 1, Quantity = 9},
                        new CartItem {Id = 2, ProductId = 3, Quantity = 4},
                        new CartItem {Id = 3, ProductId = 5, Quantity = 9}
                    }
                },
                new Cart
                {
                    Id = 5,
                    CustomerId =  Guid.Parse("38CCCDB9-27AA-43D1-B33B-AE6C56788FB0"),
                    CartItems = new List<CartItem>()
                    {
                        new CartItem {Id = 1, ProductId = 1, CartId = 5, Quantity = 9, UnitPrice = 100m},
                        new CartItem {Id = 2, ProductId = 3, CartId = 5, Quantity = 4, UnitPrice = 100m},
                        new CartItem {Id = 3, ProductId = 5, CartId = 5, Quantity = 6, UnitPrice = 100m}
                    }
                }

            };
            var cartItems = new List<CartItem>()
            {
                new CartItem {Id = 1, ProductId = 1, CartId = 5, Quantity = 9, UnitPrice = 100m},
                new CartItem {Id = 2, ProductId = 3, CartId = 5, Quantity = 4, UnitPrice = 100m},
                new CartItem {Id = 3, ProductId = 5, CartId = 5, Quantity = 6, UnitPrice = 100m}
            };

            _dbContext = new DbContextMock<AppDbContext>
                (
                   new DbContextOptionsBuilder<AppDbContext>().Options
                );
            _dbContext.CreateDbSetMock(x => x.Customers, customers);
            _dbContext.CreateDbSetMock(x => x.Carts, carts);
            _dbContext.CreateDbSetMock(x => x.Products, products);
            _dbContext.CreateDbSetMock(x => x.CartItems, cartItems);

            _handler = new(_dbContext.Object);

        }

        [Fact]
        public async Task Handler_Shoule_ReturnFailur_When_ProductIsNotFound()
        {
            var command = new AddToCartCommand(6, 20, 1);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(ProductErrors.NotFound(6));
        }
        [Fact]
        public async Task Handler_Should_ReturnFailur_When_ProductIsDeleted()
        {
            var command = new AddToCartCommand(2, 20, 1);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(ProductErrors.Discontinued);
        }
        [Fact]
        public async Task Handler_Should_ReturnFailur_When_ProductIsInSufficientStock()
        {
            var command = new AddToCartCommand(1, 11, 2);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CartErrors.InsufficientStock);
        }
        [Fact]
        public async Task Handler_Should_ReturnSuccess_When_ProductExistsAndHasSufficientStock()
        {
            var command = new AddToCartCommand(3, 11, 6);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _dbContext.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once());
        }

    }
}
