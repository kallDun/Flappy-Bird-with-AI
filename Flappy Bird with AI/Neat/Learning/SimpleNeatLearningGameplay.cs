using Flappy_Bird_with_AI.GameLogic;
using Flappy_Bird_with_AI.GameLogic.Components;
using Flappy_Bird_with_AI.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Flappy_Bird_with_AI.Neat.Learning
{
    class SimpleNeatLearningGameplay : Gameplay
    {
        #region Singleton
        private static readonly Dictionary<SimpleNeatLearningGameplay, bool> _neatGameplays = new();
        private static readonly object _lock1 = new();

        public static SimpleNeatLearningGameplay GetSingleton(NeatPlayer player)
        {
            SimpleNeatLearningGameplay neatGameplay;
            lock (_lock1)
            {
                neatGameplay = _neatGameplays.FirstOrDefault(x => x.Value is false).Key;

                if (neatGameplay is null)
                {
                    neatGameplay = new SimpleNeatLearningGameplay(player);
                    _neatGameplays.Add(neatGameplay, true);
                }
                else
                {
                    _neatGameplays[neatGameplay] = true;
                    neatGameplay.SetParams(neatGameplay.FPS, new() { { new Bird(), player } });
                }
            }
            return neatGameplay;
        }

        public override void Dispose()
        {
            base.Dispose();
            lock (_lock1)
            {
                if (_neatGameplays.ContainsKey(this))
                {
                    _neatGameplays[this] = false;
                }
            }
        }

        #endregion

        public SimpleNeatLearningGameplay(NeatPlayer player)
        {
            SetParams(Config.Data.Fps, new() { { new Bird(), player } });
            _independedThread = true;
        }

        private Bird Bird => Players.First().Key;
        public void Update() => Update(deltaTime: 1d / FPS);


        public double GetLivedSeconds() => Bird.SecondsLive;
        public int GetTubesScore() => Bird.Counter;
        public int GetStarsScore() => Bird.StarsCollected;
        public int GetRingScore() => Bird.RingCollected;

        public double GetVerticalTubeCloseness()
        {
            Tube nextTube = GetNextTube();
            if (nextTube is null) return 1;
            return nextTube.Ycenter - Bird.Y - 30;
        }
    }
}