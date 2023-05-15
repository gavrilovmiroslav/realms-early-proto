using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace realms.Code
{
    public class Timer
    {
        bool Active;
        float Time;
        public event EventHandler Callback;

        public Timer()
        {}

        public void Start(float time)
        {
            Time = time;
            Active = true;
        }

        public void Update(GameTime gameTime)
        {
            if (!Active) return;

            Time -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Time <= 0.0f)
            {
                Callback.Invoke(this, new EventArgs());
                Active = false;
            }
        }
    }
}
