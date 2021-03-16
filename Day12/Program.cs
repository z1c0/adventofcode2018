using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

// Day 12

Console.WriteLine("START");
var sw = Stopwatch.StartNew();
Part1(20);
Part1(108); // (this gives us the start index for part 2)
Part2(50000000000);

Console.WriteLine($"END (after {sw.Elapsed.TotalSeconds} seconds)");

void Part1(long generations)
{
	var (state, rules) = ReadInput();
	
	var leftIndex = 0;
	for (var g = 0; g < generations; g++)
	{
		if (state[0] != '.' || state[1] != '.' || state[2] != '.')
		{
			state.Insert(0, '.');
			state.Insert(0, '.');
			state.Insert(0, '.');
			leftIndex -= 3;
		}
		if (state[^1] != '.' || state[^2] != '.' || state[^3] != '.')
		{
			state.Add('.');
			state.Add('.');
			state.Add('.');
		}
		//System.Console.WriteLine($"{g:D2}: {new string(state.ToArray())}");
		var nextState = state.ToList();
		var first = Math.Max(2, state.IndexOf('#') - 3);
		var last = state.Count - 2;
		for (var i = first; i < last; i++)
		{
			var rule = rules.SingleOrDefault(r =>
				r.input[0] == state[i - 2] &&
				r.input[1] == state[i - 1] &&
				r.input[2] == state[i + 0] &&
				r.input[3] == state[i + 1] &&
				r.input[4] == state[i + 2]);
			nextState[i] = (rule.input != null) ? rule.result : '.';
		}
		state = nextState;
	}
	long sum = 0;
	var firstPotIndex = -1;
	foreach (var p in state)
	{
		if (p == '#')
		{
			if (firstPotIndex == -1)
			{
				firstPotIndex = leftIndex;
			}
			sum += leftIndex;
		}
		leftIndex++;
	}	
	System.Console.WriteLine($"After {generations} generations, the sum of pot plants is {sum} (first pot index = {firstPotIndex}).");
}

void Part2(long generations)
{
	//
	// Starting with generation 108 the pattern stays the same but "moves"
	// to the right from index 48.
	//
	// ...
	// 107: .......................................................#...###....###.......###.........###........###.........###.......###.....###.........###...###.......###........###.....###.......###.........###...####....
	// 108: ......................................................###...###....###.......###.........###........###.........###.......###.....###.........###...###.......###........###.....###.......###.........###...####...
	// 109: .......................................................###...###....###.......###.........###........###.........###.......###.....###.........###...###.......###........###.....###.......###.........###...####.....
	// 110: ........................................................###...###....###.......###.........###........###.........###.......###.....###.........###...###.......###........###.....###.......###.........###...####....
	// 111: .........................................................###...###....###.......###.........###........###.........###.......###.....###.........###...###.......###........###.....###.......###.........###...####...
	// 112: ..........................................................###...###....###.......###.........###........###.........###.......###.....###.........###...###.......###........###.....###.......###.........###...####.....
	// 113: ...........................................................###...###....###.......###.........###........###.........###.......###.....###.........###...###.......###........###.....###.......###.........###...####....
	// 114: ............................................................###...###....###.......###.........###........###.........###.......###.....###.........###...###.......###........###.....###.......###.........###...####...
	// 115: .............................................................###...###....###.......###.........###........###.........###.......###.....###.........###...###.......###........###.....###.......###.........###...####.....
  //...
	// so we only need to find out the first pot with with a plant after 50000000000 generations
	// and add up the following pots based on the pattern above.
	var pattern = "###...###....###.......###.........###........###.........###.......###.....###.........###...###.......###........###.....###.......###.........###...####";
	long potIndex = 48 + generations - 108;
	long sum = 0;
	foreach (var p in pattern)
	{
		if (p == '#')
		{
			sum += potIndex;
		}
		potIndex++;
	}
	System.Console.WriteLine($"After {generations} generations, the sum of pot plants is {sum}.");
}

static (List<char>, List<(char[] input, char result)>) ReadInput()
{
	var lines = File.ReadAllLines("input.txt");
	var tokens = lines[0].Split(": ");
	var state = tokens[1].ToCharArray().ToList();
	var rules = new List<(char[], char)>();
	foreach (var line in lines.Skip(2))
	{
		tokens = line.Split(" => ");
		rules.Add((tokens[0].ToCharArray(), tokens[1].Single()));
	}
	return (state, rules);
}

