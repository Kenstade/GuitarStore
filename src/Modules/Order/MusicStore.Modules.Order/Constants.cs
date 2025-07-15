namespace MusicStore.Modules.Order;

public static class Constants
{
    public const string ModuleName = "Orders";
    public static class Permissions
    {
        public const string UpdateOrder = "orders:update";
        public const string GetOrder = "orders:read";
        public const string CreateOrder = "orders:create";
    }
}