
using FluentValidation;
using Presentation.MAUI.ViewModel;


namespace Presentation.MAUI.Validators
{
    public class TravelNotePageViewModelValidator : AbstractValidator<TravelNotePageViewModel>
    {
        public TravelNotePageViewModelValidator()
        {

            RuleFor(n => n.Note.NoteContent).NotEmpty().WithMessage("La note ne doit pas être vide ");

        }
    }
}
