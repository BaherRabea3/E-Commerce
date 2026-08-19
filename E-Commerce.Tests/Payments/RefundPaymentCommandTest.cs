
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.Payments.Commands.ProcessStripeWebhook;
using Application.Features.Payments.Commands.RefundPayment;
using Domain.Entities.OrderItems;
using Domain.Entities.Payments;
using Domain.Entities.Products;
using Domain.Entities.Shipments;
using Domain.Enums;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Stripe;

namespace E_Commerce.Tests.Payments
{
    public class RefundPaymentCommandTest
    {
        private readonly RefundPaymentCommandHandler _handler;
        private readonly DbContextMock<AppDbContext> _dbContextMock;
        private readonly Mock<IPaymentGatewayService> _paymentServiceMock;
        public RefundPaymentCommandTest()
        {
            var payments = new List<Payment>()
            {
                new Payment {
                    Id = 1,
                    GatewayTransactionId = "1234",
                    Status = Domain.Enums.PaymentStatus.Pending,
                    Amount = 2200m,
                    OrderId = 1,
                    Order = new Domain.Entities.Orders.Order
                    {
                        Id = 1,
                        AddressId = 1,
                        Status = Domain.Enums.OrderStatus.AwaitingPayment
                    }
                },
                new Payment {
                    Id = 2, 
                    GatewayTransactionId = "12345",
                    Status = Domain.Enums.PaymentStatus.Completed,
                    Amount = 23200m,
                    OrderId = 2,
                    Order = new Domain.Entities.Orders.Order
                    {
                        Id = 2,
                        AddressId = 1,
                        Status = Domain.Enums.OrderStatus.Confirmed,
                        OrderItems = new List<Domain.Entities.OrderItems.OrderItem>
                        {
                            new Domain.Entities.OrderItems.OrderItem
                            {
                                Id = 1,
                                OrderId = 2,
                                ProductId = 1,
                                Quantity = 4
                            },
                            new Domain.Entities.OrderItems.OrderItem
                            {
                                Id = 2,
                                OrderId = 2,
                                ProductId = 2,
                                Quantity = 5
                            }
                        }
                    }
                },
                new Payment {Id = 3, GatewayTransactionId = "123456" },

            };
            var products = new List<Domain.Entities.Products.Product>()
            {
                new Domain.Entities.Products.Product { Id = 1, Name = "apple phone", CategoryId = 2, Quantity = 10},
                new Domain.Entities.Products.Product { Id = 2, Name = "T-Shirt", CategoryId = 1, Quantity = 20}
            };
            var orderItems = new List<OrderItem>()
            {
                new Domain.Entities.OrderItems.OrderItem
                            {
                                Id = 1,
                                OrderId = 2,
                                ProductId = 1,
                                Quantity = 4
                            },
                            new Domain.Entities.OrderItems.OrderItem
                            {
                                Id = 2,
                                OrderId = 2,
                                ProductId = 2,
                                Quantity = 5
                            }
            };


            _paymentServiceMock = new Mock<IPaymentGatewayService>();


            _dbContextMock = new DbContextMock<AppDbContext>
                (
                    new DbContextOptionsBuilder<AppDbContext>().Options
                );

            _dbContextMock.CreateDbSetMock(x => x.Payments, payments);
            _dbContextMock.CreateDbSetMock(x => x.Products, products);
            _dbContextMock.CreateDbSetMock(x => x.OrderItems, orderItems);



            _handler = new(_dbContextMock.Object, _paymentServiceMock.Object);
        }

        [Fact]
        public async Task Handler_Should_ReturnFailure_When_PaymentIsNotFound()
        {
            //Arrange
            var command = new RefundPaymentCommand(99);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(PaymentErrors.NotFound);
        }
        [Fact]
        public async Task Handler_Should_ReturnFailure_When_PaymentStatusIsNotCompleted()
        {
            //Arrange
            var command = new RefundPaymentCommand(1);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(PaymentErrors.NotRefundable(PaymentStatus.Pending));
        }
        [Fact]
        public async Task Handler_Should_ThrowStripeException_When_RefundPaymentServiceFailed()
        {
            //Arrange
            var command = new RefundPaymentCommand(2);

            _paymentServiceMock.Setup(x => x.RefundPaymentAsync(It.IsAny<string>(), It.IsAny<decimal>(), CancellationToken.None))
                .ThrowsAsync(new StripeException(It.IsAny<string>()));

            //Act
            var act = async() => await _handler.Handle(command, CancellationToken.None);

            //Assert
            await act.Should().ThrowAsync<StripeException>();
        }
        [Fact]
        public async Task Handler_Should_ReturnSuccess_When_RefundPaymentServiceSuccess()
        {
            //Arrange
            var command = new RefundPaymentCommand(2);

            _paymentServiceMock
                .Setup(x => x.RefundPaymentAsync(It.IsAny<string>(), It.IsAny<decimal>(), CancellationToken.None));

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
           result.IsSuccess.Should().BeTrue();
            result.Value.Status.Should().Be(PaymentStatus.Refunded.ToString());
            _dbContextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
            _paymentServiceMock.Verify(
                x => x.RefundPaymentAsync(It.IsAny<string>(), It.IsAny<decimal>(), CancellationToken.None), Times.Once);
        }



    }
}
