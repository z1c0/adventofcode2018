using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

Console.WriteLine("Day 21 -START");
var sw = Stopwatch.StartNew();
Part1();
Part1_Rewritten();

static void Part1_Rewritten()
{
	//
	// Rewritten code simplified and extended with cache and output.
	//
	var cache = new HashSet<int>();
	var a = 0;	
	var e = 0;
  
L_6:
	var f = e | 65536;
	e = 1765573;

L_8:
	var b = f & 255;
	e += b;
	e &= 16777215;
	e *= 65899;
	e &= 16777215;
	if (256 > f)
	{
		goto L_28;
	}	
	b = 0;

L_18:
	var d = b + 1;
	d *= 256;
	if (d > f)
	{		
		goto L_26;
	}
	b++;
	goto L_18;

L_26:
	f = b;
	goto L_8;

L_28:
	if (!cache.Contains(e))
	{
		//
		// First entry is the lower bound, last entry the upper bound.
		//
		Console.WriteLine($"Register 'a' should be: {e}");
		cache.Add(e);
	}
	if (e == a)
	{
		return;
	}
	goto L_6;
}

Console.WriteLine($"END (after {sw.Elapsed.TotalSeconds} seconds)");

static void Part1()
{
	var program = ReadInput().ToList();
	var cpu = new Cpu(72);
	cpu.Init(program.First());
	program.RemoveAt(0);
	Rewrite(program);
	//cpu.Execute(program.ToList());
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
			"eqri" => OpCode.Eqri,
			"gtrr" => OpCode.Gtrr,
			"gtir" => OpCode.Gtir,
			"bani" => OpCode.Bani,
			"bori" => OpCode.Bori,
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
			case OpCode.Eqri:
				return $"{R(C)} = {R(A)} == {B} ? 1 : 0;";
			case OpCode.Eqrr:
				return $"{R(C)} = {R(A)} == {R(B)} ? 1 : 0;";
			case OpCode.Gtir:
				return $"{R(C)} = {A} > {R(B)} ? 1 : 0;";
			case OpCode.Gtrr:
				return $"{R(C)} = {R(A)} > {R(B)} ? 1 : 0;";
			case OpCode.Bani:
				return $"{R(C)} = {R(A)} & {B};";
			case OpCode.Bori:
				return $"{R(C)} = {R(A)} | {B};";
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
			Console.WriteLine($"{this}  -  {instruction}");
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