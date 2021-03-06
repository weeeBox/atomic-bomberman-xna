﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Util
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
