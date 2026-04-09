namespace Crudspa.Framework.Core.Shared.Extensions;

public static class FileEx
{
    extension(String? filename)
    {
        public String? GetExtension()
        {
            if (filename.HasNothing())
                return null;

            return Path.GetExtension(filename).ToLower();
        }
    }

    extension(FileInfo file)
    {
        public void EnsureDirectoryExists()
        {
            file.Directory?.EnsureExists();
        }

        public void SafeDelete()
        {
            try
            {
                if (file.Exists)
                    file.Delete();
            }
            catch
            {
                // Do nothing
            }
        }
    }

    extension(DirectoryInfo folder)
    {
        public void DeleteBinaryFolders()
        {
            ArgumentNullException.ThrowIfNull(folder);

            if (!folder.Exists)
                throw new DirectoryNotFoundException($"The folder '{folder.FullName}' does not exist.");

            foreach (var subfolder in folder.GetDirectories())
            {
                if (subfolder.Name.IsBasically("bin") || subfolder.Name.IsBasically("obj"))
                    subfolder.Delete(recursive: true);
                else
                    subfolder.DeleteBinaryFolders();
            }
        }

        public Boolean IsBinaryFolder(DirectoryInfo root)
        {
            ArgumentNullException.ThrowIfNull(folder);
            ArgumentNullException.ThrowIfNull(root);

            var relativePath = Path.GetRelativePath(root.FullName, folder.FullName);
            var segments = relativePath.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);

            return segments.Any(x => x.IsBasically("bin") || x.IsBasically("obj"));
        }

        public void EnsureExists()
        {
            if (!folder.Exists)
                folder.Create();
        }
    }
}