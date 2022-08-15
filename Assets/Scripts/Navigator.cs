using System.Collections.Generic;
using System;

namespace Array2DNavigation
{
    public static class Navigator
    {
        #region Variables
        private static Dictionary<Tile, Tile> preves = new Dictionary<Tile, Tile>();
        #endregion

        #region Functions
        public static List<Tile> GetPath(Tile[,] grid, Tile start, Tile end)
        {
            preves.Clear();

            Comparison<Tile> heuristicComparison = (leftTile, rightTile) =>
            {
                int leftCost = GetCost(leftTile, end);
                int tighCost = GetCost(rightTile, end);

                return leftCost.CompareTo(tighCost);
            };

            CustomList<Tile> toCheck = new CustomList<Tile>(heuristicComparison);
            toCheck.Add(start);

            HashSet<Tile> visited = new HashSet<Tile>();
            visited.Add(start);

            preves.Add(start, null);

            while (toCheck.Count > 0)
            {
                var current = toCheck.Remove();

                if (current == end) break;

                foreach (var neighbor in GetNeighbours(current, grid))
                    if (!visited.Contains(neighbor) && neighbor.Type != Tile.TileType.Obstacle)
                    {
                        toCheck.Add(neighbor);
                        visited.Add(neighbor);
                        if (!preves.ContainsKey(neighbor))
                            preves.Add(neighbor, current);
                        else
                            preves[neighbor] = current;
                    }
            }

            return BackToPath(end);
        }

        private static List<Tile> GetNeighbours(Tile current, Tile[,] grid)
        {
            var x = current.Position.X;
            var y = current.Position.Y;
            var result = new List<Tile>(4);
            if (x > 0) result.Add(grid[x - 1, y]);
            if (x < grid.GetLongLength(0) - 1) result.Add(grid[x + 1, y]);
            if (y > 0) result.Add(grid[x, y - 1]);
            if (y < grid.GetLongLength(1) - 1) result.Add(grid[x, y + 1]);
            return result;
        }

        private static List<Tile> BackToPath(Tile end)
        {
            Tile current = end;
            List<Tile> path = new List<Tile>();

            while (current != null)
            {
                path.Add(current);
                if (preves.ContainsKey(current))
                    current = preves[current];
                else
                    current = null;
            }

            path.RemoveAt(path.Count - 1);
            path.Reverse();
            return path;
        }

        private static int GetCost(Tile current, Tile end)
        {
            int cost = current.Position.GetMagnitudeDistanceTo(end.Position);
            return cost;
        }
        #endregion
    }
}