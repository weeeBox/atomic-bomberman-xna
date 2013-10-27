
namespace BomberEngine
{
    public interface IBaseCollection<T>
    {
        bool Add(T t);
        bool Remove(T t);
        bool Contains(T t);
        void Clear();
        int Count();
    }
}
