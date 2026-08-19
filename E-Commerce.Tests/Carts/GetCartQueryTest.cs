using Application.Common.DTOs.CartDTOs;
using Application.Features.Carts.Commands.UpdateCartItem;
using Application.Features.Carts.Queries.GetCart;
using Domain.Entities.CartItems;
using Domain.Entities.Carts;
using Domain.Entities.Customers;
using Domain.Entities.OrderItems;
using Domain.Entities.Payments;
using Domain.Entities.Products;
using Domain.Entities.Shipments;
using Domain.Enums;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Tests.Carts
{
    public class GetCartQueryTest
    {
        private readonly GetCartQueryHandler _handler;

        public GetCartQueryTest()
        {
            var customers = new List<Customer>()
            {
                new Customer() {Id = Guid.Parse("A4694554-BB96-47D7-B0E6-7A41F5DB40D7"), UserId = 1, Email = "baher@gmail.com", Name = "baher"},
                new Customer() {Id = Guid.Parse("952C5524-9E2C-4D36-B2B9-909B232C8D89"), UserId = 2, Email = "baher@gmail.com", Name = "baher"},
                new Customer() {Id = Guid.Parse("66CA6F58-0CE0-4B37-BC2A-8665C8153ABF"), UserId = 3, Email = "baher@gmail.com", Name = "baher"},
                new Customer() {Id = Guid.NewGuid(), UserId = 4, Email = "baher@gmail.com", Name = "baher"}

            };

            var products = new List<Product>
                {
                    new Product { Id = 1, Name = "Laptop", CategoryId = 1, UnitPrice = 1000, Quantity = 10, Description = "kgifggeorigi", RowVersion = new byte[8], Image = "Products/i1" },
                    new Product { Id = 2, Name = "Mobile", CategoryId = 1, UnitPrice = 500, Quantity = 20 , Description = "kgifggeorigic", RowVersion = new byte[8], Image = "Products/i2"},
                    new Product { Id = 3, Name = "Airpods", CategoryId = 2, UnitPrice = 200, Quantity = 15 , Description = "kgifggeorigis", RowVersion = new byte[8], Image = "Products/i3"},
                    new Product { Id = 4, Name = "Smart Watch", CategoryId = 2, UnitPrice = 300, Quantity = 5 , Description = "kgifggeorigih", RowVersion = new byte[8], Image = "Products/i4"},
                    new Product { Id = 5, Name = "Boot", CategoryId = 3, UnitPrice = 150, Quantity = 8 , Description = "kgifggeorigij", RowVersion = new byte[8], Image = "Products/i5"}
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
                    Quantity = 2
                },
                new Cart
                {
                    Id = 3,
                    CustomerId =  Guid.Parse("42C89F86-E449-4C65-AF10-72FC0B2E9ED5"),
                },

            };
            var cartItems = new List<CartItem>()
            {
                new CartItem {Id = 1, ProductId = 1, CartId = 3, Quantity = 9, UnitPrice = 100m},
                new CartItem {Id = 2, ProductId = 3, CartId = 3, Quantity = 4, UnitPrice = 200m},
                new CartItem {Id = 3, ProductId = 5, CartId = 1, Quantity = 6, UnitPrice = 300m},
                new CartItem {Id = 4, ProductId = 1, CartId = 2, Quantity = 9, UnitPrice = 100m},
                new CartItem {Id = 5, ProductId = 3, CartId = 2, Quantity = 4, UnitPrice = 200m},
                new CartItem {Id = 6, ProductId = 5, CartId = 2, Quantity = 6, UnitPrice = 300m}
            };

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);

            context.AddRange(customers);
            context.Products.AddRange(products);
            context.Carts.AddRange(carts);
            context.CartItems.AddRange(cartItems);
            context.SaveChanges();

            _handler = new(context);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenCartIsNotFound()
        {
            // Arrange
            var command = new GetCartQuery
            (
                customerId: 4
            );

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CartErrors.NotFound);
        }
        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenCartIsFound()
        {
            // Arrange
            var command = new GetCartQuery
            (
                customerId: 2
            );

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<CartDetailsDto>();
        }


    }
}
