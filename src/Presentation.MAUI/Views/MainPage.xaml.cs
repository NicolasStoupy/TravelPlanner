using BussinessLogic.Interfaces;

namespace Presentation.MAUI
{
    public partial class MainPage : ContentPage
    {
        private int count = 0;
        private readonly IApplicationService _services;
        public MainPage(IApplicationService applicationService)
        {
            InitializeComponent();
            _services = applicationService;
        }

        private async void OnCounterClicked(object sender, EventArgs e)
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Pick Image Please",
                FileTypes = FilePickerFileType.Images
            });

            if (result != null)
            {
                using var stream = await result.OpenReadAsync();
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);

                byte[] fileBytes = ms.ToArray();

                // Exemple : sauvegarde
               var guidFile =  _services.MediaService.SaveMedia(fileBytes,mediaType: BussinessLogic.MediaType.Images);

                var file = _services.MediaService.GetMedia(guidFile.Value);

                ImageSource imageSource = ImageSource.FromStream(() => new MemoryStream(file));
                MyImage.Source = imageSource;
            }
        }
        private async void PickImages_Clicked(object sender, EventArgs e)
        {
            // For custom file types            
            //var customFileType =
            //	new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            //	{
            //		 { DevicePlatform.iOS, new[] { "com.adobe.pdf" } }, // or general UTType values
            //       { DevicePlatform.Android, new[] { "application/pdf" } },
            //		 { DevicePlatform.WinUI, new[] { ".pdf" } },
            //		 { DevicePlatform.Tizen, new[] { "*/*" } },
            //		 { DevicePlatform.macOS, new[] { "pdf"} }, // or general UTType values
            //	});

            var results = await FilePicker.PickMultipleAsync(new PickOptions
            {
                //FileTypes = customFileType
            });

            foreach (var result in results)
            {
                await DisplayAlert("You picked...", result.FileName, "OK");
            }
        }
    }

}
