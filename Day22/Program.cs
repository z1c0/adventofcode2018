using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

Console.WriteLine("Day 22 -START");
var sw = Stopwatch.StartNew();
Part1();
Part2();
Console.WriteLine($"END (after {sw.Elapsed.TotalSeconds} seconds)");

static void Part1()
{
	var (depth, target) = ReadInput();
	var w = target.X + 1;
	var h = target.Y + 1;
  var grid = CreateCharGrid(w, h, depth);
	Print(grid, target);
}

static void Part2()
{
	var (depth, target) = ReadInput();
	var d = Math.Max(target.X, target.Y) + 100;
  var grid = CreateCharGrid(d, d, depth);
	var node = UniformCostSearch(grid, target);
	Console.WriteLine(node);
}

static Node UniformCostSearch(char[,] grid, (int X, int Y) target)
{
	var startNode = new Node(0, 0, Equipment.Torch, 0);
	var visitedNodes = new HashSet<(int X, int Y, Equipment Equipment, int Cost)>
	{
		(startNode.X, startNode.Y, startNode.Equipment, startNode.Cost)
	};
	var queue = new PriorityQueue<Node>();
	queue.Add(startNode);
	while (queue.Any()) 
	{
		startNode = queue.RemoveFirst();
		foreach (var node in GetAdjacentNodes(startNode, grid))
		{
			var p = (node.X, node.Y, node.Equipment, node.Cost);
			if (!visitedNodes.Contains(p))
			{
				visitedNodes.Add(p);
				if (node.X == target.X && node.Y == target.Y)
				{
					System.Console.WriteLine(node + " from:" + startNode);
					return node;
				}
				else
				{
					queue.Add(node);
				}
			}
		}
	}
	return null;
}

static List<Node> GetAdjacentNodes(Node node, char[,] grid)
{
	var adjacent = new List<Node>();
	if (node.X > 0)
	{
		adjacent.AddRange(CalcFromTo(node, node.X - 1, node.Y, grid));
	}
	if (node.Y > 0)
	{
		adjacent.AddRange(CalcFromTo(node, node.X, node.Y - 1, grid));
	}
	adjacent.AddRange(CalcFromTo(node, node.X + 1, node.Y, grid));
	adjacent.AddRange(CalcFromTo(node, node.X, node.Y + 1, grid));
	return adjacent;
}

static IEnumerable<Node> CalcFromTo(Node node, int toX, int toY, char[,] grid)
{
	Equipment equipment1 = default;
	Equipment equipment2 = default;
	var cost1 = node.Cost + 1;
	var cost2 = node.Cost + 1;
	switch (grid[toX, toY])
	{
		case '.':  // rocky
			if (node.Equipment == Equipment.None)
			{
				cost1 += 7;
				cost2 += 7;
				equipment1 = Equipment.ClimbingGear;
				equipment2 = Equipment.Torch;
			}
			else if (node.Equipment == Equipment.Torch)
			{
				cost1 += 7;
				equipment1 = Equipment.ClimbingGear;
				equipment2 = Equipment.Torch;
			}
			else if (node.Equipment == Equipment.ClimbingGear)
			{
				cost2 += 7;
				equipment1 = Equipment.ClimbingGear;
				equipment2 = Equipment.Torch;
			}
		  break;
		case '=':  // wet
			if (node.Equipment == Equipment.Torch)
			{
				cost1 += 7;
				cost2 += 7;
				equipment1 = Equipment.ClimbingGear;
				equipment2 = Equipment.None;
			}
			else if (node.Equipment == Equipment.None)
			{
				cost1 += 7;
				equipment1 = Equipment.ClimbingGear;
				equipment2 = Equipment.None;
			}
			else if (node.Equipment == Equipment.ClimbingGear)
			{
				cost2 += 7;
				equipment1 = Equipment.ClimbingGear;
				equipment2 = Equipment.None;
			}
		  break;
		case '|':  // narrow
			if (node.Equipment == Equipment.ClimbingGear)
			{
				cost1 += 7;
				cost2 += 7;
				equipment1 = Equipment.Torch;
				equipment2 = Equipment.None;
			}
			else if (node.Equipment == Equipment.None)
			{
				cost1 += 7;
				equipment1 = Equipment.Torch;
				equipment2 = Equipment.None;
			}
			else if (node.Equipment == Equipment.Torch)
			{
				cost2 += 7;
				equipment1 = Equipment.Torch;
				equipment2 = Equipment.None;
			}
		  break;
		default:
			throw new InvalidOperationException();
	}
	yield return new Node(toX, toY, equipment1, cost1);
	yield return new Node(toX, toY, equipment2, cost2);
}

static char[,] CreateCharGrid(int w, int h, int depth)
{
	var grid = new int[h, w];
	for (var y = 0; y < h; y++)
	{
		for (var x = 0; x < w; x++)
		{
			CalculateType(x, y, depth, grid);
		}
	}
	return PrintToCharGrid(grid);
}

static char[,] PrintToCharGrid(int[,] grid)
{
	var h = grid.GetLength(0);
	var w = grid.GetLength(1);
	var charGrid = new char[h, w];
	for (var y = 0; y < h; y++)
	{
		for (var x = 0; x < w; x++)
		{
			var c = (grid[y, x] % 3) switch
			{
				0 => '.',
				1 => '=',
				2 => '|',
				_ => throw new InvalidOperationException(),
			};
			charGrid[y, x] = c;
		}
	}
	return charGrid;
}

static void Print(char[,] grid, (int X, int Y) target)
{
	var riskLevel = 0;
	for (var y = 0; y <= target.Y; y++)
	{
		for (var x = 0; x <= target.X; x++)
		{
			var c = grid[y, x];
			var risk = c switch
			{
				'.' => 0,
				'=' => 1,
				'|' => 2,
				_ => throw new InvalidOperationException(),
			};
			riskLevel += risk;
			Console.Write(c);
		}
		Console.WriteLine();		
	}
	Console.WriteLine($"Total risk level: {riskLevel}");
}

static void CalculateType(int x, int y, int depth, int[,] grid)
{
	var h = grid.GetLength(0);
	var w = grid.GetLength(1);
	int geologicIndex;
	if (x == 0 && y == 0 || x == w - 1 && y == h - 1)
	{
		geologicIndex = 0;
	}
	else if (y == 0)
	{
		geologicIndex = x * 16807;
	}
	else if (x == 0)
	{
		geologicIndex = y * 48271;
	}
	else
	{
		geologicIndex = grid[y, x - 1] * grid[y - 1, x];
	}
	var erosionLevel = (geologicIndex + depth) % 20183;
	grid[y, x] = erosionLevel;
}

static (int Depth, (int X, int Y) Target) ReadInput()
{
	var lines = File.ReadAllLines("input.txt");
	var depth = int.Parse(lines[0].Split(": ")[1]);
	var target = lines[1].Split(": ")[1].Split(',');
	var x = int.Parse(target[0]);
	var y = int.Parse(target[1]);
	return (depth, (x, y));
}

internal enum Equipment
{
	ClimbingGear,
	Torch,
	None,
}

internal record Node(int X, int Y, Equipment Equipment, int Cost) : IComparable
{
	public int CompareTo(object obj)
	{
		var other = (Node)obj;
		return Cost.CompareTo(other.Cost);
	}
}

internal class PriorityQueue<T>
{
	private readonly List<T> _items = new();

	internal void Add(T item)
	{
		_items.Add(item);
		_items.Sort();
	}

	internal bool Any()
	{
		return _items.Count > 0;
	}

	internal T RemoveFirst()
	{
		var item = _items[0];
		_items.RemoveAt(0);
		return item;
	}
}