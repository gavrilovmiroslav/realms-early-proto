using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NLua;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace realms.Code
{
    public class EventScript { }

    public class HandCard
    {
        public bool Offered = false;
        public virtual void Draw(SpriteBatch spriteBatch, int x, int y) 
        {
            spriteBatch.Draw(Realms.ResourceCard, new Vector2(x, y), Tint());
            if (Offered)
            {
                spriteBatch.Draw(Realms.ResourceCardFrame, new Vector2(x, y), Color.White);
            }
            spriteBatch.DrawString(Realms.SmallFont, Title(), new Vector2(x + 10, y + 20), Color.White);
        }

        public virtual string Title() { return ""; }

        public virtual Color Tint() { return Color.White; }

        public virtual void Exchange(ref GameState gameState) {}

        public void ToggleOffer()
        {
            Offered = !Offered;
        }
    }

    public class HeroCard : HandCard
    {
        public Hero Hero { get; set; }

        public override string Title() { return Hero.Name(); }

        public override Color Tint() { return Color.Goldenrod; }
    }

    public class UnitCard : HandCard
    {
        public Unit Unit { get; set; }

        public override string Title() { return $"{Unit}"; }

        public override Color Tint() { return Color.Blue; }
    }

    public class ResourceCard : HandCard
    {
        public ResourceType Type { get; set; }

        public override string Title() { return $"{Type}"; }

        public override void Draw(SpriteBatch spriteBatch, int x, int y)
        {
            base.Draw(spriteBatch, x, y);
        }
        public override Color Tint() { return Color.Cyan; }
    }

    public class ArtifactCard : HandCard
    {
        public Artifact Artifact { get; set; }

        public override string Title() { return $"{Artifact.Name()}"; }

        public override void Draw(SpriteBatch spriteBatch, int x, int y)
        {
            base.Draw(spriteBatch, x, y);
        }

        public override Color Tint() { return Color.Silver; }
    }

    public class DamageCard : HandCard 
    {
        public override void Draw(SpriteBatch spriteBatch, int x, int y)
        {
            base.Draw(spriteBatch, x, y);
        }

        public override string Title() { return $"Damage"; }

        public override Color Tint() { return Color.Crimson; }
    }


    public class GiveAction : EventScript { }

    public class GiveResources : GiveAction
    {
        public ResourceType Type { get; set; }
        public int Count { get; set; }
    }

    public class GiveAnyResources : GiveAction
    {        
        public int Count { get; set; }
    }

    public class GiveAnyArtifacts : GiveAction
    {
        public int Count { get; set; }
    }

    public class GiveUnits : GiveAction
    {
        public Unit Unit { get; set; }
        public int Count { get; set; }
    }

    public class GiveAnyUnits : GiveAction
    {
        public int Count { get; set; }
    }

    public class GiveNamedArtifact : GiveAction
    {
        public Artifact Artifact { get; set; }
    }

    public class GiveDamage : GiveAction
    {
        public int Count { get; set; }
    }

    public class GiveAny : GiveAction
    {
        public int Count { get; set; }
    }


    public class AddAction : EventScript 
    {
        public virtual void Apply() { }
    }

    public class AddHeroToHand : AddAction
    {
        public Hero Hero { get; set; }

        public override void Apply() 
        {
            Realms.Instance.GameState.Hand.Add(new HeroCard { Hero = Hero });
        }
    }

    public class AddResourcesToHand : AddAction
    {
        public ResourceType Type { get; set; }
        public int Count { get; set; }

        public override void Apply()
        {
            for (int i = 0; i < Count; i++)
            {
                Realms.Instance.GameState.Hand.Add(new ResourceCard { Type = Type });
            }
        }
    }

    public class AddDamageToHand : AddAction
    {
        public int Count { get; set; }

        public override void Apply()
        {
            for (int i = 0; i < Count; i++)
            {
                Realms.Instance.GameState.Hand.Add(new DamageCard { });
            }
        }
    }

    public class AddUnitsToHand : AddAction
    {
        public Unit Unit { get; set; }
        public int Count { get; set; }

        public override void Apply()
        {
            for (int i = 0; i < Count; i++)
            {
                Realms.Instance.GameState.Hand.Add(new UnitCard { Unit = Unit });
            }
        }
    }

    public class AddEventsToDeck : AddAction
    {
        public EventPack Pack { get; set; }

        public override void Apply()
        {
            Realms.Instance.GameState.CommonPacks.Add($"{Pack}");
        }
    }

    public class AddArtifactToHand : AddAction
    {
        public Artifact Item { get; set; }

        public override void Apply()
        {
            Realms.Instance.GameState.Hand.Add(new ArtifactCard { Artifact = Item });
        }
    }

    public class AddAnyArtifactsToHand : AddAction
    {
        public int Count { get; set; }

        public override void Apply()
        {
            var artifacts = Enum.GetValues<Artifact>();
            var random = new Random();

            for (int i = 0; i < Count; i++)
            {
                var index = random.Next(0, artifacts.Length);
                Realms.Instance.GameState.Hand.Add(new ArtifactCard { Artifact = artifacts[index] });
            }
        }
    }

    public class EndTheGame : AddAction
    {
        public bool Win { get; set; }

        public override void Apply()
        {
            Debug.WriteLine($"GAME ENDED: " + (Win ? "WIN!" : "LOSS!"));
        }
    }


    public class EventOption
    {
        public string Title { get; set; }
        public List<GiveAction> Requirements { get; set; }
        public List<AddAction> Responses { get; set; }
    }

    public class EventCard
    {
        public string Title { get; set; }
        public List<EventOption> Options { get; set; }
    }

    public class GameState
    {
        public Town Town { get; private set; } = Town.Castle;
        public Queue<Location> Journey = new();
        public int Day = 0;
        public Dictionary<string, List<string>> EventPacks = new();
        public List<string> Deck = new();
        public EventCard CurrentEvent = null;
        public List<string> CommonPacks = new() { "Common" };
        public List<HandCard> Hand = new();
        public List<HandCard> Exchange = new();        

        public bool HasDamage { get => Hand.Any(card => card is DamageCard); }
        public int CountDamage { get => Hand.Count(card => card is DamageCard); }

        public int CountResource(string resAsStr)
        {
            var res = Enum.Parse<ResourceType>(resAsStr);
            return Hand.Count(card => 
            { 
                if (card is ResourceCard resCard) 
                {
                    return resCard.Type == res;
                } 
                else 
                {
                    return false;
                } 
            });
        }

        public void AddJourney(string journey)
        {
            Journey.Enqueue((Location)Enum.Parse<Location>(journey));
        }

        public bool StartNewDay(bool dayPassed = false)
        {
            Deck.Clear();

            if (Journey.Count > 0)
            {
                var currentLand = Journey.Dequeue();
                var landName = $"{currentLand}";
                if (currentLand == Location.Town)
                    landName = $"{Town}";

                if (EventPacks.ContainsKey(landName))
                {
                    foreach (var e in EventPacks[landName])
                    {
                        Deck.Add(e);
                    }
                }
            } 
            else
            {
                return false;
            }

            foreach (var pack in CommonPacks)
            {
                foreach (var e in EventPacks[pack])
                {
                    Deck.Add(e);
                }
            }

            for (int i = 0; i < Day; i++)
            {
//                Deck.Add("Battle");
            }

            Day++;

            Deck = Deck.Shuffle();
            return true;
        }

        public bool CanSatisfy(List<GiveAction> requirements, List<HandCard> hand)
        {
            List<GiveNamedArtifact> namedArtifacts = new();
            List<GiveDamage> damages = new();
            List<GiveUnits> units = new();
            List<GiveResources> resources = new();
            List<GiveAnyResources> anyResources = new();
            List<GiveAnyUnits> anyUnits = new();
            List<GiveAnyArtifacts> anyArtifacts = new();
            List<GiveAny> anys = new();

            foreach (var req in requirements)
            {
                if (req is GiveResources res)
                {
                    for (int i = 0; i < res.Count; i++)
                        resources.Add(new GiveResources { Count = 1, Type = res.Type });
                }
                else if (req is GiveAnyResources ar)
                {
                    for (int i = 0; i < ar.Count; i++)
                        anyResources.Add(new GiveAnyResources { Count = 1 });
                }
                else if (req is GiveAny any)
                {
                    for (int i = 0; i < any.Count; i++)
                        anys.Add(new GiveAny { Count = 1 });
                }
                else if (req is GiveAnyArtifacts arts)
                {
                    for (int i = 0; i < arts.Count; i++)
                        anyArtifacts.Add(new GiveAnyArtifacts { Count = 1 });
                }
                else if (req is GiveAnyUnits unts)
                {
                    for (int i = 0; i < unts.Count; i++)
                        anyUnits.Add(new GiveAnyUnits{ Count = 1 });
                }
                else if (req is GiveDamage damage)
                {
                    for (int i = 0; i < damage.Count; i++)
                        damages.Add(new GiveDamage { Count = 1 });
                }
                else if (req is GiveNamedArtifact named)
                {
                    namedArtifacts.Add(new GiveNamedArtifact { Artifact = named.Artifact });
                }
                else if (req is GiveUnits uns)
                {
                    for (int i = 0; i < uns.Count; i++)
                        units.Add(new GiveUnits { Count = 1, Unit = uns.Unit });
                }
            }

            foreach(var art in namedArtifacts)
            {
                var fit = hand.
                    Where(c => c is ArtifactCard).
                    Where(c => (c as ArtifactCard).Artifact == art.Artifact);

                if (fit.Count() > 0)
                {
                    hand.Remove(fit.First());
                }
                else return false;
            }

            foreach (var dmg in damages)
            {
                var fit = hand.Where(c => c is DamageCard);
                if (fit.Count() > 0)
                {
                    hand.Remove(fit.First());
                }
                else return false;
            }

            foreach (var unit in units)
            {
                var fit = hand.
                    Where(c => c is UnitCard).
                    Where(c => (c as UnitCard).Unit == unit.Unit);

                if (fit.Count() > 0)
                {
                    hand.Remove(fit.First());
                }
                else return false;
            }

            foreach (var res in resources)
            {
                var fit = hand.
                    Where(c => c is ResourceCard).
                    Where(c => (c as ResourceCard).Type == res.Type);

                if (fit.Count() > 0)
                {
                    hand.Remove(fit.First());
                }
                else return false;
            }

            foreach (var res in anyResources)
            {
                var fit = hand.Where(c => c is ResourceCard);

                if (fit.Count() > 0)
                {
                    hand.Remove(fit.First());
                }
                else return false;
            }

            foreach (var unit in anyUnits)
            {
                var fit = hand.Where(c => c is UnitCard);

                if (fit.Count() > 0)
                {
                    hand.Remove(fit.First());
                }
                else return false;
            }

            foreach (var unit in anyArtifacts)
            {
                var fit = hand.Where(c => c is ArtifactCard);

                if (fit.Count() > 0)
                {
                    hand.Remove(fit.First());
                }
                else return false;
            }

            foreach (var any in anys)
            {
                if (hand.Count > 0)
                    hand.RemoveAt(0);
                else return false;
            }

            return true;
        }

        public List<HandCard> UseCards(List<GiveAction> requirements, List<HandCard> hand)
        {
            List<GiveNamedArtifact> namedArtifacts = new();
            List<GiveDamage> damages = new();
            List<GiveUnits> units = new();
            List<GiveResources> resources = new();
            List<GiveAnyResources> anyResources = new();
            List<GiveAnyUnits> anyUnits = new();
            List<GiveAnyArtifacts> anyArtifacts = new();
            List<GiveAny> anys = new();

            foreach (var req in requirements)
            {
                if (req is GiveResources res)
                {
                    for (int i = 0; i < res.Count; i++)
                        resources.Add(new GiveResources { Count = 1, Type = res.Type });
                }
                else if (req is GiveAnyResources ar)
                {
                    for (int i = 0; i < ar.Count; i++)
                        anyResources.Add(new GiveAnyResources { Count = 1 });
                }
                else if (req is GiveAny any)
                {
                    for (int i = 0; i < any.Count; i++)
                        anys.Add(new GiveAny { Count = 1 });
                }
                else if (req is GiveAnyArtifacts arts)
                {
                    for (int i = 0; i < arts.Count; i++)
                        anyArtifacts.Add(new GiveAnyArtifacts { Count = 1 });
                }
                else if (req is GiveAnyUnits unts)
                {
                    for (int i = 0; i < unts.Count; i++)
                        anyUnits.Add(new GiveAnyUnits { Count = 1 });
                }
                else if (req is GiveDamage damage)
                {
                    for (int i = 0; i < damage.Count; i++)
                        damages.Add(new GiveDamage { Count = 1 });
                }
                else if (req is GiveNamedArtifact named)
                {
                    namedArtifacts.Add(new GiveNamedArtifact { Artifact = named.Artifact });
                }
                else if (req is GiveUnits uns)
                {
                    for (int i = 0; i < uns.Count; i++)
                        units.Add(new GiveUnits { Count = 1, Unit = uns.Unit });
                }
            }

            foreach (var art in namedArtifacts)
            {
                var fit = hand.
                    Where(c => c is ArtifactCard).
                    Where(c => (c as ArtifactCard).Artifact == art.Artifact);

                if (fit.Count() > 0)
                {
                    hand.Remove(fit.First());
                }
                else return null;
            }

            foreach (var dmg in damages)
            {
                var fit = hand.Where(c => c is DamageCard);
                if (fit.Count() > 0)
                {
                    hand.Remove(fit.First());
                }
                else return null;
            }

            foreach (var unit in units)
            {
                var fit = hand.
                    Where(c => c is UnitCard).
                    Where(c => (c as UnitCard).Unit == unit.Unit);

                if (fit.Count() > 0)
                {
                    hand.Remove(fit.First());
                }
                else return null;
            }

            foreach (var res in resources)
            {
                var fit = hand.
                    Where(c => c is ResourceCard).
                    Where(c => (c as ResourceCard).Type == res.Type);

                if (fit.Count() > 0)
                {
                    hand.Remove(fit.First());
                }
                else return null;
            }

            foreach (var res in anyResources)
            {
                var fit = hand.Where(c => c is ResourceCard);

                if (fit.Count() > 0)
                {
                    hand.Remove(fit.First());
                }
                else return null;
            }

            foreach (var unit in anyUnits)
            {
                var fit = hand.Where(c => c is UnitCard);

                if (fit.Count() > 0)
                {
                    hand.Remove(fit.First());
                }
                else return null;
            }

            foreach (var unit in anyArtifacts)
            {
                var fit = hand.Where(c => c is ArtifactCard);

                if (fit.Count() > 0)
                {
                    hand.Remove(fit.First());
                }
                else return null;
            }

            foreach (var any in anys)
            {
                if (hand.Count > 0)
                    hand.RemoveAt(0);
                else return null;
            }

            return hand;
        }
    }

    public class Card
    {        
        public EventOption pick(string title, LuaTable req, LuaTable res)
        {
            List<GiveAction> give = new();
            foreach (var val in req.Values)
            {
                give.Add(val as GiveAction);
            }

            List<AddAction> get = new();
            foreach (var val in res.Values)
            {
                get.Add(val as AddAction);
            }

            return new EventOption
            {
                Title = title,
                Requirements = give,
                Responses = get,
            };
        }

        public EventCard create(string title, LuaTable opts)
        {
            List<EventOption> evs = new();
            foreach (var val in opts.Values)
            {
                evs.Add(val as EventOption);
            }

            return new EventCard
            {
                Title = title,
                Options = evs,
            };
        }
    }

    public class Give
    {
        public GiveResources res(string type, int count)
        {
            return new GiveResources
            {
                Type = Enum.Parse<ResourceType>(type, true),
                Count = count
            };
        }

        public GiveAny any(int count)
        {
            return new GiveAny
            {
                Count = count
            };
        }

        public GiveAnyResources any_res(int count)
        {
            return new GiveAnyResources
            {
                Count = count
            };
        }

        public GiveAnyArtifacts artifacts(int count)
        {
            return new GiveAnyArtifacts
            {
                Count = count,
            };
        }

        public GiveDamage damage(int count)
        {
            return new GiveDamage
            {
                Count = count,
            };
        }

        public GiveNamedArtifact artifact(string artifact)
        {
            return new GiveNamedArtifact
            {
                Artifact = (Artifact)Enum.Parse<Artifact>(artifact),
            };
        }

        public GiveUnits units(string unit, int count)
        {
            return new GiveUnits
            {
                Unit = (Unit)Enum.Parse<Unit>(unit, true),
                Count = count,
            };
        }


        public GiveAnyUnits any_units(int count)
        {
            return new GiveAnyUnits
            {                
                Count = count,
            };
        }
    }

    public class Get
    {
        public AddResourcesToHand res(string type, int count)
        {
            return new AddResourcesToHand()
            {
                Type = Enum.Parse<ResourceType>(type, true),
                Count = count,
            };
        }

        public AddHeroToHand hero(string hero)
        {
            return new AddHeroToHand()
            {
                Hero = Enum.Parse<Hero>(hero, true),
            };
        }

        public AddAnyArtifactsToHand any_artifacts(int count)
        {
            return new AddAnyArtifactsToHand() { Count = count };
        }

        public AddArtifactToHand artifact(string a)
        {
            return new AddArtifactToHand()
            {
                Item = Enum.Parse<Artifact>(a, true),
            };
        }

        public AddDamageToHand damage(int count)
        {
            return new AddDamageToHand()
            {
                Count = count
            };
        }

        public AddEventsToDeck pack(string pack, int count)
        {
            return new AddEventsToDeck()
            {
                Pack = Enum.Parse<EventPack>(pack, true)
            };
        }

        public AddUnitsToHand units(string unit, int count)
        {
            return new AddUnitsToHand()
            {
                Unit = Enum.Parse<Unit>(unit),
                Count = count
            };
        }
    }
}
