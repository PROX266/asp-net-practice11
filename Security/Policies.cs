namespace WebApplication1.Security;

public static class Policies
{
    public const string CanManageCatalog = "CanManageCatalog";
    
    public const string AdminOnly = "AdminOnly";
    
    public const string OrderOwnerOrAdmin = "OrderOwnerOrAdmin";
}
