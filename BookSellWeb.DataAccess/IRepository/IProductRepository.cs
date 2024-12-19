using BookEcomWeb.Models;

namespace BookEcomWeb.DataAccess.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product obj);
    }
}
