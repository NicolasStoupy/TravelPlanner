using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.MAUI.Interfaces
{
    public interface IOutputFileNameProvider
    {  /// <summary>
       /// Returns the file name for the given key, formatting in any args.
       /// If you pass no args (or a null array), the pattern is returned as-is
       /// (or with any placeholders removed by substituting empty strings).
       /// </summary>
        string GetFileName(string key, params object?[]? args);
      
    }
}
