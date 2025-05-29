using Ardalis.SmartEnum;
using Commons;
using Commons.ErrorsHandlings;

namespace BussinessLogic.Services.ServicesStatus
{
    /// <summary>
    /// Reserved 0 => 999
    /// </summary>
    public sealed class TravelServiceStatus : SmartEnum<TravelServiceStatus>, IServiceResult
    {
        public static readonly TravelServiceStatus Success =
            new(nameof(Success), 0, "Opération réussie.");
        public static readonly TravelServiceStatus InvalidTravelId =
              new(nameof(InvalidTravelId), 1, "L’identifiant du voyage est invalide.");
        public static readonly TravelServiceStatus InvalidTravel =
              new(nameof(InvalidTravel), 2, "Les données du voyage sont manquantes ou invalides.");
        public static readonly TravelServiceStatus TravelNotFound =
               new(nameof(TravelNotFound), 3, "Aucun voyage trouvé avec cet ID.");
        public static readonly TravelServiceStatus CloneFailed =
           new(nameof(CloneFailed), 4, "Impossible de cloner le voyage.");
        public static readonly TravelServiceStatus DatabaseError =
         new(nameof(DatabaseError), 5, "Une erreur est survenue lors de la sauvegarde en base.");
        public static readonly TravelServiceStatus UnknownError =
             new(nameof(UnknownError), 999, "Erreur inattendue, contactez le support.");

        public static readonly TravelServiceStatus NoMedia =
             new(nameof(NoMedia), 6, "Pas de média à ajouter !");
       
        private string _message;
        private TravelServiceStatus(string name, int value, string message)
            : base(name, value) { _message = message; }

        public static TravelServiceStatus ErrorWhenAddingFile =
             new(nameof(ErrorWhenAddingFile),6, "Erreur inattendue, les fichier n'ont pas été ajouté correctement.");

        public MessageType MessageType => this switch
        {
            var s when s == Success => MessageType.Success,           
            var s when s == InvalidTravelId => MessageType.Warning,
            var s when s == InvalidTravel => MessageType.Warning,
            var s when s == TravelNotFound => MessageType.Warning,
            _ => MessageType.Error
        };

        public string Message => this._message;

        public bool IsSuccess => MessageType == MessageType.Success;

        public static TravelServiceStatus ErrorWhenRemovingFile =
            new(nameof(ErrorWhenRemovingFile), 6, "Erreur inattendue, les souvenirs n'ont pas été supprimé correctement.");
    }
}
