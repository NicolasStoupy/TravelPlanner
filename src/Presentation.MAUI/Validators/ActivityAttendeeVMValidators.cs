using FluentValidation;
using Presentation.MAUI.ViewModel.Activity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.MAUI.Validators
{
    public class ActivityAttendeeVMValidators:AbstractValidator<ActivityFollowerVM>
    {
        public  ActivityAttendeeVMValidators()
        {
           RuleFor(f=>f.Name)
                .MaximumLength(50).WithMessage("La nom ne peut pas dépasser 50 caractères.")
                .NotEmpty()
                .NotNull();

            RuleFor(f=>f.Forname)
                .MaximumLength(50).WithMessage("La Prénom ne peut pas dépasser 50 caractères.")
                .NotEmpty().NotNull();

       
          
        }
    }
}
