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
        private const int Size = 20; // Kích thước của từng ô (rắn, mồi)
        private DispatcherTimer gameTimer = new DispatcherTimer(); // Bộ đếm thời gian để điều khiển trò chơi
        private List<Rectangle> snake = new List<Rectangle>(); // Danh sách các hình chữ nhật đại diện cho rắn
        private Point direction = new Point(1, 0); // Hướng di chuyển ban đầu (phải)
        private Rectangle food; // Đối tượng đại diện cho mồi
        private int score = 0; // Điểm số của người chơi
        private bool isPaused = false; // Trạng thái tạm dừng trò chơi

        public PlaySnakeGameWindow()
        {
            InitializeComponent();
            InitGame(); // Khởi tạo game nhưng chưa chạy ngay
            gameTimer.Interval = TimeSpan.FromMilliseconds(200); // Cập nhật trạng thái mỗi 200ms
            gameTimer.Tick += GameLoop; // Gán sự kiện lặp game
            this.KeyDown += OnKeyDown; // Lắng nghe phím điều hướng
        }

        /// <summary>
        /// Khởi tạo lại trò chơi, reset rắn và điểm số
        /// </summary>
        private void InitGame()
        {
            snake.Clear(); // Xóa rắn cũ
            GameCanvas.Children.Clear(); // Xóa toàn bộ đối tượng trong canvas
            direction = new Point(1, 0); // Đặt hướng di chuyển về bên phải
            score = 0; // Reset điểm số
            ScoreTextBlock.Text = "0"; // Hiển thị điểm số ban đầu

            // Tạo đầu rắn
            var head = CreateRectangle(Brushes.Green);
            Canvas.SetLeft(head, 100);
            Canvas.SetTop(head, 100);
            snake.Add(head);
            GameCanvas.Children.Add(head);

            SpawnFood(); // Tạo mồi đầu tiên
        }

        /// <summary>
        /// Tạo một hình chữ nhật với màu sắc chỉ định
        /// </summary>
        private Rectangle CreateRectangle(Brush color)
        {
            return new Rectangle { Width = Size, Height = Size, Fill = color };
        }

        /// <summary>
        /// Sinh mồi tại vị trí ngẫu nhiên trên canvas
        /// </summary>
        private void SpawnFood()
        {
            if (food != null)
            {
                GameCanvas.Children.Remove(food); // Xóa mồi cũ trước khi tạo mới
            }

            Random rand = new Random();
            int x = rand.Next(0, (int)(GameCanvas.ActualWidth / Size)) * Size;
            int y = rand.Next(0, (int)(GameCanvas.ActualHeight / Size)) * Size;

            food = CreateRectangle(Brushes.Red);
            Canvas.SetLeft(food, x);
            Canvas.SetTop(food, y);
            GameCanvas.Children.Add(food);
        }

        /// <summary>
        /// Vòng lặp chính của trò chơi, di chuyển rắn và kiểm tra va chạm
        /// </summary>
        private void GameLoop(object sender, EventArgs e)
        {
            if (isPaused) return; // Nếu đang tạm dừng, không làm gì cả

            // Di chuyển từng phần thân theo phần phía trước
            for (int i = snake.Count - 1; i > 0; i--)
            {
                Canvas.SetLeft(snake[i], Canvas.GetLeft(snake[i - 1]));
                Canvas.SetTop(snake[i], Canvas.GetTop(snake[i - 1]));
            }

            // Di chuyển đầu rắn theo hướng hiện tại
            double newX = Canvas.GetLeft(snake[0]) + (direction.X * Size);
            double newY = Canvas.GetTop(snake[0]) + (direction.Y * Size);

            Canvas.SetLeft(snake[0], newX);
            Canvas.SetTop(snake[0], newY);

            // Kiểm tra nếu rắn ăn mồi
            if (Math.Abs(newX - Canvas.GetLeft(food)) < Size && Math.Abs(newY - Canvas.GetTop(food)) < Size)
            {
                EatFood();
            }

            // Kiểm tra va chạm với tường hoặc thân mình
            if (newX < 0 || newY < 0 || newX >= GameCanvas.ActualWidth || newY >= GameCanvas.ActualHeight || HitSelf(newX, newY))
            {
                gameTimer.Stop();
                MessageBox.Show($"Game Over! Score: {score}");
            }
        }

        /// <summary>
        /// Khi rắn ăn mồi, tăng điểm số và làm rắn dài ra
        /// </summary>
        private void EatFood()
        {
            score++;
            ScoreTextBlock.Text = score.ToString();

            var newPart = CreateRectangle(Brushes.Green);
            snake.Add(newPart);
            GameCanvas.Children.Add(newPart);
            SpawnFood(); // Tạo mồi mới
        }

        /// <summary>
        /// Kiểm tra xem rắn có tự cắn vào thân không
        /// </summary>
        private bool HitSelf(double x, double y)
        {
            for (int i = 1; i < snake.Count; i++)
            {
                if (Canvas.GetLeft(snake[i]) == x && Canvas.GetTop(snake[i]) == y)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Điều khiển rắn bằng các phím mũi tên
        /// </summary>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up && direction.Y == 0) direction = new Point(0, -1);
            if (e.Key == Key.Down && direction.Y == 0) direction = new Point(0, 1);
            if (e.Key == Key.Left && direction.X == 0) direction = new Point(-1, 0);
            if (e.Key == Key.Right && direction.X == 0) direction = new Point(1, 0);
        }

        /// <summary>
        /// Dừng hoặc tiếp tục trò chơi
        /// </summary>
        private void StopGame(object sender, RoutedEventArgs e)
        {
            isPaused = !isPaused;

            if (isPaused)
            {
                gameTimer.Stop();
                StopButton.Content = "Continue";
            }
            else
            {
                gameTimer.Start();
                StopButton.Content = "Stop";
            }
        }

        /// <summary>
        /// Khởi động lại trò chơi
        /// </summary>
        private void RestartGame(object sender, RoutedEventArgs e)
        {
            gameTimer.Stop();
            InitGame();
            gameTimer.Start();
        }

        /// <summary>
        /// Thoát về menu chính
        /// </summary>
        private void QuitGame(object sender, RoutedEventArgs e)
        {
            gameTimer.Stop();
            this.Close();

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        /// <summary>
        /// Bắt đầu trò chơi khi nhấn Play
        /// </summary>
        private void StartGame(object sender, RoutedEventArgs e)
        {
            InitGame();
            gameTimer.Start();
            isPaused = false;

            PlayButton.Visibility = Visibility.Hidden;
            StopButton.Visibility = Visibility.Visible;
            PlayAgainButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Đóng ứng dụng
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
