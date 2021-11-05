using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

Console.WriteLine("Day 22 - START");
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
	Console.WriteLine($"Total minutes spent: {node.Cost}");
}

static Node UniformCostSearch(char[,] grid, (int X, int Y) target)
{
	var startNode = new Node(0, 0, Equipment.Torch, 0);
	var visitedNodes = new Dictionary<(int X, int Y, Equipment Equipment), int>
	{
		{ (startNode.X, startNode.Y, startNode.Equipment), startNode.Cost }
	};
	var queue = new PriorityQueue<Node>();
	queue.Add(startNode);
	while (queue.Count > 0)
	{
		startNode = queue.RemoveFirst();
		foreach (var node in GetAdjacentNodes(startNode, grid))
		{
			var p = (node.X, node.Y, node.Equipment);
			if (!visitedNodes.ContainsKey(p) || visitedNodes[p] > node.Cost)
			{
				visitedNodes[p] = node.Cost;

				if (node.X == target.X && node.Y == target.Y)
				{
					return node;
				}
				queue.Add(node);
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
	var currentRegion = grid[node.Y, node.X];
	var nextRegion = grid[toY, toX];
	switch (nextRegion)
	{
		case '.':  // rocky
			if (node.Equipment == Equipment.Neither)
			{
				cost1 += 7;
				equipment1 = Equipment.ClimbingGear;

				cost2 += 7;
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
				equipment1 = Equipment.ClimbingGear;

				cost2 += 7;
				equipment2 = Equipment.Torch;
			}
			break;
		case '=':  // wet
			if (node.Equipment == Equipment.Torch)
			{
				cost1 += 7;
				equipment1 = Equipment.ClimbingGear;

				cost2 += 7;
				equipment2 = Equipment.Neither;
			}
			else if (node.Equipment == Equipment.Neither)
			{
				cost1 += 7;
				equipment1 = Equipment.ClimbingGear;

				equipment2 = Equipment.Neither;
			}
			else if (node.Equipment == Equipment.ClimbingGear)
			{
				equipment1 = Equipment.ClimbingGear;

				cost2 += 7;
				equipment2 = Equipment.Neither;
			}
			break;
		case '|':  // narrow
			if (node.Equipment == Equipment.ClimbingGear)
			{
				cost1 += 7;
				equipment1 = Equipment.Torch;

				cost2 += 7;
				equipment2 = Equipment.Neither;
			}
			else if (node.Equipment == Equipment.Neither)
			{
				cost1 += 7;
				equipment1 = Equipment.Torch;

				equipment2 = Equipment.Neither;
			}
			else if (node.Equipment == Equipment.Torch)
			{
				equipment1 = Equipment.Torch;

				cost2 += 7;
				equipment2 = Equipment.Neither;
			}
			break;
		default:
			throw new InvalidOperationException();
	}
	if (IsValidForRegion(equipment1, currentRegion))
	{
		yield return new Node(toX, toY, equipment1, cost1);
	}
	if (IsValidForRegion(equipment2, currentRegion))
	{
		yield return new Node(toX, toY, equipment2, cost2);
	}
}

static bool IsValidForRegion(Equipment equipment, char region)
{
	return equipment switch
	{
		Equipment.ClimbingGear => region == '.' || region == '=',
		Equipment.Torch => region == '.' || region == '|',
		Equipment.Neither => region == '=' || region == '|',
		_ => throw new InvalidOperationException(),
	};
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
	Neither,
}

internal record Node(int X, int Y, Equipment Equipment, int Cost) : IComparable
{
	public int CompareTo(object obj)
	{
		var other = (Node)obj;
		return other.Cost.CompareTo(Cost);
	}
}

// from: https://stackoverflow.com/a/33888482/1051140
internal class PriorityQueue<T>
{
	readonly IComparer<T> comparer;
	T[] heap;
	public int Count { get; private set; }
	public PriorityQueue() : this(null) { }
	public PriorityQueue(int capacity) : this(capacity, null) { }
	public PriorityQueue(IComparer<T> comparer) : this(16, comparer) { }
	public PriorityQueue(int capacity, IComparer<T> comparer)
	{
		this.comparer = comparer ?? Comparer<T>.Default;
		this.heap = new T[capacity];
	}
	public void Add(T v)
	{
		if (Count >= heap.Length) Array.Resize(ref heap, Count * 2);
		heap[Count] = v;
		SiftUp(Count++);
	}
	public T RemoveFirst()
	{
		var v = Top();
		heap[0] = heap[--Count];
		if (Count > 0) SiftDown(0);
		return v;
	}
	public T Top()
	{
		if (Count > 0) return heap[0];
		throw new InvalidOperationException();
	}
	void SiftUp(int n)
	{
		var v = heap[n];
		for (var n2 = n / 2; n > 0 && comparer.Compare(v, heap[n2]) > 0; n = n2, n2 /= 2) heap[n] = heap[n2];
		heap[n] = v;
	}
	void SiftDown(int n)
	{
		var v = heap[n];
		for (var n2 = n * 2; n2 < Count; n = n2, n2 *= 2)
		{
			if (n2 + 1 < Count && comparer.Compare(heap[n2 + 1], heap[n2]) > 0) n2++;
			if (comparer.Compare(v, heap[n2]) >= 0) break;
			heap[n] = heap[n2];
		}
		heap[n] = v;
	}
}