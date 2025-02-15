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
    private readonly double r = 20;
    private double x = 0;
    private double y = 0;
    private double dx = 2;
    private double dy = 2;

    public Ball(Canvas canvas, Dispatcher dispatcher)
    {
        _canvas = canvas;
        _dispatcher = dispatcher;
        var rand = new Random();
        if (rand.NextSingle() < 0.5) {
            x = rand.NextDouble() * _canvas.ActualWidth;
        }
        else {
            y = rand.NextDouble() * _canvas.ActualHeight;
        }
        _circle = new Ellipse() {
            Width = r,
            Height = r,
            Fill = new SolidColorBrush(Colors.Black),
        };
        _canvas.Children.Add(_circle);
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        _dispatcher.Invoke(() => {
            Canvas.SetLeft(_circle, x);
            Canvas.SetTop(_circle, y);
        });
    }

    public void Move()
    {
        x += dx;
        y += dy;
        if (x < 0) {
            x = 0;
            dx = -dx;
        }
        if (x + r >= _canvas.ActualWidth) {
            x = _canvas.ActualWidth - r;
            dx = -dx;
        }
        if (y < 0) {
            y = 0;
            dy = -dy;
        }
        if (y + r >= _canvas.ActualHeight) {
            y = _canvas.ActualHeight - r;
            dy = -dy;
        }
        UpdatePosition();
    }
}
