namespace GuitarStore.Modules.Orders.Models;

internal enum OrderStatus
{
    Initializing = 1,
    AwaitingValidation = 2,
    StockConfirmed = 3,
    AwaitingPayment = 4,
    Paid = 5,
    Shipped = 6,
    Delivered = 7,
    Cancelled = 8
}
