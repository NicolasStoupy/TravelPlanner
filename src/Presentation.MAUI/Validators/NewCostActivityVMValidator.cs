using FluentValidation;
using Presentation.MAUI.ViewModel.Activity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.MAUI.Validators
{
    public class NewCostActivityVMValidator : AbstractValidator<NewCostActivityVM>
    {

        public NewCostActivityVMValidator() {

            // La devise ne doit pas être nulle ni vide
            RuleFor(c => c.NewCurrency)
                .NotNull().WithMessage("La devise est requise.")
                .NotEmpty().WithMessage("La devise ne peut pas être vide.");

            // Le montant du coût doit être renseigné et ne peut pas être négatif
            RuleFor(c => c.NewCostAmount)
                .NotEmpty().WithMessage("Le montant du coût est requis.")
                .GreaterThan(-1).WithMessage("Le montant doit être supérieur ou égal à 0.");

            // Le nom du coût doit être renseigné
            RuleFor(c => c.NewCostName)
                .NotEmpty().WithMessage("Le nom du coût est requis.");

        }
    }
}
