using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

// Day 14

Console.WriteLine("START");
var sw = Stopwatch.StartNew();
Part1();
Part2();
Console.WriteLine($"END (after {sw.Elapsed.TotalSeconds} seconds)");

void Part1()
{
	var current1 = 0;
	var current2 = 1;
	var list = new List<int> { 3, 7 };
	var startScoring = ReadInput();
	var pos = 0;
	while (true)
	{
		if (list.Count > startScoring + 10)
		{
			break;
		}
		var sum = list[current1] + list[current2];
		if (sum >= 10)
		{
			list.Add(1);
		}
		if (list.Count == startScoring)
		{
			pos = list.Count;
		}
		list.Add(sum % 10);
		if (list.Count == startScoring)
		{
			pos = list.Count;
		}
		
		current1 = (current1 + list[current1] + 1) % list.Count;
		current2 = (current2 + list[current2] + 1) % list.Count;

	}
	Print(pos, list);
}

void Part2()
{
	bool Check(int input, List<int> list, List<int> trail)
	{
		if (list.Skip(list.Count - trail.Count).SequenceEqual(trail))
		{
			System.Console.WriteLine($"{input} first appears after {list.Count - trail.Count} recipes.");
			return true;
		}
		return false;
	}
	var current1 = 0;
	var current2 = 1;
	var list = new List<int> { 3, 7 };
	var trail = new List<int>();
	var input = ReadInput();
	var tmp = input;
	while (tmp != 0)
	{
		trail.Insert(0, tmp % 10);
		tmp /= 10;
	}
	while (true)
	{
		var sum = list[current1] + list[current2];
		if (sum >= 10)
		{
			list.Add(1);
			if (Check(input, list, trail))
			{
				break;
			}
		}
		list.Add(sum % 10);
		if (Check(input, list, trail))
		{
			break;
		}

		current1 = (current1 + list[current1] + 1) % list.Count;
		current2 = (current2 + list[current2] + 1) % list.Count;
	}
}

void Print(int pos, List<int> list)
{
	for (var i = pos; i < pos + 10; i++)
	{
		System.Console.Write(list[i]);
	}
	System.Console.WriteLine();
}

static int ReadInput()
{
	return int.Parse(File.ReadAllText("input.txt"));
}
