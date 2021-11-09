using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using aoc;

internal class Unit : IComparable
{
	public Unit(bool isGoblin, int x, int y, int elfAttackPower, Grid grid)
	{
		IsGoblin = isGoblin;
		Pos = (x, y);
		_grid = grid;
		HitPoints = 200;
		AttackPower = isGoblin ? 3 : elfAttackPower;
	}

	public override string ToString()
	{
		var c = IsGoblin ? 'G' : 'E';
		return $"{c} ({HitPoints})";
	}
	public int CompareTo(object obj)
	{
		var myScore = FieldScoreInReadingOrder(Pos, _grid);
		var otherScore = FieldScoreInReadingOrder(((Unit)obj).Pos, _grid);
		return myScore.CompareTo(otherScore);
	}

	public int HitPoints { get; set; }
	public int AttackPower { get; }
	public bool IsGoblin { get; }
	public (int X, int Y) Pos { get; private set; }
	public bool IsAlive { get => HitPoints > 0; }

	private readonly Grid _grid;
	internal void Move(List<Unit> units)
	{
		var targets = units
			.Where(u => u.IsAlive && u != this && u.IsGoblin != IsGoblin)
			.SelectMany(u => GetAdjacent(u.Pos, _grid)).ToList();
		var paths = targets
			.SelectMany(n => BFS(Pos, n, _grid))
			.ToList();
		if (paths.Any())
		{
			var min = paths.Min(p => p.Count);
			var chosen = paths
				.Where(n => n.Count == min)
				.OrderBy(n => FieldScoreInReadingOrder(n.Last(), _grid))
				.ToList();
			var nextPos = chosen.First()[1];
			_grid[Pos] = '.';
			Pos = nextPos;
			_grid[Pos] = IsGoblin ? 'G' : 'E';
		}
	}

	private static List<List<(int x, int y)>> BFS((int x, int y) from, (int x, int y) to, Grid map)
	{
		var paths = new List<List<(int x, int y)>>();

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
					Debug.Assert(!prev.ContainsKey(n));
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
						paths.Add(path);
					}
					else
					{
						queue.Enqueue(n);
					}
				}
			}
		}
		return paths;
	}
	
	static bool Check((int x, int y) p, Grid grid)
	{
		return grid.IsInBounds(p.x, p.y) && grid[p.x, p.y] == '.';
	}

	internal static IEnumerable<(int x, int y)> GetAdjacent((int x, int y) pos, Grid map)
	{
		var up = (pos.x, pos.y - 1);
		var left = (pos.x - 1, pos.y);
		var right = (pos.x + 1, pos.y);
		var down = (pos.x, pos.y + 1);
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
			.OrderBy(u => u.HitPoints).ThenBy(u => FieldScoreInReadingOrder(u.Pos, _grid)).ToList();
		if (targets.Any())
		{
			var target = targets.First();
			if (target != null)
			{
					target.HitPoints = Math.Max(0, target.HitPoints - AttackPower);
					if (!target.IsAlive)
					{
						_grid[target.Pos] = '.';
					}
					return true;
			}
		}
		return false;
	}

	private static int FieldScoreInReadingOrder((int X, int Y) pos, Grid grid)
	{
		return grid.Width * pos.Y + pos.X;
	}
}



