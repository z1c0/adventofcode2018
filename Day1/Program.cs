using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

// Day 1

Console.WriteLine("START");
var sw = Stopwatch.StartNew();
Part1();
Part2();
Console.WriteLine($"END (after {sw.Elapsed.TotalSeconds} seconds)");

void Part1()
{
	System.Console.WriteLine($"Resulting frequency: {ReadInput().Sum()}");
}

void Part2()
{
	var frequency = 0;
	var frequencies = new HashSet<int>();
	var input = ReadInput();
	while (true)
	{
		foreach (var n in input)
		{
			frequency += n;
			if (frequencies.Contains(frequency))
			{
				System.Console.WriteLine($"Frequency {frequency} was reached twice.");
				return;
			}
			frequencies.Add(frequency);
		}
	}
}

IEnumerable<int> ReadInput()
{
	foreach (var line in File.ReadAllLines("input.txt"))
	{
		yield return int.Parse(line);
	}
}