namespace Advent2015;

public class RPGItem : IComparable
{
    public string Name { get; set; } = string.Empty;

    public int Cost { get; set; } = 0;

    public int Damage { get; set; } = 0;

    public int Armor { get; set; } = 0;

    public string Type { get; set; } = "Weapon";

    public int CompareTo(object? obj)
    {
        if (obj == null) return 1;

        RPGItem? otherItem = obj as RPGItem;
        if (otherItem != null)
            return this.Name.CompareTo(otherItem.Name);
        else
            throw new ArgumentException("Object is not an RPG item");
    }
}

public class Player
{
    public int Health { get; set; } = 0;
    public int Damage { get; set; } = 0;
    public int Mana { get; set; } = 0;
    public int ManaSpent { get; set; } = 0;
    public int Armor { get; set; } = 0;
    public List<Spell> Spells { get; set; } = new List<Spell>();

    public Player Copy()
    {
        return JsonConvert.DeserializeObject<Player>(JsonConvert.SerializeObject(this)) ?? new Player();
    }
}

public class Spell : IComparable
{
    public string Name { get; set; } = string.Empty;

    public int Cost { get; set; } = 0;

    public Effect Effect { get; set; } = new Effect();

    public int CompareTo(object? obj)
    {
        if (obj == null) return 1;

        Spell? otherSpell = obj as Spell;
        if (otherSpell != null)
            return this.Name.CompareTo(otherSpell.Name);
        else
            throw new ArgumentException("Object is not a Spell");
    }
}

public class Effect : ICloneable
{
    public int Damage { get; set; } = 0;
    public int Heal { get; set; } = 0;
    public int Time { get; set; } = 0;
    public int Armor { get; set; } = 0;
    public int Mana { get; set; } = 0;
    public int Tick { get; set; } = 0;
    public bool Active { get; set; } = false;

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}

public static class Day21
{
    public static void Run(string[] data)
    {
        var bossHp = int.Parse(Regex.Matches(data[0], @"\d+").First().Groups[0].Value);
        var bossDmg = int.Parse(Regex.Matches(data[1], @"\d+").First().Groups[0].Value);
        var bossArm = int.Parse(Regex.Matches(data[2], @"\d+").First().Groups[0].Value);
        var playerHp = 100;

        var store = new List<RPGItem>{
            new RPGItem{Name = "Dagger",     Cost = 8,  Damage = 4, Armor = 0, Type = "Weapon"},
            new RPGItem{Name = "Shortsword", Cost = 10, Damage = 5, Armor = 0, Type = "Weapon"},
            new RPGItem{Name = "Warhammer",  Cost = 25, Damage = 6, Armor = 0, Type = "Weapon"},
            new RPGItem{Name = "Longsword",  Cost = 40, Damage = 7, Armor = 0, Type = "Weapon"},
            new RPGItem{Name = "Greataxe",   Cost = 74, Damage = 8, Armor = 0, Type = "Weapon"},

            new RPGItem{Name = "No Armor",   Cost = 0,   Armor = 0, Damage = 0, Type = "Armor"},
            new RPGItem{Name = "Leather",    Cost = 13,  Armor = 1, Damage = 0, Type = "Armor"},
            new RPGItem{Name = "Chainmail",  Cost = 31,  Armor = 2, Damage = 0, Type = "Armor"},
            new RPGItem{Name = "Splintmail", Cost = 53,  Armor = 3, Damage = 0, Type = "Armor"},
            new RPGItem{Name = "Bandedmail", Cost = 75,  Armor = 4, Damage = 0, Type = "Armor"},
            new RPGItem{Name = "Platemail",  Cost = 102, Armor = 5, Damage = 0, Type = "Armor"},

            new RPGItem{Name = "No Ring",    Cost = 0,   Damage = 0, Armor = 0, Type = "Ring"},
            new RPGItem{Name = "Damage +1",  Cost = 25,  Damage = 1, Armor = 0, Type = "Ring"},
            new RPGItem{Name = "Damage +2",  Cost = 50,  Damage = 2, Armor = 0, Type = "Ring"},
            new RPGItem{Name = "Damage +3",  Cost = 100, Damage = 3, Armor = 0, Type = "Ring"},
            new RPGItem{Name = "Defense +1", Cost = 20,  Armor = 1, Damage = 0, Type = "Ring"},
            new RPGItem{Name = "Defense +2", Cost = 40,  Armor = 2, Damage = 0, Type = "Ring"},
            new RPGItem{Name = "Defense +3", Cost = 80,  Armor = 3, Damage = 0, Type = "Ring"}
        };

        //WithRepetition will allow for optional rings and armor
        var purchaseOpportunities = new Combinations<RPGItem>(store, 4, GenerateOption.WithRepetition)
                                                .Where(items => items.Count(item => item.Type == "Ring") < 3
                                                             && items.Count(item => item.Type == "Armor") < 2
                                                             && items.Count(item => item.Type == "Weapon") == 1);

        var optimalPurchase = purchaseOpportunities.Where(items =>
            {
                var playerDamage = items.DistinctBy(item => item.Name).Sum(item => item.Damage);
                var playerArmor = items.DistinctBy(item => item.Name).Sum(item => item.Armor);

                var playerTurns = Math.Ceiling(bossHp / (double)Math.Max(playerDamage - bossArm, 1));
                var bossTurns = Math.Ceiling(playerHp / (double)Math.Max(bossDmg - playerArmor, 1));

                return playerTurns <= bossTurns;
            })
            .OrderBy(items => items.DistinctBy(item => item.Name).Sum(item => item.Cost))
            .First();

        Console.WriteLine($"Part 1: {optimalPurchase.Sum(item => item.Cost)}");

        var inoptimalPurchase = purchaseOpportunities.Where(items =>
            {
                var playerDamage = items.DistinctBy(item => item.Name).Sum(item => item.Damage);
                var playerArmor = items.DistinctBy(item => item.Name).Sum(item => item.Armor);

                var playerTurns = Math.Ceiling(bossHp / (double)Math.Max(playerDamage - bossArm, 1));
                var bossTurns = Math.Ceiling(playerHp / (double)Math.Max(bossDmg - playerArmor, 1));

                return playerTurns > bossTurns;
            })
            .OrderByDescending(items => items.DistinctBy(item => item.Name).Sum(item => item.Cost))
            .First();

        Console.WriteLine($"Part 2: {inoptimalPurchase.Sum(item => item.Cost)}");
    }

}