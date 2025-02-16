using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ThreadBalls;

public class Ball: IDisposable
{
    public readonly double R = 10;
    private readonly Dispatcher _dispatcher;
    private readonly Canvas _canvas;
    private readonly Ellipse _circle;
    private double dx = 2;
    private double dy = 2;

    public double X { get; private set; }
    public double Y { get; private set; }

    public Ball(Canvas canvas, Dispatcher dispatcher)
    {
        _canvas = canvas;
        _dispatcher = dispatcher;
        var rand = new Random();
        if (rand.NextSingle() < 0.5) {
            X = R + rand.NextDouble() * (_canvas.ActualWidth - 2 * R);
            Y = R;
        }
        else {
            X = R;
            Y = R + rand.NextDouble() * (_canvas.ActualHeight - 2 * R);
        }
        _circle = new Ellipse() {
            Width = 2 * R,
            Height = 2 * R,
            Fill = new SolidColorBrush(Colors.Black),
        };
        _canvas.Children.Add(_circle);
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        _dispatcher.Invoke(() => {
            Canvas.SetLeft(_circle, X - R);
            Canvas.SetTop(_circle, Y - R);
        });
    }

    public void Move()
    {
        X += dx;
        Y += dy;
        if (X - R < 0) {
            X = R;
            dx = -dx;
        }
        if (X + R >= _canvas.ActualWidth) {
            X = _canvas.ActualWidth - R;
            dx = -dx;
        }
        if (Y - R < 0) {
            Y = R;
            dy = -dy;
        }
        if (Y + R >= _canvas.ActualHeight) {
            Y = _canvas.ActualHeight - R;
            dy = -dy;
        }
        UpdatePosition();
    }

    public void Dispose()
    {
        _dispatcher.Invoke(() => _canvas.Children.Remove(_circle));
        GC.SuppressFinalize(this);
    }
}
