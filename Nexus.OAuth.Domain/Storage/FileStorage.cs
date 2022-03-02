using Nexus.OAuth.Dal.Models.Enums;
using Nexus.OAuth.Domain.Properties;
using Nexus.OAuth.Domain.Storage.Enums;

namespace Nexus.OAuth.Domain.Storage
{
    /// <summary>
    /// 
    /// </summary>
    public static class FileStorage
    {
        public static string BasePath => $"{Environment.CurrentDirectory}\\Content";
        /// <summary>
        /// Save File
        /// </summary>
        /// <param name="fileType"></param>
        /// <param name="extension"></param>
        /// <param name="file"></param>
        public static async Task<(string fileName, string directory)> WriteFileAsync(FileType fileType, DirectoryType directoryType, Extension extension, byte[] file)
        {
            (string fileName, string directory, string filePath) = GetFilePath(fileType, directoryType, extension);

            await File.WriteAllBytesAsync(filePath, file);

            return (fileName, directory);
        }

        private static string PathByType(FileType type) => type switch
        {
            FileType.Image => "Images",
            FileType.Document => "Documents",
            _ => string.Empty
        };

        private static string PathByDirectory(DirectoryType type) => type switch
        {
            DirectoryType.AccountsProfile => "Accounts\\Profile",
            DirectoryType.ApplicationsLogo => "Applications\\Logos",
            _ => "Defaults"
        };

        private static (string fileName, string directory, string filePath) GetFilePath(FileType fileType, DirectoryType directoryType, Extension extension, string? fileName = null)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = $"{GeneralHelpers.GenerateToken(32)}.{Enum.GetName(extension)}";
            }

            string directory = $"{PathByType(fileType)}\\{PathByDirectory(directoryType)}";
            string completDirectory = $"{BasePath}\\{directory}";

            if (!Directory.Exists(completDirectory))
                Directory.CreateDirectory(completDirectory);

            return (fileName, completDirectory, $"{completDirectory}\\{fileName}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileType"></param>
        /// <param name="directoryType"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task<byte[]> ReadFileAsync(FileType fileType, DirectoryType directoryType, string fileName)
        {
            if (directoryType == DirectoryType.Defaults)
            {
                return Resources.example;
            }

            (fileName, string directory, string filePath) = GetFilePath(fileType, directoryType, Enum.Parse<Extension>(fileName.Split('.')[1]), fileName);

            return await File.ReadAllBytesAsync(filePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileType"></param>
        /// <param name="directoryType"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task DeleteFileAsync(FileType fileType, DirectoryType directoryType, string fileName)
        {
            await Task.Run(() =>
            {
                (fileName, string directory, string filePath) = GetFilePath(fileType, directoryType, Enum.Parse<Extension>(fileName.Split('.')[1]));

                File.Delete(filePath);
            });
        }
    }
}
