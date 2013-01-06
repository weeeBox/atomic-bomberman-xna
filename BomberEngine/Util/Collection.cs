using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Util
{
    public interface Collection<T>
    {
        bool Add(T t);
        bool Remove(T t);
        void Clear();
        int Count();
    }
}
