using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator
{
    public class Cell
    {
        public bool visited;
        public bool top = true;
        public bool right = true;
        public bool bottom = true;
        public bool left = true;
    }

    public int Width { get; private set; }
    public int Height { get; private set; }
    public Cell[,] Cells { get; private set; }
    public List<Vector2Int> Solution { get; private set; } = new();

    private System.Random rng;

    public void Generate(int width, int height, int seed, Vector2Int start, Vector2Int finish, int minimumPathLength = 20)
    {
        Width = width;
        Height = height;
        rng = new System.Random(seed);

        int attempts = 0;
        do
        {
            BuildMaze();
            Solution = FindPath(start, finish);
            attempts++;
        }
        while (Solution.Count < minimumPathLength && attempts < 30);
    }

    private void BuildMaze()
    {
        Cells = new Cell[Width, Height];
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                Cells[x, y] = new Cell();

        var stack = new Stack<Vector2Int>();
        Vector2Int current = new Vector2Int(0, 0);
        Cells[current.x, current.y].visited = true;
        stack.Push(current);

        while (stack.Count > 0)
        {
            current = stack.Peek();
            List<Vector2Int> neighbors = UnvisitedNeighbors(current);

            if (neighbors.Count == 0)
            {
                stack.Pop();
                continue;
            }

            Vector2Int next = neighbors[rng.Next(neighbors.Count)];
            RemoveWall(current, next);
            Cells[next.x, next.y].visited = true;
            stack.Push(next);
        }
    }

    private List<Vector2Int> UnvisitedNeighbors(Vector2Int c)
    {
        var result = new List<Vector2Int>(4);
        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

        foreach (var d in dirs)
        {
            Vector2Int n = c + d;
            if (n.x >= 0 && n.x < Width && n.y >= 0 && n.y < Height && !Cells[n.x, n.y].visited)
                result.Add(n);
        }

        return result;
    }

    private void RemoveWall(Vector2Int a, Vector2Int b)
    {
        int dx = b.x - a.x;
        int dy = b.y - a.y;

        if (dx == 1)
        {
            Cells[a.x, a.y].right = false;
            Cells[b.x, b.y].left = false;
        }
        else if (dx == -1)
        {
            Cells[a.x, a.y].left = false;
            Cells[b.x, b.y].right = false;
        }
        else if (dy == 1)
        {
            Cells[a.x, a.y].top = false;
            Cells[b.x, b.y].bottom = false;
        }
        else if (dy == -1)
        {
            Cells[a.x, a.y].bottom = false;
            Cells[b.x, b.y].top = false;
        }
    }

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int finish)
    {
        var queue = new Queue<Vector2Int>();
        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        var visited = new HashSet<Vector2Int>();

        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            Vector2Int c = queue.Dequeue();
            if (c == finish) break;

            foreach (Vector2Int n in OpenNeighbors(c))
            {
                if (visited.Contains(n)) continue;
                visited.Add(n);
                cameFrom[n] = c;
                queue.Enqueue(n);
            }
        }

        var path = new List<Vector2Int>();
        if (start == finish)
        {
            path.Add(start);
            return path;
        }

        if (!cameFrom.ContainsKey(finish)) return path;

        Vector2Int current = finish;
        path.Add(current);
        while (current != start)
        {
            current = cameFrom[current];
            path.Add(current);
        }
        path.Reverse();
        return path;
    }

    private List<Vector2Int> OpenNeighbors(Vector2Int c)
    {
        var result = new List<Vector2Int>(4);
        Cell cell = Cells[c.x, c.y];

        if (!cell.top && c.y + 1 < Height) result.Add(new Vector2Int(c.x, c.y + 1));
        if (!cell.right && c.x + 1 < Width) result.Add(new Vector2Int(c.x + 1, c.y));
        if (!cell.bottom && c.y - 1 >= 0) result.Add(new Vector2Int(c.x, c.y - 1));
        if (!cell.left && c.x - 1 >= 0) result.Add(new Vector2Int(c.x - 1, c.y));

        return result;
    }
}
