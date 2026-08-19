using Application.Common.DTOs;
using Application.Common.DTOs.OrderDTOs;
using Application.Features.Orders.Queries.GetOrderById;
using Application.Features.Orders.Queries.GetOrdersByCustomerId;
using Domain.Entities.Addresses;
using Domain.Entities.Customers;
using Domain.Entities.OrderItems;
using Domain.Entities.Orders;
using Domain.Entities.Payments;
using Domain.Entities.Products;
using Domain.Entities.Shipments;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Tests.Orders
{
    public class GetOrdersByCustomerIdQueryTest
    {
        private readonly GetOrdersByCustomerIdQueryHandler _handler;

        public GetOrdersByCustomerIdQueryTest()
        {
            var customers = new List<Customer>()
            {
                new Customer() {Id = Guid.Parse("A4694554-BB96-47D7-B0E6-7A41F5DB40D7"), UserId = 1, Email = "baher@gmail.com", Name = "baher"},
                new Customer() {Id = Guid.Parse("952C5524-9E2C-4D36-B2B9-909B232C8D89"), UserId = 2, Email = "baher@gmail.com", Name = "baher"},
                new Customer() {Id = Guid.Parse("66CA6F58-0CE0-4B37-BC2A-8665C8153ABF"), UserId = 3, Email = "baher@gmail.com", Name = "baher"},
                new Customer() {Id = Guid.Parse("65D67A60-4A6C-4CD5-9215-E946A867FC99"), UserId = 4, Email = "kenzy@gmail.com", Name = "kenzy"},

            };
            var orders = new List<Order>()
            {
                 new Order {
                     Id = 1,
                     CustomerId = Guid.Parse("A4694554-BB96-47D7-B0E6-7A41F5DB40D7"),
                     IdempotencyKey = Guid.NewGuid(),
                     Status = OrderStatus.AwaitingPayment,
                     Date = new DateTime(2026,2,1),
                     Total = 2000m,
                     OrderItems = new List<OrderItem>
                     {
                         new OrderItem {Id = 1, OrderId = 1, ProductId = 1, Quantity = 2},
                         new OrderItem {Id = 2, OrderId = 1, ProductId = 4, Quantity = 5},
                         new OrderItem {Id = 3, OrderId = 1, ProductId = 3, Quantity = 7},
                     },
                     Payment = new Payment
                     {
                         Id = 1,
                         Status = PaymentStatus.Pending,
                         OrderId = 1,
                         GatewayTransactionId = "6545465",
                         Method = "master cart",
                         CustomerId = Guid.Parse("A4694554-BB96-47D7-B0E6-7A41F5DB40D7"),
                         Amount = 2000m
                     },
                     Shipment = new Shipment
                     {
                         Id = 1,
                         OrderId = 1,
                         Status = ShippingStatus.Pending,
                         Method = "Car",
                         city = "Cairo"
                     },
                     Address = new Address
                     {
                         Id = 1,
                         country = "Egypt",
                         city = "Cairo",
                         Area = "Downtown",
                         StreetBlock = "12",
                         PostalCode = "35511",
                         state = "Dakahlia",
                         HouseNo = "4",
                         Province = "hgskd"
                     }
                 },
                 new Order
                 {
                     Id = 2,
                     CustomerId =  Guid.Parse("952C5524-9E2C-4D36-B2B9-909B232C8D89"),
                     IdempotencyKey = Guid.NewGuid(),
                     Status = OrderStatus.Delivered ,
                     Date = new DateTime(2026,5,1),
                     Total = 90m,
                     OrderItems = new List<OrderItem>
                     {
                         new OrderItem {Id = 4, OrderId = 2, ProductId = 1, Quantity = 2},
                         new OrderItem {Id = 5, OrderId = 2, ProductId = 4, Quantity = 5},
                         new OrderItem {Id = 6, OrderId = 2, ProductId = 3, Quantity = 7},
                     },
                     Payment = new Payment
                     {
                         Id = 2,
                         Status = PaymentStatus.Completed,
                         OrderId = 2,
                         GatewayTransactionId = "65445465",
                         Method = "master cart",
                         CustomerId = Guid.Parse("952C5524-9E2C-4D36-B2B9-909B232C8D89"),
                         Amount = 90m
                     },
                     Shipment = new Shipment
                     {
                         Id = 2,
                         OrderId = 2,
                         Status = ShippingStatus.Delivered,
                          Method = "Car",
                         city = "Cairo"
                     },
                     Address = new Address
                     {
                         Id = 2,
                         country = "Egypt",
                         city = "Cairo",
                         Area = "Downtown",
                         StreetBlock = "12",
                         PostalCode = "35511",
                         state = "Dakahlia",
                         HouseNo = "4",
                         Province = "hgskd"
                     }
                 },
                 new Order
                 {
                     Id = 3,
                     CustomerId =Guid.Parse("66CA6F58-0CE0-4B37-BC2A-8665C8153ABF"),
                     IdempotencyKey = Guid.NewGuid(),
                     Status = OrderStatus.Confirmed,
                     Date = new DateTime(2026,1,1),
                     Total = 1000m,
                     Payment = new Payment
                     {
                         Id = 3,
                         GatewayTransactionId = "1656463",
                         Status = PaymentStatus.Completed,
                         OrderId = 3,
                         Method = "master cart",
                         CustomerId = Guid.Parse("66CA6F58-0CE0-4B37-BC2A-8665C8153ABF"),
                         Amount = 1000m
                     },
                     OrderItems = new List<OrderItem>
                     {
                         new OrderItem {Id = 7, OrderId = 3, ProductId = 1, Quantity = 2},
                         new OrderItem {Id = 8, OrderId = 3, ProductId = 2, Quantity = 5},
                         new OrderItem {Id = 9, OrderId = 3, ProductId = 5, Quantity = 7},
                     },
                     Shipment = new Shipment
                     {
                         Id = 3,
                         OrderId = 3,
                         Status = ShippingStatus.InTransit,
                         Method = "Car",
                         city = "Cairo"
                     },
                     Address = new Address
                     {
                         Id = 3,
                         country = "Egypt",
                         city = "Cairo",
                         Area = "Downtown",
                         StreetBlock = "12",
                         PostalCode = "35511",
                         state = "Dakahlia",
                         HouseNo = "4",
                         Province = "hgskd"
                     }
                 },
                 new Order
                 {
                     Id = 4,
                     CustomerId =Guid.Parse("A4694554-BB96-47D7-B0E6-7A41F5DB40D7"),
                     IdempotencyKey = Guid.NewGuid(),
                     Status = OrderStatus.Shipped,
                     Date = new DateTime(2000,1,1),
                     Total = 2000m,
                     Payment = new Payment
                     {
                         Id = 4,
                         GatewayTransactionId = "1656463",
                         Status = PaymentStatus.Completed,
                         OrderId = 4,
                         Method = "master cart",
                         CustomerId = Guid.Parse("66CA6F58-0CE0-4B37-BC2A-8665C8153ABF"),
                         Amount = 2000m
                     },
                     OrderItems = new List<OrderItem>
                     {
                         new OrderItem {Id = 10, OrderId = 4, ProductId = 1, Quantity = 2},
                         new OrderItem {Id = 11, OrderId = 4, ProductId = 2, Quantity = 5},
                         new OrderItem {Id = 12, OrderId = 4, ProductId = 3, Quantity = 7},
                     },
                     Shipment = new Shipment
                     {
                         Id = 4,
                         OrderId = 4,
                         Status = ShippingStatus.InTransit,
                         Method = "Car",
                         city = "Cairo"
                     },
                     Address = new Address
                     {
                         Id = 4,
                         country = "Egypt",
                         city = "Cairo",
                         Area = "Downtown",
                         StreetBlock = "12",
                         PostalCode = "35511",
                         state = "Dakahlia",
                         HouseNo = "4",
                         Province = "hgskd"
                     }
                 }
            };
            var products = new List<Product>
                {
                    new Product { Id = 1, Name = "Laptop", CategoryId = 1, UnitPrice = 1000, Quantity = 10, Description = "kgifggeorigi", RowVersion = new byte[8], Image = "Products/i1" },
                    new Product { Id = 2, Name = "Mobile", CategoryId = 1, UnitPrice = 500, Quantity = 20 , Description = "kgifggeorigic", RowVersion = new byte[8], Image = "Products/i2"},
                    new Product { Id = 3, Name = "Airpods", CategoryId = 2, UnitPrice = 200, Quantity = 15 , Description = "kgifggeorigis", RowVersion = new byte[8], Image = "Products/i3"},
                    new Product { Id = 4, Name = "Smart Watch", CategoryId = 2, UnitPrice = 300, Quantity = 5 , Description = "kgifggeorigih", RowVersion = new byte[8], Image = "Products/i4"},
                    new Product { Id = 5, Name = "Boot", CategoryId = 3, UnitPrice = 150, Quantity = 8 , Description = "kgifggeorigij", RowVersion = new byte[8], Image = "Products/i5"}
                };
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);

            context.AddRange(customers);
            context.AddRange(products);
            context.AddRange(orders);
            context.SaveChanges();

            _handler = new(context);
        }

        [Fact]
        public async Task Handler_Should_ReturnFailure_When_CustomerHasNoOrders()
        {
            //Arrange
            var command = new GetOrdersByCustomerIdQuery(4, null, null, null, null, null);
            //Act
            var result = await _handler.Handle(command, CancellationToken.None);
            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be(OrderErrors.CustomerHasNoOrders);
        }
        [Fact]
        public async Task Handler_Should_ReturnAllCustomerOrdersWithNoFilter_When_CustomerHasOrders()
        {
            //Arrange
            var command = new GetOrdersByCustomerIdQuery(1, null, null, null, null, null);
            //Act
            var result = await _handler.Handle(command, CancellationToken.None);
            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<PaginatedResult<OrderSummaryDto>>();
        }
        [Fact]
        public async Task Handler_Should_ApplyFilterWithOrderStatus_When_CustomerHasOrders()
        {
            //Arrange
            var command = new GetOrdersByCustomerIdQuery(1, null, null, OrderStatus.AwaitingPayment, null, null);
            //Act
            var result = await _handler.Handle(command, CancellationToken.None);
            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<PaginatedResult<OrderSummaryDto>>();
        }
        [Fact]
        public async Task Handler_Should_ApplyFilterWithDate_When_CustomerHasOrders()
        {
            //Arrange
            var command = new GetOrdersByCustomerIdQuery(1, null, null, null, new DateTime(2026,1,1), new DateTime(2026, 5, 1));
            //Act
            var result = await _handler.Handle(command, CancellationToken.None);
            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeOfType<PaginatedResult<OrderSummaryDto>>();
        }

    }
}
