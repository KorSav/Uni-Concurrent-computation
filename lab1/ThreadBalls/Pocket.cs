using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ThreadBalls;

public class Pocket
{
    private readonly Ellipse _circle;
    private readonly Canvas _canvas;
    private readonly double _r;
    private readonly double _relativeX;
    private readonly double _relativeY;
    private double _x;
    private double _y;

    private readonly Lock _pottedCountLock = new();
    public int PottedCount { get; private set; }

    public Pocket(Canvas canvas, double r, double relativeX, double relativeY)
    {
        _canvas = canvas;
        _relativeX = relativeX;
        _relativeY = relativeY;
        _r = r;

        _circle = new Ellipse() {
            Width = 2 * _r,
            Height = 2 * _r,
            Fill = new SolidColorBrush(Colors.Gray),
        };
        UpdatePosition();
        _canvas.Children.Add(_circle);
    }

    public void UpdatePosition()
    {
        _x = _canvas.ActualWidth * _relativeX;
        _y = _canvas.ActualHeight * _relativeY;
        Canvas.SetLeft(_circle, _x - _r);
        Canvas.SetTop(_circle, _y - _r);
    }

    public bool TryCaptureBall(Ball ball)
    {
        double d = Math.Sqrt(
            Math.Pow(_x - ball.X, 2) +
            Math.Pow(_y - ball.Y, 2)
        );
        if (d < _r + ball.R) {
            lock(_pottedCountLock) PottedCount++;
            ball.Dispose();
            return true;
        }
        return false;
    }
}