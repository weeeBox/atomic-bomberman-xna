using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BomberEngine
{
    public class FileUtils
    {
        public static List<String> Read(String path)
        {
            try
            {
                String absolutePath = GetAbsolutePath(path);

                if (!File.Exists(absolutePath))
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
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(GetAbsolutePath(path), false, Encoding.UTF8))
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
            String absolutePath = GetAbsolutePath(path);
            if (File.Exists(absolutePath))
            {
                File.Delete(absolutePath);
            }

            return false;
        }

        public static bool FileExists(String path)
        {
            String absolutePath = GetAbsolutePath(path);
            return File.Exists(absolutePath);
        }

        public static Stream OpenRead(String path)
        {
            String absolutePath = GetAbsolutePath(path);
            return File.OpenRead(absolutePath);
        }

        public static Stream OpenWrite(String path)
        {
            String absolutePath = GetAbsolutePath(path);
            return File.OpenWrite(absolutePath);
        }

        private static String GetAbsolutePath(String path)
        {
            return path;
        }
    }
}
