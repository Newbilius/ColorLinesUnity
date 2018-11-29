using System.Collections.Generic;

//ищет в массиве горизонтальные и вертикальные линии, заполненные шариками одного цвета
//todo есть копипаста, но я пока не придумал, как сделать эффективнее 
public class LinesSearcher
{
    private const int minimumLineLength = 5;
    private const int fieldSize = 9;

    public class Result
    {
        public List<Point> Points { get; private set; }
        public int Score { get; private set; }

        public Result(List<Point> points, int score)
        {
            Points = points;
            Score = score;
        }
    }

    public static Result SearchBy(BallBehavior[,] field)
    {
        var pointsForDelete = new List<Point>();
        var scoreX = SearchByX(field, pointsForDelete);
        var scoreY = SearchByY(field, pointsForDelete);
        return new Result(pointsForDelete, scoreX + scoreY);
    }

    private static int SearchByX(BallBehavior[,] field, List<Point> pointForDelete)
    {
        var score = 0;
        for (int y = 0; y < fieldSize; y++)
        {
            var someColorLineLength = 1;
            var lastColorValue = 0;

            for (int x = 0; x < fieldSize; x++)
            {
                if (x == 0)
                {
                    lastColorValue = GetColorValue(field, x, y);
                    continue;
                }
                var currentColorValue = GetColorValue(field, x, y);
                if (currentColorValue == lastColorValue)
                    someColorLineLength++;
                else
                {
                    score += SetDeletedBallsByX(pointForDelete, y, someColorLineLength, lastColorValue, x);
                    someColorLineLength = 1;
                }
                lastColorValue = currentColorValue;
            }

            score += SetDeletedBallsByX(pointForDelete, y, someColorLineLength, lastColorValue, fieldSize);
        }
        return score;
    }

    private static int SetDeletedBallsByX(List<Point> pointForDelete, int y, int someColorLineLength, int lastColorValue, int x)
    {
        if (lastColorValue != 0 && someColorLineLength >= minimumLineLength)
        {
            for (int x2 = x - someColorLineLength; x2 < x; x2++)
                pointForDelete.Add(new Point(x2, y));
            return GetScore(someColorLineLength);
        }
        return 0;
    }

    private static int SearchByY(BallBehavior[,] field, List<Point> pointForDelete)
    {
        var score = 0;
        for (int x = 0; x < fieldSize; x++)
        {
            var someColorLineLength = 1;
            var lastColorValue = 0;

            for (int y = 0; y < fieldSize; y++)
            {
                if (y == 0)
                {
                    lastColorValue = GetColorValue(field, x, y);
                    continue;
                }
                var currentColorValue = GetColorValue(field, x, y);
                if (currentColorValue == lastColorValue)
                    someColorLineLength++;
                else
                {
                    score += SetDeletedBallsByY(pointForDelete, x, someColorLineLength, lastColorValue, y);
                    someColorLineLength = 1;
                }
                lastColorValue = currentColorValue;
            }
            score += SetDeletedBallsByY(pointForDelete, x, someColorLineLength, lastColorValue, fieldSize);
        }
        return score;
    }

    private static int SetDeletedBallsByY(List<Point> pointForDelete, int x, int someColorLineLength, int lastColorValue, int y)
    {
        if (lastColorValue != 0 && someColorLineLength >= minimumLineLength)
        {
            for (int y2 = y - someColorLineLength; y2 < y; y2++)
                pointForDelete.Add(new Point(x, y2));
            return GetScore(someColorLineLength);
        }
        return 0;
    }

    private static int GetColorValue(BallBehavior[,] field, int x, int y)
    {
        return field[x, y] == null ? 0 : field[x, y].BallCode;
    }

    private static int GetScore(int length)
    {
        return 2 * length;
    }
}