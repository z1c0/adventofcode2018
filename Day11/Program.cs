using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

// Day 10

Console.WriteLine("START");
var sw = Stopwatch.StartNew();
Part1(3);
Part1(300);
Console.WriteLine($"END (after {sw.Elapsed.TotalSeconds} seconds)");

void Part1(int sizeLimit)
{
	const int serialNr = 9221;
	const int h = 301;
	const int w = 301;
	var grid = new int [h, w];
	for (var y = 1; y < h; y++)
	{
		for (var x = 1; x < w; x++)
		{
			grid[y, x] = CalculatePowerLevel(x, y, serialNr);
		}
	}

  var cache = new Dictionary<(int, int, int), int>();
	var totalMaxPower = 0;
	(int x, int y) totalMaxCoords = (0, 0);
	var maxSize = 0;
	for (var size = 1; size <= sizeLimit; size++)
	{
		var (maxCoords, maxPower) = GetMaxPowerForSquare(grid, size, cache);
		if (maxPower > totalMaxPower)
		{
			totalMaxPower = maxPower;
			totalMaxCoords = maxCoords;
			maxSize = size;
		}
	}
	System.Console.WriteLine($"The square at {totalMaxCoords.x},{totalMaxCoords.y},{maxSize} has the maximum power of {totalMaxPower}.");
}


((int x, int y), int maxPower) GetMaxPowerForSquare(int[,] grid, int size, Dictionary<(int, int, int), int> cache)
{
	var h = grid.GetLength(0);
	var w = grid.GetLength(1);
	var maxPower = 0;
	(int x, int y) maxCoords = (0, 0);
	for (var y = 1; y < h - size + 1; y++)
	{
		for (var x = 1; x < w - size + 1; x++)
		{
			var power = GetPowerForSquare(grid, x, y, size, cache);
			if (power > maxPower)
			{
				maxPower = power;
				maxCoords = (x, y);
			}
		}
	}
	return (maxCoords, maxPower);
}

int GetPowerForSquare(int[,] grid, int x, int y, int size, Dictionary<(int, int, int), int> cache)
{
	var power = 0;
	if (size == 1)
	{
		power = grid[y, x];
	}
	else
	{
		power = cache[(x, y, size - 1)];
		for (var i = 0; i < size - 1; i++)
		{
			power += grid[y + i       , x + size - 1];
			power += grid[y + size - 1, x + i];
		}
		power += grid[y + size - 1, x + size - 1];
	}
	cache[(x, y, size)] = power;
	return power;
}

int CalculatePowerLevel(int x, int y, int serialNr)
{
	var rackId = x + 10;
	var powerLevel = rackId * y;
	powerLevel += serialNr;
	powerLevel *= rackId;
	powerLevel /= 100;
	powerLevel %= 10;
	powerLevel -= 5;
	return powerLevel;
}