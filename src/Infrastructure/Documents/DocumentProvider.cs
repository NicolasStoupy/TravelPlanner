using System;
using Commons;
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
    /// Récupère un fichier binaire à partir de son identifiant.
    /// </summary>
    /// <param name="guid">Identifiant du fichier à récupérer.</param>
    /// <returns>Contenu binaire du fichier, ou null s'il est introuvable ou en cas d'erreur.</returns>
    public byte[]? GetFile(Guid guid)
    {
        if (string.IsNullOrWhiteSpace(_mediaFilePath))
        {
            _logger.LogWarning("Chemin média non défini. Appelez SetMediaType d'abord.");
            return null;
        }

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
    /// Vérifie si le dossier existe, sinon le crée.
    /// </summary>
    /// <param name="path">Chemin du dossier à créer.</param>
    /// <returns>True si le dossier est prêt à être utilisé, sinon False.</returns>
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

    public bool RemoveFile(Guid fileGuid, TypeMedia typeMedia)
    {
        SetMediaType(typeMedia);
        var fileName = fileGuid.ToString();
        var fullPath = Path.Combine(_mediaFilePath, fileName);
        try
        {

            if (!File.Exists(fullPath))
            {
                _logger.LogWarning("Fichier non trouvé : {Path}", fullPath);
                return false;
            }
            var file = new FileInfo(fullPath);

            file.Delete();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la lecture du fichier : {Path}", fullPath);
            return false;
        }
    }
}
