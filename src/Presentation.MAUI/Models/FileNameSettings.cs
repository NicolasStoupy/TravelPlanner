using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.MAUI.Models
{
    public class FileNameSettings
    {

        public Dictionary<string, string> Patterns { get; set; } = new();
    }
}
