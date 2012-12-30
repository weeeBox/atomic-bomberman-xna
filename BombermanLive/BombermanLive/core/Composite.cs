using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace core
{
    public interface Composite<T>
    {
        void Add(T t);
        void Remove(T t);
        
        int Count();
    }
}
