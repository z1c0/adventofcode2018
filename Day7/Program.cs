using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

// Day 7

Console.WriteLine("START");
var sw = Stopwatch.StartNew();
Part1(1);
Part1(5);
Console.WriteLine($"END (after {sw.Elapsed.TotalSeconds} seconds)");

void Part1(int availableWorkers)
{
	var instructions = ReadInput();
	var workQueue = new SortedDictionary<int, char>();
	var time = 0;
	while (instructions.Any() || workQueue.Any())
	{
		// workpool
		if (workQueue.Any())
		{
			var next = workQueue.First();
			time = next.Key;
			System.Console.Write(next.Value);
			//System.Console.WriteLine($"<- {next.Value} finished at {time}");
			workQueue.Remove(next.Key);
			availableWorkers++;
		}

		var nextList = instructions.Where(e => IsRoot(instructions, workQueue, e))
			.Select(e => e.Key).OrderBy(e => e).ToList();
		foreach (var next in nextList)
		{
			if (availableWorkers > 0)
			{
				// start work
				instructions.Remove(next);
				workQueue.Add(time + next - 'A' + 61, next);
				//System.Console.WriteLine($"-> {next} started at {time}");
				availableWorkers--;
			}
		}
	}
	// too low 424
	// too high 1674
	System.Console.WriteLine();
	System.Console.WriteLine($"Completed after {time} steps");
}

static bool IsRoot(Dictionary<char, List<char>> instructions, SortedDictionary<int, char> workQueue, KeyValuePair<char, List<char>> e)
{
	if( !e.Value.Any())
	{
		return true;
	}
	foreach (var k in e.Value)
	{
		if (instructions.ContainsKey(k) || workQueue.Values.Contains(k))
		{
			return false;
		}
	}
	return true;
}

static Dictionary<char, List<char>> ReadInput()
{
	var instructions = new Dictionary<char, List<char>>();
	foreach (var line in File.ReadAllLines("input.txt"))
	{
		var tokens = line.Split(' ');
		var step = tokens[7].Single();
		var prerequisite = tokens[1].Single();

		instructions.TryAdd(step, new List<char>());
		instructions.TryAdd(prerequisite, new List<char>());
		instructions[step].Add(prerequisite);
	}
	return instructions;
}
