namespace Advent2015;

public static class Day22
{
    public static void Run(string[] data)
    {
        /*
            Apparently this is solvable but I got the most difficult input.
            This was the most difficult one I've worked on so far. My mind just
            wouldn't work with it. The order of events is super important and
            the text on the page isn't necessarily how it's implemented

            https://www.reddit.com/r/adventofcode/comments/3xspyl/day_22_solutions/cy86y2x/
            boss = { hp: 71, damage: 10 }
            player = { hp: 50, mana: 500 }

            hard: 1937
            * Shield -> Recharge -> Poison -> Shield -> Recharge -> Poison -> Shield -> Recharge -> Poison -> Shield -> Magic Missile -> Poison -> Magic Missile

            easy: 1824
            * Poison -> Recharge -> Shield -> Poison -> Recharge -> Shield -> Poison -> Recharge -> Shield -> Magic Missile -> Poison -> Magic Missile
            * Poison -> Recharge -> Shield -> Poison -> Recharge -> Shield -> Poison -> Recharge -> Shield -> Poison -> Magic Missile -> Magic Missile
            * Recharge -> Poison -> Shield -> Recharge -> Poison -> Shield -> Recharge -> Poison -> Shield -> Magic Missile -> Poison -> Magic Missile

            I can't cast spells at random and hope I get the right combination like other solutions can
        */
        var minMana = int.MaxValue;

        var playerSpells = new List<Spell>{
                new Spell{ Name = "Magic Missile", Cost = 53,  Effect = new Effect{             Damage = 4 } },
                new Spell{ Name = "Drain",         Cost = 73,  Effect = new Effect{ Damage = 2, Heal = 2   } },
                new Spell{ Name = "Shield",        Cost = 113, Effect = new Effect{ Time = 6,   Armor = 7  } },
                new Spell{ Name = "Poison",        Cost = 173, Effect = new Effect{ Time = 6,   Damage = 3 } },
                new Spell{ Name = "Recharge",      Cost = 229, Effect = new Effect{ Time = 5,   Mana = 101 } }
        };

        var player = new Player
        {
            Mana = 500,
            Health = 50,
            Spells = playerSpells
        };

        var boss = new Player
        {
            Health = int.Parse(Regex.Matches(data[0], @"\d+").First().Groups[0].Value),
            Damage = int.Parse(Regex.Matches(data[1], @"\d+").First().Groups[0].Value)
        };

        Action<Player, Player> ResolveEffects = (player, boss) =>
        {
            var shield = player.Spells.Single(spell => spell.Name == "Shield");
            var poison = player.Spells.Single(spell => spell.Name == "Poison");
            var recharge = player.Spells.Single(spell => spell.Name == "Recharge");

            boss.Health -= poison.Effect.Active ? poison.Effect.Damage : 0;
            player.Mana += recharge.Effect.Active ? recharge.Effect.Mana : 0;
            player.Armor = shield.Effect.Active ? shield.Effect.Armor : 0;

            if (shield.Effect.Active)
            {
                shield.Effect.Tick += 1;

                if (shield.Effect.Tick == shield.Effect.Time)
                {
                    shield.Effect.Active = false;
                    shield.Effect.Tick = 0;
                }
            }

            if (recharge.Effect.Active)
            {
                recharge.Effect.Tick += 1;

                if (recharge.Effect.Tick == recharge.Effect.Time)
                {
                    recharge.Effect.Active = false;
                    recharge.Effect.Tick = 0;
                }
            }

            if (poison.Effect.Active)
            {
                poison.Effect.Tick += 1;

                if (poison.Effect.Tick == poison.Effect.Time)
                {
                    poison.Effect.Active = false;
                    poison.Effect.Tick = 0;
                }
            }

        };

        Action<Player, Player, bool> Fight = (player, boss, hardMode) => { };

        Fight = (player, boss, hardMode) =>
        {
            foreach (var spell in player.Spells.Where(spell => !spell.Effect.Active
                                                        && spell.Cost <= player.Mana
                                                        && spell.Cost + player.ManaSpent < minMana))
            {
                var newPlayer = player.Copy();
                var newBoss = boss.Copy();

                #region Player

                if (hardMode)
                    newPlayer.Health -= 1;

                newPlayer.Mana -= spell.Cost;
                newPlayer.ManaSpent += spell.Cost;

                if (spell.Name == "Poison" || spell.Name == "Shield" || spell.Name == "Recharge")
                    newPlayer.Spells.Single(newSpell => newSpell.Name == spell.Name).Effect.Active = true;

                if (spell.Name == "Drain")
                {
                    newPlayer.Health += spell.Effect.Heal;
                    newBoss.Health -= spell.Effect.Damage;
                }

                if (spell.Name == "Magic Missile")
                {
                    newBoss.Health -= spell.Effect.Damage;
                }

                ResolveEffects(newPlayer, newBoss);

                if (newBoss.Health <= 0)
                {
                    minMana = Math.Min(newPlayer.ManaSpent, minMana);
                    break;
                }

                #endregion
                ResolveEffects(newPlayer, newBoss);

                if (newBoss.Health <= 0)
                {
                    minMana = Math.Min(newPlayer.ManaSpent, minMana);
                    break;
                }

                newPlayer.Health -= Math.Max(newBoss.Damage - newPlayer.Armor, 1);

                if (newPlayer.Health > 0)
                {
                    Fight(newPlayer, newBoss, hardMode);
                }
            }
        };

        Fight(player, boss, false);

        Console.WriteLine($"Part 1: {minMana}");

        minMana = int.MaxValue;
        Fight(player, boss, true);

        Console.WriteLine($"Part 2: {minMana}");
    }

}