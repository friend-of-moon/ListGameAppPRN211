﻿using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ListAppGame.Caro;
using ListAppGame.SnakeGame;

namespace ListAppGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void OpenSnakeGame(object sender, RoutedEventArgs e)
        {
            PlaySnakeGameWindow snakeGame = new PlaySnakeGameWindow();
            snakeGame.Show();
            this.Hide();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CaroGame caroGame = new CaroGame();
            caroGame.Show();
            this.Hide();
        }
    }
}