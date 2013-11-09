using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine
{
    public static class ClassUtils
    {
        public static T Cast<T>(Object obj) where T : class
        {
            Assert.IsNotNull(obj);
            Assert.IsInstance<T>(obj);

            return obj as T;
        }

        public static T TryCast<T>(Object obj) where T : class
        {
            return obj as T;
        }
    }
}
