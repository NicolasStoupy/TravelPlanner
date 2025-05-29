using Ardalis.SmartEnum;
using Commons;
using Commons.ErrorsHandlings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.Services.ServicesStatus
{
    /// <summary>
    /// Reserved 2000=> 3999
    /// </summary>
    public sealed class NoteServiceStatus : SmartEnum<NoteServiceStatus>, IServiceResult
    {
        // --- AddNote statuses ---
        public static readonly NoteServiceStatus AddSuccess =
            new("Success", 2000, true, "Note ajoutée avec succès");
        public static readonly NoteServiceStatus TravelNotFound =
            new("TravelNotFound", 2001, false, "Le voyage n'existe pas");

        // --- DeleteNote statuses ---
        public static readonly NoteServiceStatus DeleteSuccess =
            new("DeleteSuccess", 2002, true, "Note supprimée avec succès");
        public static readonly NoteServiceStatus DeleteNotFound =
            new("DeleteNotFound", 2003, false, "Note introuvable");
        public static readonly NoteServiceStatus DeleteError =
            new("DeleteError", 2004, false, "Erreur lors de la suppression");

        // --- EditNote statuses ---
        public static readonly NoteServiceStatus EditSuccess =
            new("EditSuccess", 2005, true, "Note mise à jour avec succès");
        public static readonly NoteServiceStatus EditNotFound =
            new("EditNotFound", 2006, false, "Note introuvable");
        public static readonly NoteServiceStatus EditError =
            new("EditError", 2007, false, "Erreur lors de la mise à jour");

        private readonly bool _isSuccess;
        private readonly string _message;

        private NoteServiceStatus(
            string name,
            int value,
            bool isSuccess,
            string message
        ) : base(name, value)
        {
            _isSuccess = isSuccess;
            _message = message;
        }

        /// <inheritdoc/>
        public bool IsSuccess => _isSuccess;

        /// <inheritdoc/>
        public string Message => _message;

        /// <summary>
        /// General category: Success / Warning / Error
        /// </summary>
        public MessageType MessageType => this switch
        {
            var s when s == AddSuccess => MessageType.Success,
        
            _ => MessageType.Error
        };
    }
}

