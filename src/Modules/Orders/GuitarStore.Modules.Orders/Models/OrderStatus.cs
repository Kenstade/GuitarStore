namespace GuitarStore.Modules.Orders.Models;

internal enum OrderStatus
{
    Placed = 1,
    AwaitingValidation = 2,
    AwaitingPayment = 5,
    Paid = 6,
    Shipped = 7,
    Delivered = 8,
    Cancelled = 9
}
