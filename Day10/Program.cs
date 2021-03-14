using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

// Day 10

Console.WriteLine("START");
var sw = Stopwatch.StartNew();
Part1();
Console.WriteLine($"END (after {sw.Elapsed.TotalSeconds} seconds)");

void Part1()
{
	var particles = ReadInput().ToList();
  var time = 1;
	while (time < 150_000)
	{
		foreach (var p in particles)
		{
			p.Update();
		}

		var maxX = particles.Max(p => p.X);
		var maxY = particles.Max(p => p.Y);
		var minX = particles.Min(p => p.X);
		var minY = particles.Min(p => p.Y);

		var w = maxX - minX;
		var h = maxY - minY;
		if (maxX - minX < 100 && maxY - minY < 12)
		{
			var display = new bool[h + 1, w + 1];
			foreach (var p in particles)
			{
				display[p.Y - minY, p.X - minX] = true;
			}
			Render(display, time);
		}
		time++;
	}
}

void Render(bool[,] display, int time)
{
	System.Console.WriteLine($"Time: {time}");
	var h = display.GetLength(0);
	var w = display.GetLength(1);
	for (var y = 0; y < h; y++)
	{
		for (var x = 0; x < w; x++)
		{
			System.Console.Write(display[y, x] ? '#' : '.');
		}
		System.Console.WriteLine();
	}
	System.Console.WriteLine();
}

static IEnumerable<Particle> ReadInput()
{
	foreach (var line in File.ReadAllLines("input.txt"))
	{
		var tokens = line.Split("> ");
		var tokens2 = tokens[0].Split('=');
		var tokens3 = tokens2[1][1..].Split(',', StringSplitOptions.RemoveEmptyEntries);
		var x = int.Parse(tokens3[0]);
		var y = int.Parse(tokens3[1]);

		tokens2 = tokens[1].Split('=');
		tokens3 = tokens2[1][1..^1].Split(',', StringSplitOptions.RemoveEmptyEntries);
		var velocity = (int.Parse(tokens3[0]), int.Parse(tokens3[1]));

		yield return new Particle
		{
			X = x,
			Y = y,
			Velocity = velocity,
		};
	}
}

internal class Particle
{
	internal int X { get; set; }
	internal int Y { get; set; }

	internal (int x, int y) Velocity { get; init; }

	internal void Update()
	{
		X += Velocity.x;
		Y += Velocity.y;
	}
}