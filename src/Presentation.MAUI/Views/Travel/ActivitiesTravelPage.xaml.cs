using BussinessLogic.Entities;
using Presentation.MAUI.ViewModel;
using System.Threading.Tasks;

namespace Presentation.MAUI.Views.Travel;

public partial class ActivitiesTravelPage : ContentPage
{
    private TravelActivity? _draggedItem;
    public ActivitiesTravelPage(ActivitiesTravelVM vm)
	{		
		InitializeComponent();
		BindingContext = vm;
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ActivitiesTravelVM vm)
            await vm.OnAppearingAsync();
        return;
    }

    // Début du drag : on retient l'item
    void OnDragStarting(object sender, DragStartingEventArgs e)
    {
        if (((Microsoft.Maui.Controls.Element)sender).Parent is Border border && border.BindingContext is TravelActivity activity)
            _draggedItem = activity;
    }

    // Drop : on réordonne et on ré-indexe les séquences
     void OnDrop(object sender, DropEventArgs e)
    {
        if (_draggedItem == null || BindingContext is not ActivitiesTravelVM vm)
            return;

        var list = vm.Activities;                          
        if (((Microsoft.Maui.Controls.Element)sender).Parent is Border border && border.BindingContext is TravelActivity target)
        {
            int oldIndex = list.IndexOf(_draggedItem);
            int newIndex = list.IndexOf(target);
            if (oldIndex < 0 || newIndex < 0 || oldIndex == newIndex)
                return;

            // 1) Déplace l'item dans la collection
            list.Move(oldIndex, newIndex);
            vm.ModificationNotSaved = true;
            for (int i = 0; i < list.Count; i++)
            {
                list[i].Sequence = i + 1;   // on repart de 1
            }

        }
    }

}