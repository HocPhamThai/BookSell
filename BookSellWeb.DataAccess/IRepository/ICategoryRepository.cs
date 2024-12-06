using BookEcomWeb.Models;

namespace BookEcomWeb.DataAccess.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void Update(Category obj);
    }
}
