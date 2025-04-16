namespace BookEcomWeb.DataAccess.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        IProductRepository Product { get; }
        ICompanyRepository Company { get; }
        IShoppingCartRepository ShoppingCart { get; }
        IProductImageRepository ProductImage { get; }
        IApplicationUserRepository ApplicationUser { get; }
        IOrderHeaderRepository OrderHeader{ get; }
        IOrderDetailRepository OrderDetail{ get; }
        void Save();
    }
}
