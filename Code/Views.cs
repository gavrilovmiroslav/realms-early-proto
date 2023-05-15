using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace realms.Code
{
    public class CardView
    {
        public Vector2 XY;
        public Rectangle Bounds { get; set; }

        public virtual void Update(GameTime time) {}

        public virtual void Draw(SpriteBatch spriteBatch) {}
    }

    public enum MouseButtonState
    {
        None,
        Down,
        Up
    }

    public static class MouseCursor
    {
        static MouseButtonState _button = MouseButtonState.None;
        public static bool Click = false;

        public static void Update(GameTime time)
        {
            bool setClick = false;
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                switch (_button)
                {
                    case MouseButtonState.None: _button = MouseButtonState.Down; break;
                    case MouseButtonState.Down: _button = MouseButtonState.Up; break;
                    case MouseButtonState.Up: break;
                }
            }
            else
            {
                switch (_button)
                {
                    case MouseButtonState.None: break;
                    case MouseButtonState.Down: break;
                    case MouseButtonState.Up: setClick = true; _button = MouseButtonState.None; break;
                }
            }

            Click = setClick;
        }
    }

    public class EventCardView : CardView
    {
        private GameState _state;
        public int? SelectedOption = null;
        
        public EventCardView(GameState state)
        {
            this._state = state;
            XY = new Vector2(300, 100);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_state.CurrentEvent == null) return;

            var card = _state.CurrentEvent;

            Bounds = new Rectangle((int)XY.X, (int)XY.Y, 
                Realms.EventCard.Width, 
                Realms.EventCard.Height);

            var topLeft = new Vector2(Bounds.X, Bounds.Y);
            spriteBatch.Draw(Realms.EventCard, Bounds, Color.White);
            spriteBatch.DrawString(Realms.BigFont, card.Title, topLeft + new Vector2(30, 30), Color.White);

            for (int i = 0; i < card.Options.Count; i++)
            {
                var opt = card.Options[i];
                var titleText = opt.Title;
                var optionText = GetEventOptionAsString(opt);
                var titleXY = topLeft + new Vector2(40, 100 + 80 * i);
                var width = Math.Max(Realms.BigFont.MeasureString(titleText).X, Realms.BigFont.MeasureString(optionText).X + 20);
                var height = 40;
                var rect = new Rectangle((int)titleXY.X, (int)titleXY.Y, (int)width, height);
                var mouse = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

                var textColor = Color.White;
                var ruleColor = Color.Yellow;

                var selected = rect.Contains(mouse);
                if (selected)
                {
                    textColor = Color.Orange;
                    ruleColor = Color.DarkOrange;
                }

                var hand = new List<HandCard>(_state.Hand);
                if (!_state.CanSatisfy(card.Options[i].Requirements, hand))
                {
                    textColor = Color.Gray;
                    ruleColor = Color.Gray;
                    selected = false;
                }

                spriteBatch.DrawString(Realms.BigFont, titleText, titleXY, textColor);
                spriteBatch.DrawString(Realms.BigFont, optionText, titleXY + new Vector2(20, 20), ruleColor);

                if (selected && MouseCursor.Click)
                {
                    SelectedOption = i;
                    var exch = new List<HandCard>(Realms.Instance.GameState.Hand.Where(card => card.Offered));
                    if (Realms.Instance.GameState.CanSatisfy(opt.Requirements, new List<HandCard>(exch)))
                    {
                        exch = Realms.Instance.GameState.UseCards(opt.Requirements, exch);
                        if (exch.Count == 0)
                        {
                            Realms.Instance.Progression.Trigger("option chosen");
                        }
                    }
                }
            }
        }

        private string GetEventOptionAsString(EventOption option)
        {
            var reqs = new List<string>();
            foreach(var req in option.Requirements)
            {
                if (req is GiveAny any) reqs.Add($"{any.Count} cards");
                else if (req is GiveAnyUnits units) reqs.Add($"{units.Count} units");
                else if (req is GiveAnyArtifacts artifacts) reqs.Add($"{artifacts.Count} artifacts");
                else if (req is GiveNamedArtifact nmd) reqs.Add($"{nmd.Artifact}");
                else if (req is GiveResources res) reqs.Add($"{res.Count} {res.Type}");
                else if (req is GiveAnyResources rs) reqs.Add($"{rs.Count} resources");
                else if (req is GiveDamage dmg) reqs.Add($"{dmg.Count} Damage");
                else if (req is GiveUnits us) reqs.Add($"{us.Count} {us.Unit}s");
            }

            var reps = new List<string>();
            foreach (var rep in option.Responses)
            {                
                if (rep is AddAnyArtifactsToHand arts) reps.Add($"{arts.Count} random artifact{(arts.Count == 1 ? "" : "s")}");
                else if (rep is AddArtifactToHand art) reps.Add($"{art.Item}");
                else if (rep is AddDamageToHand dmg) reps.Add($"{dmg.Count} damage");
                else if (rep is AddEventsToDeck evs) reps.Add($"{evs.Pack} events");
                else if (rep is AddHeroToHand hero) reps.Add($"{hero.Hero.Name()}");
                else if (rep is AddResourcesToHand res) reps.Add($"{res.Count} {res.Type}");
                else if (rep is AddUnitsToHand uns) reps.Add($"{uns.Count} {uns.Unit}{(uns.Count == 1 ? "" : "s")}");
            }

            if (reqs.Count == 0 && reps.Count == 0) return "Nothing happens";
            else if (reqs.Count == 0)
            {
                return $"Gain {string.Join(", ", reps)}";
            }
            else if (reps.Count == 0)
            {
                return $"Lose {string.Join(", ", reqs)}";
            }
            else
            {
                return $"Lose {string.Join(", ", reqs)} -> Gain {string.Join(", ", reps)}";
            }
        }
    }
}
