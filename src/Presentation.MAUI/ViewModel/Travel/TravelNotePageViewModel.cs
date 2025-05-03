using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentValidation;
using Presentation.MAUI.Models;
using Presentation.MAUI.Services;
using Presentation.MAUI.Validators;



namespace Presentation.MAUI.ViewModel
{

    public partial class TravelNotePageViewModel : BaseViewModel
    {
        protected override IValidator? GetValidator() => new TravelNotePageViewModelValidator();
        [ObservableProperty] Travel? _travels;

        [ObservableProperty] Note _note = new();
        partial void OnNoteChanged(Note? oldValue, Note newValue)
        {
            if (newValue != null)
            {
                Note.NoteContent = newValue.NoteContent;
            }
        }

        public TravelNotePageViewModel(INavigationService navigationService, IApplicationService applicationService) : base(navigationService, applicationService)
        {
            loadData();

        }

        public void loadData()
        {
            if (CurrentTravel != null && CurrentTravel.Id != 0)
            {
                Travels = _applicationService.TravelService.GetTravel(CurrentTravel.Id);
            }
            else
            {

            }
        }
        [RelayCommand]
        public async Task AddNote()
        {
            if (!await ValidateAll())
                return;
            if (Travels != null)
            {
                var result = await _applicationService.TravelService.AddNote(Note, Travels.Id);
                loadData();
                if (result.IsSuccess) Note = new Note();
            }
            else
            {
                await DisplayAlert(MessageType.Warning, "Merci de sélectionner un voyage avant d’ajouter une note.");

            }

        }

        [RelayCommand]
        public async Task DeleteNote(Note note)
        {
            await _applicationService.TravelService.DeleteNote(note);

            loadData();

        }

        [RelayCommand]
        public async Task EditNote(Note note)
        {
            await _applicationService.TravelService.EditNote(note);
            loadData();
        }
        public override void Reset()
        {
            loadData();

        }
    }
}
