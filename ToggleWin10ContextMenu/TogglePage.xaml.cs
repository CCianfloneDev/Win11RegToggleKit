using System.Diagnostics;

namespace ToggleWin10ContextMenu
{
    /// <summary>
    /// Represents the main page.
    /// </summary>
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// Initializes the main page.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the switch toggled event.
        /// </summary>
        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            Debug.Print(e.ToString());
        }
    }

}
