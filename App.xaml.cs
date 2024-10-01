namespace Lab3;

public partial class App : Application
{
	/// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// This constructor sets up the main components of the application
    /// and defines the starting page (AppShell).
    /// </summary>
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell(); // sets starting page to app shell
	}
}
