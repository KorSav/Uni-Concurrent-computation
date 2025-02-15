using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ThreadBalls;

public class Ball
{
    private readonly Dispatcher _dispatcher;
    private readonly Ellipse _circle;
    private readonly Canvas _canvas;
    private readonly double r = 15;
    private double x;
    private double y;
    private double dx = 2;
    private double dy = 2;

    public Ball(Canvas canvas, Dispatcher dispatcher)
    {
        _canvas = canvas;
        _dispatcher = dispatcher;
        var rand = new Random();
        if (rand.NextSingle() < 0.5) {
            x = r + rand.NextDouble() * (_canvas.ActualWidth - 2 * r);
            y = r;
        }
        else {
            x = r;
            y = r + rand.NextDouble() * (_canvas.ActualHeight - 2 * r);
        }
        _circle = new Ellipse() {
            Width = 2 * r,
            Height = 2 * r,
            Fill = new SolidColorBrush(Colors.Black),
        };
        _canvas.Children.Add(_circle);
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        _dispatcher.Invoke(() => {
            Canvas.SetLeft(_circle, x - r);
            Canvas.SetTop(_circle, y - r);
        });
    }

    public void Move()
    {
        x += dx;
        y += dy;
        if (x - r < 0) {
            x = r;
            dx = -dx;
        }
        if (x + r >= _canvas.ActualWidth) {
            x = _canvas.ActualWidth - r;
            dx = -dx;
        }
        if (y - r < 0) {
            y = r;
            dy = -dy;
        }
        if (y + r >= _canvas.ActualHeight) {
            y = _canvas.ActualHeight - r;
            dy = -dy;
        }
        UpdatePosition();
    }
}
