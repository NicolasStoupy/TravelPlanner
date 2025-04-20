using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.MAUI
{


    public enum Mode
    {
        [Display(Name = "Ajouter")]
        New,
        [Display(Name = "Édition")]
        Edit
    }

}
