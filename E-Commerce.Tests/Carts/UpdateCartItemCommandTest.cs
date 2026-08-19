using Application.Features.Carts.Commands.DeleteCartItem;
using Application.Features.Carts.Commands.UpdateCartItem;
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
    public class UpdateCartItemCommandTest
    {
        private readonly UpdateCartItemCommandHandler _handler;
        private readonly DbContextMock<AppDbContext> _dbContext;

        public UpdateCartItemCommandTest()
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
                        CustomerId = Guid.Parse("A4694554-BB96-47D7-B0E6-7A41F5DB40D7"),
                    },
                    new Cart
                    {
                        Id = 2,
                        CustomerId = Guid.Parse("952C5524-9E2C-4D36-B2B9-909B232C8D89"),
                    },
                    new Cart
                    {
                        Id = 3,
                        CustomerId = Guid.Parse("42C89F86-E449-4C65-AF10-72FC0B2E9ED5"),
                    }
                };

            var cartItems = new List<CartItem>()
                {
                    new CartItem {
                        Id = 1,
                        ProductId = 1,
                        CartId = 2,
                        Cart = carts[1],
                        Product = new Domain.Entities.Products.Product {Id = 1 , Quantity = 10 , Name = "Phone" },
                        Quantity = 9,
                        UnitPrice = 100m
                    },
                    new CartItem {Id = 2, ProductId = 3, CartId = 3, Quantity = 4, UnitPrice = 100m},
                    new CartItem {Id = 3, ProductId = 5, CartId = 3, Quantity = 6, UnitPrice = 100m}
                };

            _dbContext = new DbContextMock<AppDbContext>(
                new DbContextOptionsBuilder<AppDbContext>().Options
            );
            _dbContext.CreateDbSetMock(x => x.Customers, customers);
            _dbContext.CreateDbSetMock(x => x.Carts, carts);
            _dbContext.CreateDbSetMock(x => x.CartItems, cartItems);

            _handler = new UpdateCartItemCommandHandler(_dbContext.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenCartItemNotFound()
        {
            // Arrange
            var command = new UpdateCartItemCommand
            (
                cartItemId : 99,
                customerId : 2,
                quantity : 5
            );

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CartErrors.ItemNotFound(99));
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenCartItemNotOwnedByCustomer()
        {
            // Arrange
            var command = new UpdateCartItemCommand
            (
                cartItemId : 1,
                customerId : 4, 
                quantity: 5
            );

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CartErrors.NotOwned);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenInsufficientStock()
        {
            // Arrange
            var command = new UpdateCartItemCommand
            (
                cartItemId: 1,
                customerId: 3,
                quantity: 20
            );

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CartErrors.InsufficientStock);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenUpdateIsValid()
        {
            // Arrange
            var command = new UpdateCartItemCommand
            (
                cartItemId : 1,
                customerId : 3,
                quantity   : 5
            );

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var updatedCartItem = await _dbContext.Object.CartItems.FindAsync(1);
            updatedCartItem.Quantity.Should().Be(5);
        }
    }
}
