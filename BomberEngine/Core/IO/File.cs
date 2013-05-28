using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Debugging;

namespace BomberEngine.Core.IO
{
    using SystemFile = System.IO.File;

    public class File
    {
        public static List<String> Read(String path)
        {
            try
            {
                String absolutePath = AbsolutePath(path);

                if (!SystemFile.Exists(absolutePath))
                {
                    Log.e("File does not exist: " + path);
                    return null;
                }

                using (System.IO.StreamReader reader = new System.IO.StreamReader(absolutePath, Encoding.UTF8))
                {
                    List<String> lines = new List<String>();
                    String line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        lines.Add(line);
                    }

                    return lines;
                }
            }
            catch (Exception ex)
            {
                Log.error(ex, "Unable to read file: " + path);
            }

            return null;
        }

        public static bool Write(String path, List<String> lines)
        {
            try
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(AbsolutePath(path), false, Encoding.UTF8))
                {
                    foreach (String line in lines)
                    {
                        writer.WriteLine(line);
                    }
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                Log.error(ex, "Unable to write file: " + path);
            }

            return false;
        }

        public static bool Delete(String path)
        {
            String absolutePath = AbsolutePath(path);
            if (SystemFile.Exists(absolutePath))
            {
                SystemFile.Delete(absolutePath);
            }

            return false;
        }

        private static String AbsolutePath(String path)
        {
            return path;
        }
    }
}
