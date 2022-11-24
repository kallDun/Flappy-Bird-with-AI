using Flappy_Bird_with_AI.GameLogic.Components;
using Flappy_Bird_with_AI.GameLogic.Interfaces;
using Flappy_Bird_with_AI.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Flappy_Bird_with_AI.GameLogic
{
    public abstract class Gameplay : IDisposable
    {
        #region GAME RULES
        private static double SPEED_MODIFIER => 450;
        private static double JUMP_MODIFIER => 18;
        private static double ACCELERATION_MODIFIER => 90;
        private static double GRAVITATION_MODIFIER => 50;
        protected static double GameSpeedUpModifier { get; set; } = 1;
        public static double SpeedModifier => GameSpeedUpModifier * SPEED_MODIFIER;
        public static double JumpModifier => GameSpeedUpModifier * JUMP_MODIFIER;
        public static double AccelerationModifier => GameSpeedUpModifier * ACCELERATION_MODIFIER;
        public static double GravitationModifier => GameSpeedUpModifier * GRAVITATION_MODIFIER;

        #endregion


        #region Components
        protected Dictionary<Bird, IPlayer> Players { get; private set; }
        protected readonly List<Tube> _tubesList = new();
        private readonly List<Tube> _checkedTubes = new();
        #endregion


        #region MainParameters

        public event Action<double> OnUpdate;
        public double FPS { get; private set; }
        public bool IsThreadWorking { get; private set; }
        public bool IsGameOver { get; private set; } = true;

        protected void SetParams(double fps, Dictionary<Bird, IPlayer> players)
        {
            FPS = fps;
            Players = players;
        }

        #endregion


        #region Thread

        private Thread _updateThread;
        protected bool _independedThread = false;

        private void StartThread()
        {
            _updateThread = new Thread(() =>
            {
                //Stopwatch timer = new();
                //timer.Start();
                //long timerSavedTimeMilli, threadCallTimeMilli = 0;
                long wantedDeltaMilli = (long)Math.Round(1000d / FPS);

                while (IsThreadWorking)
                {
                    //Thread.Sleep((int)Math.Max(0, wantedDeltaMilli - threadCallTimeMilli));
                    Thread.Sleep((int)wantedDeltaMilli);

                    //timerSavedTimeMilli = timer.ElapsedMilliseconds;

                    //ThreadCall(Math.Max(wantedDeltaMilli, threadCallTimeMilli) / 1000d);
                    ThreadCall(wantedDeltaMilli / 1000d);

                    //threadCallTimeMilli = timer.ElapsedMilliseconds - timerSavedTimeMilli;
                }

                //timer.Stop();
            });
            _updateThread.Start();
        }
        protected virtual void ThreadCall(double deltaTime)
        {
            Update(deltaTime);
            OnUpdate?.Invoke(deltaTime);
        }

        public virtual void Dispose()
        {
            IsThreadWorking = false;
            _updateThread?.Abort();
        }
        #endregion


        #region UPDATE_LOGIC

        private int _randomPosition = 4;
        private double _tubeTime;
        private int _tubeIndex;
        private readonly double SPAWN_TUBE_EVERY = Config.Data.TubeTimer;

        protected void Update(double deltaTime)
        {
            if (!IsGameOver)
            {
                // create tubes
                _tubeTime += deltaTime;
                if (_tubeTime > SPAWN_TUBE_EVERY)
                {
                    _tubeTime -= SPAWN_TUBE_EVERY;
                    CreateTube();
                }

                // check input
                foreach (var player in Players)
                {
                    if (player.Value.IsJumping(GetGameState(player.Key)))
                    {
                        player.Key.Jump();
                    }
                }

                // update birds
                foreach (var bird in Players.Keys)
                {
                    bird.Update(deltaTime);
                }

                // moving & deleting tubes
                Tube destrucTube = null;
                foreach (var tubeN in _tubesList)
                {
                    if (tubeN.X <= -150) // destruction tubes behind the window
                    {
                        destrucTube = tubeN;
                    }
                    tubeN.Update(deltaTime);
                }
                _tubesList.Remove(destrucTube);
                _checkedTubes.Remove(destrucTube);

                // checking component's collisions
                CheckForGaming();
                CheckForCounting();
                CheckForRing();
                CheckForStar();
            }
        }

        private void CheckForRing()
        {
            foreach (var bird in Players.Keys)
            {
                if (bird.IsGameOver) continue;

                foreach (var tube in _tubesList)
                {
                    if (tube.Ring?.Coordinates.X >= bird.X && tube.Ring?.Coordinates.X <= bird.X + 50 &&
                        tube.Ring?.Coordinates.Y >= bird.Y && tube.Ring?.Coordinates.Y <= bird.Y + 50)
                    {
                        tube.EatTheRing();
                        bird.EatTheRing();
                        break;
                    }
                }
            }
        }
        private void CheckForStar()
        {
            foreach (var bird in Players.Keys)
            {
                if (bird.IsGameOver) continue;

                foreach (var tube in _tubesList)
                {
                    if (tube.Star?.Coordinates.X >= bird.X && tube.Star?.Coordinates.X <= bird.X + 50 &&
                        tube.Star?.Coordinates.Y >= bird.Y && tube.Star?.Coordinates.Y <= bird.Y + 50)
                    {
                        tube.EatTheStar();
                        bird.EatTheStar();
                        break;
                    }
                }
            }
        }
        private void CheckForCounting()
        {
            foreach (var bird in Players.Keys)
            {
                if (bird.IsGameOver) continue;

                foreach (var tube in _tubesList)
                {
                    if (bird.X >= tube.X + 180 && bird.X <= tube.X + 200)
                    {
                        var flag = true;
                        foreach (var checkedTube in _checkedTubes)
                        {
                            if (tube == checkedTube)
                            {
                                flag = false;
                                break;
                            }
                        }

                        if (flag)
                        {
                            bird.IncrementCounter();
                            tube.IsToobClosed = true;
                            _checkedTubes.Add(tube);
                        }
                    }
                }
            }
        }
        private void CheckForGaming()
        {
            foreach (var bird in Players.Keys)
            {
                if (bird.IsGameOver) continue;

                if (bird.Y >= 630 || bird.Y < 0)
                {
                    bird.SetGameOver();
                }

                foreach (var tube in _tubesList)
                {
                    if ((bird.X >= tube.X + 20 && bird.X <= tube.X + 160) &&
                        (bird.Y <= tube.Ytop + 517 || bird.Y >= tube.Ybottom - 42))
                    {
                        bird.SetGameOver();
                    }
                }
            }

            if (Players.Keys.All(x => x.IsGameOver))
            {
                IsGameOver = true;
            }
        }
        private void CreateTube()
        {
            var newTube = new Tube(_randomPosition, _tubeIndex++);
            _randomPosition = newTube.TubeRandomPos;
            _tubesList.Add(newTube);
        }

        #endregion


        #region MAIN_LOGIC
        public virtual void RestartGame()
        {
            foreach (var bird in Players.Keys)
            {
                bird.SetNewGame();
            }

            _tubesList.Clear();
            _tubeIndex = 0;
            _checkedTubes.Clear();
            IsGameOver = false;

            _tubeTime = 0;
            CreateTube();

            if (IsThreadWorking is false && _independedThread is false)
            {
                IsThreadWorking = true;
                StartThread();
            }
        }

        #endregion


        #region GameState

        public GameState GetGameState(Bird bird)
        {
            const double WIDTH = 1280, HEIGHT = 720;

            Tube nextTube = GetNextTube();
            if (nextTube is null) return new(0, 0, 0, 0);

            double horizontalDistance = nextTube.X - bird.X;
            double verticalDistance = nextTube.Ycenter - bird.Y - 30;

            GameState gameState = new(
                horizontalDistance / WIDTH, 
                verticalDistance / HEIGHT,
                GetRingDistance(nextTube, bird) / HEIGHT,
                GetStarDistance(nextTube, bird) / HEIGHT);

            return gameState;
        }

        protected Tube GetNextTube()
        {
            Tube nextTube = _tubesList.FirstOrDefault(x => !x.IsToobClosed);
            if (nextTube is null) return null;

            _tubesList.ForEach(tube =>
            {
                if (tube.X < nextTube.X && !tube.IsToobClosed)
                {
                    nextTube = tube;
                }
            });

            return nextTube;
        }

        private double GetStarDistance(Tube nextTube, Bird bird)
        {
            if (nextTube.Star is null) return 0;
            return nextTube.Star.Coordinates.Y - bird.Y - 30;
        }

        private double GetRingDistance(Tube nextTube, Bird bird)
        {
            if (nextTube.Ring is null) return 0;
            return nextTube.Ring.Coordinates.Y - bird.Y - 30;
        }

        #endregion
    }
}