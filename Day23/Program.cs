using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

Console.WriteLine("Day 23 - START");
var sw = Stopwatch.StartNew();
Part1();
Part2();
Console.WriteLine($"END (after {sw.Elapsed.TotalSeconds} seconds)");

static void Part1()
{
	var nanobots = ReadInput().ToList();
	var strongest = nanobots.OrderByDescending(n => n.R).First();
	var inRange = nanobots.Count(n => GetManhattanDistance(strongest.Pos, n.Pos) <= strongest.R);
	Console.WriteLine($"Nanobots in range: {inRange}");
}

static void Part2()
{
  // I binary-searched my way into this one.

	var nanobots = ReadInput().ToList();
	var minX = nanobots.Min(n => n.Pos.X - n.R);
	var maxX = nanobots.Max(n => n.Pos.X + n.R);
	var minY = nanobots.Min(n => n.Pos.Y - n.R);
	var maxY = nanobots.Max(n => n.Pos.Y + n.R);
	var minZ = nanobots.Min(n => n.Pos.Z - n.R);
	var maxZ = nanobots.Max(n => n.Pos.Z + n.R);
	var minR = nanobots.Min(n => n.R);

  // (50611087, 48290405, 40780204): 852 -> distance 139681696
	// (48952888, 46632206, 42438403): 863 -> distance 138023497
	// (44350854, 43622040, 38764949): 879 -> distance 126737843
	// (44251361, 43522544, 38665443): 879 -> distance 126439348
	// (44241181, 43512364, 38655263): 879 -> distance 126408808
	// (44174815, 43512324, 38622060): 879 -> distance 126309199
	// (44196497, 43492552, 38610579): 879 -> distance 126299628
	// (44176497, 43492552, 38590579): 879 -> distance 126259628
	// (44166797, 43482552, 38587779): 917 -> distance 126237128
	// (44166797, 43481552, 38586779): 917 -> distance 126235128
	// (44166757, 43480552, 38585779): 941 -> distance 126233088
	// (44166777, 43480572, 38585799): 948 -> distance 126233148
	// (44166767, 43480552, 38585779): 961 -> distance 126233098
	// (44166767, 43480552, 38585779): 961 -> distance 126233098
	// (44166763, 43480550, 38585775): 972 -> distance 126233088
	// (44166763, 43480550, 38585775): 972 -> distance 126233088 <- That's it!
	
	var dx = 50;
	var dy = 50;
	var dz = 50;
	var step = 1;
	var cx = 44166763;
	minX = cx - dx;
	maxX = cx + dx;
	var cy = 43480550;
	minY = cy - dy;
	maxY = cy + dy;
	var cz = 38585775;
	minZ = cz - dz;
	maxZ = cz + dz;

	var maxCount = 0;
	for (var x = minX; x <= maxX; x += step)
	{
		for (var y = minY; y <= maxY; y += step)
		{
			for (var z = minZ; z <= maxZ; z += step)
			{
				var count = 0;
				foreach (var b in nanobots)
				{
					if (GetManhattanDistance((x, y, z), b.Pos) <= b.R)
					{
						count++;
					}
				}
				if (count > maxCount)
				{
					maxCount = count;
					var dist = GetManhattanDistance((x, y, z), (0, 0, 0));
					Console.WriteLine($"{(x, y, z)}: {maxCount} -> distance {dist}");
				}
			}
		}
	}
}

static int GetManhattanDistance((int X, int Y, int Z) p1, (int X, int Y, int Z) p2)
{
	return
		Math.Abs(p2.X - p1.X) +
		Math.Abs(p2.Y - p1.Y) +
		Math.Abs(p2.Z - p1.Z);
}

static IEnumerable<((int X, int Y, int Z) Pos, int R)> ReadInput()
{
	foreach (var line in File.ReadAllLines("input.txt"))
	{
		var tokens = line.Split(", ");
		var tokens2 = tokens[0][5..^1].Split(',');
		yield return
		(
			(
				int.Parse(tokens2[0]),
				int.Parse(tokens2[1]),
				int.Parse(tokens2[2])
			),
			int.Parse(tokens[1].Split('=')[1])
		);
	}
}

internal class Segment
{
	public Segment(int left, int right)
	{
		Left = left;
		Right = right;
		Count = 1;
	}

	public int Left;
	public int Right;
	public int Count;
}