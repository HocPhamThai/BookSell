using BookEcomWeb.DataAccess.Data;
using BookEcomWeb.DataAccess.IRepository;

namespace BookEcomWeb.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext db;
        public ICategoryRepository Category { get; private set; }
        public UnitOfWork(ApplicationDbContext db)
        {
            this.db = db;
            Category = new CategoryRepository(db);
        }

        public void Save()
        {
            db.SaveChanges();
        }
    }
}
