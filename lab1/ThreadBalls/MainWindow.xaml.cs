using System.Windows;
using System.Windows.Media;

namespace ThreadBalls;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow: Window
{
    private HashSet<Pocket> _pockets = [];

    private const double pocketR = 40;

    public MainWindow() => InitializeComponent();

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        _pockets = [
            new(canvas, pocketR, 0, 0),
            new(canvas, pocketR, 0.5, 0),
            new(canvas, pocketR, 1, 0),
            new(canvas, pocketR, 0, 0.5),
            new(canvas, pocketR, 1, 0.5),
            new(canvas, pocketR, 0, 1),
            new(canvas, pocketR, 0.5, 1),
            new(canvas, pocketR, 1, 1),
        ];
    }

    private void Window_SizeChanged(object sender, RoutedEventArgs e)
    {
        foreach (var pocket in _pockets) {
            pocket.UpdatePosition();
        }
    }

    /// <summary>
    /// Creates red and blue ball in separate threads.
    /// Red one is joined to blue, so red ball will stand still
    /// until blue one is terminated.
    /// </summary>
    private void ButtonStart_Click(object sender, RoutedEventArgs e)
    {
        const double ballR = 20;
        var ballBlue = new Ball(
            canvas, Dispatcher, Colors.Blue, ballR,
            pocketR + ballR, 0.5*canvas.ActualHeight - ballR
        );
        var threadBlue = new Thread(() => {
            while (true) {
                ballBlue.Move();
                foreach (var pocket in _pockets) {
                    if (pocket.TryCaptureBall(ballBlue)) {
                        Dispatcher.Invoke(UpdatePottedCount);
                        return;
                    }
                }
                Thread.Sleep(5);
            }
        }) {
            IsBackground = true,
        };
        threadBlue.Start();

        var ballRed = new Ball(
            canvas, Dispatcher, Colors.Red, ballR,
            pocketR + ballR, 0.5*canvas.ActualHeight + ballR
        );
        var threadRed = new Thread(() => {
            threadBlue.Join();
            while (true) {
                ballRed.Move();
                foreach (var pocket in _pockets) {
                    if (pocket.TryCaptureBall(ballRed)) {
                        Dispatcher.Invoke(UpdatePottedCount);
                        return;
                    }
                }
                Thread.Sleep(5);
            }
        }) {
            IsBackground = true,
        };
        threadRed.Start();
    }

    private void UpdatePottedCount()
    {
        int pc = 0;
        foreach (var pocket in _pockets) {
            pc += pocket.PottedCount;
        }
        pottedCount.Text = pc.ToString();
    }

    private void ButtonStop_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}