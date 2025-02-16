using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ThreadBalls;

public class Ball: IDisposable
{
    public readonly double R;
    private readonly Dispatcher _dispatcher;
    private readonly Canvas _canvas;
    private readonly Ellipse _circle;
    private double dx = 2;
    private double dy = 0;

    public double X { get; private set; }
    public double Y { get; private set; }

    public Ball(Canvas canvas, Dispatcher dispatcher, Color color, double r)
        : this(canvas, dispatcher, color, r, 0, 0)
    {
        var rand = new Random();
        if (rand.NextSingle() < 0.5) {
            X = R + rand.NextDouble() * (_canvas.ActualWidth - 2 * R);
            Y = R;
        }
        else {
            X = R;
            Y = R + rand.NextDouble() * (_canvas.ActualHeight - 2 * R);
        }
        UpdatePosition();
    }

    public Ball(Canvas canvas, Dispatcher dispatcher, Color color, double r, double x, double y)
    {
        R = r;
        _canvas = canvas;
        _dispatcher = dispatcher;
        X = x;
        Y = y;
        _circle = new Ellipse() {
            Width = 2 * R,
            Height = 2 * R,
            Fill = new SolidColorBrush(color),
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
