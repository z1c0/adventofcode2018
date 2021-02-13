using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

// Day 2

Console.WriteLine("START");
var sw = Stopwatch.StartNew();
Part1();
Part2();
Console.WriteLine($"END (after {sw.Elapsed.TotalSeconds} seconds)");

void Part2()
{
	var ids = ReadInput().ToList();
	var sb = new StringBuilder();
	for (var i = 0; i < ids.Count; i++)
	{
		var id1 = ids[i];
		for (var j = i + 1; j < ids.Count; j++)
		{
			var	id2 = ids[j];
			sb.Clear();
			for (var k = 0; k < id1.Length; k++)
			{
				if (id1[k] == id2[k])
				{
					sb.Append(id1[k]);
				}
			}
			if (sb.Length == id1.Length - 1)
			{
				System.Console.WriteLine(sb);
				return;
			}
		}
	}
}

void Part1()
{
	var twos = 0;
	var threes = 0;
	foreach (var id in ReadInput())
	{
		var groups = id.GroupBy(c => c);
		if (groups.Any(g => g.Count() == 2))
		{
			twos++;
		}
		if (groups.Any(g => g.Count() == 3))
		{
			threes++;
		}
	}
	System.Console.WriteLine($"{twos} * {threes} = {twos * threes}");
}

IEnumerable<string> ReadInput()
{
	return File.ReadAllLines("input.txt");
}