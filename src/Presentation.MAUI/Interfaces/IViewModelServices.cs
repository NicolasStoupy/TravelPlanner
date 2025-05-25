using BussinessLogic.Interfaces;
using Presentation.MAUI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.MAUI.Interfaces
{
    /// <summary>
    /// Bundles up all of the core services that view-models need.
    /// </summary>
    public interface IViewModelServices
    {
        /// <summary>
        /// Provides access to navigation functionality within the application.
        /// </summary>
        /// 
        INavigationService Navigation { get; }
        /// <summary>
        /// Provides access to application-level services (business logic, persistence, etc.).
        /// </summary>
        IApplicationService Application { get; }
        /// <summary>
        /// Provides access to AlerServices
        /// </summary>
        IAlertService Alert { get; }
        /// <summary>
        /// Provides access to validations behaviors
        /// </summary>
        IValidationService Validation { get; }


        IFilePresentationService DialogFile { get; }

        IOutputFileNameProvider OutputFileNameProvider { get; }
    }
}
