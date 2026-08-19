using Application.Features.Carts.Commands.ClearCart;
using Application.Features.Carts.Commands.DeleteCartItem;
using Domain.Entities.CartItems;
using Domain.Entities.Carts;
using Domain.Entities.Customers;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace E_Commerce.Tests.Carts
{
    public class DeleteCartItemCommandTest
    {

        private readonly DeleteCartItemCommandHandler _handler;
        private readonly DbContextMock<AppDbContext> _dbContext;

        public DeleteCartItemCommandTest()
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
                },
                new Cart
                {
                    Id = 3,
                    CustomerId =  Guid.Parse("42C89F86-E449-4C65-AF10-72FC0B2E9ED5"),
                }

            };
            var cartItems = new List<CartItem>()
            {
                new CartItem {Id = 1, ProductId = 1, CartId = 2, Quantity = 9, UnitPrice = 100m},
                new CartItem {Id = 2, ProductId = 3, CartId = 3, Quantity = 4, UnitPrice = 100m},
                new CartItem {Id = 3, ProductId = 5, CartId = 3, Quantity = 6, UnitPrice = 100m}
            };

            _dbContext = new DbContextMock<AppDbContext>
                (
                   new DbContextOptionsBuilder<AppDbContext>().Options
                );
            _dbContext.CreateDbSetMock(x => x.Customers, customers);
            _dbContext.CreateDbSetMock(x => x.Carts, carts);
            _dbContext.CreateDbSetMock(x => x.CartItems, cartItems);

            _handler = new(_dbContext.Object);

        }

        [Fact]
        public async Task Handler_Shoule_ReturnFailur_When_CartItemIsNotFound()
        {
            var command = new DeleteCartItemCommand(7, 3);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CartErrors.ItemNotFound(7));
        }
        [Fact]
        public async Task Handler_Shoule_ReturnSuccess_When_CartItemeIsFound()
        {
            var command = new DeleteCartItemCommand(3, 4);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            _dbContext.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

    }
}
