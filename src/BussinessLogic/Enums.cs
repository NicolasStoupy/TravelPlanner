
namespace BussinessLogic
{
    public enum ExecutionStatus
    {
        Success = 1,
        Failure = 0,
        Pending = -1

    }
   

    public enum Currency
    {
        EUR,USD
    }

    public enum ActivityType
    {
        Dining,           // Manger, restaurants, repas
        Accommodation,    // Séjour à l'hôtel ou autre hébergement
        Entertainment,    // Activités de loisirs, spectacles, cinémas, etc.
        Transportation,    // Déplacements : avion, train, taxi, etc.
        NotDefined
    }



}
