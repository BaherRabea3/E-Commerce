
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.Payments.Commands.ProcessStripeWebhook;
using Castle.Core.Logging;
using Domain.Entities.Payments;
using Domain.Entities.Shipments;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Hangfire;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace E_Commerce.Tests.Payments
{
    public class ProcessStripeWebHookCommandTest
    {
        private readonly ProcessStripeWebhookCommandHandler _handler;
        private readonly DbContextMock<AppDbContext> _dbContextMock;
        private readonly Mock<IPaymentGatewayService> _paymentServiceMock;
        private readonly Mock<ILogger<ProcessStripeWebhookCommandHandler>> _iLogger;
        private readonly Mock<IBackgroundJobClient> _backgroundJobClientMock;
        public ProcessStripeWebHookCommandTest()
        {
            var paymentEvents = new List<PaymentGatewayEvent>()
            {
                new PaymentGatewayEvent {Id = 1, GatewayEventId = "124", EventType = "confirmed"}
            };
            var payments = new List<Payment>()
            { 
                new Payment {
                    Id = 1,
                    GatewayTransactionId = "1234",
                    Status = Domain.Enums.PaymentStatus.Pending,
                    Order = new Domain.Entities.Orders.Order
                    {
                        Id = 1,
                        AddressId = 1,
                        Status = Domain.Enums.OrderStatus.AwaitingPayment
                    }
                },
                new Payment {Id = 2, GatewayTransactionId = "12345" },
                new Payment {Id = 3, GatewayTransactionId = "123456" },

            };
            var shipments = new List<Shipment>();
            _paymentServiceMock = new Mock<IPaymentGatewayService>();

            _iLogger = new Mock<ILogger<ProcessStripeWebhookCommandHandler>>();

            _dbContextMock = new DbContextMock<AppDbContext>
                (
                    new DbContextOptionsBuilder<AppDbContext>().Options
                );

            _dbContextMock.CreateDbSetMock(x => x.PaymentGatewayEvents, paymentEvents);
            _dbContextMock.CreateDbSetMock(x => x.Payments, payments);
            _dbContextMock.CreateDbSetMock(x => x.Shipments, shipments);

            _backgroundJobClientMock = new Mock<IBackgroundJobClient>();

            _handler = new(_dbContextMock.Object, _iLogger.Object, _paymentServiceMock.Object, _backgroundJobClientMock.Object);
        }

        [Fact]
        public async Task Handler_Should_ReturnSuccess_When_GatewayEventIdAlreadyProcessed()
        {
            //Arrange
            var EventId = "124";
            var command = new ProcessStripeWebhookCommand(EventId, "lgref", "lgsdgfkld");

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeTrue();
        }
        [Fact]
        public async Task Handler_Should_ReturnSuccess_When_EventTypeIsPaymentSuccess()
        {
            //Arrange
            var EventId = "1234ga";
            var rawJson = "glgrpgkrew[fk";
            var command = new ProcessStripeWebhookCommand(EventId, "payment_intent.succeeded", rawJson);

            _paymentServiceMock.Setup(x => x.GetGatewayTransactionId(rawJson))
                .Returns("1234");
            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeTrue();
            _paymentServiceMock.Verify(x => x.GetGatewayTransactionId(rawJson), Times.Once);
            _dbContextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None),Times.Once);
        }
        [Fact]
        public async Task Handler_Should_ReturnSuccess_When_EventTypeIsPaymentFailed()
        {
            //Arrange
            var EventId = "1234ga";
            var rawJson = "glgrpgkrew[fk";
            var command = new ProcessStripeWebhookCommand(EventId, "payment_intent.payment_failed", rawJson);

            _paymentServiceMock.Setup(x => x.GetGatewayTransactionId(rawJson))
                .Returns("1234");
            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeTrue();
            _paymentServiceMock.Verify(x => x.GetGatewayTransactionId(rawJson), Times.Once);
            _dbContextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        }
        [Fact]
        public async Task Handler_Should_ReturnSuccess_When_EventTypeIsPaymentRefund()
        {
            //Arrange

            var EventId = "1234ga";
            var rawJson = "glgrpgkrew[fk";
            var command = new ProcessStripeWebhookCommand(EventId, "charge.refunded", rawJson);

            _paymentServiceMock.Setup(x => x.GetGatewayTransactionId(rawJson))
                .Returns("1234");
            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeTrue();
            _paymentServiceMock.Verify(x => x.GetGatewayTransactionId(rawJson), Times.Once);
            _dbContextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        }
        [Fact]
        public async Task Handler_Should_ThrowPaymentGatewayException_When_PaymentIntentIsNull()
        {
            //Arrange
            var EventId = "1234ga";
            var rawJson = "glgrpgkrew[fk";
            var command = new ProcessStripeWebhookCommand(EventId, "charge.refunded", rawJson);

            _paymentServiceMock.Setup(x => x.GetGatewayTransactionId(rawJson))
                .Throws(new PaymentGatewayException("Could not parse PaymentIntent from webhook payload."));
            //Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            //Assert
            await act.Should().ThrowAsync<PaymentGatewayException>();
        }

    }
}
