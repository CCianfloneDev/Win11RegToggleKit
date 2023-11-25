namespace ToggleWin10ContextMenu
{
    /// <summary>
    /// Represents the Application.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the main page.
        /// </summary>
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        /// <summary>
        /// Creates the window.
        /// </summary>
        /// <param name="activationState">Activation state.</param>
        /// <returns>Window.</returns>
        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);
            window.Created += Window_Created;
            return window;
        }

        /// <summary>
        /// Handles the WindowCreated event.
        /// </summary>
        private async void Window_Created(object sender, EventArgs e)
        {
            const int defaultWidth = 580;
            const int defaultHeight = 155;

            var window = (Window)sender;
            window.Width = defaultWidth;
            window.Height = defaultHeight;
            window.X = -defaultWidth;
            window.Y = -defaultHeight;

            await window.Dispatcher.DispatchAsync(() => { });

            var displayInfo = DeviceDisplay.Current.MainDisplayInfo;
            window.X = (displayInfo.Width / displayInfo.Density - window.Width) / 2;
            window.Y = (displayInfo.Height / displayInfo.Density - window.Height) / 2;
        }
    }
}
