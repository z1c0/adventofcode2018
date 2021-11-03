using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

Console.WriteLine("Day 19 -START");
var sw = Stopwatch.StartNew();
Part1(0);
//Part1(1);  // This would not terminate.
Part2();
Console.WriteLine($"END (after {sw.Elapsed.TotalSeconds} seconds)");

static void Part1(int register0)
{
	var program = ReadInput().ToList();
	var cpu = new Cpu(register0);
	cpu.Init(program.First());
	program.RemoveAt(0);
	//Rewrite(program);
	cpu.Execute(program.ToList());
	Console.WriteLine(cpu);
}

static void Part2()
{
	//
	// The "disassembled code" (using Rewrite and manual)
	// turned out to calculate pairs of numbers that add up to 10551340
	// and sum up the first of those.
	// This can be simplified to the snippet below.
	//
	var a = 0;
	var c = 10551340;
	for (var i = 1; i <= c; i++)
	{
		if (c % i == 0)
		{
			a += i;
		}
	}

	/*
	long a = 0;
	long b = 0;
	long c = 10551340;
	long e = 1;
	do
	{
		b = 1;

		do
		{
			if (e * b == c)
			{
				a += e;
				System.Console.WriteLine($"{e} * {b} = {e * b}");
			}
			b++;
		}
		while (b <= c);

		e++;
	}
	while (e <= c);
	*/
	Console.WriteLine("a: " + a);
}

static void Rewrite(List<Instruction> program)
{
	var sb = new StringBuilder();
	sb.AppendLine("int a = 1, b = 0, c = 0, d = 0, e = 0, f = 0;");
	for (var i = 0; i < program.Count; i++)
	{
		var instruction = program[i];
		sb.AppendLine($"/* {i} */ {instruction.Rewrite()}  /* {instruction} */");
	}
	Console.WriteLine(sb);
}

static IEnumerable<Instruction> ReadInput()
{
	foreach (var line in File.ReadAllLines("input.txt"))
	{
		var tokens = line.Split(' ');
		var opCode = tokens[0] switch {
			"#ip" => OpCode.BindIp,
			"seti" => OpCode.Seti,
			"setr" => OpCode.Setr,
			"addi" => OpCode.Addi,
			"addr" => OpCode.Addr,
			"mulr" => OpCode.Mulr,
			"muli" => OpCode.Muli,
			"eqrr" => OpCode.Eqrr,
			"gtrr" => OpCode.Gtrr,
			_ => throw new InvalidProgramException(tokens[0]),
		};
		var v1 = int.Parse(tokens[1]);
		var v2 = tokens.Length > 2 ? int.Parse(tokens[2]) : -1;
		var v3 = tokens.Length > 3 ? int.Parse(tokens[3]) : -1;
		yield return new Instruction(opCode, v1, v2, v3);
	}
}

internal record Instruction(OpCode OpCode, int A, int B, int C)
{
	public override string ToString()
	{
		return $"{OpCode} {A} {B} {C}";
	}

	internal string Rewrite()
	{
		char R(int r)
		{
			return r switch
			{
				0 => 'a',
				1 => 'b',
				2 => 'c',
				3 => 'd',
				4 => 'e',
				5 => 'f',
				_ => throw new NotImplementedException()
			};
		}
		switch (OpCode)
		{
			case OpCode.Addi:
				return $"{R(C)} = {R(A)} + {B};";
			case OpCode.Addr:
				return $"{R(C)} = {R(A)} + {R(B)};";
			case OpCode.Seti:
				return $"{R(C)} = {A};";
			case OpCode.Setr:
				return $"{R(C)} = {R(A)};";
			case OpCode.Muli:
				return $"{R(C)} = {R(A)} * {B};";
			case OpCode.Mulr:
				return $"{R(C)} = {R(A)} * {R(B)};";
			case OpCode.Eqrr:
				return $"{R(C)} = {R(A)} == {R(B)} ? 1 : 0;";
			case OpCode.Gtrr:
				return $"{R(C)} = {R(A)} > {R(B)} ? 1 : 0;";
			default:
				throw new NotImplementedException($"[{this}]");
		}
	}
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
	BindIp,
}

internal class Cpu
{
	private int _ipRegister = 0;
	private readonly int[] _reg = new int[6];

	public Cpu(int register0)
	{
		_reg[0] = register0;
	}

	internal void Execute(List<Instruction> program)
	{
		while (_reg[_ipRegister] < program.Count)
		{
			var instruction = program[_reg[_ipRegister]];
			//Console.WriteLine($"{this}  -  {instruction}");
			Execute(instruction);
		}
	}

	private void Execute(Instruction instruction)
	{
		var A = instruction.A;
		var B = instruction.B;
		var C = instruction.C;
		switch (instruction.OpCode)
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
		_reg[_ipRegister]++;
	}

	public override string ToString()
	{
		var s = string.Join(", ", _reg);
		return $"ip={_reg[_ipRegister]} [{s})]";
	}

	internal void Init(Instruction instruction)
	{
		if (instruction.OpCode != OpCode.BindIp)
		{
			throw new InvalidOperationException();
		}
		_ipRegister = instruction.A;
	}
}