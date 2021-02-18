using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

// Day 6

Console.WriteLine("START");
var sw = Stopwatch.StartNew();
Part1();
Part2();
Console.WriteLine($"END (after {sw.Elapsed.TotalSeconds} seconds)");

void Part1()
{
	var coords = ReadInput().ToList();
	var minX = coords.Select(t => t.x).Min();
	var minY = coords.Select(t => t.y).Min();
	var maxX = coords.Select(t => t.x).Max();
	var maxY = coords.Select(t => t.y).Max();
	
	var grid = new int[maxY + 1, maxX + 1];
	for (var y = minY; y <= maxY; y++)
	{
		for (var x = minX; x < maxX; x++)
		{
			// Find nearest point
			foreach (var t in GetPointWithMinimumDistance((x, y), coords))
			{
				if (grid[y, x] == 0)
				{
					grid[y, x] = coords.IndexOf(t) + 1;
				}
				else
				{
					grid[y, x] = -1;
				}
			}
		}
	}
	// count areas
	var areas = new Dictionary<int, int>();
	for (var y = minY; y <= maxY; y++)
	{
		for (var x = minX; x <= maxX; x++)
		{
			var n = grid[y, x];
			if (n > 0)
			{
				areas.TryAdd(n, 0);
				areas[n]++;
			}
		}
	}
	// maximum only counts for finite areas
	var max = 0;
	var finites = coords.Where(t => IsFinite(t, coords)).ToList();
	foreach (var t in finites)
	{
		var index = coords.IndexOf(t) + 1;
		max = Math.Max(max, areas[index]);
	}
	System.Console.WriteLine($"The largest area has a size of {max}.");
}

void Part2()
{
	var coords = ReadInput().ToList();
	var minX = coords.Select(t => t.x).Min();
	var minY = coords.Select(t => t.y).Min();
	var maxX = coords.Select(t => t.x).Max();
	var maxY = coords.Select(t => t.y).Max();
	
	var grid = new int[maxY + 1, maxX + 1];
	var count = 0;
	for (var y = minY; y <= maxY; y++)
	{
		for (var x = minX; x < maxX; x++)
		{
			var distance = 0;
			foreach (var t in coords)
			{
				distance += Math.Abs(x - t.x) + Math.Abs(y - t.y);
			}
			if (distance < 10000)
			{
				count++;
			}
		}
	}
	System.Console.WriteLine($"Size of the region with a distance < 10000 is {count}");
}

static bool IsFinite((int x, int y) c, List<(int x, int y)> coords)
{
	var cIndex = coords.IndexOf(c);
	var idx1 = coords.IndexOf(GetPointWithMinimumDistance((c.x + 1000, c.y), coords).Single());
	var idx2 = coords.IndexOf(GetPointWithMinimumDistance((c.x - 1000, c.y), coords).Single());
	var idx3 = coords.IndexOf(GetPointWithMinimumDistance((c.x, c.y + 1000), coords).Single());
	var idx4 = coords.IndexOf(GetPointWithMinimumDistance((c.x, c.y - 1000), coords).Single());
	return
		cIndex != idx1 &&
		cIndex != idx2 &&
		cIndex != idx3 &&
		cIndex != idx4;
}

static IEnumerable<(int x, int y)> GetPointWithMinimumDistance((int x, int y) p, List<(int x, int y)> coords)
{
	var minDist = coords.Min(t => Math.Abs(p.x - t.x) + Math.Abs(p.y - t.y));
	return coords.Where(t => Math.Abs(p.x - t.x) + Math.Abs(p.y - t.y) == minDist);
}

IEnumerable<(int x, int y)> ReadInput()
{
	foreach (var line in File.ReadAllLines("input.txt"))
	{
		var tokens = line.Split(", ");
		yield return (int.Parse(tokens[0]), int.Parse(tokens[1]));
	}
}
