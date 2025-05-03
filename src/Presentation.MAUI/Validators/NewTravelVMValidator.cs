using BussinessLogic.Entities;
using FluentValidation;
using Presentation.MAUI.ViewModel;


namespace Presentation.MAUI.Validators
{
    public class NewTravelVMValidator : AbstractValidator<NewTravelVM>
    {
        public NewTravelVMValidator()
        {

            RuleFor(x => x.Travel.name)
                .NotEmpty()
                .WithMessage("Le nom est obligatoire.");

            RuleFor(x => x.Travel.budget)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Le budget doit être supérieur ou égal à 0.");
            
            RuleFor(x => x.Travel.people)
                .GreaterThan(0)
                .WithMessage("Le nombre de personnes doit être supérieur à 0.");
            
            RuleFor(x => x.Travel.description)
                .NotEmpty()
                .WithMessage("La description est obligatoire.");

            RuleFor(x => x.Travel.currencie)
                .NotEmpty()
                .WithMessage("La devise est obligatoire.");
            
            RuleFor(x => x.Travel.StartDate)
                .NotEmpty().WithMessage("La date de début est obligatoire.")               
                .WithMessage("La date de début ne peut pas être dans le passé.");

            
            RuleFor(x => x.Travel.EndDate)
                .NotEmpty().WithMessage("La date de fin est obligatoire.")
                .Must((model, endDate) => endDate > model.Travel.StartDate)
                .WithMessage("La date de fin doit être postérieure à la date de début.");



        }
    }
}
