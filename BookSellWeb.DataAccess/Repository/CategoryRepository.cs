using BookEcomWeb.DataAccess.Data;
using BookEcomWeb.DataAccess.IRepository;
using BookEcomWeb.Models;

namespace BookEcomWeb.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Category obj)
        {
            _db.Categories.Update(obj);
        }
    }
}
