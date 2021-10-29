using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

Console.WriteLine("Day 20 - START");
var sw = Stopwatch.StartNew();
Part1();
Console.WriteLine($"END (after {sw.Elapsed.TotalSeconds} seconds)");

static void Part1()
{
	var steps = ReadInput();
	var map = new Dictionary<(int, int), char>();
	var x = 0;
	var y = 0;
	var cache = new HashSet<string>();
	Wander(ref x, ref y, 0, steps, map, cache);
	Print(map);
	var result = BFS(((0, 0), 0), map);
	var maxDoors = result.Max(n => n.Doors);
	var rooms = result.Where(n => n.Doors >= 1000).Count();
	Console.WriteLine($"Largest number of doors: {maxDoors}");
	Console.WriteLine($"{rooms} rooms pass at least 1000 doors.");
}

static List<((int X, int Y), int Doors)> BFS(((int, int) Pos, int) startNode, Dictionary<(int, int), char> map)
{
	var traversedNodes = new List<((int, int), int)>
	{
		startNode
	};
	var visitedNodes = new HashSet<(int, int)>
	{
		startNode.Pos
	};
	var queue = new LinkedList<((int, int), int Doors)>();
	queue.AddLast(startNode);
	while (queue.Any())
	{
		startNode = queue.First.Value;
		queue.RemoveFirst();
		foreach (var n in GetAdjacentNodes(startNode.Pos, map))
		{
			if (!visitedNodes.Contains(n))
			{
				traversedNodes.Add((n, startNode.Item2 + 1));
				visitedNodes.Add(n);
				queue.AddLast((n, startNode.Item2 + 1));
			}
		}
	}
	return traversedNodes;
}

static IEnumerable<(int, int)> GetAdjacentNodes((int X, int Y) node, Dictionary<(int, int), char> map)
{
	if (CheckNode((node.X - 1, node.Y), (node.X - 2, node.Y), map)) yield return (node.X - 2, node.Y);
	if (CheckNode((node.X + 1, node.Y), (node.X + 2, node.Y), map)) yield return (node.X + 2, node.Y);
	if (CheckNode((node.X, node.Y - 1), (node.X, node.Y - 2), map)) yield return (node.X, node.Y - 2);
	if (CheckNode((node.X, node.Y + 1), (node.X, node.Y + 2), map)) yield return (node.X, node.Y + 2);
}

static bool CheckNode((int, int) n1, (int, int) n2, Dictionary<(int, int), char> map)
{
	return
		map.ContainsKey(n1) && (map[n1] == '|' || map[n1] == '-') &&
		map.ContainsKey(n2) && map[n2] == '.';
}

static void Wander(ref int x, ref int y, int i, List<Step> steps, Dictionary<(int, int), char> map, HashSet<string> cache)
{
	var fingerPrint = FingerPrint(x, y, i, steps);
	if (cache.Contains(fingerPrint))
	{
		return;
	}
	cache.Add(fingerPrint);


	while (i < steps.Count)
	{
		var step = steps[i++];
		switch (step.Kind)
		{
			case StepKind.Start:
				map[(x, y)] = 'X';
				break;

			case StepKind.W:
				x--;
				map[(x, y)] = '|';
				x--;
				map[(x, y)] = '.';
				break;

			case StepKind.E:
				x++;
				map[(x, y)] = '|';
				x++;
				map[(x, y)] = '.';
				break;

			case StepKind.N:
				y--;
				map[(x, y)] = '-';
				y--;
				map[(x, y)] = '.';
				break;

			case StepKind.S:
				y++;
				map[(x, y)] = '-';
				y++;
				map[(x, y)] = '.';
				break;

			case StepKind.Options:
				foreach (var o in step.Options)
				{
					var xx = x;
					var yy = y;
					Wander(ref xx, ref yy, 0, o, map, cache);
					Wander(ref xx, ref yy, i, steps, map, cache);
				}
				return; // done

			case StepKind.End:
				break;

			default:
				throw new InvalidOperationException($"{step}");
		}
	}
}

static string FingerPrint(int x, int y, int i, List<Step> steps)
{
	return $"{x}|{y}|{i}|{string.Join("", steps)}";
}

static void Print(Dictionary<(int X, int Y), char> map)
{
	var minX = map.Keys.Min(k => k.X) - 1;
	var maxX = map.Keys.Max(k => k.X) + 1;
	var minY = map.Keys.Min(k => k.Y) - 1;
	var maxY = map.Keys.Max(k => k.Y) + 1;
	for (var y = minY; y <= maxY; y++)
	{
		for (var x = minX; x <= maxX; x++)
		{
			var p = (x, y);
			var c = '#';
			if (map.ContainsKey(p))
			{
				c = map[p];
			}
			Console.Write(c);
		}
		Console.WriteLine();
	}
}

static Step ParseStep(string text, ref int pos)
{
	var c = text[pos - 1];
	return c switch
	{
		'N' => Step.N,
		'S' => Step.S,
		'W' => Step.W,
		'E' => Step.E,
		'(' => new Step(ParseOptions(text, ref pos)),
		_ => throw new InvalidOperationException($"Unexpected token {c}"),
	};
}

static List<List<Step>> ParseOptions(string text, ref int pos)
{
	var options = new List<List<Step>>
	{
		new()
	};
	var c = text[pos++];
	while (c != ')')
	{
		if (c == '|')
		{
			options.Add(new());
		}
		else
		{
			options.Last().Add(ParseStep(text, ref pos));
		}
		c = text[pos++];
	}
	return options;
}

static List<Step> Parse(string text, ref int pos)
{	
	var steps = new List<Step>();
	while (pos < text.Length)
	{
		var c = text[pos++];
		steps.Add(c switch
		{
			'^' => Step.Start,
			'$' => Step.End,
			_ => ParseStep(text, ref pos),
		});
	}
	return steps;
}

static List<Step> ReadInput()
{
	var text = File.ReadAllText("input.txt");
	var pos = 0;
	return Parse(text, ref pos);
}

internal class Step
{
	private Step(StepKind kind)
	{
		Kind = kind;
	}
	internal Step(List<List<Step>> options) : this(StepKind.Options)
	{
		Options = options;
	}

	public static Step Start { get; } = new Step(StepKind.Start);
	public static Step End { get; } = new Step(StepKind.End);
	public static Step W { get; } = new Step(StepKind.W);
	public static Step N { get; } = new Step(StepKind.N);
	public static Step S { get; } = new Step(StepKind.S);
	public static Step E { get; } = new Step(StepKind.E);
	public StepKind Kind { get; }
	public List<List<Step>> Options { get; }

	public override string ToString()
	{
		return $"{Kind}";
	}
}

internal enum StepKind
{
	Start,
	End,
	W,
	N,
	E,
	Options,
	S
}
