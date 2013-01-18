using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BombermanCommon.Resources.Values
{
    public class ValuesList
    {
        public List<ValuePair> list;

        public ValuesList()
        {
            list = new List<ValuePair>();
        }

        public void Add(String name, int value)
        {
            list.Add(new ValuePair(name, value));
        }
    }

    public struct ValuePair
    {
        public String name;
        public int value;

        public ValuePair(String name, int value)
        {
            this.name = name;
            this.value = value;
        }
    }
}
