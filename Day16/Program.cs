using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

// Day 16

Console.WriteLine("START");
var sw = Stopwatch.StartNew();
Part1();
Console.WriteLine($"END (after {sw.Elapsed.TotalSeconds} seconds)");

static void Part1()
{
	var opCodeMap = new Dictionary<int, List<OpCode>>();
	var count = 0;
	var cpu = new Cpu();
	var input = ReadInput();
	foreach (var (registersBefore, instruction, registersAfter) in input.samples)
	{
		var opCodeMatches = 0;
		foreach (var o in Enum.GetValues(typeof(OpCode)).Cast<OpCode>())
		{
			cpu.Init(registersBefore);
			cpu.Execute(o, instruction[1], instruction[2], instruction[3]);
			if (cpu.CheckRegisters(registersAfter))
			{
				opCodeMatches++;
				var i = instruction[0];
				opCodeMap.TryAdd(i, new List<OpCode>());
				if (!opCodeMap[i].Contains(o))
				{
					opCodeMap[i].Add(o);
				}
			}
		}
		if (opCodeMatches >= 3)
		{
			count++;
		}
	}
	Console.WriteLine($"{count} samples match 3 or more opcodes.");

	// Part2
	while (opCodeMap.Values.Any(l => l.Count > 1))
	{
		var mapped = opCodeMap.Values.Where(l => l.Count == 1).Select(l => l.Single()).ToList();
		foreach (var m in mapped)
		{
			foreach (var list in opCodeMap.Values.Where(l => l.Count > 1))
			{
				list.Remove(m);
			}
		}
	}
	cpu.Init();
	foreach (var i in input.program)
	{
		var op = opCodeMap[i[0]].Single();
		cpu.Execute(op, i[1], i[2], i[3]);
	}
	System.Console.WriteLine(cpu);
}

static (List<(int[] registersBefore, int[] instruction, int[] registersAfter)> samples, List<int[]> program) ReadInput()
{
	var samples = new List<(int[], int[], int[])>();
	var program = new List<int[]>();
	var lines = File.ReadAllLines("input.txt");
	var i = 0;
	while (true)
	{
		if (lines[i].StartsWith("Before:"))
		{
			var before = lines[i][9..^1].Split(", ").Select(t => int.Parse(t)).ToArray();
			var instruction = lines[i + 1].Split(' ').Select(t => int.Parse(t)).ToArray();
			var after = lines[i + 2][9..^1].Split(", ").Select(t => int.Parse(t)).ToArray();
			samples.Add((before, instruction, after));
			i+= 4;
		}
		else
		{
			while (i < lines.Length)
			{
				var line = lines[i];
				if (!string.IsNullOrEmpty(line))
				{
					program.Add(line.Split(' ').Select(t => int.Parse(t)).ToArray());
				}
				i++;
			}
			break;
		}
	}
	return (samples, program);
}

internal enum OpCode
{
	Addr,
	Addi,
	Mulr,
	Muli,
	Banr,
	Bani,
	Borr,
	Bori,
	Setr,
	Seti,
	Gtir,
	Gtri,
	Gtrr,
	Eqir,
	Eqri,
	Eqrr,
	
}

internal class Cpu
{
	private readonly int[] _reg = new int[4];
	internal void Execute(OpCode o, int A, int B, int C)
	{
		switch (o)
		{
			case OpCode.Addr:
				_reg[C] = _reg[A] + _reg[B];
				break;
			case OpCode.Addi:
				_reg[C] = _reg[A] + B;
				break;
			case OpCode.Mulr:
				_reg[C] = _reg[A] * _reg[B];
				break;
			case OpCode.Muli:
				_reg[C] = _reg[A] * B;
				break;
			case OpCode.Banr:
				_reg[C] = _reg[A] & _reg[B];
				break;
			case OpCode.Bani:
				_reg[C] = _reg[A] & B;
				break;
			case OpCode.Borr:
				_reg[C] = _reg[A] | _reg[B];
				break;
			case OpCode.Bori:
				_reg[C] = _reg[A] | B;
				break;
			case OpCode.Setr:
				_reg[C] = _reg[A];
				break;
			case OpCode.Seti:
				_reg[C] = A;
				break;
			case OpCode.Gtir:
				_reg[C] = A > _reg[B] ? 1 : 0;
				break;
			case OpCode.Gtri:
				_reg[C] = _reg[A] > B ? 1 : 0;
				break;
			case OpCode.Gtrr:
				_reg[C] = _reg[A] > _reg[B] ? 1 : 0;
				break;
			case OpCode.Eqir:
				_reg[C] = A == _reg[B] ? 1 : 0;
				break;
			case OpCode.Eqri:
				_reg[C] = _reg[A] == B ? 1 : 0;
				break;
			case OpCode.Eqrr:
				_reg[C] = _reg[A] == _reg[B] ? 1 : 0;
				break;
			default:
				throw new NotImplementedException();
		}
	}

	internal void Init(int[] registers = null)
	{
		_reg[0] = registers != null ? registers[0] : 0;
		_reg[1] = registers != null ? registers[1] : 0;
		_reg[2] = registers != null ? registers[2] : 0;
		_reg[3] = registers != null ? registers[3] : 0;
	}
	internal bool CheckRegisters(int[] registers)
	{
		return 
			_reg[0] == registers[0] &&
			_reg[1] == registers[1] &&
			_reg[2] == registers[2] &&
			_reg[3] == registers[3];
	}

	public override string ToString()
	{
		return $"[{_reg[0]}, {_reg[1]}, {_reg[2]}, {_reg[3]}]";
	}
}
