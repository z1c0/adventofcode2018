using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

// Day 9

Console.WriteLine("START");
var sw = Stopwatch.StartNew();
Part1(71032);
Part1(71032 * 100);
Console.WriteLine($"END (after {sw.Elapsed.TotalSeconds} seconds)");

void Part1(int lastMarble)
{
	const int numberOfPlayers = 441;
	var score = new Dictionary<int, long>();
	var marbles = new LinkedList<int>();
  var current = marbles.AddLast(0);
	var player = 0;
	for (var i = 1; i <= lastMarble; i++)
	{
		if (i % 23 == 0)
		{
			score.TryGetValue(player, out var s);
			score[player] = s + i;
			for (var j = 0; j < 7; j++)
			{
				current = current.Previous ?? marbles.Last;
			}
			score[player] += current.Value;
			var next = current.Next ?? marbles.First;
			marbles.Remove(current);
			current = next;
		}
		else
		{
			var next = current.Next ?? marbles.First;
			current = marbles.AddAfter(next, i);
		}
		
		player = (player + 1) % numberOfPlayers;
	}

	var hiScore = score.Values.Max();
	var winningElf = score.Single(e => e.Value == hiScore).Key;
	System.Console.WriteLine($"Elf {winningElf + 1} wins with a score of {hiScore}.");
}