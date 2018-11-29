using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace LeePathSearchAlgorithm
{
    public class PathSearchLee
    {
        Point[] modificationCells4Path = new Point[]{
            new Point(-1,0),//слева
            new Point(1,0),//справа,
            new Point(0,1),//сверху
            new Point(0,-1),//снизеу
        };

        Point[] modificationCells8Path = new Point[]{
            new Point(-1,0),//слева
            new Point(-1,-1),//слева сверху
            new Point(-1,1),//слева снизу
            new Point(1,0),//справа,
            new Point(1,-1),//справа сверху
            new Point(1,1),//справа снизу
            new Point(0,1),//снизу
            new Point(0,-1),//сверху
        };

        Point[] modificationCells;

        public PathSearchLee(SearchMethod method)
        {
            switch (method)
            {
                case SearchMethod.Path4:
                    modificationCells = modificationCells4Path;
                    break;
                case SearchMethod.Path8:
                    modificationCells = modificationCells8Path;
                    break;
                default:
                    throw new InvalidEnumArgumentException("Неизвестный режим " + ((int)method).ToString());
            }
        }

        public Point[] Search(int[,] map, Point start, Point end)
        {
            var width = map.GetLength(0);
            var height = map.GetLength(1);

            if (start.X == end.X && start.Y == end.Y)
                return new Point[]{
                    end
                };

            if (map[start.X, start.Y] != 0)
                return new Point[0];
            if (map[end.X, end.Y] != 0)
                return new Point[0];
            if (!IsValidCoordinates(start, width, height))
                throw new Exception("Стартовая точка находится вне карты");
            if (!IsValidCoordinates(end, width, height))
                throw new Exception("Конечная точка находится вне карты");
            if (!IsDestinationPotentiallyReachable(map, end))
                return new Point[0];

            var pathMap = (int[,])map.Clone();
            PreparePathMap(pathMap, width);

            var pathQueue = new Queue<Point>();
            pathQueue.Enqueue(start);

            while (pathQueue.Any())
            {
                var currentPoint = pathQueue.Dequeue();
                foreach (var modificationCell in modificationCells)
                {
                    var nextCell = currentPoint.NewDiffPoint(modificationCell);
                    if (IsValidCoordinates(nextCell, width, height)
                        && pathMap[nextCell.X, nextCell.Y] == 0
                        && !nextCell.Equals(start))
                    {
                        pathMap[nextCell.X, nextCell.Y] = pathMap[currentPoint.X, currentPoint.Y] + 1;
                        if (nextCell.Equals(end))
                        {
                            pathQueue.Clear();
                            break;
                        }
                        pathQueue.Enqueue(nextCell);
                    }
                }
            }

            if (pathMap[end.X, end.Y] == 0)
                return new Point[0];

            return GatherPath(pathMap, start, end);
        }

        private bool IsDestinationPotentiallyReachable(int[,] map, Point end)
        {
            var potentialWays = 0;
            var width = map.GetLength(0);
            var height = map.GetLength(1);

            foreach (var modificationCell in modificationCells)
            {
                var nextCell = end.NewDiffPoint(modificationCell);
                if (IsValidCoordinates(nextCell, width, height)
                    && map[nextCell.X, nextCell.Y] == 0)
                    potentialWays++;
            }
            return potentialWays != 0;
        }

        private Point[] GatherPath(int[,] map, Point start, Point end)
        {
            var path = new Stack<Point>();
            var width = map.GetLength(0);
            var height = map.GetLength(1);

            var pathQueue = new Queue<Point>();
            pathQueue.Enqueue(end);
            path.Push(end);

            while (pathQueue.Any())
            {
                var currentPoint = pathQueue.Dequeue();
                var currentValue = map[currentPoint.X, currentPoint.Y];

                foreach (var modificationCell in modificationCells)
                {
                    var nextCell = currentPoint.NewDiffPoint(modificationCell);
                    if (IsValidCoordinates(nextCell, width, height)
                        && map[nextCell.X, nextCell.Y] == currentValue - 1
                        && map[nextCell.X, nextCell.Y] > 0)
                    {
                        path.Push(nextCell);
                        pathQueue.Enqueue(nextCell);
                        break;
                    }
                }
            }
            return path.ToArray();
        }

        private static bool IsValidCoordinates(Point point, int width, int height)
        {
            if (point.X < 0
                || point.Y < 0
                || point.X >= width
                || point.Y >= height)
                return false;
            return true;
        }

        private static void PreparePathMap(int[,] mapForSearch, int width)
        {
            for (var x = 0; x < width; x++)
                for (var y = 0; y < width; y++)
                    if (mapForSearch[x, y] == 1)
                        mapForSearch[x, y] = -1;
        }

        public enum SearchMethod
        {
            Path4 = 1, //только горизонтальные и вертикальные пути
            Path8 = 2 //ищем и диагональные пути
        }
    }
}