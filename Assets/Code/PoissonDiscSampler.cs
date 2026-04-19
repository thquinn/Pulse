using System.Collections.Generic;
using UnityEngine;

// Courtesy Claude
public static class PoissonDiskSampler {
    public static List<Vector2> Sample(
        float width, float height,
        float minDist,
        int maxAttempts = 30) {
        float cellSize = minDist / Mathf.Sqrt(2f);
        int cols = Mathf.CeilToInt(width / cellSize);
        int rows = Mathf.CeilToInt(height / cellSize);

        // Background grid: stores index+1 of the point in that cell (0 = empty)
        int[,] grid = new int[cols, rows];
        var points = new List<Vector2>();
        var active = new List<Vector2>();

        // Seed with a random point
        var seed = new Vector2(Random.Range(0, width), Random.Range(0, height));
        points.Add(seed);
        active.Add(seed);
        InsertToGrid(seed, grid, cellSize, points.Count);

        while (active.Count > 0) {
            int idx = Random.Range(0, active.Count);
            Vector2 origin = active[idx];
            bool found = false;

            for (int attempt = 0; attempt < maxAttempts; attempt++) {
                // Random point in the [minDist, 2*minDist] annulus
                float angle = Random.Range(0f, Mathf.PI * 2f);
                float radius = Random.Range(minDist, minDist * 2f);
                var candidate = origin + new Vector2(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius);

                if (candidate.x < 0 || candidate.x >= width ||
                    candidate.y < 0 || candidate.y >= height)
                    continue;

                if (IsFarEnough(candidate, grid, points, cellSize, cols, rows, minDist)) {
                    points.Add(candidate);
                    active.Add(candidate);
                    InsertToGrid(candidate, grid, cellSize, points.Count);
                    found = true;
                    break;
                }
            }

            if (!found)
                active.RemoveAt(idx);
        }

        return points;
    }

    static void InsertToGrid(Vector2 p, int[,] grid, float cellSize, int index) {
        int gx = (int)(p.x / cellSize);
        int gy = (int)(p.y / cellSize);
        grid[gx, gy] = index; // 1-based index
    }

    static bool IsFarEnough(
        Vector2 candidate, int[,] grid, List<Vector2> points,
        float cellSize, int cols, int rows, float minDist) {
        int gx = (int)(candidate.x / cellSize);
        int gy = (int)(candidate.y / cellSize);

        // Only check the 5x5 neighborhood in the grid
        int x0 = Mathf.Max(0, gx - 2), x1 = Mathf.Min(cols - 1, gx + 2);
        int y0 = Mathf.Max(0, gy - 2), y1 = Mathf.Min(rows - 1, gy + 2);

        for (int x = x0; x <= x1; x++)
            for (int y = y0; y <= y1; y++) {
                int storedIdx = grid[x, y];
                if (storedIdx == 0) continue;
                if (Vector2.SqrMagnitude(candidate - points[storedIdx - 1]) < minDist * minDist)
                    return false;
            }

        return true;
    }
}