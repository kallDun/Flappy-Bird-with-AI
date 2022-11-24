using Flappy_Bird_with_AI.GameLogic.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Flappy_Bird_with_AI.GameLogic.Obsolete
{
    public class ObsoleteGameplay
    {
        public List<Bird> Birds { get; } = new List<Bird>();
        private readonly List<Tube> _tubesList = new List<Tube> { };
        private readonly List<Tube> _checkedTubes = new List<Tube> { };
        public bool IsGameOver { get; private set; } = true;

        private int _randomPosition = 4;
        private double _tubeTime;

        public void UpdateCall(double deltaMsTime)
        {
            _tubeTime += deltaMsTime;
            if (_tubeTime > 2500)
            {
                _tubeTime -= 2500;
                CreateTube();
            }

            foreach (var bird in Birds)
            {
                bird.Update(deltaMsTime);
            }

            if (!IsGameOver)
            {
                Tube destrucTube = null;

                foreach (var tubeN in _tubesList)
                {
                    if (tubeN.X <= -150) // destruction tubes behind the window
                    {
                        destrucTube = tubeN;
                    }
                    tubeN.Update(deltaMsTime);
                }

                _tubesList.Remove(destrucTube);
                _checkedTubes.Remove(destrucTube);

                CheckForGaming(deltaMsTime);
                CheckForCounting();
                CheckForRing();
                CheckForStar();
            }
        }

        private void CheckForRing()
        {
            foreach (var bird in Birds)
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
            foreach (var bird in Birds)
            {
                if (bird.IsGameOver) continue;

                foreach (var tube in _tubesList)
                {
                    if (tube.Star?.Coordinates.X >= bird.X && tube.Star?.Coordinates.X <= bird.X + 50 &&
                        tube.Star?.Coordinates.Y >= bird.Y && tube.Star?.Coordinates.Y <= bird.Y + 50)
                    {
                        tube.EatTheStar();
                        bird.EatTheStar();
                        bird.IncrementCounter();
                        break;
                    }
                }
            }
        }
        private void CheckForCounting()
        {
            foreach (var bird in Birds)
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
        private void CheckForGaming(double deltaTime)
        {
            foreach (var bird in Birds)
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

            if (Birds.All(x => x.IsGameOver))
            {
                IsGameOver = true;
            }
        }

        private void CreateTube()
        {
            var newTube = new Tube(_randomPosition, 0);
            _randomPosition = newTube.TubeRandomPos;
            _tubesList.Add(newTube);
        }



        public void DrawCall(Graphics g)
        {
            foreach (var tube in _tubesList)
            {
                tube.DrawCall(g);
            }
            foreach (var bird in Birds)
            {
                bird.DrawCall(g);
            }
        }

        public async Task Restart(int populationCount)
        {
            _tubeTime = 0;
            _tubesList.Clear();
            _checkedTubes.Clear();
            CreateTube();

            Birds.Clear();
            for (int i = 0; i < populationCount; i++)
            {
                var bird = new Bird();
                Birds.Add(bird);
                await Task.Delay(1);
            }

            IsGameOver = false;
        }
        public void RestartWithPrevBirds()
        {
            for (int i = 0; i < Birds.Count; i++)
            {
                Birds[i].SetNewGame();
            }

            _tubesList.Clear();
            _checkedTubes.Clear();
            IsGameOver = false;

            _tubeTime = 0;
            CreateTube();
        }


        public (double HorizontalTubeDistance, double VerticalTubeDistance, double Top, double Bottom, double TubeDistance) GetGameState(Bird bird)
        {
            double top = bird.Y;
            double bottom = 630 - bird.Y;

            Tube nextTube = _tubesList.FirstOrDefault();
            if (nextTube == null) return (0, 0, top, bottom, 0);

            _tubesList.ForEach(tube =>
            {
                if (tube.X - bird.X >= -100 && tube.X < nextTube.X)
                {
                    nextTube = tube;
                }
            });

            double horizontalDistance = nextTube.X - bird.Y;
            double verticalDistance = nextTube.Ycenter - bird.Y;
            double distanceToTube = Math.Sqrt(Math.Pow(horizontalDistance, 2) + Math.Pow(verticalDistance, 2));

            return (horizontalDistance, verticalDistance, top, bottom, distanceToTube);
        }
        public double GetDistanceToTube(Bird bird)
        {
            Tube nextTube = _tubesList.FirstOrDefault();
            if (nextTube == null) return 1;

            _tubesList.ForEach(tube =>
            {
                if (tube.X - bird.X >= -100 && tube.X < nextTube.X)
                {
                    nextTube = tube;
                }
            });

            double horizontalDistance = nextTube.X - bird.Y;
            double verticalDistance = nextTube.Ycenter - bird.Y;
            return Math.Sqrt(Math.Pow(horizontalDistance, 2) + Math.Pow(verticalDistance, 2));
        }


        public void Click(double deltaTime)
        {
            foreach (var bird in Birds)
            {
                bird.Jump();
            }
        }
    }
}