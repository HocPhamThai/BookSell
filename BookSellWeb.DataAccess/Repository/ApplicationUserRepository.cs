using BookEcomWeb.DataAccess.Data;
using BookEcomWeb.DataAccess.IRepository;
using BookEcomWeb.Models;

namespace BookEcomWeb.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private ApplicationDbContext _db;
        public ApplicationUserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
