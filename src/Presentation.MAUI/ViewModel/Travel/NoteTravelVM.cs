using FluentValidation;
using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using CommunityToolkit.Mvvm.Input;
using Presentation.MAUI.Validators;
using CommunityToolkit.Mvvm.ComponentModel;
using Presentation.MAUI.Interfaces;


namespace Presentation.MAUI.ViewModel
{
    /// <summary>
    /// ViewModel for managing notes associated with a travel entry in the MAUI application.
    /// Inherits from <see cref="TravelVM"/> to reuse base travel-related functionality. Provides
    /// features to add, edit, and delete notes with validation support.
    /// </summary>
    public partial class NoteTravelVM : TravelVM
    {

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
            return;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoteTravelVM"/> class.
        /// Loads data for the current travel entry using the application service.
        /// </summary>

        public NoteTravelVM(IViewModelServices viewModelServices) : base(viewModelServices)
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
                var result = _services.Application.TravelService.GetTravel(CurrentTravel.Id);
                if (result.IsSuccess)
                {
                    CurrentTravel = result.Value;
                }
                else
                {
                    await _services.Alert.ShowAsync(result);
                }
            }
            else
            {
                await NoTravelSelected();
                return;
            }


        }

        /// <summary>
        /// Adds the current note to the selected travel.
        /// Displays a warning if no travel is selected.
        /// </summary>
        [RelayCommand]
        public async Task AddNote()
        {
            if (!await _services.Validation.ValidateAndNotifyAsync(this))
                return;
            if (CurrentTravel != null)
            {
                var result = await _services.Application.LogBookService.AddNoteAsync(Note, CurrentTravel.Id);

                if (result.IsSuccess) Note = new Note();
                await _services.Alert.ShowAsync(result);
                loadData();


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
            var result = await _services.Application.LogBookService.DeleteNoteAsync(note);
            await _services.Alert.ShowAsync(result);
            loadData();
        }

        /// <summary>
        /// Edits the specified note using the travel service.
        /// </summary>
        /// <param name="note">The note to edit.</param>
        [RelayCommand]
        public async Task EditNote(Note note)
        {
            var serviceResult = await _services.Application.LogBookService.EditNoteAsync(note);
            await _services.Alert.ShowAsync(serviceResult);
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