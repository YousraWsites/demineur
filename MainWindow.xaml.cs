using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MinesweeperWPF
{
    public partial class MainWindow : Window
    {
        private int gridSize = 10; // taille de la grille
        private int nbMine = 10; // nombre de bombes
        private int nbCellOpen = 0; // nombre de cellules qui ont été vérifiées, ouvertes
        private int nbCellSafeTotal;
        private int[,] matrix; // matrice conservant les valeurs de la grille

        public MainWindow()
        {
            InitializeComponent();
            ShowStartMenu();
        }

        private void ShowStartMenu()
        {
            StartMenu.Visibility = Visibility.Visible;
            GameGrid.Visibility = Visibility.Collapsed;
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(GridSizeTextBox.Text, out int newGridSize) && int.TryParse(MineCountTextBox.Text, out int newMineCount))
            {
                MessageBox.Show("C'est partie !!");
                gridSize = newGridSize;
                nbMine = newMineCount;
                Reinit();
                StartMenu.Visibility = Visibility.Collapsed;
                GameGrid.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Entrées invalides. Veuillez entrer des nombres valides.", "Erreur de saisie", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Reinit()
        {
            nbCellOpen = 0;
            GRDGame.Children.Clear();
            GRDGame.ColumnDefinitions.Clear();
            GRDGame.RowDefinitions.Clear();
            matrix = new int[gridSize, gridSize];
            initialiserMatrix();
            nbCellSafeTotal = gridSize * gridSize - nbMine;

            for (int i = 0; i < gridSize; i++)
            {
                GRDGame.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                GRDGame.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            }

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    Border b = new Border();
                    b.BorderThickness = new Thickness(1);
                    b.BorderBrush = new SolidColorBrush(Colors.LightBlue);
                    b.SetValue(Grid.RowProperty, i);
                    b.SetValue(Grid.ColumnProperty, j);
                    GRDGame.Children.Add(b);

                    Grid innerGrid = new Grid();
                    b.Child = innerGrid;
                    Label l = new Label();
                    l.Content = matrix[i, j].ToString();
                    l.Visibility = Visibility.Hidden;
                    innerGrid.Children.Add(l);

                    Button bu = new Button();
                    bu.Click += ButtonClick;
                    innerGrid.Children.Add(bu);
                }
            }
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            Grid innerGrid = clickedButton.Parent as Grid;
            Border border = innerGrid.Parent as Border;
            int row = Grid.GetRow(border);
            int column = Grid.GetColumn(border);

            verifieCellule(column, row);
        }

        private void initialiserMatrix()
        {
            Random random = new Random();
            int bombsPlaced = 0;
            while (bombsPlaced < nbMine)
            {
                int row = random.Next(0, gridSize);
                int col = random.Next(0, gridSize);
                if (matrix[row, col] != -1)
                {
                    matrix[row, col] = -1;
                    bombsPlaced++;
                    UpdateAdjacentCells(row, col);
                }
            }
        }

        private void UpdateAdjacentCells(int row, int col)
        {
            for (int i = Math.Max(0, row - 1); i <= Math.Min(gridSize - 1, row + 1); i++)
            {
                for (int j = Math.Max(0, col - 1); j <= Math.Min(gridSize - 1, col + 1); j++)
                {
                    if (matrix[i, j] != -1)
                    {
                        matrix[i, j]++;
                    }
                }
            }
        }

        private UIElement GetUIElementFromPosition(Grid g, int col, int row)
        {
            return g.Children.Cast<UIElement>().FirstOrDefault(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == col);
        }

        private void verifieCellule(int column, int row)
        {
            if (column < 0 || column >= gridSize || row < 0 || row >= gridSize) return;

            Border b = (Border)GetUIElementFromPosition(GRDGame, column, row);
            Grid g = (Grid)b.Child;
            Button button = g.Children.OfType<Button>().FirstOrDefault();
            Label label = g.Children.OfType<Label>().FirstOrDefault();

            if (button.Visibility == Visibility.Visible)
            {
                button.Visibility = Visibility.Hidden;
                label.Visibility = Visibility.Visible;

                if (matrix[row, column] == -1)
                {
                    if (MessageBox.Show("Perdu ! Vous avez cliqué sur une bombe. Voulez-vous rejouer ?", "Perdu", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                    {
                        ShowStartMenu();
                        MessageBox.Show("Let's GOO !!");
                    }
                    else
                    {
                        ShowStartMenu();

                    }              
                }
                else
                {
                    nbCellOpen++;
                    if (nbCellOpen == nbCellSafeTotal)
                    {
                        if (MessageBox.Show("Gagné ! Voulez-vous rejouer ?", "Gagné", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            ShowStartMenu();
                        }
                        else
                        {
                            ShowStartMenu();

                        }
                    }
                    

                    if (matrix[row, column] == 0)
                    {
                        for (int i = Math.Max(0, column - 1); i <= Math.Min(gridSize - 1, column + 1); i++)
                        {
                            for (int j = Math.Max(0, row - 1); j <= Math.Min(gridSize - 1, row + 1); j++)
                            {
                                if (i != column || j != row)
                                {
                                    verifieCellule(i, j);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
