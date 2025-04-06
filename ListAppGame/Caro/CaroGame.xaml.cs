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
using System.Windows.Shapes;

namespace ListAppGame.Caro
{
    /// <summary>
    /// Interaction logic for CaroGame.xaml
    /// </summary>
    public partial class CaroGame : Window
    {
        private string currentPlayer = "X";
        private Button[,] buttons = new Button[3, 3];
        private Random random = new Random();
        public CaroGame()
        {
            InitializeComponent();
            InitializeBoard();
        }
        private void InitializeBoard()
        {
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    Button btn = new Button
                    {
                        FontSize = 35,
                        Width = 150,
                        Height = 150
                    };
                    btn.Click += Button_Click;
                    buttons[row, col] = btn;
                    GameGrid.Children.Add(btn);
                    Grid.SetRow(btn, row);
                    Grid.SetColumn(btn, col);
                }
            }
        }
        private void AiMove()
        {
            List<Button> emptyButtons = new List<Button>();
            foreach (var btn in buttons)
            {
                if (btn.Content == null)
                    emptyButtons.Add(btn);
            }

            if (emptyButtons.Count > 0)
            {
                Button aiBtn = emptyButtons[random.Next(emptyButtons.Count)];
                aiBtn.Content = "O";
                if (CheckWin())
                {
                    MessageBox.Show($"{currentPlayer} wins!");
                    ResetGame(null, null);
                    return;
                }
                currentPlayer = "X";
            }
        }
        public void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn.Content == null)
            {
                btn.Content = currentPlayer;
                if (CheckWin())
                {
                    MessageBox.Show($"{currentPlayer} wins!");
                    ResetGame(sender, e);
                    return;
                }
                currentPlayer = "O";
                AiMove();
            }
        }

        public bool CheckWin() //kiểm tra 3 ô hàng ngang và dọc thì thắng, 2 đường chéo
        {
            for (int i = 0; i < 3; i++)
            {
                if (buttons[i, 0].Content?.ToString() == currentPlayer &&
                    buttons[i, 1].Content?.ToString() == currentPlayer &&
                    buttons[i, 2].Content?.ToString() == currentPlayer)
                    return true;

                if (buttons[0, i].Content?.ToString() == currentPlayer &&
                    buttons[1, i].Content?.ToString() == currentPlayer &&
                    buttons[2, i].Content?.ToString() == currentPlayer)
                    return true;
            }
            if (buttons[0, 0].Content?.ToString() == currentPlayer &&
                buttons[1, 1].Content?.ToString() == currentPlayer &&
                buttons[2, 2].Content?.ToString() == currentPlayer)
                return true;

            if (buttons[0, 2].Content?.ToString() == currentPlayer &&
                buttons[1, 1].Content?.ToString() == currentPlayer &&
                buttons[2, 0].Content?.ToString() == currentPlayer)
                return true;

            return false;
        }

        public void ResetGame(object sender, RoutedEventArgs e)
        {
            foreach (var btn in buttons)
            {
                btn.Content = null;
            }
            currentPlayer = "X";
        }
    }
}

