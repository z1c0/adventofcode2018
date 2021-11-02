using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

Console.WriteLine("Day 20 - START");
var sw = Stopwatch.StartNew();
Part1();
Console.WriteLine($"END (after {sw.Elapsed.TotalSeconds} seconds)");

static void Part1()
{
	var clay = ReadInput();
	var map = new Dictionary<(int X, int Y), char>();
	foreach (var c in clay)
	{
		c.PrintToMap(map);
	}
	//Simulate((int X, int Y ))
	Print(map);
}

static void Print(Dictionary<(int X, int Y), char> map)
{
	var minX = map.Keys.Min(k => k.X) - 1;
	var maxX = map.Keys.Max(k => k.X) + 1;
	var minY = map.Keys.Min(k => k.Y) - 1;
	var maxY = map.Keys.Max(k => k.Y) + 1;
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

