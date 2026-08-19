using Application.Common.DTOs.OrderDTOs;
using Application.Common.Interfaces;
using Application.Features.Orders.Commands.PlaceOrder;
using AutoFixture;
using Domain.Entities.Addresses;
using Domain.Entities.CartItems;
using Domain.Entities.Carts;
using Domain.Entities.Categories;
using Domain.Entities.Customers;
using Domain.Entities.Orders;
using Domain.Entities.Payments;
using Domain.Entities.Products;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Moq;
using Stripe.Climate;
using static Application.Common.Interfaces.IPaymentGatewayService;

namespace E_Commerce.Tests.Orders
{
    public class PlaceOrderCommandTest
    {
        private readonly PlaceOrderCommandHandler _handler;
        private readonly DbContextMock<AppDbContext> _dbContext;
        private readonly Mock<IPaymentGatewayService> _paymentServiceMock;
        private readonly IFixture _fixture;

        public PlaceOrderCommandTest()
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
            var categories = new List<Category>
                {
                    new Category { Id = 1, Name = "Electronics", Description = "greeafherh" },
                    new Category { Id = 2, Name = "Accessories", Description = "greeafhenrh" },
                    new Category { Id = 3, Name = "Footwear", Description = "greeafaherh" }
                };

            var products = new List<Domain.Entities.Products.Product>
                {
                    new Domain.Entities.Products.Product { Id = 1, Name = "Laptop", CategoryId = 1, UnitPrice = 1000, Quantity = 10, Description = "kgifggeorigi", RowVersion = new byte[8] },
                    new Domain.Entities.Products.Product { Id = 2, Name = "Mobile", CategoryId = 1, UnitPrice = 500, Quantity = 20 , Description = "kgifggeorigic", RowVersion = new byte[8]},
                    new Domain.Entities.Products.Product { Id = 3, Name = "Airpods", CategoryId = 2, UnitPrice = 200, Quantity = 15 , Description = "kgifggeorigis", RowVersion = new byte[8]},
                    new Domain.Entities.Products.Product { Id = 4, Name = "Smart Watch", CategoryId = 2, UnitPrice = 300, Quantity = 5 , Description = "kgifggeorigih", RowVersion = new byte[8], IsDeleted = true},
                    new Domain.Entities.Products.Product { Id = 5, Name = "Boot", CategoryId = 3, UnitPrice = 150, Quantity = 8 , Description = "kgifggeorigij", RowVersion = new byte[8]}
                };

            var orders = new List<Domain.Entities.Orders.Order>()
            {
                new Domain.Entities.Orders.Order {Id = 1, CustomerId = Guid.NewGuid(), IdempotencyKey = Guid.Parse("2705DC0E-A7FC-443E-A0D9-A31B37A2E372"), Payment = null},
                new Domain.Entities.Orders.Order {Id = 2, CustomerId = Guid.NewGuid(), IdempotencyKey = Guid.Parse("71C8E981-7923-4B44-AB1E-1CCACE95B520"), Payment = new Payment {Id = 1, GatewayTransactionId = "1321566", OrderId = 2} },
                 new Domain.Entities.Orders.Order {Id = 3, CustomerId = Guid.Parse("38CCCDB9-27AA-43D1-B33B-AE6C56788FB0"), IdempotencyKey = Guid.Parse("AFBC4A43-98B0-4421-B96A-8ED2E3308E33") }
            };

            var payments = new List<Payment>()
            {
                new Payment {Id = 1, GatewayTransactionId = "1321566", OrderId = 2}
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
            var addresses = new List<Address>()
            {
                new Address {Id = 1, CustomerId = Guid.NewGuid()},
                new Address {Id = 2, CustomerId = Guid.Parse("A4694554-BB96-47D7-B0E6-7A41F5DB40D7")},
                new Address {Id = 3, CustomerId = Guid.Parse("952C5524-9E2C-4D36-B2B9-909B232C8D89")},
                new Address {Id = 4, CustomerId = Guid.Parse("42C89F86-E449-4C65-AF10-72FC0B2E9ED5")},
                new Address {Id = 5, CustomerId = Guid.Parse("BF4DF798-A2BF-4B9C-8F99-21C0D6CFDD05")},
                new Address {Id = 6, CustomerId = Guid.Parse("38CCCDB9-27AA-43D1-B33B-AE6C56788FB0")},
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
            _dbContext.CreateDbSetMock(x => x.Orders, orders);
            _dbContext.CreateDbSetMock(x => x.Payments, payments);
            _dbContext.CreateDbSetMock(x => x.Carts, carts);
            _dbContext.CreateDbSetMock(x => x.Addresses, addresses);
            _dbContext.CreateDbSetMock(x => x.Products, products);
            _dbContext.CreateDbSetMock(x => x.Categories, categories);
            _dbContext.CreateDbSetMock(x => x.CartItems, cartItems);

            _paymentServiceMock = new Mock<IPaymentGatewayService>();

            _handler = new PlaceOrderCommandHandler(_dbContext.Object, _paymentServiceMock.Object);

            _fixture = new Fixture();
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_When_OrderAlreadyExistedAndPaymentIsNull()
        {
            //Arrange
            var command = new PlaceOrderCommand(1, 1, Guid.Parse("2705DC0E-A7FC-443E-A0D9-A31B37A2E372"));

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<PlaceOrderResponseDto>();
        }
        [Fact]
        public async Task Handle_Should_ReturnSuccess_When_OrderAlreadyExistedAndTransactionIdIsNotNull()
        {
            //Arrange
            var command = new PlaceOrderCommand(1, 1, Guid.Parse("71C8E981-7923-4B44-AB1E-1CCACE95B520"));

            var mockResult = new IPaymentGatewayService.GetClientSecretResult("Mocked_client_secret");

            _paymentServiceMock.Setup(x => x.GetClientSecretAsync("1321566", CancellationToken.None))
                .ReturnsAsync(mockResult);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<PlaceOrderResponseDto>();
            _paymentServiceMock.Verify(x => x.GetClientSecretAsync("1321566", CancellationToken.None), Times.Once);
        }
        [Fact]
        public async Task Handle_Should_ReturnFailure_When_CartIsNull()
        {
            //Arrange
            var command = new PlaceOrderCommand(1, 1, Guid.NewGuid());

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CartErrors.NotFound);
        }
        [Fact]
        public async Task Handle_Should_ReturnFailure_When_AddressIsNull()
        {
            //Arrange
            var command = new PlaceOrderCommand(2, 1, Guid.NewGuid());

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(AddressErrors.NotFound);
        }
        [Fact]
        public async Task Handle_Should_ReturnFailure_When_CartIsEmpty()
        {
            //Arrange
            var command = new PlaceOrderCommand(2, 2, Guid.NewGuid());

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CartErrors.IsEmpty);
        }
        [Fact]
        public async Task Handle_Should_ReturnFailure_When_ProductIsNotFound()
        {
            //Arrange
            var command = new PlaceOrderCommand(3, 3, Guid.NewGuid());

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(ProductErrors.NotFound(7));
        }
        [Fact]
        public async Task Handle_Should_ReturnFailure_When_ProductIsDeleted()
        {
            //Arrange
            var command = new PlaceOrderCommand(4, 4, Guid.NewGuid());

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(ProductErrors.Discontinued);
        }
        [Fact]
        public async Task Handle_Should_ReturnFailure_When_ProductStockIsInsufficient()
        {
            //Arrange
            var command = new PlaceOrderCommand(5, 5, Guid.NewGuid());

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(CartErrors.InsufficientStock);
        }
        [Fact]
        public async Task Handle_Should_ReturnSuccess()
        {
            //Arrange
            var command = new PlaceOrderCommand(6, 6, Guid.NewGuid());

            var mockPaymentIntent = new IPaymentGatewayService.CreatePaymentIntentResult("Mock_client_secret", "124567890");

            _paymentServiceMock.Setup(x => x.CreatePaymentIntentAsync(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<int>(), CancellationToken.None))
                .ReturnsAsync(mockPaymentIntent);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<PlaceOrderResponseDto>();
            _dbContext.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.AtMost(2));
            _paymentServiceMock.Verify(x => x.CreatePaymentIntentAsync(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<int>(), CancellationToken.None), Times.Once);
        }
        [Fact]
        public async Task Handle_Should_ReturnInsufficientStock_When_ConcurrencyConflictOccurs()
        {
            //Arrange
            var command = new PlaceOrderCommand(6, 6, Guid.NewGuid());

            _dbContext.Setup(x => x.SaveChangesAsync(CancellationToken.None))
                .ThrowsAsync(new DbUpdateConcurrencyException());

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(CartErrors.InsufficientStock);

            _paymentServiceMock.Verify(
                x => x.CreatePaymentIntentAsync(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

    }
}
