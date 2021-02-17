using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

// Day 5

Console.WriteLine("START");
var sw = Stopwatch.StartNew();
Part1();
Part2();
Console.WriteLine($"END (after {sw.Elapsed.TotalSeconds} seconds)");

void Part2()
{
	var minLength = Int32.MaxValue;
	var input = ReadInput();
	var sb = new StringBuilder();
	for (var c = 'a'; c <= 'z'; c++)
	{
		sb.Clear();
		foreach (var cc in input)
		{
			if (Char.ToLower(cc) != c)
			{
				sb.Append(cc);
			}
		}
		minLength = Math.Min(minLength, Reduce(sb));
	}
	System.Console.WriteLine($"Shortest polymer length: {minLength}");
}

void Part1()
{
	System.Console.WriteLine($"{Reduce(new StringBuilder(ReadInput()))} units remain");
}

int Reduce(StringBuilder sb1)
{
	var sb2 = new StringBuilder(sb1.Length);
	do
	{
		sb2.Clear();
		for (var i = 0; i < sb1.Length; i++)
		{
			var c = sb1[i];
			var cc = i < sb1.Length - 1 ? sb1[i + 1] : '0';
			if (Math.Abs(c - cc) != ('a' - 'A'))
			{
				sb2.Append(c);
			}
			else
			{
				i++;
			}
		}
		var tmp = sb2;
		sb2 = sb1;
		sb1 = tmp;
	}
	while (sb1.Length != sb2.Length);

	return sb1.Length;
}

string ReadInput()
{
	return File.ReadAllText("input.txt");
}
