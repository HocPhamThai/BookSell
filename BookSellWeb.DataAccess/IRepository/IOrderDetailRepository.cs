using BookEcomWeb.Models;

namespace BookEcomWeb.DataAccess.IRepository
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        void Update(OrderDetail obj);
    }
}
