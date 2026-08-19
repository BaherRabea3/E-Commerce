using Application.Common.DTOs.PaymentDTOs;
using Application.Common.Interfaces;
using Application.Features.Payments.Commands.RefundPayment;
using Application.Features.Payments.Commands.UpdatePaymentStatus;
using Castle.Core.Logging;
using Domain.Entities.OrderItems;
using Domain.Entities.Payments;
using Domain.Enums;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Stripe;

namespace E_Commerce.Tests.Payments
{
    public class UpdatePaymentStatusCommandTest
    {
        private readonly UpdatePaymentStatusCommandHandler _handler;
        private readonly DbContextMock<AppDbContext> _dbContextMock;
        private readonly Mock<ILogger<UpdatePaymentStatusCommandHandler>> _logger;
        public UpdatePaymentStatusCommandTest()
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


            _dbContextMock = new DbContextMock<AppDbContext>
                (
                    new DbContextOptionsBuilder<AppDbContext>().Options
                );

            _dbContextMock.CreateDbSetMock(x => x.Payments, payments);

            _logger = new Mock<ILogger<UpdatePaymentStatusCommandHandler>> ();


            _handler = new(_dbContextMock.Object, _logger.Object);
        }

        [Fact]
        public async Task Handler_Should_ReturnFailure_WhenPaymentIsNotFound()
        {
            //Arrange
            var command = new UpdatePaymentStatusCommand(99, PaymentStatus.Completed);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(PaymentErrors.NotFound);
        }
        [Fact]
        public async Task Handler_Should_ReturnFailure_WhenNewPaymentStatusCannotTransitionTo()
        {
            //Arrange
            var command = new UpdatePaymentStatusCommand(1, PaymentStatus.Refunded);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(PaymentErrors.InvalidStatusTransition(PaymentStatus.Pending, PaymentStatus.Refunded));
        }
        [Fact]
        public async Task Handler_Should_ReturnFailure_WhenNewPaymentStatusCanTransitionTo()
        {
            //Arrange
            var command = new UpdatePaymentStatusCommand(1, PaymentStatus.Completed);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType< UpdatePaymentStatusResponseDto>();
            _dbContextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

    }
}
