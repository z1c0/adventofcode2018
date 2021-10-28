using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

Console.WriteLine("Day 19 -START");
var sw = Stopwatch.StartNew();
Part1(0);
Part1(1);
Console.WriteLine($"END (after {sw.Elapsed.TotalSeconds} seconds)");

static void Part1(int register0)
{
	var program = ReadInput();
	var cpu = new Cpu(register0);
	cpu.Init(program.First());
	cpu.Execute(program.Skip(1).ToList());
	Console.WriteLine(cpu);
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

internal record Instruction(OpCode OpCode, int A, int B, int C);

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
			Execute(program[_reg[_ipRegister]]);
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