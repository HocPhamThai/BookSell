using BookEcomWeb.Models;

namespace BookEcomWeb.DataAccess.IRepository
{
    public interface ICompanyRepository : IRepository<Company>
    {
        void Update(Company obj);
    }
}
