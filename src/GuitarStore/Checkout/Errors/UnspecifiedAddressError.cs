using GuitarStore.Common.Web.Errors;

namespace GuitarStore.Checkout.Errors;

public class UnspecifiedAddressError : BadRequestError
{
    public UnspecifiedAddressError() : base("Bad Request", "Delivery address not specified")
    {
    }
}
