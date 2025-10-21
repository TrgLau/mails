using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Writers;

using TrgHelpers.Logging;

namespace TrgHelpers.SharpProvider
{
    public enum OverwriteMode
    {
        Yes,
        No,
        Ask
    }

    public interface ICompressionProvider
    {
        string Extension { get; }
        void CreateArchiveFromDirectory(string sourceDirectory, string destinationArchive);
        void CreateArchiveFromFile(string sourceFile, string destinationArchive);
        void CreateArchiveFromFileList(IEnumerable<string> sourceFiles, string destinationArchive);
        void ExtractToDirectory(string archivePath, string? destinationDirectory, OverwriteMode overwriteMode, Func<string, bool> askForOverwrite);
        public void ExtractFile(string archivePath, string? destinationFile, string fileNameInArchive, OverwriteMode overwriteMode, Func<string, bool>? askForOverwrite = null);
    }

    public class SharpCompressProvider : ICompressionProvider
    {
        public string Extension { get; }
        private readonly ArchiveType _archiveType;
        private readonly CompressionType _compressionType;

        public SharpCompressProvider(string extension, ArchiveType archiveType, CompressionType compressionType)
        {
            Extension = extension;
            _archiveType = archiveType;
            _compressionType = compressionType;
        }

        public void CreateArchiveFromFile(string sourceFile, string destinationArchive) =>
            CreateArchiveFromFileList(new[] { sourceFile }, destinationArchive);

        public void CreateArchiveFromFileList(IEnumerable<string> sourceFiles, string destinationArchive)
        {
            if (sourceFiles == null || !sourceFiles.Any())
                throw new ArgumentException("La liste de fichiers source ne peut pas être vide.", nameof(sourceFiles));

            var options = new WriterOptions(_compressionType)
            {
                LeaveStreamOpen = false
            };

            using var fs = File.Create(destinationArchive);
            using var writer = WriterFactory.Open(fs, _archiveType, options);

            foreach (var file in sourceFiles)
            {
                if (!File.Exists(file))
                    throw new FileNotFoundException("Le fichier source n'a pas été trouvé.", file);

                writer.Write(Path.GetFileName(file), file);
            }
        }

        public void CreateArchiveFromDirectory(string sourceDirectory, string destinationArchive)
        {
            if (!Directory.Exists(sourceDirectory))
                throw new DirectoryNotFoundException(sourceDirectory);

            var options = new WriterOptions(_compressionType)
            {
                LeaveStreamOpen = false
            };

            using var fs = File.Create(destinationArchive);
            using var writer = WriterFactory.Open(fs, _archiveType, options);

            foreach (var filePath in Directory.EnumerateFiles(sourceDirectory, "*", SearchOption.AllDirectories))
            {
                string entryKey = Path.GetRelativePath(sourceDirectory, filePath).Replace('\\', '/');
                writer.Write(entryKey, filePath);
            }
        }

        public void ExtractToDirectory(string archivePath, string? destinationDirectory, OverwriteMode overwriteMode, Func<string, bool> askForOverwrite)
        {
            if (!File.Exists(archivePath))
                throw new FileNotFoundException("L'archive n'existe pas.", archivePath);

            if (destinationDirectory != null) Directory.CreateDirectory(destinationDirectory);

            using var archive = ArchiveFactory.Open(archivePath);

            var fileEntries = archive.Entries.ToList();

            if (!fileEntries.Any())
            {
                throw new InvalidDataException("L'archive ne contient aucune entry.");
            }

            foreach (var entry in fileEntries)
            {
                string destinationPath = Path.Combine(destinationDirectory, entry.Key);

                if (entry.IsDirectory)
                {
                    Directory.CreateDirectory(destinationPath);
                }
                else
                {
                    string? parentDir = Path.GetDirectoryName(destinationPath);

                    if (!string.IsNullOrEmpty(parentDir))
                        Directory.CreateDirectory(parentDir);

                    bool fileExists = File.Exists(destinationPath);
                    bool shouldWrite = fileExists switch
                    {
                        false => true,
                        true when overwriteMode == OverwriteMode.Yes => true,
                        true when overwriteMode == OverwriteMode.No => false,
                        true when overwriteMode == OverwriteMode.Ask => askForOverwrite(entry.Key),
                        _ => false
                    };

                    if (shouldWrite)
                        entry.WriteToFile(destinationPath, new ExtractionOptions { Overwrite = true, ExtractFullPath = true });
                }
            }
        }

        public void ExtractFile(string archivePath, string? destinationFile, string fileNameInArchive, OverwriteMode overwriteMode, Func<string, bool>? askForOverwrite = null)
        {
            if (!File.Exists(archivePath))
                throw new FileNotFoundException("L'archive n'existe pas.", archivePath);

            using var archive = ArchiveFactory.Open(archivePath);

            var entry = archive.Entries.FirstOrDefault(e => !e.IsDirectory && e.Key == fileNameInArchive);
            if (entry == null)
                throw new InvalidDataException($"Le fichier '{fileNameInArchive}' n'a pas été trouvé dans l'archive.");

            if (string.IsNullOrWhiteSpace(destinationFile))
                destinationFile = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileName(fileNameInArchive));

            string? parentDir = Path.GetDirectoryName(destinationFile);
            if (!string.IsNullOrEmpty(parentDir))
                Directory.CreateDirectory(parentDir);

            bool fileExists = File.Exists(destinationFile);
            bool shouldWrite = fileExists switch
            {
                false => true,
                true when overwriteMode == OverwriteMode.Yes => true,
                true when overwriteMode == OverwriteMode.No => false,
                true when overwriteMode == OverwriteMode.Ask => askForOverwrite?.Invoke(fileNameInArchive) ?? false,
                _ => false
            };

            if (shouldWrite)
                entry.WriteToFile(destinationFile, new ExtractionOptions { Overwrite = true, ExtractFullPath = false });
        }

    }
}
