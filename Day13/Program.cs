using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

// Day 13

Console.WriteLine("START");
var sw = Stopwatch.StartNew();
Part1();
Part2();
Console.WriteLine($"END (after {sw.Elapsed.TotalSeconds} seconds)");

void Part1()
{
	var map = ReadInput();
	var carts = FindCarts(map).ToList();
	//Print(map, carts);
	var crash = false;
	while (!crash)
	{
		carts.Sort();
		foreach (var c in carts)
		{
			c.Update();
			if (carts.Any(cc => c != cc && c.X == cc.X && c.Y == cc.Y))
			{
				System.Console.WriteLine($"Location of the first crash: {c}");
				crash = true;
				break;
			}
		}
		//Print(map, carts);
	}
}

void Part2()
{
	var map = ReadInput();
	var carts = FindCarts(map).ToList();
	while (carts.Count > 1)
	{
		carts.Sort();
		var removals = new List<Cart>();
		foreach (var c in carts)
		{
			c.Update();
			var cc = carts.SingleOrDefault(cc => c != cc && c.X == cc.X && c.Y == cc.Y);
			if (cc != null)
			{
				removals.Add(c);
				removals.Add(cc);
			}
		}
		foreach (var c in removals)
		{
			carts.Remove(c);
		}
	}
	System.Console.WriteLine($"The location of the last car is {carts.Single()}");
}


IEnumerable<Cart> FindCarts(char[,] map)
{
	var h = map.GetLength(0);
	var w = map.GetLength(1);
	for (var y = 0; y < h; y++)
	{
		for (var x = 0; x < w; x++)
		{
			var c = map[y, x];
			if (c == '>' || c == '<' || c == '^' || c == 'v')
			{
				yield return new Cart(c, x, y, map);
			}
		}
	}
}

void Print(char[,] map, List<Cart> carts)
{
	var h = map.GetLength(0);
	var w = map.GetLength(1);
	for (var y = 0; y < h; y++)
	{
		for (var x = 0; x < w; x++)
		{
			var c = carts.Where(c => c.X == x && c.Y == y);
			System.Console.Write(c.Count() > 1 ? 'X' : c.Any() ? c.First().C : map[y, x]);
		}
		System.Console.WriteLine();
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

internal class Cart : IComparable
{
	public Cart(char c, int x, int y, char[,] map)
	{
		C = c;
		X = x;
		Y = y;
		_map = map;
		_numberOfTurns = 0;

		if (c == '<' || c == '>')
		{
			map[y, x] = '-';
		}
		else
		{
			map[y, x] = '|';
		}
	}

	public char C { get; private set; }
	public int X { get; private set; }
	public int Y { get; private set; }

	private char[,] _map;
	private int _numberOfTurns;

	public override string ToString()
	{
		return $"{X},{Y}";
	}

	public int CompareTo(object obj)
	{
		var other = (Cart)obj;
		if (Y < other.Y)
		{
			return -1;
		}
		if (Y > other.Y)
		{
			return 1;
		}
		return X.CompareTo(other.X);
	}

	internal void Update()
	{
		switch (C)
		{
			case '>':
				X++;
				if (_map[Y, X] == '/')
				{
					C = '^';
				}
				else if (_map[Y, X] == '\\')
				{
					C = 'v';
				}
				break;

			case '<':
				X--;
				if (_map[Y, X] == '/')
				{
					C = 'v';
				}
				else if (_map[Y, X] == '\\')
				{
					C = '^';
				}
				break;

			case 'v':
				Y++;
				if (_map[Y, X] == '/')
				{
					C = '<';
				}
				else if (_map[Y, X] == '\\')
				{
					C = '>';
				}
				break;

			case '^':
				Y--;
				if (_map[Y, X] == '/')
				{
					C = '>';
				}
				else if (_map[Y, X] == '\\')
				{
					C = '<';
				}
				break;

			default:
				throw new InvalidOperationException();
		}
		// turn
		if (_map[Y, X] == '+')
		{
			if (_numberOfTurns == 0)
			{
				// left
				if (C == 'v')
				{
					C = '>';
				}
				else if (C == '>')
				{
					C = '^';
				}
				else if (C == '^')
				{
					C = '<';
				}
				else if (C == '<')
				{
					C = 'v';
				}
			}
			else if (_numberOfTurns == 1)
			{
				// straight
			}
			else if (_numberOfTurns == 2)
			{
				// right
				if (C == 'v')
				{
					C = '<';
				}
				else if (C == '>')
				{
					C = 'v';
				}
				else if (C == '^')
				{
					C = '>';
				}
				else if (C == '<')
				{
					C = '^';
				}
			}
			_numberOfTurns = (_numberOfTurns + 1) % 3;
		}
	}
}
