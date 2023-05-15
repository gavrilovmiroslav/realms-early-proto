
namespace realms.Code
{
    public enum ResourceType
    {
        Gold, 
        Wood, 
        Ore,
        Gem,
        Food,
        Mercury,
        Morale, 
        Luck,
    }

    public enum EventPack
    {
        Battles,
        Common,

        // towns
        Castle,

        // locations
        Graveyard,

        // abilities
        Griffins,
        Leadership,
        Treasure,
        Healing,
    }

    public enum Town
    {
        Castle,
        Rampart,
        Inferno,
        Necropolis
    }

    public enum Hero
    {
        Castle_Edric_Knight,
        Castle_Caitlin_Cleric,
    }

    public static class HeroExtensions
    {
        public static string Name(this Hero hero)
        {
            switch (hero)
            {
                case Hero.Castle_Edric_Knight:
                    return "Edric the Knight";
                case Hero.Castle_Caitlin_Cleric:
                default:
                    return "Caitlin the Cleric";
            }
        }
    }

    public enum Artifact
    {
        CapeOfConjuring,
        RingOfLife,
        OrbOfVulnerability,
        ArmageddonsBlade,
    }

    public static class ArtifactExtensions
    {
        public static string Name(this Artifact artifact)
        {
            switch (artifact)
            {
                case Artifact.CapeOfConjuring: return "Cape of Conjuring";
                case Artifact.RingOfLife: return "Ring of Life";
                case Artifact.OrbOfVulnerability: return "Orb of Vulnerability";
                case Artifact.ArmageddonsBlade: return "Armageddon's Blade";
                default: return "";
            }
        }
    }

    public enum Unit
    {
        Pikeman,
        Halberdier,
        Griffin,
        RoyalGriffin,
        Archer,
        Marksman,
    }

    public enum Location
    {
        Town,
        Plains,
        Graveyard,
        Battle,
        Endgame,
    }
}
