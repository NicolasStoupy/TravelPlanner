using FluentValidation;
using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using Presentation.MAUI.Services;
using CommunityToolkit.Mvvm.Input;
using Presentation.MAUI.Validators;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Presentation.MAUI.ViewModel
{
    /// <summary>
    /// ViewModel for managing notes associated with a travel entry in the MAUI application.
    /// Inherits from <see cref="TravelVM"/> to reuse base travel-related functionality. Provides
    /// features to add, edit, and delete notes with validation support.
    /// </summary>
    public partial class NoteTravelVM : TravelVM
    {
        /// <summary>
        /// Retrieves the validator used for this ViewModel.
        /// </summary>
        protected override IValidator? GetValidator() => new NoteTravelVMValidator();

        [ObservableProperty] private Travel? _travels;

        [ObservableProperty] private Note _note = new();

        /// <summary>
        /// Triggered when the <see cref="Note"/> property changes.
        /// Copies the note content to maintain consistency.
        /// </summary>
        /// <param name="oldValue">The previous note value.</param>
        /// <param name="newValue">The new note value.</param>
        partial void OnNoteChanged(Note? oldValue, Note newValue)
        {
            if (newValue != null)
            {
                Note.NoteContent = newValue.NoteContent;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoteTravelVM"/> class.
        /// Loads data for the current travel entry using the application service.
        /// </summary>
        /// <param name="navigationService">Injected navigation service.</param>
        /// <param name="applicationService">Injected application service for travel and note operations.</param>
        public NoteTravelVM(INavigationService navigationService, IApplicationService applicationService) : base(navigationService, applicationService)
        {
            loadData();
        }

        /// <summary>
        /// Loads the travel data based on the current selected travel ID.
        /// </summary>
        public async void loadData()
        {
            if (CurrentTravel != null && CurrentTravel.Id != 0)
            {
                Travels = _applicationService.TravelService.GetTravel(CurrentTravel.Id);
            }
            else
            {
                await NoTravelSelected();
            }
        }

        /// <summary>
        /// Adds the current note to the selected travel.
        /// Displays a warning if no travel is selected.
        /// </summary>
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
                await NoTravelSelected();
            }
        }

        /// <summary>
        /// Deletes the specified note using the travel service.
        /// </summary>
        /// <param name="note">The note to delete.</param>
        [RelayCommand]
        public async Task DeleteNote(Note note)
        {
            await _applicationService.TravelService.DeleteNote(note);
            loadData();
        }

        /// <summary>
        /// Edits the specified note using the travel service.
        /// </summary>
        /// <param name="note">The note to edit.</param>
        [RelayCommand]
        public async Task EditNote(Note note)
        {
            await _applicationService.TravelService.EditNote(note);
            loadData();
        }

        /// <summary>
        /// Resets the ViewModel by reloading the travel data.
        /// </summary>
        public override void Reset()
        {
            loadData();
        }
    }
}