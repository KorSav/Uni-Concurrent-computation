using System.Windows;

namespace ThreadBalls;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow: Window
{
    public MainWindow() => InitializeComponent();

    private void ButtonStart_Click(object sender, RoutedEventArgs e)
    {
        for (int j = 0; j < 1; j++) {
            var ball = new Ball(canvas, Dispatcher);
            var thread = new Thread(() => {
                for (int i = 0; i > -1; i++) {
                    ball.Move();
                    Thread.Sleep(1);
                }
            }) {
                IsBackground = true,
            };
            thread.Start();
        }
    }

    private void ButtonStop_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}