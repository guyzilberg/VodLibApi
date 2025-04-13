namespace Utils
{
    public class FilesUtils
    {
        public static void EnsureDirectories(string path)
        {
            string[] dirs = path.Split(Path.DirectorySeparatorChar);
                   string dir = string.Empty;
            foreach (var d in dirs)
            {
                dir +=  d + Path.DirectorySeparatorChar;
                if (string.IsNullOrEmpty(dir) == false && Directory.Exists(dir) == false) 
                    Directory.CreateDirectory(dir);
            }
        }
    }
}
