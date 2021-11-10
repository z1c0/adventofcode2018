using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

Console.WriteLine("Day 24 - START");
var sw = Stopwatch.StartNew();
Part1();
Part2();
Console.WriteLine($"END (after {sw.Elapsed.TotalSeconds} seconds)");

static void Part1()
{
	Simulate(0);
}

static void Part2()
{
	var boost = 31;  // binary searched
	if (Simulate(boost))
	{
		Console.WriteLine($"Immune system wins with a boost of {boost}.");
	}
}

static bool Simulate(int immuneBoost)
{
	var (immuneSystem, infection) = ReadInput();

	foreach (var g in immuneSystem.Groups)
	{
		g.AttackDamage += immuneBoost;
	}

	var round = 1;
	while (immuneSystem.IsAlive && infection.IsAlive)
	{
		//Print(round++, immuneSystem, infection);

		var pairings = SelectTargets(infection.Groups, immuneSystem.Groups).Concat(SelectTargets(immuneSystem.Groups, infection.Groups))
			.OrderByDescending(p => p.Attacker.Initiative)
			.ToList();
		foreach (var (attacker, defender) in pairings)
		{
			attacker.Attack(defender);
		}
	}

	Print(round++, immuneSystem, infection);
	var units = immuneSystem.IsAlive ? immuneSystem.TotalUnits : infection.TotalUnits;
	Console.WriteLine($"The winning army ends up with {units} units.");
	
	return immuneSystem.IsAlive;
}

static IEnumerable<(Group Attacker, Group Defender)> SelectTargets(IEnumerable<Group> groups1, IEnumerable<Group> groups2)
{
	var attackers = groups1.Where(g => g.IsAlive)
		.OrderByDescending(g => g.EffectivePower).ThenByDescending(g => g.Initiative).ToList();
	var defenders = groups2.Where(g => g.IsAlive).ToList();
	foreach (var a in attackers)
	{
		var victims = new List<(int Damage, Group Group)>();
		foreach (var d in defenders)
		{
			var damage = a.AssessDamageTo(d);
			victims.Add((damage, d));
		}
		var victim = victims.OrderByDescending(a => a.Damage)
			.ThenByDescending(a => a.Group.EffectivePower)
			.ThenByDescending(a => a.Group.Initiative).FirstOrDefault();
		if (victim.Damage > 0 && victim.Group != null)
		{
			defenders.Remove(victim.Group);
			yield return(a, victim.Group);
		}
	}
}

static void Print(int round, Army immuneSystem, Army infection)
{
	Console.WriteLine($"Round {round}:");
	Console.WriteLine($"----------");
	Console.WriteLine(immuneSystem);
	Console.WriteLine(infection);
	Console.WriteLine();
}

static (Army immuneSystem, Army infection) ReadInput()
{
	static AttackType ParseAttackType(string s)
	{
		return s switch
		{
			"bludgeoning" => AttackType.Bludgeoning,
			"fire" => AttackType.Fire,
			"slashing" => AttackType.Slashing,
			"radiation" => AttackType.Radiation,
			"cold" => AttackType.Cold,
			_ => throw new InvalidOperationException(s),
		};
	}

	static (List<AttackType> weaknesses, List<AttackType> immunities) ParseWeaknessAndImmunities(string[] tokens, ref int i)
	{
		var weaknesses = new List<AttackType>();
		var immunities = new List<AttackType>();

		if (tokens[i].StartsWith('('))
		{
			var line = tokens[i++][1..];
			var isWeakness = line == "weak";
			Debug.Assert(tokens[i++] == "to");
			while (true)
			{
				line = tokens[i++];
				var c = line.Last();
				line = line[..^1];
				var attackType = ParseAttackType(line);
				if (isWeakness)
				{
					weaknesses.Add(attackType);
				}
				else
				{
					immunities.Add(attackType);
				}

				if (c == ')')
				{
					break;
				}
				else if (c == ';')
				{
					isWeakness = !isWeakness;
					i++;
					Debug.Assert(tokens[i++] == "to");
				}
				else
				{
					Debug.Assert(c == ',');
				}
			}
		}

		return (weaknesses, immunities);
	}

	static Army ParseArmy(string[] lines, ref int pos)
	{
		var name = lines[pos++][0..^1];

		var groups = new List<Group>();
		var line = lines[pos++];
		while (!string.IsNullOrEmpty(line))
		{
			var i = 0;
			var tokens = line.Split(' ');
			var units = int.Parse(tokens[i++]);
			Debug.Assert(tokens[i++] == "units");

			Debug.Assert(tokens[i++] == "each");
			Debug.Assert(tokens[i++] == "with");
			var hitPoints = int.Parse(tokens[i++]);
			Debug.Assert(tokens[i++] == "hit");
			Debug.Assert(tokens[i++] == "points");

			var (weaknesses, immunities) = ParseWeaknessAndImmunities(tokens, ref i);

			Debug.Assert(tokens[i++] == "with");
			Debug.Assert(tokens[i++] == "an");
			Debug.Assert(tokens[i++] == "attack");
			Debug.Assert(tokens[i++] == "that");
			Debug.Assert(tokens[i++] == "does");
			var attackDamage = int.Parse(tokens[i++]);
			var attackType = ParseAttackType(tokens[i++]);
			Debug.Assert(tokens[i++].StartsWith("damage"));

			Debug.Assert(tokens[i++] == "at");
			Debug.Assert(tokens[i++] == "initiative");
			var initiative = int.Parse(tokens[i++]);

			var groupName = $"Group #{groups.Count + 1}";
			groups.Add(new Group(groupName, units, hitPoints, weaknesses, immunities, attackDamage, attackType, initiative));
			line = lines[pos++];
		}

		return new Army(name, groups);
	}

	var pos = 0;
	var lines = File.ReadAllLines("input.txt");
	return (ParseArmy(lines, ref pos), ParseArmy(lines, ref pos));
}

internal record Army(string Name, List<Group> Groups)
{
	public override string ToString()
	{
		var sb = new StringBuilder();
		sb.AppendLine($"{Name}:");
		foreach (var g in Groups.Where(g => g.IsAlive))
		{
			sb.AppendLine(g.ToString());
		}
		return sb.ToString();
	}

	internal bool IsAlive { get => Groups.Any(g => g.IsAlive); }

	internal int TotalUnits { get => Groups.Sum(g => g.Units); }
}

internal enum AttackType
{
	Bludgeoning,
	Fire,
	Slashing,
	Radiation,
	Cold
}
internal record Group(string Name, int Units, int HitPoints, List<AttackType> Weaknesses, List<AttackType> Immunities, int AttackDamage, AttackType AttackType, int Initiative)
{
	public override string ToString()
	{
		return $"{Name} contains {Units} units.";
	}

	internal bool IsAlive { get => Units > 0; }

	internal int Units { get; set; } = Units;

	internal int AttackDamage { get; set; } = AttackDamage;

	internal int AssessDamageTo(Group other)
	{
		var damage = EffectivePower;
		if (other.Immunities.Contains(AttackType))
		{
			damage = 0;
		}
		else if (other.Weaknesses.Contains(AttackType))
		{
			damage *= 2;
		}
		return damage;
	}

	internal void Attack(Group defender)
	{
		var damage = AssessDamageTo(defender);
		var unitDamage = damage / defender.HitPoints;
		defender.Units = Math.Max(0, defender.Units - unitDamage);
	}

	internal int EffectivePower { get => Units * AttackDamage; }
}