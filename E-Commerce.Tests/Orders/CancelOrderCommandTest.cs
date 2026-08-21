using Application.Common.DTOs.OrderDTOs;
using Application.Common.Interfaces;
using Application.Features.Orders.Commands.CancelOrder;
using Domain.Entities.Customers;
using Domain.Entities.OrderItems;
using Domain.Entities.Orders;
using Domain.Entities.Payments;
using Domain.Enums;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Hangfire;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace E_Commerce.Tests.Orders
{
    public class CancelOrderCommandTest
    {
        private readonly CancelOrderCommandHandler _handler;
        private readonly DbContextMock<AppDbContext> _dbContextMock;
        private readonly Mock<IPaymentGatewayService> _paymentServiceMock;
        private readonly Mock<IBackgroundJobClient> _backgroundJobClientMock;

        public CancelOrderCommandTest()
        {
            var customers = new List<Customer>()
            {
                new Customer() {Id = Guid.Parse("A4694554-BB96-47D7-B0E6-7A41F5DB40D7"), UserId = 1},
                new Customer() {Id = Guid.Parse("952C5524-9E2C-4D36-B2B9-909B232C8D89"), UserId = 2},
                new Customer() {Id = Guid.Parse("66CA6F58-0CE0-4B37-BC2A-8665C8153ABF"), UserId = 3},

            };
            var products = new List<Domain.Entities.Products.Product>
                {
                    new Domain.Entities.Products.Product { Id = 1, Name = "Laptop", CategoryId = 1, UnitPrice = 1000, Quantity = 10, Description = "kgifggeorigi", RowVersion = new byte[8] },
                    new Domain.Entities.Products.Product { Id = 2, Name = "Mobile", CategoryId = 1, UnitPrice = 500, Quantity = 20 , Description = "kgifggeorigic", RowVersion = new byte[8]},
                    new Domain.Entities.Products.Product { Id = 3, Name = "Airpods", CategoryId = 2, UnitPrice = 200, Quantity = 15 , Description = "kgifggeorigis", RowVersion = new byte[8]},
                    new Domain.Entities.Products.Product { Id = 4, Name = "Smart Watch", CategoryId = 2, UnitPrice = 300, Quantity = 5 , Description = "kgifggeorigih", RowVersion = new byte[8], IsDeleted = true},
                    new Domain.Entities.Products.Product { Id = 5, Name = "Boot", CategoryId = 3, UnitPrice = 150, Quantity = 8 , Description = "kgifggeorigij", RowVersion = new byte[8]}
                };

            var orders = new List<Order>()
            {
                 new Order {Id = 1, CustomerId = Guid.Parse("A4694554-BB96-47D7-B0E6-7A41F5DB40D7"), IdempotencyKey = Guid.NewGuid(), Status = OrderStatus.AwaitingPayment },
                 new Order {Id = 2, CustomerId =  Guid.Parse("952C5524-9E2C-4D36-B2B9-909B232C8D89"), IdempotencyKey = Guid.NewGuid(), Status = OrderStatus.Delivered },
                 new Order
                 {
                     Id = 3,
                     CustomerId =Guid.Parse("66CA6F58-0CE0-4B37-BC2A-8665C8153ABF"), 
                     IdempotencyKey = Guid.NewGuid(),
                     Status = OrderStatus.Confirmed,
                     Total = 2000m,
                     Payment = new Payment
                     {
                         Id = 1,
                         GatewayTransactionId = "1656463",
                         Status = PaymentStatus.Completed
                     },
                     OrderItems = new List<OrderItem>
                     {
                         new OrderItem {Id = 1, OrderId = 3, ProductId = 1, Quantity = 2},
                         new OrderItem {Id = 1, OrderId = 3, ProductId = 2, Quantity = 5},
                         new OrderItem {Id = 1, OrderId = 3, ProductId = 3, Quantity = 7},
                     }
                 }
            };

            _dbContextMock = new DbContextMock<AppDbContext>
                (
                    new DbContextOptionsBuilder<AppDbContext>().Options
                );
            
            _dbContextMock.CreateDbSetMock(x => x.Orders, orders);
            _dbContextMock.CreateDbSetMock(x => x.Customers, customers);
            _dbContextMock.CreateDbSetMock(x => x.Products, products);

            _paymentServiceMock = new Mock<IPaymentGatewayService>();
            _backgroundJobClientMock = new Mock<IBackgroundJobClient>();

            _handler = new(_dbContextMock.Object, _paymentServiceMock.Object, _backgroundJobClientMock.Object);
            
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_OrderIsNotFound()
        {
            //Arrange
            var command = new CancelOrderCommand(4, 1);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(OrderErrors.NotFound);
        }
        [Fact]
        public async Task Handle_Should_ReturnFailure_When_OrderNotCancellable()
        {
            //Arrange
            var command = new CancelOrderCommand(2, 2);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(OrderErrors.NotCancellable);
        }
        [Fact]
        public async Task Handle_Should_ReturnSuccess_When_OrderStatusIsAwaitingPayment()
        {
            //Arrange
            var command = new CancelOrderCommand(1, 1);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<CancelOrderResponseDto>();
            result.Value.OrderStatus.Should().Be(OrderStatus.Cancelled.ToString());
            result.Value.RefundIssued.Should().BeFalse();
            _dbContextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        }
        [Fact]
        public async Task Handle_Should_ReturnSuccess_When_OrderStatusIsConfirmed()
        {
            //Arrange
            var command = new CancelOrderCommand(3, 3);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<CancelOrderResponseDto>();
            result.Value.OrderStatus.Should().Be(OrderStatus.Cancelled.ToString());
            result.Value.RefundIssued.Should().BeTrue();
            result.Value.RefundAmount.Should().Be(2000m);
            _dbContextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        }
    }
}
