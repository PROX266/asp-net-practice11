namespace WebApplication1.Security;

public static class Policies
{
    // Role-based: управление каталогом — Admin и Manager
    public const string CanManageCatalog = "CanManageCatalog";

    // Role-based: только Admin
    public const string AdminOnly = "AdminOnly";

    // Resource-based: заказ может читать/менять его владелец или Admin
    public const string OrderOwnerOrAdmin = "OrderOwnerOrAdmin";
}
