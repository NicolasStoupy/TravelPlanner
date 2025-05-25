using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.MAUI.Services
{
    public interface IValidationService
    {
        /// <summary>
        /// Valide l’objet donné et retourne le résultat FluentValidation.
        /// </summary>
        Task<ValidationResult> ValidateAsync<T>(T instance);
        Task<bool> Validate<T>(T instance);
        /// <summary>
        /// Valide l’objet donné, affiche les erreurs via IAlertService si besoin,
        /// et renvoie <c>true</c> si tout est valide, sinon <c>false</c>.
        /// </summary>
        Task<bool> ValidateAndNotifyAsync<T>(T instance);
    }
}
