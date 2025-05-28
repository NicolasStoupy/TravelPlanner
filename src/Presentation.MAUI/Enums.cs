using System.ComponentModel.DataAnnotations;

namespace Presentation.MAUI
{
    /// <summary>
    /// Specifies whether the current operation is creating a new item or editing an existing one.
    /// </summary>

    public enum Mode
    {
        /// <summary>
        /// Represents the state for adding a new item.
        /// Display name: "Ajouter".
        /// </summary>
        [Display(Name = "Ajouter")]
        New,
        /// <summary>
        /// Represents the state for editing an existing item.
        /// Display name: "Édition".
        /// </summary>
        [Display(Name = "Édition")]
        Edit
    }

}
