using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RateChanger.Util
{
    public static class DirectoryUtil
    {
        public static void CopyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);

            var files = Directory.GetFiles(sourceFolder);
            var folders = Directory.GetDirectories(sourceFolder);

            foreach (var file in files)
            {
                var name = Path.GetFileName(file);
                Debug.Assert(name != null);
                var dest = Path.Combine(destFolder, name);
                File.Copy(file, dest);
            }

            foreach (var folder in folders)
            {
                var name = Path.GetFileName(folder);
                Debug.Assert(name != null);
                var dest = Path.Combine(destFolder, name);
                CopyFolder(folder, dest);
            }
        }

        public static void DeleteFiles(string folder, string[] ext, string[] exception = null)
        {
            var files = Directory.GetFiles(folder);

            foreach (var cur in files)
            {
                var filename = Path.GetFileName(cur);

                if (!ext.Contains(Path.GetExtension(filename)))
                    continue;

                if (exception != null && exception.Contains(filename))
                    continue;

                File.Delete(cur);
            }
        }
    }
}
