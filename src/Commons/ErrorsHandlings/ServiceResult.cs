using Ardalis.SmartEnum;
using Commons.Models;

namespace Commons.ErrorsHandlings
{
    public abstract class ServiceResult<T, TStatus>
        where TStatus : SmartEnum<TStatus>, IServiceResult
    {
        public TStatus Status { get; }
        public T Value { get; }

        // On considère systématiquement la première valeur SmartEnum
        // déclarée (celle qu’on nomme "Success") comme le statut de succès.
        private static readonly TStatus SuccessStatus =
            SmartEnum<TStatus>.List.First();

        protected ServiceResult(TStatus status, T value)
        {
            Status = status ?? throw new ArgumentNullException(nameof(status));
            Value = value;
        }

        public bool IsSuccess => Status.Equals(SuccessStatus);
    }

    public sealed class SuccessResult<T, TStatus> : ServiceResult<T, TStatus>
        where TStatus : SmartEnum<TStatus>, IServiceResult
    {
        public SuccessResult(T value)
            : base(SmartEnum<TStatus>.FromName("Success"), value)
        {
        }

        /// <summary>
        /// Si vous préférez passer explicitement le statut (utile si vous avez
        /// plusieurs niveaux de succès), exposez aussi ce constructeur :
        /// </summary>
        public SuccessResult(TStatus successStatus, T value)
            : base(successStatus, value)
        {
            if (!successStatus.Equals(SmartEnum<TStatus>.FromName("Success")))
                throw new ArgumentException("Le statut fourni n'est pas la valeur de succès par défaut.");
        }
    }

    public sealed class ErrorResult<T, TStatus> : ServiceResult<T, TStatus>
        where TStatus : SmartEnum<TStatus>, IServiceResult
    {
        public ErrorResult(TStatus errorStatus)
            : base(errorStatus, default!)
        {
            if (errorStatus.Equals(SmartEnum<TStatus>.List.First()))
                throw new ArgumentException("Le statut d'erreur ne peut pas être la valeur 'Success'.");
        }
    }

}
