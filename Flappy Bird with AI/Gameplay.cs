using System.Collections.Generic;
using System.Drawing;
using System.Linq;


namespace Flappy_Bird_with_AI
{
    class Gameplay
    {
        private Bird bird;
        private List<Tube> tubesList = new List<Tube> { };
        private List<Tube> checkedTubes = new List<Tube> { };
        private int thatRandomPos = 4;
        public bool gameOver { get; private set; } = false;
        public int counter { get; private set; } = 0; 


        public Gameplay()
        {
            bird = new Bird();
        }


        /*public Tube theLastTube()
        {
            Tube lastTube = tubesList.First();
            double distance = 5000;
            foreach (var tube in tubesList)
            {
                if (tube.x - bird.x < distance && tube.x - bird.x >= -100)
                {
                    lastTube = tube;
                }
            }
            return lastTube;
        }*/

        public void update()
        {
            //var tubie = theLastTube();
            //var distanceToUp = Math.Sqrt(Math.Pow(tubie.x - bird.x, 2) + Math.Pow(tubie.y_tDown - tubie.betweenTubes - bird.y, 2));
            //var distanceToDown = Math.Sqrt(Math.Pow(tubie.x - bird.x, 2) + Math.Pow(tubie.y_tDown - bird.y, 2));

            bird.update();

            if (!gameOver)
            {
                Tube destrucTube = null;

                foreach (var tubeN in tubesList)
                {
                    if (tubeN.x <= -150) // destruction tubes behind the window
                    {
                        destrucTube = tubeN;
                    }
                    tubeN.update();
                }

                tubesList.Remove(destrucTube);
                checkedTubes.Remove(destrucTube);

                checkForGaming();
                checkForCounting();
                checkForRing();
                checkForStar();
            }
        }

        private void checkForRing()
        {
            foreach (var tube in tubesList)
            {
                if (tube.ring?.coordinates.X >= bird.x && tube.ring?.coordinates.X <= bird.x + 50 &&
                    tube.ring?.coordinates.Y >= bird.y && tube.ring?.coordinates.Y <= bird.y + 50)
                {
                    tube.eatTheRing();
                    break;
                }
            }
        }

        private void checkForStar()
        {
            foreach (var tube in tubesList)
            {
                if (tube.star?.coordinates.X >= bird.x && tube.star?.coordinates.X <= bird.x + 50 &&
                    tube.star?.coordinates.Y >= bird.y && tube.star?.coordinates.Y <= bird.y + 50)
                {
                    tube.eatTheStar();
                    counter++;
                    break;
                }
            }
        }

        public void createTube()
        {
            var newTube = new Tube(thatRandomPos);
            thatRandomPos = newTube.thisRandomPos;
            tubesList.Add(newTube);
        }

        public void draw(Graphics g)
        {
            
            foreach (var tube in tubesList)
            {
                tube.draw(g);
            }
            bird.draw(g);
        }

        public void restart()
        {
            tubesList.Clear();
            checkedTubes.Clear();
            gameOver = false;
            counter = 0;
            bird.newGame();
        }


        public void checkForCounting()
        {
            foreach (var tube in tubesList)
            {
                if (bird.x >= tube.x + 180 && bird.x <= tube.x + 200)
                {
                    var f = true;
                    foreach (var checkedTube in checkedTubes)
                    {
                        if (tube == checkedTube)
                        {
                            f = false;
                            break;
                        }
                    }

                    if (f)
                    {
                        counter++;
                        tube.closeToob = true;
                        checkedTubes.Add(tube);
                    }
                }
            }
        }

        public void checkForGaming()
        {
            if (bird.y >= 630)
            {
                bird.gameOver();
                gameOver = true;
            }

            foreach (var tube in tubesList)
            {
                if ((bird.x >= tube.x + 20 && bird.x <= tube.x + 160) &&
                    (bird.y <= tube.y_tUp + 525 || bird.y >= tube.y_tDown - 45)) 
                {
                    bird.gameOver();
                    gameOver = true;
                } 
            }
        }

        public void Click() { if (!gameOver) bird.clicked(); }


    }
}
