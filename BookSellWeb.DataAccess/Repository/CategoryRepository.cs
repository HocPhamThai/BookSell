using BookEcomWeb.DataAccess.Data;
using BookEcomWeb.DataAccess.IRepository;
using BookEcomWeb.Models;

namespace BookEcomWeb.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private ApplicationDbContext db;
        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(Category obj)
        {
            db.Categories.Update(obj);
        }
    }
}
