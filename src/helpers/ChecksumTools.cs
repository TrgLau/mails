using System.Security.Cryptography;
using System.Text.Json;
using System.Collections.Concurrent;
using System.Security;

using static TrgHelpers.Logging.Logging;

namespace TrgHelpers.ChecksumTools
{
    public enum ChecksumAlgorithm
    {
        MD5,
        SHA1,
        SHA256,
        SHA384,
        SHA512
    }

    public enum FileChangeType
    {
        Unchanged,
        Modified,
        Added,
        Deleted
    }

    public static class FileChecksum
    {
        public static string Compute(string filePath, ChecksumAlgorithm algorithm = ChecksumAlgorithm.SHA256)
        {
            if (!File.Exists(filePath))
            {
                LogError($"Fichier introuvable: {filePath}");
                throw new FileNotFoundException("Fichier introuvable.", filePath);
            }

            try
            {
                using FileStream stream = File.OpenRead(filePath);
                using HashAlgorithm hasher = algorithm switch
                {
                    ChecksumAlgorithm.MD5 => MD5.Create(),
                    ChecksumAlgorithm.SHA1 => SHA1.Create(),
                    ChecksumAlgorithm.SHA256 => SHA256.Create(),
                    ChecksumAlgorithm.SHA384 => SHA384.Create(),
                    ChecksumAlgorithm.SHA512 => SHA512.Create(),
                    _ => throw new ArgumentOutOfRangeException(nameof(algorithm), "Algorithme non supporté.")
                };

                byte[] hashBytes = hasher.ComputeHash(stream);
                string checksum = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                LogDebug($"Checksum calculé pour {filePath}: {checksum}");
                return checksum;
            }
            catch (IOException ex)
            {
                LogError($"Erreur d'E/S pour le fichier {filePath}: {ex.Message}");
                throw new InvalidOperationException($"Une erreur d'E/S est survenue pour le fichier {filePath}", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                LogError($"Accès non autorisé au fichier {filePath}: {ex.Message}");
                throw new SecurityException($"Accès non autorisé au fichier {filePath}", ex);
            }
        }

        public static Dictionary<string, string> ComputeDirectory(string directoryPath, ChecksumAlgorithm algorithm = ChecksumAlgorithm.SHA256)
        {
            if (!Directory.Exists(directoryPath))
            {
                LogError($"Répertoire introuvable: {directoryPath}");
                throw new DirectoryNotFoundException(directoryPath);
            }

            var result = new ConcurrentDictionary<string, string>();
            var allFiles = Directory.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories);

            try
            {
                Parallel.ForEach(allFiles, filePath =>
                {
                    try
                    {
                        string relativePath = Path.GetRelativePath(directoryPath, filePath);
                        string checksum = Compute(filePath, algorithm);
                        result[relativePath] = checksum;
                        LogInfo($"Fichier traité: {relativePath}");
                    }
                    catch (Exception ex)
                    {
                        LogWarning($"Erreur lors du traitement du fichier '{filePath}': {ex.Message}");
                    }
                });
            }
            catch (AggregateException ae)
            {
                LogError($"Erreur parallèle: {ae.Message}");
            }

            return new Dictionary<string, string>(result);
        }

        public static Dictionary<string, FileChangeType> CompareDirectories(Dictionary<string, string> previousChecksums, Dictionary<string, string> currentChecksums)
        {
            var result = new Dictionary<string, FileChangeType>();
            var allKeys = previousChecksums.Keys.Union(currentChecksums.Keys);

            foreach (var key in allKeys)
            {
                bool inPrevious = previousChecksums.TryGetValue(key, out var previousValue);
                bool inCurrent = currentChecksums.TryGetValue(key, out var currentValue);

                if (inCurrent && !inPrevious)
                {
                    result[key] = FileChangeType.Added;
                    LogInfo($"Fichier ajouté: {key}");
                }
                else if (!inCurrent && inPrevious)
                {
                    result[key] = FileChangeType.Deleted;
                    LogInfo($"Fichier supprimé: {key}");
                }
                else if (inCurrent && inPrevious)
                {
                    if (previousValue != currentValue)
                    {
                        result[key] = FileChangeType.Modified;
                        LogInfo($"Fichier modifié: {key}");
                    }
                    else
                    {
                        result[key] = FileChangeType.Unchanged;
                    }
                }
            }
            return result;
        }

        public static Dictionary<string, string> LoadFromJson(string jsonFilePath)
        {
            if (!File.Exists(jsonFilePath))
            {
                LogWarning($"Fichier JSON introuvable, création d'un dictionnaire vide: {jsonFilePath}");
                return new Dictionary<string, string>();
            }

            try
            {
                string json = File.ReadAllText(jsonFilePath);
                LogDebug($"Fichier JSON chargé: {jsonFilePath}");
                return JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
            }
            catch (Exception ex)
            {
                LogError($"Impossible de lire le fichier JSON '{jsonFilePath}': {ex.Message}");
                return new Dictionary<string, string>();
            }
        }

        public static void SaveToJson(string jsonFilePath, Dictionary<string, string> checksums)
        {
            try
            {
                string json = JsonSerializer.Serialize(checksums, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(jsonFilePath, json);
                LogDebug($"Fichier JSON sauvegardé: {jsonFilePath}");
            }
            catch (Exception ex)
            {
                LogError($"Impossible de sauvegarder le fichier JSON '{jsonFilePath}': {ex.Message}");
            }
        }

        public static Dictionary<string, FileChangeType> UpdateChecksums(string directoryPath, string jsonFilePath, ChecksumAlgorithm algorithm = ChecksumAlgorithm.SHA256)
        {
            LogInfo($"Mise à jour des checksums pour le répertoire: {directoryPath}");
            var previous = LoadFromJson(jsonFilePath);
            var current = ComputeDirectory(directoryPath, algorithm);
            var changes = CompareDirectories(previous, current);
            SaveToJson(jsonFilePath, current);
            LogInfo($"Mise à jour terminée pour le répertoire: {directoryPath}");
            return changes;
        }
    }
}
