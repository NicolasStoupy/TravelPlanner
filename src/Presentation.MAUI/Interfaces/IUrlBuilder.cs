using Presentation.MAUI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.MAUI.Interfaces
{
    public interface IUrlBuilder
    {

        public UrlBuilder Url { get; }
     
        string BuildSearchUrl(string query);
    }
}
