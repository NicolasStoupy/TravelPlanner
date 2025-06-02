using FluentValidation;
using FluentValidation.Results;

namespace Presentation.MAUI.ViewModel.Activity
{
    public class NewActivityVMValidator : AbstractValidator<NewActivityVM>
    {

        public NewActivityVMValidator() {

          
            // Le nom de l'activité est requis
            RuleFor(x => x.CurrentTravelActivity.Name)
                .MaximumLength(50).WithMessage("Le nom ne peut pas dépasser 50 caractères.")
                .NotEmpty()
                .WithMessage("Le nom de l'activité est requis.");

            // La description est optionnelle, mais si présente elle ne peut pas dépasser 500 caractères
            RuleFor(x => x.CurrentTravelActivity.Description)
                .MaximumLength(500)
                .WithMessage("La description ne peut pas dépasser 500 caractères.");

            // La séquence doit être nulle ou positive
            RuleFor(x => x.CurrentTravelActivity.Sequence)
                .GreaterThanOrEqualTo(0)
                .WithMessage("La séquence doit être supérieure ou égale à 0.");

            // Si un lien Google est fourni, il doit être une URL valide
            RuleFor(x => x.CurrentTravelActivity.GoogleLink)
                .Must(link => string.IsNullOrWhiteSpace(link)
                              || Uri.IsWellFormedUriString(link, UriKind.Absolute))
                .WithMessage("Le lien Google doit être une URL absolue valide.");

            // Le coût planifié, s'il est fourni, doit être >= 0
            RuleFor(x => x.CurrentTravelActivity.PlannedCost)
                .GreaterThanOrEqualTo(0m)
                .When(x => x.CurrentTravelActivity.PlannedCost.HasValue)
                .WithMessage("Le coût planifié doit être supérieur ou égal à 0.");

           

            // Le nom du type d'activité est requis
            RuleFor(x => x.CurrentTravelActivity.ActivityType.ID).NotNull()               
                .WithMessage("Le nom du type d'activité est requis.");

            // La date de l'activité doit être une date valide (non par défaut)
            RuleFor(x => x.CurrentTravelActivity.ActivityDate)
                .Must(date => date > DateTime.MinValue)
                .WithMessage("La date de l'activité doit être renseignée.");

        }
      
    }
}