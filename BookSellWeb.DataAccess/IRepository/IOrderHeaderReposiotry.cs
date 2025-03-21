using BookEcomWeb.Models;

namespace BookEcomWeb.DataAccess.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader obj);
    }
}
