using Game2048.Engine;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Game2048.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly TextBlock[,] tileTextBlocks;
        private readonly Border[,] tileBorders;
        public MainWindow()
        {
            InitializeComponent();

            this.Board = new Board(4,4);
            this.tileTextBlocks = new TextBlock[this.Board.Rows, this.Board.Columns];
            this.tileBorders = new Border[this.Board.Rows, this.Board.Columns];
            this.OllamaLlmHintService = new OllamaLlmHintService();
        }

        private void InitializeBoard()
        {
            for (int i = 0; i < this.Board.Rows; i++)
            {
                for (int j = 0; j < this.Board.Columns; j++)
                {
                    var tileValue = this.Board.Tiles[i, j];
                    var tileText = tileValue.HasValue ? tileValue.Value.ToString() : string.Empty;

                    var textBlock = new TextBlock
                    {
                        Text = tileText,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = 24,
                    };
                    this.tileTextBlocks[i, j] = textBlock;

                    var border = new Border
                    {
                        CornerRadius = new CornerRadius(10),
                        BorderBrush = Brushes.Transparent,
                        BorderThickness = new Thickness(1),
                        Background = Brushes.LightGray,
                        Child = textBlock,
                        Margin = new Thickness(5)
                    };
                    this.tileBorders[i, j] = border;
                    Grid.SetRow(border, i);
                    Grid.SetColumn(border, j);
                    this.BoardGrid.Children.Add(border);
                }
            }
        }

        public Board Board { get; private set; }
        public OllamaLlmHintService OllamaLlmHintService { get; private set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < Board.Rows; i++)
            {
                this.BoardGrid.RowDefinitions.Add(new RowDefinition());
            }
                        
            for (int i = 0; i < this.Board.Columns; i++)
            {
                this.BoardGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            this.InitializeBoard();

            this.UpdateUI();
        }

        private void UpdateUI()
        {
            if (this.Board != null)
            {
                for (int i = 0; i < this.Board.Rows; i++)
                {
                    for (int j = 0; j < this.Board.Columns; j++)
                    {
                        var tileValue = this.Board.Tiles[i, j];
                        var tileText = tileValue.HasValue ? tileValue.Value.ToString() : string.Empty;

                        tileTextBlocks[i, j].Text = tileText;
                        tileBorders[i,j].Background = SelectBrush(tileValue);
                    }
                }
            }
        }

        private static SolidColorBrush SelectBrush(int? tileValue) => tileValue switch
        {
            null => Brushes.LightGray,
            2 => new BrushConverter().ConvertFrom("#FFF5F2") as SolidColorBrush,
            4 => new BrushConverter().ConvertFrom("#FFE6DE") as SolidColorBrush,
            8 => new BrushConverter().ConvertFrom("#FFCFC2") as SolidColorBrush,
            16 => new BrushConverter().ConvertFrom("#FFB09A") as SolidColorBrush,
            32 => new BrushConverter().ConvertFrom("#FF8A73") as SolidColorBrush,
            64 => new BrushConverter().ConvertFrom("#FF6A4F") as SolidColorBrush,
            128 => new BrushConverter().ConvertFrom("#F24E3E") as SolidColorBrush,
            256 => new BrushConverter().ConvertFrom("#D93A2B") as SolidColorBrush,
            512 => new BrushConverter().ConvertFrom("#B22A22") as SolidColorBrush,
            1024 => new BrushConverter().ConvertFrom("#7A1E1A") as SolidColorBrush,
            2048 => new BrushConverter().ConvertFrom("#4A1210") as SolidColorBrush,
            _ => Brushes.Black,
        };

        private void KeyPressed(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!this.Board.CanMoveVertically() && (e.Key == Key.Up || e.Key == Key.Down))
            {
                return;
            }

            if (!this.Board.CanMoveHorizontally() && (e.Key == Key.Left || e.Key == Key.Right))
            {
                return;
            }

            if (e.Key == Key.Up)
            {   
                this.Board?.MoveUp();
            }
            else if (e.Key == Key.Down)
            {
                this.Board?.MoveDown();
            }
            else if (e.Key == Key.Left)
            {
                this.Board?.MoveLeft();
            }
            else if (e.Key == Key.Right)
            {
                this.Board?.MoveRight();
            }
            this.UpdateUI();

            var state = this.Board?.CheckWin();

            if (state == GameState.Won)
            {
                MessageBox.Show("Congratulations! You won!");
            }

            this.Board?.AddRandomTiles();

            this.UpdateUI();
            if (!this.Board.CanMoveHorizontally() && !this.Board.CanMoveVertically())
            {
                MessageBox.Show("Game Over! No more moves available.");
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Spinner.Visibility = Visibility.Visible;
            this.HintIcon.Visibility = Visibility.Collapsed;

            var (move, reason) = await this.OllamaLlmHintService.GetHintAsync(this.Board.Tiles);
            MessageBox.Show(reason == "Ollama is not running" ? "Ollama is not running. Please start Ollama and try again." : $"Move: {move}\nReason: {reason}");

            this.Spinner.Visibility = Visibility.Collapsed;
            this.HintIcon.Visibility = Visibility.Visible;
        }
    }
}
