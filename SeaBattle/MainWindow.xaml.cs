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

namespace SeaBattle
{
    public partial class MainWindow : Window
    {
        private const int GridSize = 10;
        private Button[,] player1Buttons = new Button[GridSize, GridSize];
        private Button[,] player2Buttons = new Button[GridSize, GridSize];
        private bool[,] player1Ships = new bool[GridSize, GridSize];
        private bool[,] player2Ships = new bool[GridSize, GridSize];
        private int player1ShipsLeft = 5;
        private int player2ShipsLeft = 5;
        private bool isPlayer1Turn = true;
        private bool isPlacingShips = false;
        private bool isGameStarted = false;
        private int shipsToPlace = 5;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGrid(Player1Grid, player1Buttons, true);
            InitializeGrid(Player2Grid, player2Buttons, false);
        }

        private void InitializeGrid(Grid grid, Button[,] buttons, bool isPlayer1)
        {
            for (int i = 0; i < GridSize; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());

                for (int j = 0; j < GridSize; j++)
                {
                    Button button = new Button();
                    button.Click += (sender, e) => GridButton_Click(sender, e, isPlayer1);
                    grid.Children.Add(button);
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);
                    buttons[i, j] = button;
                }
            }
        }

        private void GridButton_Click(object sender, RoutedEventArgs e, bool isPlayer1)
        {
            if (!isPlacingShips && !isGameStarted)
                return;

            Button button = sender as Button;
            int row = Grid.GetRow(button);
            int column = Grid.GetColumn(button);

            if (isPlacingShips)
            {
                if (isPlayer1Turn == isPlayer1)
                {
                    if (button.Background != System.Windows.Media.Brushes.Gray)
                    {
                        button.Background = System.Windows.Media.Brushes.Gray;
                        if (isPlayer1)
                            player1Ships[row, column] = true;
                        else
                            player2Ships[row, column] = true;

                        shipsToPlace--;

                        if (shipsToPlace == 0)
                        {
                            isPlayer1Turn = !isPlayer1Turn;
                            shipsToPlace = 5;

                            if (!isPlayer1Turn)
                            {
                                MessageBox.Show("Игрок 2, расставьте свои корабли.");
                            }
                            else
                            {
                                isPlacingShips = false;
                                StartGameButton.IsEnabled = true;
                                HideShips(player1Buttons);
                                HideShips(player2Buttons);
                            }
                        }
                    }
                }
            }
            else if (isGameStarted)
            {
                if (isPlayer1Turn == isPlayer1)
                    return;

                if (button.Background == System.Windows.Media.Brushes.LightGray)
                {
                    bool hit = isPlayer1 ? player2Ships[row, column] : player1Ships[row, column];

                    button.Content = hit ? "X" : "O";
                    if (hit)
                    {
                        button.Background = System.Windows.Media.Brushes.Red;
                        if (isPlayer1)
                            player2ShipsLeft--;
                        else
                            player1ShipsLeft--;

                        if (CheckWin(isPlayer1))
                        {
                            MessageBox.Show(isPlayer1 ? "Игрок 2 победил!" : "Игрок 1 победил!");
                            ResetGame();
                        }
                    }
                    else
                    {
                        button.Background = System.Windows.Media.Brushes.Blue;
                    }

                    isPlayer1Turn = !isPlayer1Turn;
                }
            }
        }

        private bool CheckWin(bool isPlayer1)
        {
            return isPlayer1 ? player2ShipsLeft == 0 : player1ShipsLeft == 0;
        }

        private void StartSetup_Click(object sender, RoutedEventArgs e)
        {
            ResetGame();
            isPlacingShips = true;
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            isGameStarted = true;
            StartGameButton.IsEnabled = false;
        }

        private void ShowRules_Click(object sender, RoutedEventArgs e)
        {
            string rules = "Правила игры Морской бой:\n\n" +
                           "1. Каждый игрок расставляет на своем поле 5 кораблей.\n" +
                           "2. Корабли не могут касаться друг друга.\n" +
                           "3. После расстановки кораблей начинается игра.\n" +
                           "4. Игроки по очереди стреляют по клеткам на поле противника.\n" +
                           "5. Цель игры — потопить все корабли противника.";
            MessageBox.Show(rules, "Правила игры");
        }

        private void ResetGame()
        {
            ResetGrid(player1Buttons, player1Ships);
            ResetGrid(player2Buttons, player2Ships);
            isPlayer1Turn = true;
            isPlacingShips = false;
            isGameStarted = false;
            shipsToPlace = 5;
            player1ShipsLeft = 5;
            player2ShipsLeft = 5;
            StartGameButton.IsEnabled = false;
        }

        private void ResetGrid(Button[,] buttons, bool[,] ships)
        {
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    buttons[i, j].Content = "";
                    buttons[i, j].Background = System.Windows.Media.Brushes.LightGray;
                    ships[i, j] = false;
                }
            }
        }

        private void HideShips(Button[,] buttons)
        {
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    if (buttons[i, j].Background == System.Windows.Media.Brushes.Gray)
                    {
                        buttons[i, j].Background = System.Windows.Media.Brushes.LightGray;
                    }
                }
            }
        }
    }
}