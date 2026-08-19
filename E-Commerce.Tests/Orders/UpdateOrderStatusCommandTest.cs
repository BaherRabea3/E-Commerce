
using Application.Common.DTOs.OrderDTOs;
using Application.Common.Interfaces;
using Application.Features.Orders.Commands.CancelOrder;
using Application.Features.Orders.Commands.UpdateOrderStatus;
using Domain.Entities.Customers;
using Domain.Entities.OrderItems;
using Domain.Entities.Orders;
using Domain.Entities.Payments;
using Domain.Entities.Shipments;
using Domain.Enums;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace E_Commerce.Tests.Orders
{
    public class UpdateOrderStatusCommandTest
    {
        private readonly UpdateOrderStatusCommandHandler _handler;
        private readonly DbContextMock<AppDbContext> _dbContextMock;
        private readonly Mock<IPaymentGatewayService> _paymentServiceMock;

        public UpdateOrderStatusCommandTest()
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
                 new Order {
                     Id = 1,
                     CustomerId = Guid.Parse("A4694554-BB96-47D7-B0E6-7A41F5DB40D7"),
                     IdempotencyKey = Guid.NewGuid(),
                     Status = OrderStatus.AwaitingPayment,
                     OrderItems = new List<OrderItem>
                     {
                         new OrderItem {Id = 1, OrderId = 3, ProductId = 1, Quantity = 2},
                         new OrderItem {Id = 2, OrderId = 3, ProductId = 4, Quantity = 5},
                         new OrderItem {Id = 3, OrderId = 3, ProductId = 3, Quantity = 7},
                     },
                     Payment = new Payment {Id = 1, Status = PaymentStatus.Pending, OrderId = 1}
                 },
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
                         new OrderItem {Id = 2, OrderId = 3, ProductId = 2, Quantity = 5},
                         new OrderItem {Id = 3, OrderId = 3, ProductId = 3, Quantity = 7},
                     },
                     Shipment = new Shipment
                     {
                         Id = 1,
                         OrderId = 3,
                         Status = ShippingStatus.Pending
                     }
                 },
                 new Order
                 {
                     Id = 4,
                     CustomerId =Guid.Parse("66CA6F58-0CE0-4B37-BC2A-8665C8153ABF"),
                     IdempotencyKey = Guid.NewGuid(),
                     Status = OrderStatus.Shipped,
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
                         new OrderItem {Id = 2, OrderId = 3, ProductId = 2, Quantity = 5},
                         new OrderItem {Id = 3, OrderId = 3, ProductId = 3, Quantity = 7},
                     },
                     Shipment = new Shipment
                     {
                         Id = 1,
                         OrderId = 3,
                         Status = ShippingStatus.InTransit
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

            _handler = new(_dbContextMock.Object, _paymentServiceMock.Object);

        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_OrderIsNotFound()
        {
            //Arrange
            var command = new UpdateOrderStatusCommand(5, OrderStatus.Cancelled);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(OrderErrors.NotFound);
        }
        [Fact]
        public async Task Handle_Should_ReturnFailure_When_OrderStatusCannotTransitionTo()
        {
            //Arrange
            var command = new UpdateOrderStatusCommand(2, OrderStatus.Cancelled);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(OrderErrors.InvalidStatusTransition(OrderStatus.Delivered, OrderStatus.Cancelled));
        }
        [Fact]
        public async Task Handle_Should_ReturnSuccess_When_NewOrderStatusIsCancelledAndCurrentIsAwaitingPayment()
        {
            //Arrange
            var newStatus = OrderStatus.Cancelled;
            var command = new UpdateOrderStatusCommand(1, newStatus);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<UpdateOrderStatusResponseDto>();
            result.Value.NewStatus.Should().Be(newStatus.ToString());
            _dbContextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        }
        [Fact]
        public async Task Handle_Should_ReturnSuccess_When_NewOrderStatusIsCancelledAndCurrentConfirmed()
        {
            //Arrange
            var newStatus = OrderStatus.Cancelled;
            var command = new UpdateOrderStatusCommand(3, newStatus);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<UpdateOrderStatusResponseDto>();
            result.Value.NewStatus.Should().Be(newStatus.ToString());
            _dbContextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
            _paymentServiceMock.Verify(x => x.RefundPaymentAsync(It.IsAny<string>(), It.IsAny<decimal>(), CancellationToken.None), Times.Once);
        }
        [Fact]
        public async Task Handle_Should_ReturnSuccess_When_NewOrderStatusIsShippedAndCurrentIsConfirmed()
        {
            //Arrange
            var newStatus = OrderStatus.Shipped;
            var command = new UpdateOrderStatusCommand(3, newStatus);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<UpdateOrderStatusResponseDto>();
            result.Value.NewStatus.Should().Be(newStatus.ToString());
            _dbContextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        }
        [Fact]
        public async Task Handle_Should_ReturnSuccess_When_NewOrderStatusIsDeliveredAndCurrentIsShipped()
        {
            //Arrange
            var newStatus = OrderStatus.Delivered;
            var command = new UpdateOrderStatusCommand(4, newStatus);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<UpdateOrderStatusResponseDto>();
            result.Value.NewStatus.Should().Be(newStatus.ToString());
            _dbContextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        }


    }
}
