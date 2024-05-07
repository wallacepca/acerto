namespace Acerto.Shared.Contracts
{
    public enum OrderStatus
    {
#pragma warning disable SA1602 // Enumeration items should be documented
        Received,
        Processing,
        Canceled,
        PaymentPending,
        PaymentCompleted,
        Shipped,
        Completed
#pragma warning restore SA1602 // Enumeration items should be documented
    }
}
