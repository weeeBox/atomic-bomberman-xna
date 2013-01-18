using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using BombermanCommon.Resources.Values;

namespace Bomberman.Content
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content
    /// Pipeline to read the specified data type from binary .xnb format.
    /// 
    /// Unlike the other Content Pipeline support classes, this should
    /// be a part of your main game project, and not the Content Pipeline
    /// Extension Library project.
    /// </summary>
    public class ValuesReader : ContentTypeReader<ValuesList>
    {
        protected override ValuesList Read(ContentReader input, ValuesList existingInstance)
        {
            ValuesList valuesList = new ValuesList();

            int count = input.ReadInt32();
            for (int i = 0; i < count; ++i)
            {
                String name = input.ReadString();
                int value = input.ReadInt32();

                valuesList.Add(name, value);
            }

            return valuesList;
        }
    }
}
