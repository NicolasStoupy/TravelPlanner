using FluentValidation;
using FluentValidation.Results;

namespace Presentation.MAUI.ViewModel.Activity
{
    public class NewActivityVMValidator : AbstractValidator<NewActivityVM>
    {

        public NewActivityVMValidator()
        {
            // 1) CurrentTravelActivity ne doit pas être null
            RuleFor(x => x.CurrentTravelActivity)
                .NotNull()
                .WithMessage("L’objet CurrentTravelActivity est requis.");

            // 2) Tant que CurrentTravelActivity est non-null, on définit les règles sur ses sous-propriétés
            When(x => x.CurrentTravelActivity != null, () =>
            {
                RuleFor(x => x.CurrentTravelActivity.Name)
                    .NotEmpty().WithMessage("Le nom de l’activité est requis.")
                    .MaximumLength(50).WithMessage("Le nom ne peut pas dépasser 50 caractères.");

                RuleFor(x => x.CurrentTravelActivity.Description)
                    .MaximumLength(500)
                    .WithMessage("La description ne peut pas dépasser 500 caractères.");

                RuleFor(x => x.CurrentTravelActivity.Sequence)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("La séquence doit être supérieure ou égale à 0.");

                RuleFor(x => x.CurrentTravelActivity.GoogleLink)
                    .Must(link => string.IsNullOrWhiteSpace(link)
                                  || Uri.IsWellFormedUriString(link, UriKind.Absolute))
                    .WithMessage("Le lien Google doit être une URL absolue valide.");

                RuleFor(x => x.CurrentTravelActivity.PlannedCost)
                    .GreaterThanOrEqualTo(0m)
                    .When(x => x.CurrentTravelActivity.PlannedCost.HasValue)
                    .WithMessage("Le coût planifié doit être supérieur ou égal à 0.");

                // 3) Vérifier que ActivityType n’est pas null avant d’accéder à son ID
                RuleFor(x => x.CurrentTravelActivity.ActivityType)
                    .NotNull()
                    .WithMessage("Le type d’activité doit être renseigné.");

                When(x => x.CurrentTravelActivity.ActivityType != null, () =>
                {
                    RuleFor(x => x.CurrentTravelActivity.ActivityType.ID)
                        .NotNull()
                        .WithMessage("L’identifiant du type d’activité est requis.");
                });

                RuleFor(x => x.CurrentTravelActivity.ActivityDate)
                    .Must(date => date > DateTime.MinValue)
                    .WithMessage("La date de l’activité doit être renseignée.");
            });
        }

    }
}