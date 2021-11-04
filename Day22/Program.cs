using System;
using System.Diagnostics;
using System.IO;

Console.WriteLine("Day 22 -START");
var sw = Stopwatch.StartNew();
Part1();
Console.WriteLine($"END (after {sw.Elapsed.TotalSeconds} seconds)");

static void Part1()
{
	var (depth, target) = ReadInput();
	var w = target.X + 1;
	var h = target.Y + 1;
	var grid = new long[h, w];
	for (var y = 0; y < h; y++)
	{
		for (var x = 0; x < w; x++)
		{
			CalculateType(x, y, depth, grid);
		}
	}
	Print(grid);
}

static void Print(long[,] grid)
{
	var riskLevel = 0L;
	var h = grid.GetLength(0);
	var w = grid.GetLength(1);
	for (var y = 0; y < h; y++)
	{
		for (var x = 0; x < w; x++)
		{
			var risk = grid[y, x] % 3;
			riskLevel += risk;
			var c = risk switch
			{
				0 => '.',
				1 => '=',
				2 => '|',
				_ => throw new InvalidOperationException(),
			};
			Console.Write(c);
		}
		Console.WriteLine();
	}
	Console.WriteLine($"Total risk level: {riskLevel}");
}

static void CalculateType(int x, int y, int depth, long[,] grid)
{
	var h = grid.GetLength(0);
	var w = grid.GetLength(1);
	long geologicIndex;
	if (x == 0 && y == 0 || x == w - 1 && y == h - 1)
	{
		geologicIndex = 0;
	}
	else if (y == 0)
	{
		geologicIndex = x * 16807;
	}
	else if (x == 0)
	{
		geologicIndex = y * 48271;
	}
	else
	{
		geologicIndex = grid[y, x - 1] * grid[y - 1, x];
	}
	var erosionLevel = (geologicIndex + depth) % 20183;
	grid[y, x] = erosionLevel;
}

static (int Depth, (int X, int Y) Target) ReadInput()
{
	var lines = File.ReadAllLines("input.txt");
	var depth = int.Parse(lines[0].Split(": ")[1]);
	var target = lines[1].Split(": ")[1].Split(',');
	var x = int.Parse(target[0]);
	var y = int.Parse(target[1]);
	return (depth, (x, y));
}
