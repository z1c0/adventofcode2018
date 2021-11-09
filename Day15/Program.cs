using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using aoc;

Console.WriteLine("START - Day 15");
var sw = Stopwatch.StartNew();
Part1();
Part2();
Console.WriteLine($"END (after {sw.Elapsed.TotalSeconds} seconds)");

void Part1()
{
	Simulate(3);
}

void Part2()
{
	var elfAttackPower = 25;
	while (true)
	{
		if (Simulate(elfAttackPower))
		{
			break;
		}
		elfAttackPower++;
	}
}

bool Simulate(int elfAttackPower)
{
	var map = ReadInput();
	var units = FindUnits(map, elfAttackPower).ToList();
	var elvesAtStart = units.Count(u => !u.IsGoblin);
  var round = 0;
	//Print(round, map, units, elfAttackPower);
	while (true)
	{
		units.Sort();

		foreach (var u in units)
		{
			if (u.IsAlive)
			{
				if (!u.Attack(units))
				{
					u.Move(units);
					u.Attack(units);
				}

				if (GameOver(units))
				{
					if (u == units.Last())
					{
						round++;
					}
					Print(round, map, units, elfAttackPower);
					var elvesAtEnd = units.Count(u => u.IsAlive && !u.IsGoblin);
					return elvesAtStart == elvesAtEnd;
				}
			}
		}
		units.RemoveAll(u => !u.IsAlive);
		round++;
		//Print(round, map, units, elfAttackPower);
	}

	throw new InvalidOperationException();
}


static bool GameOver(List<Unit> units)
{
	// game over?
	var elves = units.Count(u => u.IsAlive && !u.IsGoblin);
	var goblins = units.Count(u => u.IsAlive && u.IsGoblin);
	return elves == 0 || goblins == 0;
}

static void Print(int round, Grid grid, List<Unit> units, int elfAttackPower)
{
	Console.WriteLine();
	Console.WriteLine($"After round {round} (Elf AttackPower = {elfAttackPower}):");
	for (var y = 0; y < grid.Height; y++)
	{
		for (var x = 0; x < grid.Width; x++)
		{
			Console.Write(grid[x, y]);
		}
		foreach (var u in units.Where(u => u.IsAlive && u.Pos.Y == y).OrderBy(u => u.Pos.X))
		{
			Console.Write($" {u}");
		}
		Console.WriteLine();
	}
	Console.WriteLine();
	if (GameOver(units))
	{
		Console.WriteLine($"Combat ends after {round} full rounds.");
		var elves = units.Count(u => u.IsAlive && !u.IsGoblin);
		Console.WriteLine($"{elves} elves.");
		var points = units.Sum(u => u.HitPoints);
		Console.WriteLine($"Outcome: {round} * {points} = {round * points}");
	}
}

IEnumerable<Unit> FindUnits(Grid grid, int elfAttackPower)
{
	for (var y = 0; y < grid.Height; y++)
	{
		for (var x = 0; x < grid.Width; x++)
		{
			var c = grid[x, y];
			if (c == 'G' || c == 'E')
			{
				yield return new Unit(c == 'G', x, y, elfAttackPower, grid);
			}
		}
	}
}

static Grid ReadInput()
{
	return Grid.FromFile("input.txt");
}



