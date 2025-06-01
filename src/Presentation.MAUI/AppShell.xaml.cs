using CommunityToolkit.Mvvm.ComponentModel;
using Presentation.MAUI.ViewModel;
using Presentation.MAUI.Views;
using Presentation.MAUI.Views.Activity;
using System.ComponentModel;

namespace Presentation.MAUI
{
    public partial class AppShell : Shell, INotifyPropertyChanged
    {
        public AppShell()
        {
            InitializeComponent();
            AppRouting.RegisterRoutes();
        }

        private void MenuItem_Clicked_1(object sender, EventArgs e)
        {
        }
    }
}