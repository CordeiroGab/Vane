using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ProjetoFinal.Views; // Adicionado para referenciar a nova pasta Views

namespace ProjetoFinal
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window? m_window;

        /// <summary>
        /// Initializes the singleton application object. This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user. Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new Window();

            // Cria um Frame para atuar como o contexto de navegação e navega para a primeira página.
            var rootFrame = new Frame();

            // Navega para a StartPage em vez da MainPage
            rootFrame.Navigate(typeof(StartPage), args.Arguments);

            // Coloca o frame na Janela atual
            m_window.Content = rootFrame;

            m_window.Activate();
        }
    }
}
