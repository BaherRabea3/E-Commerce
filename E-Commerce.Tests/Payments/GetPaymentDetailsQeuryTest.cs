
using Application.Common.DTOs.OrderDTOs;
using Application.Common.DTOs.PaymentDTOs;
using Application.Features.Orders.Queries.GetOrderById;
using Application.Features.Payments.Queries.GetPaymentDetails;
using Domain.Entities.Customers;
using Domain.Entities.OrderItems;
using Domain.Entities.Orders;
using Domain.Entities.Payments;
using Domain.Entities.Shipments;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Tests.Payments
{
    public class GetPaymentDetailsQeuryTest
    {
        private readonly GetPaymentDetailsQueryHandler _handler;

        public GetPaymentDetailsQeuryTest()
        {
            var customers = new List<Customer>()
            {
                new Customer() {Id = Guid.Parse("A4694554-BB96-47D7-B0E6-7A41F5DB40D7"), UserId = 1, Email = "baher@gmail.com", Name = "baher"},
                new Customer() {Id = Guid.Parse("952C5524-9E2C-4D36-B2B9-909B232C8D89"), UserId = 2, Email = "baher@gmail.com", Name = "baher"},
                new Customer() {Id = Guid.Parse("66CA6F58-0CE0-4B37-BC2A-8665C8153ABF"), UserId = 3, Email = "baher@gmail.com", Name = "baher"},
            };
            var payments = new List<Payment>()
            {
                new Payment
                {
                    Id = 1,
                    Status = PaymentStatus.Pending,
                    OrderId = 1,
                    GatewayTransactionId = "6545465",
                    Method = "master cart",
                    CustomerId = Guid.Parse("A4694554-BB96-47D7-B0E6-7A41F5DB40D7"),
                    Amount = 2000m
                },
                new Payment
                {
                    Id = 2,
                    Status = PaymentStatus.Completed,
                    OrderId = 2,
                    GatewayTransactionId = "65445465",
                    Method = "master cart",
                    CustomerId = Guid.Parse("952C5524-9E2C-4D36-B2B9-909B232C8D89"),
                    Amount = 90m
                },
                new Payment
                {
                    Id = 3,
                    GatewayTransactionId = "1656463",
                    Status = PaymentStatus.Completed,
                    OrderId = 3,
                    Method = "master cart",
                    CustomerId = Guid.Parse("66CA6F58-0CE0-4B37-BC2A-8665C8153ABF"),
                    Amount = 1000m
                },
                new Payment
                {
                    Id = 4,
                    GatewayTransactionId = "1656463",
                    Status = PaymentStatus.Completed,
                    OrderId = 4,
                    Method = "master cart",
                    CustomerId = Guid.Parse("66CA6F58-0CE0-4B37-BC2A-8665C8153ABF"),
                    Amount = 2000m
                },

            };
           
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);

            context.AddRange(customers);
            context.AddRange(payments);
            context.SaveChanges();

            _handler = new(context);
        }

        [Fact]
        public async Task Handler_Should_ReturnFailure_WhenPaymentIsNotFound()
        {
            //Arrange
            var command = new GetPaymentDetailsQuery(99, 1);
            //Act
            var result = await _handler.Handle(command, CancellationToken.None);
            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(PaymentErrors.NotFound);
        }
        [Fact]
        public async Task Handler_Should_ReturnOrderResponseDto_WhenOrderIsFound()
        {
            //Arrange
            var command = new GetPaymentDetailsQuery(1, 1);
            //Act
            var result = await _handler.Handle(command, CancellationToken.None);
            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<PaymentDetailsDto>();
        }
    }
}
