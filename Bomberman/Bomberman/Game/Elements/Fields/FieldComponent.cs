using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bomberman.Game.Elements.Fields
{
    public class FieldComponent
    {
        private Field mField;

        protected FieldComponent(Field field)
        {
            mField = field;
        }

        protected Field field
        {
            get { return mField; }
        }
    }
}
