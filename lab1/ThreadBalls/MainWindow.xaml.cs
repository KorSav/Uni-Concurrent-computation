using System.Windows;
using System.Windows.Media;

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
        // const double r = 40;
        // _pockets = [
        //     new(canvas, r, 0, 0),
        //     new(canvas, r, 0.5, 0),
        //     new(canvas, r, 1, 0),
        //     // new(canvas, r, 0, 0.5),
        //     // new(canvas, r, 1, 0.5),
        //     new(canvas, r, 0, 1),
        //     new(canvas, r, 0.5, 1),
        //     new(canvas, r, 1, 1),
        // ];
    }

    private void Window_SizeChanged(object sender, RoutedEventArgs e)
    {
        foreach (var pocket in _pockets) {
            pocket.UpdatePosition();
        }
    }

    private void ButtonStart_Click(object sender, RoutedEventArgs e)
    {
        Action test = 5 switch {
            // Test 1
            1 => () => {
                CreateBalls(10, Colors.Blue);
                CreateBalls(10, Colors.Red);
            }
            , // Test 2
            2 => () => {
                CreateBalls(50, Colors.Blue);
                CreateBalls(50, Colors.Red);
            }
            , // Test 3
            3 => () => {
                CreateBalls(10, Colors.Blue);
                CreateBalls(1, Colors.Red);
            }
            , // Test 4
            4 => () => {
                CreateBalls(50, Colors.Blue);
                CreateBalls(1, Colors.Red);
            }
            , // Test 5
            5 => () => {
                CreateBalls(100, Colors.Blue);
                CreateBalls(1, Colors.Red);
            }
            , // Incorrect number
            _ => throw new NotImplementedException()
        };
        test();
    }

    private void CreateBalls(int count, Color color)
    {
        for (int i = 0; i < count; i++) {
            var ball = new Ball(canvas, Dispatcher, color);
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
                IsBackground = true,
                Priority = true switch {
                    _ when color == Colors.Red => ThreadPriority.AboveNormal,
                    _ when color == Colors.Blue => ThreadPriority.Normal,
                    _ => throw new NotImplementedException()
                }
            };
            thread.Start();
        }
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