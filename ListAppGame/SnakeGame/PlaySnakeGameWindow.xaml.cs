using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ListAppGame.SnakeGame
{
    public partial class PlaySnakeGameWindow : Window
    {
        private const int Size = 20;
        private DispatcherTimer gameTimer = new DispatcherTimer();
        private List<Rectangle> snake = new List<Rectangle>();
        private Point direction = new Point(1, 0);
        private Rectangle food;
        private int score = 0;
        private bool isPaused = false;

        public PlaySnakeGameWindow()
        {
            InitializeComponent();
            InitGame(); // Khởi tạo game nhưng không chạy ngay
            gameTimer.Interval = TimeSpan.FromMilliseconds(200);
            gameTimer.Tick += GameLoop;
            this.KeyDown += OnKeyDown;
        }


        private void InitGame()
        {
            snake.Clear();
            GameCanvas.Children.Clear();
            direction = new Point(1, 0);
            score = 0;
            ScoreTextBlock.Text = "0";

            var head = CreateRectangle(Brushes.Green);
            Canvas.SetLeft(head, 100);
            Canvas.SetTop(head, 100);
            snake.Add(head);
            GameCanvas.Children.Add(head);

            SpawnFood();
        }

        private Rectangle CreateRectangle(Brush color)
        {
            return new Rectangle { Width = Size, Height = Size, Fill = color };
        }

        private void SpawnFood()
        {
            if (food != null)
            {
                GameCanvas.Children.Remove(food); // Xóa viên mồi cũ trước khi tạo viên mới
            }

            Random rand = new Random();
            int x = rand.Next(0, (int)(GameCanvas.ActualWidth / Size)) * Size;
            int y = rand.Next(0, (int)(GameCanvas.ActualHeight / Size)) * Size;

            food = CreateRectangle(Brushes.Red);
            Canvas.SetLeft(food, x);
            Canvas.SetTop(food, y);
            GameCanvas.Children.Add(food);
        }

        private void GameLoop(object sender, EventArgs e)
        {
            if (isPaused) return;

            for (int i = snake.Count - 1; i > 0; i--)
            {
                Canvas.SetLeft(snake[i], Canvas.GetLeft(snake[i - 1]));
                Canvas.SetTop(snake[i], Canvas.GetTop(snake[i - 1]));
            }

            double newX = Canvas.GetLeft(snake[0]) + (direction.X * Size);
            double newY = Canvas.GetTop(snake[0]) + (direction.Y * Size);

            Canvas.SetLeft(snake[0], newX);
            Canvas.SetTop(snake[0], newY);

            if (Math.Abs(newX - Canvas.GetLeft(food)) < Size && Math.Abs(newY - Canvas.GetTop(food)) < Size)
            {
                EatFood();
            }

            if (newX < 0 || newY < 0 || newX >= GameCanvas.ActualWidth || newY >= GameCanvas.ActualHeight || HitSelf(newX, newY))
            {
                gameTimer.Stop();
                MessageBox.Show($"Game Over! Score: {score}");
            }
        }

        private void EatFood()
        {
            score++;
            ScoreTextBlock.Text = score.ToString();

            var newPart = CreateRectangle(Brushes.Green);
            snake.Add(newPart);
            GameCanvas.Children.Add(newPart);
            SpawnFood();
        }

        private bool HitSelf(double x, double y)
        {
            for (int i = 1; i < snake.Count; i++)
            {
                if (Canvas.GetLeft(snake[i]) == x && Canvas.GetTop(snake[i]) == y)
                    return true;
            }
            return false;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up && direction.Y == 0) direction = new Point(0, -1);
            if (e.Key == Key.Down && direction.Y == 0) direction = new Point(0, 1);
            if (e.Key == Key.Left && direction.X == 0) direction = new Point(-1, 0);
            if (e.Key == Key.Right && direction.X == 0) direction = new Point(1, 0);
        }

        private void StopGame(object sender, RoutedEventArgs e)
        {
            isPaused = !isPaused;

            if (isPaused)
            {
                gameTimer.Stop();
                StopButton.Content = "Continue"; // Đổi thành "Tiếp tục" khi đang dừng
            }
            else
            {
                gameTimer.Start();
                StopButton.Content = "Stop"; // Đổi lại thành "Stop" khi đang chạy
            }
        }


        private void RestartGame(object sender, RoutedEventArgs e)
        {
            gameTimer.Stop();
            InitGame();
            gameTimer.Start();
        }

        private void QuitGame(object sender, RoutedEventArgs e)
        {
            gameTimer.Stop(); // Dừng game trước khi thoát
            this.Close();

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void StartGame(object sender, RoutedEventArgs e)
        {
            InitGame();
            gameTimer.Start(); // Chỉ bắt đầu game khi bấm Play
            isPaused = false;

            PlayButton.Visibility = Visibility.Hidden;


            // Hiện nút Stop và Play Again
            StopButton.Visibility = Visibility.Visible;
            PlayAgainButton.Visibility = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
