namespace GuitarStore.Modules.Orders.Models;

internal enum OrderStatus
{
    Placed = 1,
    AwaitingValidation = 2,
    Validated = 3,
    AwaitingPayment = 4,
    Paid = 5,
    Shipped = 6,
    Delivered = 7,
    Cancelled = 8
}
