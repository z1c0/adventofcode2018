using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

Console.WriteLine("Day 18 -START");
var sw = Stopwatch.StartNew();
Part1(10);
//
// The "pattern" repeats all 35.000 iterations (see pattern.txt)
// So instead of solving for 1000000000, we can solve for 1000000000 % 35000.
//
Part1(1000000000 % 35000);
Console.WriteLine($"END (after {sw.Elapsed.TotalSeconds} seconds)");

static void Part1(int iterations)
{
	var grid = ReadInput();
	var grid2 = new char[grid.GetLength(0), grid.GetLength(1)];
	var i = 0;
	Print(grid, 0);
	while (i++ < iterations)
	{
		Simulate(grid, grid2);
		(grid, grid2) = (grid2, grid);
		if (i % 5000 == 0) Evaluate(grid, i);
	}
	Evaluate(grid, iterations);
}

static void Evaluate(char[,] grid, int iteration)
{	
	var h = grid.GetLength(0);
	var w = grid.GetLength(1);
	var trees = 0;
	var lumberyards = 0;
	for (var y = 0; y < h; y++)
	{
		for (var x = 0; x < w; x++)
		{
			Check(grid, x, y, ref trees, ref lumberyards);
		}
	}
	Console.WriteLine($"Total resource value after {iteration} iterations: {trees * lumberyards}");
}

static void Simulate(char[,] grid, char[,] grid2)
{
	var h = grid.GetLength(0);
	var w = grid.GetLength(1);
	for (var y = 0; y < h; y++)
	{
		for (var x = 0; x < w; x++)
		{
			var c = grid[y, x];
			var (trees, lumberyards) = GetAdjacent(grid, x, y);
			if (c == '.')
			{
				if (trees >= 3)
				{
					c = '|';
				}
			}
			else if (c == '|')
			{
				if (lumberyards >= 3)
				{
					c = '#';
				}
			}
			else if (c == '#')
			{
				if (lumberyards == 0 || trees == 0)
				{
					c = '.';
				}
			}
			grid2[y, x] = c;
		}
	}
}

static (int Trees, int Lumberyards) GetAdjacent(char[,] grid, int x, int y)
{
	var trees = 0;
	var lumberyards = 0;
	Check(grid, x - 1, y,     ref trees, ref lumberyards);
	Check(grid, x - 1, y - 1, ref trees, ref lumberyards);
	Check(grid, x    , y - 1, ref trees, ref lumberyards);
	Check(grid, x + 1, y - 1, ref trees, ref lumberyards);
	Check(grid, x + 1, y,     ref trees, ref lumberyards);
	Check(grid, x + 1, y + 1, ref trees, ref lumberyards);
	Check(grid, x,     y + 1, ref trees, ref lumberyards);
	Check(grid, x - 1, y + 1, ref trees, ref lumberyards);
	return (trees, lumberyards);
}

static void Check(char[,] grid, int x, int y, ref int trees, ref int lumberyards)
{
	var h = grid.GetLength(0);
	var w = grid.GetLength(1);
	if (x >= 0 && y >= 0 && x < w && y < h)
	{
		switch (grid[y, x])
		{
			case '|':
				trees++;
				break;
			case '#':
				lumberyards++;
				break;
			default:
				break;
		}
	}
}

static void Print(char[,] grid, int iteration)
{
	Console.WriteLine();
	Console.WriteLine($"After iteration {iteration}:");
	var h = grid.GetLength(0);
	var w = grid.GetLength(1);
	for (var y = 0; y < h; y++)
	{
		for (var x = 0; x < w; x++)
		{
			Console.Write(grid[y, x]);
		}
		Console.WriteLine();
	}
}

static char[,] ReadInput()
{
	var lines = File.ReadAllLines("input.txt");
	var h = lines.Length;
	var w = lines.First().Length;
	var grid = new char[h, w];
	for (var y = 0; y < h; y++)
	{
		for (var x = 0; x < w; x++)
		{
			grid[y, x] = lines[y][x];
		}
	}
	return grid;
}
