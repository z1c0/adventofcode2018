using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

Console.WriteLine("Day 27 - START");
var sw = Stopwatch.StartNew();
Part1();
Console.WriteLine($"END (after {sw.Elapsed.TotalSeconds} seconds)");

static void Part1()
{
	var clay = ReadInput().ToList();
	var map = new Dictionary<(int X, int Y), char>();
	foreach (var c in clay)
	{
		c.PrintToMap(map);
	}
	var origin = (500, 0);
	map[origin] = '+';
	Simulate(origin, map);
	Print(map);

	var minY = clay.Min(c => c.FromY);
	var maxY = clay.Max(c => c.ToY);
	var countSand = map.Count(e => e.Key.Y >= minY && e.Key.Y <= maxY && e.Value == '|');
	var countWater = map.Count(e => e.Key.Y >= minY && e.Key.Y <= maxY && e.Value == '~');
	Console.WriteLine($"Number of water-reachable tiles: {countWater + countSand}");
	Console.WriteLine($"Number of water tiles: {countWater}");
}

static void Simulate((int X, int Y) origin, Dictionary<(int X, int Y), char> map)
{
	var maxY = map.Keys.Max(k => k.Y) + 1;
	var queue = new List<Water>()
	{
		new Water(origin.X, origin.Y, Orientation.Vertical)
	};
	while (queue.Any())
	{
		var water = queue[0];
		queue.RemoveAt(0);
		if (water.Orientation == Orientation.Vertical)
		{
			// scan down
			var x = water.X;
			var y = water.Y + 1;
			while ((!map.ContainsKey((x, y + 1)) || map[(x, y + 1)] == '|') && y < maxY)
			{
				y++;
			}
			Fill(water.X, water.Y + 1, x, y, map, '|');
			if (y < maxY)
			{
				// flow left-right
				queue.Add(new Water(x, y, Orientation.Horizontal));
			}
		}
		else
		{
			var r1 = ScanHorizontal(water.X, water.Y, true, map);
			var r2 = ScanHorizontal(water.X, water.Y, false, map);
			if (r1.C == '#' && r2.C == '#')
			{
				Fill(r1.X + 1, r1.Y, r2.X, r2.Y, map, '~');
				// Bubble up a level.
				queue.Add(new Water(water.X, water.Y - 1, Orientation.Horizontal));
			}
			else if (r1.C == '.' || r2.C == '.' || r1.C == '|' || r2.C == '|')
			{
				var fx1 = r1.X + 1;
				var fx2 = r2.X;
				if (r1.C == '.')
				{
					fx1--;
					queue.Add(new Water(r1.X, r1.Y, Orientation.Vertical));
				}
				if (r2.C == '.')
				{
					fx2++;
					queue.Add(new Water(r2.X, r2.Y, Orientation.Vertical));
				}
				Fill(fx1, r1.Y, fx2, r1.Y, map, '|');
			}
		}
	}
}

static (int X, int Y, char C) ScanHorizontal(int x, int y, bool left, Dictionary<(int X, int Y), char> map)
{
	char Get((int , int) p)
	{
		char c = '.';
		if (map.ContainsKey(p))
		{
			c = map[p];
		}
		return c;
	}
	var d = left ? -1 : 1;
	while (true)
	{
		x += d;
		var c1 = Get((x, y + 1));
		if (c1 == '.' || c1 == '|')
		{
			break;
		}
		var c2 = Get((x, y));
		if (c2 == '#')
		{
			break;
		}
	}
	return (x, y, Get((x, y)));
}

static void Fill(int x1, int y1, int x2, int y2, Dictionary<(int X, int Y), char> map, char c)
{
	var dx = x1 != x2 ? 1: 0;
	var dy = y1 != y2 ? 1: 0;
	while (x1 != x2 || y1 != y2)
	{
		Debug.Assert(!map.ContainsKey((x1, y1)) || map[(x1, y1)] != '#');
		map[(x1, y1)] = c;
		x1 += dx;
		y1 += dy;
	}
}

static void Print(Dictionary<(int X, int Y), char> map)
{
	var minX = map.Keys.Min(k => k.X);
	var maxX = map.Keys.Max(k => k.X);
	var minY = map.Keys.Min(k => k.Y);
	var maxY = map.Keys.Max(k => k.Y);
	for (var y = minY; y <= maxY; y++)
	{
		for (var x = minX; x <= maxX; x++)
		{
			var c = '.';
			if (map.ContainsKey((x, y)))
			{
				c = map[(x, y)];
			}
			Console.Write(c);
		}
		Console.WriteLine();
	}
}

static IEnumerable<Clay> ReadInput()
{
	foreach (var line in File.ReadAllLines("input.txt"))
	{
		var tokens = line.Split(", ");
		var fromX = 0;
		var fromY = 0;
		var toX = 0;
		var toY = 0;
		if (tokens[0][0] == 'y')
		{
			(tokens[0], tokens[1]) = (tokens[1], tokens[0]);
		}
		static void Parse(string s, ref int from, ref int to)
		{
			var tokens = s.Split('=');
			tokens = tokens[1].Split("..");
			from = int.Parse(tokens[0]);
			to = tokens.Length > 1 ? int.Parse(tokens[1]) : from;
		}
		Parse(tokens[0], ref fromX, ref toX);
		Parse(tokens[1], ref fromY, ref toY);
		yield return new Clay(fromX, toX, fromY, toY);
	}
}

internal record Clay(int FromX, int ToX, int FromY, int ToY)
{
	internal void PrintToMap(Dictionary<(int X, int Y), char> map)
	{
		for (var y = FromY; y <= ToY; y++)
		{
			for (var x = FromX; x <= ToX; x++)
			{
				map[(x, y)] = '#';
			}
		}
	}
}

internal enum Orientation
{
	Vertical,
	Horizontal,
}

internal record Water(int X, int Y, Orientation Orientation);
