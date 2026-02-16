using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SynthTest.Presentation.ViewModels.Menus
{
    /// <summary>
    /// Represents a single item in a custom popup menu.
    /// It acts as a bridge between the visual menu (front) and the ViewModel commands (back).
    /// </summary>
    public class MenuItemViewModel
    {
        /// <summary>
        /// Text displayed on the menu item.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// The command to execute when the item is clicked. (AddModule, RemoveModule, CreateCable...)
        /// </summary>
        public ICommand Command { get; set; }

        /// <summary>
        /// Optional parameter to pass to the Command. If nothing are given, it will be null by default (and it's OK !)
        /// </summary>
        /// <remarks>RemoveCable(cable) for exemple</remarks>
        public object CommandParameter { get; set; }

        /// <summary>
        /// If true, the item will be styled differently (Red text) to indicate a destructive action.
        /// </summary>
        public bool IsDestructive { get; set; }

        /// <summary>
        /// Is this item just a visual separator? (like a line with margin)
        /// </summary>
        public bool IsSeparator { get; set; }

        // Helper Constructors for cleaner code
        public static MenuItemViewModel Action(string header, ICommand command, object param = null, bool isDestructive = false)
        {
            return new MenuItemViewModel { 
                Header = header, 
                Command = command, 
                CommandParameter = param, 
                IsDestructive = isDestructive 
            };
        }

        public static MenuItemViewModel Separator()
        {
            return new MenuItemViewModel { IsSeparator = true };
        }
    }
}
