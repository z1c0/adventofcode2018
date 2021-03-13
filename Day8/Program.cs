using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

// Day 8

Console.WriteLine("START");
var sw = Stopwatch.StartNew();
Part1();
Console.WriteLine($"END (after {sw.Elapsed.TotalSeconds} seconds)");

void Part1()
{
	var numbers = ReadInput().ToList();
	var pos = 0;
	var node = ParseNode(numbers, ref pos);
	System.Console.WriteLine($"Sum of metadata: {node.GetMetaSum()}");
	// Part 2
	System.Console.WriteLine($"Value of A: {node.GetValue()}");
}

Node ParseNode(List<int> numbers, ref int pos)
{
	var childCount = numbers[pos++];
	var metaCount = numbers[pos++];
	var node = new Node();
	for (var i = 0; i < childCount; i++)
	{
		node.Children.Add(ParseNode(numbers, ref pos));
	}
	for (var i = 0; i < metaCount; i++)
	{
		node.Meta.Add(numbers[pos++]);
	}
	return node;
}

static IEnumerable<int> ReadInput()
{
	foreach (var n in File.ReadAllText("input.txt").Split(' '))
	{
		yield return int.Parse(n);
	}
}

internal class Node
{
	internal List<Node> Children { get; } = new List<Node>();
	internal List<int> Meta { get; } = new List<int>();

	internal int GetMetaSum()
	{
		var metaSum = 0;
		foreach (var c in Children)
		{
			metaSum += c.GetMetaSum();
		}
		foreach (var m in Meta)
		{
			metaSum += m;
		}
		return metaSum;
	}

	internal int GetValue()
	{
		var value = 0;
		if (!Children.Any())
		{
			foreach (var m in Meta)
			{
				value += m;
			}
		}
		else
		{
			foreach (var m in Meta)
			{
				var c = Children.ElementAtOrDefault(m - 1);
				if (c != null)
				{
					value += c.GetValue();
				}
			}
		}
		return value;
	}
}