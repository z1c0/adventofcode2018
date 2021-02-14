using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

// Day 4

Console.WriteLine("START");
var sw = Stopwatch.StartNew();
Part1();
Console.WriteLine($"END (after {sw.Elapsed.TotalSeconds} seconds)");

void Part1()
{
	var logs = ReadInput().OrderBy(l => l.dt) .ToList();
	var sleepLog = new Dictionary<int, Dictionary<int, int>>();
	var id = 0;
	var startSleepingAt = 0;
	foreach (var log in logs)
	{
		if (log.evt == Event.Begin)
		{
			id = log.id;
		}
		else if (log.evt == Event.FallAsleep)
		{
			startSleepingAt = log.dt.Minute;
		}
		else if (log.evt == Event.WakeUp)
		{
			for (var i = startSleepingAt; i < log.dt.Minute; i++)
			{
				sleepLog.TryAdd(id, new Dictionary<int, int>());
				sleepLog[id].TryAdd(i, 0);
				sleepLog[id][i]++;
			}
		}
	}

	var max = 0;
	var maxId = 0;
	foreach (var l in sleepLog)
	{
		var tmp = 0;
		foreach (var m in l.Value.Values)
		{
			tmp += m;
		}
		if (tmp > max)
		{
			max = tmp;
			maxId = l.Key;
		}
	}

	max = sleepLog[maxId].Values.Max();
	var sleepiestMinute = sleepLog[maxId].Single(e => e.Value == max).Key;
	System.Console.WriteLine($"{maxId} * {sleepiestMinute} = {maxId * sleepiestMinute}");

	// Part 2
	var maxSleepsPerMinute = 0;
	foreach (var l in sleepLog)
	{
		max = l.Value.Values.Max();
		if (max > maxSleepsPerMinute)
		{
			maxSleepsPerMinute = max;
			sleepiestMinute = l.Value.First(e => e.Value == max).Key;
			maxId = l.Key;
		}
	}
	System.Console.WriteLine($"{maxId} * {sleepiestMinute} = {maxId * sleepiestMinute}");
}

IEnumerable<(DateTime dt, int id, Event evt)> ReadInput()
{
	foreach (var line in File.ReadAllLines("input.txt"))
	{
		var dt = DateTime.Parse(line[1..17]);
		var id = -1;
		var evt = default(Event);
		var rest = line[19..];
		if (rest == "falls asleep")
		{
			evt = Event.FallAsleep;
		}
		else if (rest == "wakes up")
		{
			evt = Event.WakeUp;	
		}
		else if (rest.StartsWith("Guard"))
		{
			var tokens = rest.Split(' ');
			id = int.Parse(tokens[1][1..]);
		}
		else
		{
			throw new InvalidProgramException();
		}
		yield return (dt, id, evt);
	}
}

enum Event
{
	Begin,
	WakeUp,
	FallAsleep,
}