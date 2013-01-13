using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Visual;
using Assets;

namespace Bomberman.Game.Elements.Fields
{
    public class FieldDrawable : VisualElement
    {
        private Field field;

        public FieldDrawable(Field field, int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            this.field = field;
        }

        public override void Draw(Context context)
        {
        }
    }
}
