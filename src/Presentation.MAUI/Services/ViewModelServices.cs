using BussinessLogic.Interfaces;
using Presentation.MAUI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.MAUI.Services
{
    /// <summary>
    /// Default implementation, simply delegates to the individual services.
    /// </summary>
    public class ViewModelServices : IViewModelServices
    {
        public ViewModelServices(
            INavigationService navigation,
            IApplicationService application,
            IAlertService alert,
            IValidationService validation,
            IFilePresentationService filePresentationService, 
            IOutputFileNameProvider outputFileNameProvider)
        {
            Navigation = navigation;
            Application = application;
            Alert = alert;
            Validation = validation;
            DialogFile = filePresentationService;
            OutputFileNameProvider = outputFileNameProvider;
        }

        public INavigationService Navigation { get; }
        public IApplicationService Application { get; }
        public IAlertService Alert { get; }
        public IValidationService Validation { get; }

        public IFilePresentationService DialogFile { get; }

        public IOutputFileNameProvider OutputFileNameProvider { get; }
    }
}
