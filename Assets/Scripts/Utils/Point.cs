public struct Point
{
    public readonly int X, Y;
    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Point NewDiffPoint(Point newPoint)
    {
        return new Point(X + newPoint.X, Y + newPoint.Y);
    }
}