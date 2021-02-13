using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

// Day 3

Console.WriteLine("START");
var sw = Stopwatch.StartNew();
Part1();
Console.WriteLine($"END (after {sw.Elapsed.TotalSeconds} seconds)");

void Part1()
{
	var grid = new List<int>[1000, 1000];
	var claimed = 0;
	var claims = ReadInput().ToList();

	foreach (var claim in claims)
	{
		for (var y = claim.y; y < claim.y + claim.h; y++)
		{
			for (var x = claim.x; x < claim.x + claim.w; x++)
			{
				if (grid[y, x] == null)
				{
					grid[y, x] = new List<int>();
				}
				grid[y, x].Add(claim.id);
				if (grid[y, x].Count == 2)
				{
					claimed++;
				}
			}
		}
	}
	System.Console.WriteLine($"{claimed} square inches are claimed more than once.");

	foreach (var claim in claims)
	{
		var intact = true;
		for (var y = claim.y; y < claim.y + claim.h; y++)
		{
			for (var x = claim.x; x < claim.x + claim.w; x++)
			{
				if (grid[y, x].Count > 1)
				{
					intact = false;
				}
			}
		}
		if (intact)
		{
			System.Console.WriteLine($"Claim #{claim.id} is intact.");
			break;
		}
	}
}

IEnumerable<(int id, int x, int y, int w, int h)> ReadInput()
{
	foreach (var line in File.ReadAllLines("input.txt"))
	{
		var tokens = line.Split(" @ ");
		var id = int.Parse(tokens[0][1..]);
		tokens = tokens[1].Split(": ");
		var tokens1 = tokens[0].Split(',');
		var x = int.Parse(tokens1[0]);
		var y = int.Parse(tokens1[1]);
		tokens1 = tokens[1].Split('x');
		var w = int.Parse(tokens1[0]);
		var h = int.Parse(tokens1[1]);
		yield return (id, x, y, w, h);
	}
}