using BookEcomWeb.Models;

namespace BookEcomWeb.DataAccess.IRepository
{
    public interface IProductImageRepository : IRepository<ProductImage>
    {
        void Update(ProductImage obj);
    }
}
