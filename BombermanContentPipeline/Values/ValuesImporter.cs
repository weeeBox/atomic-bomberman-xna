using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

using System.IO;
using BombermanCommon.Resources.Values;

namespace BombermanContentPipeline.Values
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to import a file from disk into the specified type, TImport.
    /// 
    /// This should be part of a Content Pipeline Extension Library project.
    /// 
    /// TODO: change the ContentImporter attribute to specify the correct file
    /// extension, display name, and default processor for this importer.
    /// </summary>
    [ContentImporter(".values", DisplayName = "Values Importer", DefaultProcessor = "ValuesProcessor")]
    public class ValuesImporter : ContentImporter<ValuesList>
    {
        public override ValuesList Import(string filename, ContentImporterContext context)
        {
            String[] lines = File.ReadAllLines(filename);
            return Read(lines);
        }

        private ValuesList Read(String[] lines)
        {
            ValuesList list = new ValuesList();

            foreach (String line in lines)
            {
                if (!line.StartsWith(";") && !line.StartsWith("//"))
                {
                    String[] tokens = line.Split(' ');
                    String name = tokens[0];
                    int value = int.Parse(tokens[1]);

                    list.Add(name, value);
                }
            }

            return list;
        }
    }
}
