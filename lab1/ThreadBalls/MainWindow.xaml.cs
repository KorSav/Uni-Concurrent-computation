using System.Windows;

namespace ThreadBalls;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow: Window
{
    private HashSet<Pocket> _pockets = [];

    public MainWindow() => InitializeComponent();

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        const double r = 40;
        _pockets = [
            new(canvas, r, 0, 0),
            new(canvas, r, 0.5, 0),
            new(canvas, r, 1, 0),
            // new(canvas, r, 0, 0.5),
            // new(canvas, r, 1, 0.5),
            new(canvas, r, 0, 1),
            new(canvas, r, 0.5, 1),
            new(canvas, r, 1, 1),
        ];
    }

    private void Window_SizeChanged(object sender, RoutedEventArgs e)
    {
        foreach (var pocket in _pockets) {
            pocket.UpdatePosition();
        }
    }

    private void ButtonStart_Click(object sender, RoutedEventArgs e)
    {
        // ball should be created only on main UI Thread
        var ball = new Ball(canvas, Dispatcher);
        var thread = new Thread(() => {
            while (true) {
                ball.Move();
                foreach (var pocket in _pockets) {
                    if (pocket.TryCaptureBall(ball)) {
                        Dispatcher.Invoke(UpdatePottedCount);
                        return;
                    }
                }
                Thread.Sleep(5);
            }
        }) {
            IsBackground = true
        };
        thread.Start();
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