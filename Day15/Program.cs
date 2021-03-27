using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

// Day 15

Console.WriteLine("START");
var sw = Stopwatch.StartNew();
Part1();
Part2(); // too hi: 56906
Console.WriteLine($"END (after {sw.Elapsed.TotalSeconds} seconds)");

void Part1()
{
	Simulate(3);
}

void Part2()
{
	var elfAttackPower = 4;
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
	var elves = units.Count(u => u.IsAlive&& !u.IsGoblin);
	var goblins = units.Count(u => u.IsAlive && u.IsGoblin);
	return elves == 0 || goblins == 0;
}

static void Print(int round, char[,] map, List<Unit> units, int elfAttackPower)
{
	System.Console.WriteLine();
	System.Console.WriteLine($"After round {round} (Elf AttackPower = {elfAttackPower}):");
	var h = map.GetLength(0);
	var w = map.GetLength(1);
	for (var y = 0; y < h; y++)
	{
		for (var x = 0; x < w; x++)
		{
			System.Console.Write(map[y, x]);
		}
		foreach (var u in units.Where(u => u.IsAlive && u.Pos.Y == y).OrderBy(u => u.Pos.X))
		{
			System.Console.Write($" {u}");
		}
		System.Console.WriteLine();
	}
	System.Console.WriteLine();
	if (GameOver(units))
	{
		System.Console.WriteLine($"Combat ends after {round} full rounds.");
		var points = units.Sum(u => u.HitPoints);
		System.Console.WriteLine($"Outcome: {round} * {points} = {round * points}");
	}
}

IEnumerable<Unit> FindUnits(char[,] map, int elfAttackPower)
{
	var h = map.GetLength(0);
	var w = map.GetLength(1);
	for (var y = 0; y < h; y++)
	{
		for (var x = 0; x < w; x++)
		{
			var c = map[y, x];
			if (c == 'G' || c == 'E')
			{
				yield return new Unit(c == 'G', x, y, elfAttackPower, map);
			}
		}
	}
}

static char[,] ReadInput()
{
	var lines = File.ReadAllLines("input.txt");
	var map = new char[lines.Count(), lines[0].Length];
	for (var y = 0; y < lines.Count(); y++)
	{
		var line = lines.ElementAt(y);
		for (var x = 0; x < line.Length; x++)
		{
			map[y, x] = line[x];
		}
	}
	return map;
}

internal class Unit : IComparable
{
	public Unit(bool isGoblin, int x, int y, int elfAttackPower, char[,] map)
	{
		IsGoblin = isGoblin;
		Pos = (x, y);
		_map = map;
		HitPoints = 200;
		AttackPower = isGoblin ? 3 : elfAttackPower;
	}

	public override string ToString()
	{
		var c = IsGoblin ? 'G' : 'E';
		return $"{c}({HitPoints})";
	}
	public int CompareTo(object obj)
	{
		var other = (Unit)obj;
		return FieldScoreInReadingOrder(Pos, _map).CompareTo(FieldScoreInReadingOrder(other.Pos, _map));
	}

	public int HitPoints { get; set; }
	public int AttackPower { get; }
	public bool IsGoblin { get; }
	public (int X, int Y) Pos { get; private set; }
	public bool IsAlive { get => HitPoints > 0; }

	private readonly char[,] _map;
	internal void Move(List<Unit> units)
	{
		var inRange = units
			.Where(u => u.IsAlive && u != this && u.IsGoblin != IsGoblin)
			.SelectMany(u => GetAdjacent(u.Pos, _map)).ToList();
		var nearest = inRange
			.SelectMany(n => FindShortestPaths(Pos, n, _map))
			.OrderBy(p => p.Count)
			.ToList();
		if (nearest.Any())
		{
			var min = nearest.First().Count;
			var chosen = nearest
				.Where(n => n.Count == min)
				.OrderBy(n => FieldScoreInReadingOrder(n.Last(), _map))
				.ToList();
			var nextPos = chosen
				.Select(n => n.First())
				.OrderBy(n => FieldScoreInReadingOrder(n, _map))
				.First();
			// now, actually move
			_map[Pos.Y, Pos.X] = '.';
			Pos = nextPos;
			_map[Pos.Y, Pos.X] = IsGoblin ? 'G' : 'E';
		}
	}
	private static List<List<(int x, int y)>> FindShortestPaths((int x, int y) from, (int x, int y) to, char[,] map)
	{
		var results = new List<List<(int x, int y)>>();
		var up = (from.x, from.y - 1);
		var down = (from.x, from.y + 1);
		var left = (from.x - 1, from.y);
		var right = (from.x + 1, from.y);

		results.Add(BFS(up, to, map));
		results.Add(BFS(left, to, map));
		results.Add(BFS(right, to, map));
		results.Add(BFS(down, to, map));
		results.RemoveAll(r => r == null);
		
		return results;
	}

	private static List<(int x, int y)> BFS((int x, int y) from, (int x, int y) to, char[,] map)
	{
		if (!Check(from, map))
		{
			return null;
		}
		if (from == to)
		{
			return new List<(int x, int y)> { from };
		}
		var visited = new HashSet<(int x, int y)>() { from };
		var queue = new Queue<(int x, int y)>();
		var prev = new Dictionary<(int x, int y), (int x, int y)>();
		queue.Enqueue(from);
		while (queue.Any())
		{
			var current = queue.Dequeue();
			foreach (var n in GetAdjacent(current, map))
			{
				if (!visited.Contains(n))
				{
					visited.Add(n);
					prev[n] = current;
					if (n == to)
					{
						// Rebuild path
						var x = n;
						var path = new List<(int x, int y)>() { x };
						do
						{
							x = prev[x];
							path.Insert(0, x);
						}
						while (x != from);
						return path;
					}
					queue.Enqueue(n);
				}
			}
		}
		return null;
	}
	
	static bool Check((int x, int y) p, char[,] map)
	{
		return 
			p.y >= 0 &&
			p.y < map.GetLength(0) &&
			p.x >= 0 &&
			p.x < map.GetLength(1) &&
			map[p.y, p.x] == '.';
	}

	internal static IEnumerable<(int x, int y)> GetAdjacent((int x, int y) pos, char[,] map)
	{
		var up = (pos.x, pos.y - 1);
		var down = (pos.x, pos.y + 1);
		var left = (pos.x - 1, pos.y);
		var right = (pos.x + 1, pos.y);
		if (Check(up, map)) yield return up;
		if (Check(left, map)) yield return left;
		if (Check(right, map)) yield return right;
		if (Check(down, map)) yield return down;
	} 

	internal bool Attack(List<Unit> units)
	{
		var targets = units
			.Where(u => 
				u.IsAlive &&
				u != this &&
				u.IsGoblin != IsGoblin &&
				Math.Abs(Pos.X - u.Pos.X) + Math.Abs(Pos.Y - u.Pos.Y) == 1)
			.OrderBy(u => u.HitPoints).ToList();
		if (targets.Any())
		{
			var min = targets.First().HitPoints;
			var target = targets
				.Where(t => t.HitPoints == min)
				.OrderBy(t => FieldScoreInReadingOrder(t.Pos, _map))
				.FirstOrDefault();
			if (target != null)
			{
					target.HitPoints = Math.Max(0, target.HitPoints - AttackPower);
					if (!target.IsAlive)
					{
						_map[target.Pos.Y, target.Pos.X] = '.';
					}
					return true;
			}
		}
		return false;
	}

	private static int FieldScoreInReadingOrder((int X, int Y) pos, char[,] map)
	{
		var w = map.GetLength(1);
		return w * pos.Y + pos.X;
	}
}



