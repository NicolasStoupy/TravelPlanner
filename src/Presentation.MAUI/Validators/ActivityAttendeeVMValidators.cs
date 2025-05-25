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
           RuleFor(f=>f.Name).NotEmpty().NotNull();

            RuleFor(f=>f.Forname).NotEmpty().NotNull();

       
          
        }
    }
}
