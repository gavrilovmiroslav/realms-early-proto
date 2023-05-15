using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace realms.Code
{
    public enum GameStage
    {
        Starting,
        NewDay,
        NextEvent,
        ResolveExchange,
        UpdateHand,
        End
    }
    
    public class Progression
    {
        private GameStage _current = GameStage.Starting;
        private Dictionary<GameStage, Dictionary<string, GameStage>> _fsm = new();

        public GameStage Stage => _current;
        public event EventHandler<string> OnEnter_Starting;
        public event EventHandler<string> OnEnter_NewDay;
        public event EventHandler<string> OnEnter_NextEvent;
        public event EventHandler<string> OnEnter_ResolveExchange;
        public event EventHandler<string> OnEnter_UpdateHand;
        public event EventHandler<string> OnEnter_End;

        public Progression()
        {
            _fsm.Add(GameStage.Starting, new());
            _fsm.Add(GameStage.NewDay, new());
            _fsm.Add(GameStage.NextEvent, new());
            _fsm.Add(GameStage.ResolveExchange, new());
            _fsm.Add(GameStage.UpdateHand, new());
            _fsm.Add(GameStage.End, new());

            _fsm[GameStage.Starting].Add("start", GameStage.NewDay);
            _fsm[GameStage.Starting].Add("start event", GameStage.NextEvent);
            _fsm[GameStage.NewDay].Add("start event", GameStage.NextEvent);
            _fsm[GameStage.NewDay].Add("end", GameStage.End);
            _fsm[GameStage.NextEvent].Add("start event", GameStage.NextEvent);
            _fsm[GameStage.NextEvent].Add("new day", GameStage.NewDay);
            _fsm[GameStage.NextEvent].Add("option chosen", GameStage.ResolveExchange);            
            _fsm[GameStage.ResolveExchange].Add("all resolved", GameStage.UpdateHand);
            _fsm[GameStage.ResolveExchange].Add("game over", GameStage.End);
            _fsm[GameStage.UpdateHand].Add("done", GameStage.NextEvent);
        }

        public void Trigger(string trig)
        {
            if (_fsm[_current].ContainsKey(trig))
            {
                var next = _fsm[_current][trig];
                _current = next;
                switch (next)
                {
                    case GameStage.Starting: OnEnter_Starting?.Invoke(this, trig); break;
                    case GameStage.NewDay: OnEnter_NewDay?.Invoke(this, trig); break;
                    case GameStage.NextEvent: OnEnter_NextEvent?.Invoke(this, trig); break;
                    case GameStage.ResolveExchange: OnEnter_ResolveExchange?.Invoke(this, trig); break;
                    case GameStage.UpdateHand: OnEnter_UpdateHand?.Invoke(this, trig); break;
                    case GameStage.End: OnEnter_End?.Invoke(this, trig); break;
                }
            }
        }
    }
}
