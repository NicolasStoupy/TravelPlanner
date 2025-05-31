using Presentation.MAUI.Views;
using Presentation.MAUI.Views.Activity;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Presentation.MAUI
{
    public partial class AppShell : Shell
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
