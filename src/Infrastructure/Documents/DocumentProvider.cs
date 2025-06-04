using System.IO.Compression;
using Commons;
using Commons.ErrorsHandlings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Documents;

/// <summary>
/// Fournisseur de documents pour gérer la sauvegarde des fichiers selon leur type média.
/// </summary>
public class DocumentProvider
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DocumentProvider> _logger;
    private readonly string _basePath;
    private string? _mediaFilePath;

    /// <summary>
    /// Initialise une nouvelle instance de <see cref="DocumentProvider"/>.
    /// </summary>
    /// <param name="configuration">Configuration de l'application.</param>
    /// <param name="logger">Logger pour la journalisation.</param>
    public DocumentProvider(
        IConfiguration configuration,
        ILogger<DocumentProvider> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _basePath = _configuration["Storage:BasePath"] ?? "storage";
        _logger.LogInformation("DocumentProvider initialisé avec base path : {BasePath}", _basePath);
    }

    /// <summary>
    /// Définit le type de média à utiliser pour le chemin de sauvegarde.
    /// </summary>
    /// <param name="mediaType">Le nom du type de média (ex: 'images', 'videos').</param>
    public void SetMediaType(TypeMedia typeMedia)
    {
        _mediaFilePath = Path.Combine(_basePath, typeMedia.ToString());
        _logger.LogInformation("Chemin média défini sur : {Path}", _mediaFilePath);
    }

    /// <summary>
    /// Sauvegarde un fichier binaire dans le dossier associé au type média.
    /// </summary>
    /// <param name="file">Contenu binaire du fichier.</param>
    /// <returns>L'identifiant du fichier, ou null en cas d'échec.</returns>
    public Guid? SaveFile(byte[] file)
    {
        if (string.IsNullOrWhiteSpace(_mediaFilePath))
        {
            _logger.LogWarning("Chemin média non défini. Appelez SetMediaType d'abord.");
            return null;
        }

        var fileId = Guid.NewGuid();
        var fileName = $"{fileId}";
        var fullPath = Path.Combine(_mediaFilePath, fileName);

        if (!CreatePath(_mediaFilePath))
        {
            _logger.LogWarning("Échec de la création du chemin : {Path}", _mediaFilePath);
            return null;
        }

        try
        {
            File.WriteAllBytes(fullPath, file);
            _logger.LogInformation("Fichier sauvegardé à : {FilePath}", fullPath);
            return fileId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'écriture du fichier : {Path}", fullPath);
            return null;
        }
    }

    /// <summary>
    /// Remplace le fichier identifié par <paramref name="fileToReplaceID"/> avec <paramref name="newFile"/>.
    /// Si <paramref name="fileToReplaceID"/> est null, délègue à <see cref="SaveFile(byte[])"/> pour créer un nouveau fichier.
    /// </summary>
    /// <param name="fileToReplaceID">
    /// GUID du fichier à remplacer. Si null, un nouveau fichier sera créé.
    /// </param>
    /// <param name="newFile">
    /// Contenu binaire du nouveau fichier à écrire.
    /// </param>
    /// <returns>
    /// <see cref="ServiceResult{Guid}"/> :
    /// <list type="bullet">
    ///   <item><description>
    ///     <see cref="ServiceResult{Guid}.Success"/> contenant le GUID du fichier remplacé ou créé si l’écriture a réussi.
    ///   </description></item>
    ///   <item><description>
    ///     <see cref="ServiceResult{Guid}.Failure"/> avec un message d’erreur en cas d’échec :
    ///     <list type="bullet">
    ///       <item><description>Impossible de créer un nouveau fichier si <see cref="SaveFile"/> échoue.</description></item>
    ///       <item><description>Le chemin de stockage n’a pas été configuré si <c>_mediaFilePath</c> est vide.</description></item>
    ///       <item><description>Accès refusé si droits insuffisants (UnauthorizedAccessException).</description></item>
    ///       <item><description>Répertoire introuvable si le dossier n’existe pas (DirectoryNotFoundException).</description></item>
    ///       <item><description>Problème d’E/S si une erreur I/O survient (IOException).</description></item>
    ///       <item><description>Erreur interne pour toute autre exception imprévue.</description></item>
    ///     </list>
    ///   </description></item>
    /// </list>
    /// </returns>
    public ServiceResult<Guid> ReplaceFile(Guid? fileToReplaceID, byte[] newFile)
    {
        // Si on n’a pas de fichier à remplacer, on délègue directement à SaveFile
        if (fileToReplaceID == null)
        {
            var saveRes = SaveFile(newFile);
            if (saveRes.HasValue)
                return ServiceResult<Guid>.Success(saveRes.Value);

            return ServiceResult<Guid>.Failure("Impossible de créer un nouveau fichier.");
        }

        //Vérifier le chemin media
        if (string.IsNullOrWhiteSpace(_mediaFilePath))
        {
            _logger.LogWarning("Chemin média non défini. Appelez d’abord SetMediaType().");
            return ServiceResult<Guid>.Failure("Le chemin de stockage n’a pas été configuré.");
        }

        //Préparer le chemin complet du fichier à remplacer
        var fileName = fileToReplaceID.Value.ToString();
        var fullPath = Path.Combine(_mediaFilePath, fileName);

        try
        {
            //S’assurer que le dossier existe
            var directory = Path.GetDirectoryName(fullPath);
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                _logger.LogInformation("Répertoire créé : {Directory}", directory);
            }

            //Si le fichier n’existe pas, on peut choisir de le créer ou de renvoyer une erreur
            if (!File.Exists(fullPath))
            {
                _logger.LogWarning("Le fichier {Path} n’existe pas. Il sera créé.", fullPath);
            }

            //Écriture sur disque
            File.WriteAllBytes(fullPath, newFile);
            _logger.LogInformation("Fichier remplacé avec succès : {Path}", fullPath);

            return ServiceResult<Guid>.Success(fileToReplaceID.Value);
        }
        catch (UnauthorizedAccessException uaEx)
        {
            _logger.LogError(uaEx, "Pas les droits pour écrire dans {Path}", fullPath);
            return ServiceResult<Guid>.Failure("Accès refusé au fichier (droits insuffisants).");
        }
        catch (DirectoryNotFoundException dnEx)
        {
            _logger.LogError(dnEx, "Le répertoire n’existe pas pour {Path}", fullPath);
            return ServiceResult<Guid>.Failure("Répertoire de destination introuvable.");
        }
        catch (IOException ioEx)
        {
            _logger.LogError(ioEx, "Erreur I/O lors de l’écriture du fichier {Path}", fullPath);
            return ServiceResult<Guid>.Failure("Problème d’E/S : impossible d’écrire le fichier.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur inattendue lors du remplacement de {Path}", fullPath);
            return ServiceResult<Guid>.Failure("Erreur interne lors du remplacement du fichier.");
        }
    }

    /// <summary>
    /// Récupère un fichier binaire à partir de son identifiant.
    /// </summary>
    /// <param name="guid">Identifiant du fichier à récupérer.</param>
    /// <returns>Contenu binaire du fichier, ou null s'il est introuvable ou en cas d'erreur.</returns>
    public byte[]? GetFile(Guid? guid, TypeMedia typeMedia)
    {
        if (guid == null) return null;
        SetMediaType(typeMedia);
        var fileName = guid.ToString();
        var fullPath = Path.Combine(_mediaFilePath, fileName);

        try
        {
            if (!File.Exists(fullPath))
            {
                _logger.LogWarning("Fichier non trouvé : {Path}", fullPath);
                return null;
            }

            var fileBytes = File.ReadAllBytes(fullPath);
            _logger.LogInformation("Fichier récupéré : {Path}", fullPath);
            return fileBytes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la lecture du fichier : {Path}", fullPath);
            return null;
        }
    }

    /// <summary>
    /// Crée un dossier à l’emplacement spécifié si celui-ci n’existe pas déjà.
    /// </summary>
    /// <param name="path">
    /// Chemin complet du dossier à créer. Si <c>null</c>, vide ou ne contenant que des espaces, la création est ignorée.
    /// </param>
    /// <returns>
    /// <c>true</c> si le dossier existe déjà ou a été créé avec succès;
    /// <c>false</c> en cas de chemin invalide, d’accès refusé, de chemin trop long ou d’autre erreur inattendue.
    /// </returns>
    private bool CreatePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;

        try
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                _logger.LogInformation("Dossier créé : {Path}", path);
            }

            return true;
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Accès refusé lors de la création du dossier : {Path}", path);
        }
        catch (PathTooLongException ex)
        {
            _logger.LogWarning(ex, "Chemin trop long : {Path}", path);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erreur inattendue lors de la création du dossier : {Path}", path);
        }

        return false;
    }

    /// <summary>
    /// Supprime plusieurs fichiers identifiés par leur GUID dans le répertoire associé à <paramref name="typeMedia"/>.
    /// </summary>
    /// <param name="fileGuids">
    /// Liste des GUID des fichiers à supprimer. Si la liste est vide ou <c>null</c>, aucune suppression n’est effectuée.
    /// </param>
    /// <param name="typeMedia">
    /// Type de média utilisé pour déterminer le chemin de stockage (doit avoir été configuré via <see cref="SetMediaType(TypeMedia)"/>).
    /// </param>
    /// <returns>
    /// <c>true</c> si la tentative de suppression a été effectuée pour tous les GUID de la liste.
    /// Renvoie <c>false</c> uniquement si <paramref name="fileGuids"/> est <c>null</c>.
    /// </returns>
    public bool RemoveFiles(List<Guid> fileGuids, TypeMedia typeMedia)
    {
        SetMediaType(typeMedia);
        foreach (var fileGuid in fileGuids)
        {
            RemoveFile(fileGuid, typeMedia);
        }

        return true;
    }

    /// <summary>
    /// Supprime le fichier correspondant au <paramref name="fileGuid"/> dans le répertoire associé à <paramref name="typeMedia"/>.
    /// </summary>
    /// <param name="fileGuid">
    /// Identifiant unique du fichier à supprimer. Si <c>null</c>, l’opération est ignorée et retourne <c>false</c>.
    /// </param>
    /// <param name="typeMedia">
    /// Type de média utilisé pour déterminer le chemin de stockage (doit avoir été configuré via <see cref="SetMediaType(TypeMedia)"/>).
    /// </param>
    /// <returns>
    /// <c>true</c> si le fichier a été trouvé et supprimé avec succès ;
    /// <c>false</c> dans les cas suivants :
    /// <list type="bullet">
    ///   <item><description><paramref name="fileGuid"/> est <c>null</c>.</description></item>
    ///   <item><description>Le chemin de stockage n’a pas été configuré (<c>_mediaFilePath</c> vide ou nul).</description></item>
    ///   <item><description>Le fichier n’existe pas.</description></item>
    ///   <item><description>Une erreur survient lors de la suppression (droits insuffisants, I/O, répertoire introuvable, etc.).</description></item>
    /// </list>
    /// </returns>
    public bool RemoveFile(Guid? fileGuid, TypeMedia typeMedia)
    {
        //  Vérifier que l’ID n’est pas null
        if (fileGuid == null)
        {
            _logger.LogWarning("RemoveFile appelé avec fileGuid null.");
            return false;
        }

        // Configurer le chemin en fonction du type de média
        SetMediaType(typeMedia);
        if (string.IsNullOrWhiteSpace(_mediaFilePath))
        {
            _logger.LogWarning("Chemin média non défini. Appelez d’abord SetMediaType().");
            return false;
        }

        //Construire le chemin complet du fichier
        var fileName = fileGuid.Value.ToString();
        var fullPath = Path.Combine(_mediaFilePath, fileName);

        try
        {
            //Vérifier l’existence du fichier
            if (!File.Exists(fullPath))
            {
                _logger.LogWarning("Fichier non trouvé : {Path}", fullPath);
                return false;
            }

            //Supprimer le fichier
            var fileInfo = new FileInfo(fullPath);
            fileInfo.Delete();
            _logger.LogInformation("Fichier supprimé avec succès : {Path}", fullPath);
            return true;
        }
        catch (UnauthorizedAccessException uaEx)
        {
            _logger.LogError(uaEx, "Accès refusé lors de la suppression du fichier : {Path}", fullPath);
            return false;
        }
        catch (DirectoryNotFoundException dnEx)
        {
            _logger.LogError(dnEx, "Répertoire introuvable pour le chemin : {Path}", fullPath);
            return false;
        }
        catch (IOException ioEx)
        {
            _logger.LogError(ioEx, "Erreur I/O lors de la suppression du fichier : {Path}", fullPath);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur inattendue lors de la suppression du fichier : {Path}", fullPath);
            return false;
        }
    }

    /// <summary>
    /// Récupère le contenu binaire de plusieurs fichiers identifiés par leurs GUID.
    /// </summary>
    /// <param name="filesGuids">
    /// Collection de GUID correspondant aux fichiers à lire.
    /// </param>
    /// <param name="typeMedia">
    /// Type de média (détermine le chemin de stockage via SetMediaType).
    /// </param>
    /// <returns>
    /// Une collection d’octets (<see cref="IEnumerable{byte[]}"/>) :
    /// pour chaque GUID, renvoie soit le contenu du fichier, soit un tableau vide si le fichier est introuvable ou en cas d’erreur.
    /// </returns>
    private IEnumerable<byte[]> GetFiles(IEnumerable<Guid> filesGuids, TypeMedia typeMedia)
    {
        var result = new List<byte[]>();
        foreach (var fileGuid in filesGuids)
        {
            var file = GetFile(fileGuid, typeMedia) ?? Array.Empty<byte>();
            result.Add(file);
        }

        return result;
    }

    public async Task<string> ExportToZipAsync(IEnumerable<Guid> filesGuids, TypeMedia typeMedia, string folderPath, string fileName)
    {
        var files = GetFiles(filesGuids, typeMedia);
        var zipPath = Path.Combine(folderPath, fileName);
        using (var zipStream = new FileStream(zipPath, FileMode.Create))
        using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create))
        {
            int i = 1;
            foreach (var file in files)
            {
                var entry = archive.CreateEntry($"image_{i:D3}.jpg", CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(file, 0, file.Length);
                i++;
            }
        }
        return zipPath;
    }
}