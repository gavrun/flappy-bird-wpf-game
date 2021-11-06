using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//added for game timer
using System.Windows.Threading;


namespace flappy_bird_wpf_game
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //game timer
        DispatcherTimer gameTimer = new DispatcherTimer();

        double score;
        int gravity = 8;
        bool gameOver;
        //geometric object for a bird to detect collision
        Rect flappyBirdHitBox;

        public MainWindow()
        {
            InitializeComponent();

            gameTimer.Tick += MainEventTimer;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            
            StartGame();
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                //set rotation back
                flappyBird.RenderTransform = new RotateTransform(-20, flappyBird.Width / 2, flappyBird.Height / 2);
                gravity = -8;
            }

            //restart game
            if (e.Key == Key.R && gameOver == true)
            {
                StartGame();
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            //rotation for the bird when it moves up
            flappyBird.RenderTransform = new RotateTransform(5, flappyBird.Width / 2, flappyBird.Height / 2);

            gravity = 8;
        }

        private void StartGame()
        {
            //set focus for canvas element; idk something importants for xamarin
            MyCanvas.Focus();
            //later used for clouds
            int temp = 300;
            score = 0;
            //bool to reset the game
            gameOver = false;

            //bird initial position
            Canvas.SetTop(flappyBird, 190);

            foreach (var x in MyCanvas.Children.OfType<Image>())
            {
                //set pipes by image class filtered by a tag
                if ((string)x.Tag == "obs1")
                {
                    Canvas.SetLeft(x, 500);
                }
                if ((string)x.Tag == "obs2")
                {
                    Canvas.SetLeft(x, 800);
                }
                if ((string)x.Tag == "obs3")
                {
                    Canvas.SetLeft(x, 1100);
                }
                //set clouds 
                if ((string)x.Tag == "clouds")
                {
                    Canvas.SetLeft(x, 300 + temp);
                    //move the second cloud further
                    temp = 800;
                }
            }
            //set timer
            gameTimer.Start();

        }

        private void EndGame()
        {
            //stop game timer and reset game
            gameTimer.Stop();
            gameOver = true;
            
            txtScore.Content += " Game Over! Press R to restart.";
        }

        private void MainEventTimer(object sender, EventArgs e)
        {
            //prepare score 0
            txtScore.Content = "Score: " + score;
            //initialize object for the bird to detect collision
            flappyBirdHitBox = new Rect(Canvas.GetLeft(flappyBird), Canvas.GetTop(flappyBird), flappyBird.Width - 12, flappyBird.Height);
            //gravity 
            Canvas.SetTop(flappyBird, Canvas.GetTop(flappyBird) + gravity);
            
            //fail game when touch top edge of the window
            if (Canvas.GetTop(flappyBird) < -30 || Canvas.GetTop(flappyBird) + flappyBird.Height > 460)
            {
                EndGame();
            }

            //
            foreach (var x in MyCanvas.Children.OfType<Image>())
            {
                //moving pipe by image class filtered by a tag
                if ((string)x.Tag == "obs1" || (string)x.Tag == "obs2" || (string)x.Tag == "obs3")
                {
                    Canvas.SetLeft(x, Canvas.GetLeft(x) - 5);

                    if (Canvas.GetLeft(x) < -100)
                    {
                        Canvas.SetLeft(x, 800);
                        //get score when pass pipes; 0.5 for each pipe so it is +1 every time
                        score += .5;
                    }

                    //initialize object for pipes to detect collision
                    Rect PillarHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    //fail when collide with pipes
                    if (flappyBirdHitBox.IntersectsWith(PillarHitBox))
                    {
                        //EndGame();
                    }
                }

                //moving counds
                if ((string)x.Tag == "clouds")
                {
                    Canvas.SetLeft(x, Canvas.GetLeft(x) - 1);

                    if (Canvas.GetLeft(x) < -250)
                    {
                        Canvas.SetLeft(x, 550);
                        // do i get score for passing clouds?? weird but okay
                        score += 5;
                    }
                }
            }
        }
    }
}
