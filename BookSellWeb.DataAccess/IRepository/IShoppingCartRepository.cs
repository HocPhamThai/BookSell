using BookEcomWeb.Models;

namespace BookEcomWeb.DataAccess.IRepository
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        void Update(ShoppingCart obj);
    }
}
