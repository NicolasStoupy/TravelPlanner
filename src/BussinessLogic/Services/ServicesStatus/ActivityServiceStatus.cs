using Ardalis.SmartEnum;
using Commons.ErrorsHandlings;
using Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.Services.ServicesStatus
{
    /// <summary>
    /// Reserved 4000 => 4999
    /// </summary>
    public sealed class ActivityServiceStatus : SmartEnum<ActivityServiceStatus>, IServiceResult
    {
        // Success
        // Succès
        public static readonly ActivityServiceStatus Success =
            new(nameof(Success), 4000, "Opération réalisée avec succès.");

        // Validation / Introuvable
        public static readonly ActivityServiceStatus InvalidActivity =
            new(nameof(InvalidActivity), 4001, "Données de l'activité invalides.");
        public static readonly ActivityServiceStatus ActivityNotFound =
            new(nameof(ActivityNotFound), 4002, "Aucune activité trouvée pour cet ID.");

        // Mapping / Persistance
        public static readonly ActivityServiceStatus MappingError =
            new(nameof(MappingError), 4003, "Erreur lors du mapping des données de l'activité.");
        public static readonly ActivityServiceStatus PersistenceError =
            new(nameof(PersistenceError), 4004, "Erreur de base de données lors de la sauvegarde de l'activité.");

        // Spécifique au domaine
        public static readonly ActivityServiceStatus NoMedia =
            new(nameof(NoMedia), 4005, "Aucun média fourni pour l'activité.");
        public static readonly ActivityServiceStatus ErrorWhenAddingFile =
            new(nameof(ErrorWhenAddingFile), 4006, "Erreur lors de l'enregistrement d'un ou plusieurs fichiers médias de l'activité.");
        public static readonly ActivityServiceStatus ErrorWhenRemovingFile =
            new(nameof(ErrorWhenRemovingFile), 4007, "Erreur lors de la suppression d'un ou plusieurs fichiers médias de l'activité.");
        public static readonly ActivityServiceStatus ErrorWhenAddingAttendee =
            new(nameof(ErrorWhenAddingAttendee), 4008, "Erreur lors de l'ajout d'un participant à l'activité.");

        // Cas non prévu
        public static readonly ActivityServiceStatus UnknownError =
            new(nameof(UnknownError), 4999, "Erreur inattendue, veuillez contacter le support.");

        private readonly string _message;
        private ActivityServiceStatus(string name, int value, string message)
            : base(name, value)
        {
            _message = message;
        }

        /// <summary>
        /// General category: Success / Warning / Error
        /// </summary>
        public MessageType MessageType => this switch
        {
            var s when s == Success => MessageType.Success,
            var s when s == InvalidActivity => MessageType.Warning,
            var s when s == ActivityNotFound => MessageType.Warning,
            var s when s == NoMedia => MessageType.Warning,
            _ => MessageType.Error
        };

        /// <summary>
        /// The user-facing message associated with this status.
        /// </summary>
        public string Message => _message;

        /// <summary>
        /// True if this status represents success.
        /// </summary>
        public bool IsSuccess => this == Success;
    }

}
