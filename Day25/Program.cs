using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

Console.WriteLine("Day 25 - START");
var sw = Stopwatch.StartNew();
Part1();
Console.WriteLine($"END (after {sw.Elapsed.TotalSeconds} seconds)");

static void Part1()
{
	var constellations = 0;
	var coordinates = ReadInput().ToList();
	while (coordinates.Any())
	{
		constellations++;
		var constellation = new List<(int X, int Y, int Z, int W)> { coordinates[0] };
		coordinates.RemoveAt(0);
		while (true)
		{
			var countBefore = constellation.Count;
			foreach (var c in constellation.ToList())
			{
				var l = coordinates.Where(cc => GetManhattanDistance(c, cc) <= 3).ToList();
				foreach (var e in l)
				{
					coordinates.Remove(e);
				}
				constellation.AddRange(l);
			}
			var countAfter = constellation.Count;
			if (countBefore == countAfter)
			{
				break;
			}
		}
	}
	Console.WriteLine($"Number of constellations: {constellations}");
}

static int GetManhattanDistance((int X, int Y, int Z, int W) p1, (int X, int Y, int Z, int W) p2)
{
	return
		Math.Abs(p2.X - p1.X) +
		Math.Abs(p2.Y - p1.Y) +
		Math.Abs(p2.Z - p1.Z) +
		Math.Abs(p2.W - p1.W);
}

static IEnumerable<(int X, int Y, int Z, int W)> ReadInput()
{
	foreach (var line in File.ReadAllLines("input.txt"))
	{
		var tokens = line.Split(',');
		yield return
		(
			int.Parse(tokens[0]),
			int.Parse(tokens[1]),
			int.Parse(tokens[2]),
			int.Parse(tokens[3])
		);
	}
}
