using System.Linq.Expressions;

namespace BookEcomWeb.DataAccess.IRepository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(string? inCludeProperties = null);
        T Get(Expression<Func<T, bool>> filter, string? inCludeProperties = null);
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}
