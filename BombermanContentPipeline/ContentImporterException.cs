using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BombermanContentPipeline
{
    public class ContentImporterException : Exception
    {
        public ContentImporterException(String message)
            : base(message)
        {
        }
    }
}
