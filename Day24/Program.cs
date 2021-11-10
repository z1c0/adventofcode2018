using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

Console.WriteLine("Day 24 - START");
var sw = Stopwatch.StartNew();
Part1();
Console.WriteLine($"END (after {sw.Elapsed.TotalSeconds} seconds)");

static void Part1()
{
	var coordinates = ReadInput().ToList();
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
