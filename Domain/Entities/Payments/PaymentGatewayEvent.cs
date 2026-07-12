namespace Domain.Entities.Payments
{
    public class PaymentGatewayEvent
    {
        public int Id { get; set; }
        public string GatewayEventId { get; set; } = default!;  
        public string EventType { get; set; } = default!;        
        public DateTime ReceivedAt { get; set; }
    }

}
