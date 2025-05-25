using FluentValidation;
using FluentValidation.Results;
using Presentation.MAUI.Interfaces;
using Presentation.MAUI.Models;

namespace Presentation.MAUI.Services
{
    public class ValidationService : IValidationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IAlertService _alertService;

        public ValidationService(IServiceProvider serviceProvider,
                                 IAlertService alertService)
        {
            _serviceProvider = serviceProvider;
            _alertService = alertService;
        }

        public async Task<ValidationResult> ValidateAsync<T>(T instance)
        {
            // Récupère IValidator<T> directement depuis le conteneur
            var validator = _serviceProvider
                                .GetService(typeof(IValidator<T>))
                            as IValidator<T>
                        ?? throw new InvalidOperationException(
                            $"Aucun IValidator<{typeof(T).Name}> n'est enregistré.");

            return await validator.ValidateAsync(new ValidationContext<T>(instance));
        }
        public async Task<bool> Validate<T>(T instance)
        {
            var validationResult = await ValidateAsync(instance);
            return validationResult.IsValid;
        }
        public async Task<bool> ValidateAndNotifyAsync<T>(T instance)
        {
            var result = await ValidateAsync(instance);
            if (result.IsValid)
                return true;

            var errors = result.Errors
                               .Select(e => e.ErrorMessage)
                               .Distinct();

            await _alertService.ShowAsync(MessageType.Warning, errors);
            return false;
        }
    }
}
