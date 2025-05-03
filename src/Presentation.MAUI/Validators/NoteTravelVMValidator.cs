
using FluentValidation;
using Presentation.MAUI.ViewModel;


namespace Presentation.MAUI.Validators
{
    public class NoteTravelVMValidator : AbstractValidator<NoteTravelVM>
    {
        public NoteTravelVMValidator()
        {

            RuleFor(n => n.Note.NoteContent).NotEmpty().WithMessage("La note ne doit pas être vide ");

        }
    }
}
