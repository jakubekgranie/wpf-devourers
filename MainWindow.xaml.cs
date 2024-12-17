using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace wpf_devourers
{
    public partial class MainWindow : Window
    {
        Window1 Menu = new Window1();
        int dimensions = 0;
        int[][] pucks = [[0, 0], [0, 0]];
        SolidColorBrush[][] Colors = [[new(), new()], [new(), new()], [new(Color.FromRgb(191, 237, 192)), new(Color.FromRgb(127, 219, 129))]];
        List<List<Button>> gameMatrix = [];
        public MainWindow()
        {
            InitializeComponent();
            Menu.ShowDialog();
            dimensions = Int32.Parse(Menu.dimensions.Text.ToString());
            if (dimensions > 20)
                dimensions = 20;
            int sqrt = (int)Math.Ceiling(Math.Sqrt(dimensions));
            pucks[0][0] = pucks[1][0] = sqrt;
            pucks[0][1] = pucks[1][1] = sqrt / 2;
            Colors[0][0] = (SolidColorBrush)Menu.P1.Background;
            Colors[0][1] = (SolidColorBrush)Menu.P1.BorderBrush;
            Colors[1][0] = (SolidColorBrush)Menu.P2.Background;
            Colors[1][1] = (SolidColorBrush)Menu.P2.BorderBrush;
            double size = 110 / dimensions + 10;
            GameGridBorder.Width = GameGridBorder.Height = size * dimensions + 4;
            GameGridBorder.BorderBrush = Colors[2][1];
            for (int i = 0; i < dimensions; i++)
            {
                RowDefinition rdef = new()
                {
                    Height = new GridLength()
                };
                GameGrid.RowDefinitions.Add(rdef);
                ColumnDefinition cdef = new()
                {
                    Width = new GridLength()
                };
                GameGrid.ColumnDefinitions.Add(cdef);
            }
            for(int i = 0;i < dimensions; i++)
            {
                gameMatrix.Add([]);
                for(int j = 0; j < dimensions; j++)
                {
                    Button Element = new()
                    {
                        BorderThickness = new(1),
                        Background = Colors[2][0],
                        BorderBrush = Colors[2][1],
                        Width = Math.Ceiling(size),
                        Height = Math.Ceiling(size)
                    };
                    GameGrid.Children.Add(Element);
                    Grid.SetColumn(Element, i);
                    Grid.SetRow(Element, j);
                    gameMatrix[i].Add(Element);
                }
            }
        }
    }
}