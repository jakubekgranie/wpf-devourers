﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Timers;
using Microsoft.Win32;
using System.IO;


namespace wpf_devourers
{
    public partial class MainWindow : Window
    {
        Window1 Menu = new();   // customization dialogue box
        int dimensions = 0, // dimensions of the game
            currentWeight = 0, // weight of the button
            player = 0;
        String[] weightClasses = ["DBS", "DBM", "DBB"]; // weight classes
        int[][] pucks = [[0, 0], [0, 0]]; // pucks
        int[] time; // time
        Label[][] puckLabels = [[new(), new()], [new(), new()]];
        SolidColorBrush[][] Colors = [[new(), new()], [new(), new()], [new(Color.FromRgb(255, 218, 185)), new(Color.FromRgb(205, 133, 63))]]; // colors
        List<List<Button>> gameMatrix; // to change buttons respectively
        List<List<int>> states; // button weights
        ResourceDictionary dictionary = []; // styles
        double size = 110;
        double[] styleConstraints = [0.2, 0.35, 0.5];
        static System.Timers.Timer timeCounter = new System.Timers.Timer();
        SolidColorBrush unselected = new SolidColorBrush(Color.FromRgb(0, 0 ,0)); 
        protected void createGrid()
        {
            gameMatrix = [];
            states = [];
            time = [0, 0];
            player = 0;
            int sqrt = (dimensions < (int)Math.Ceiling(Math.Sqrt(dimensions))) ? (int)Math.Ceiling(Math.Sqrt(dimensions)) : dimensions;
            size = 110 / dimensions + 10;
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
            for (int i = 0; i < dimensions; i++)
            {
                gameMatrix.Add([]);
                states.Add([]);
                for (int j = 0; j < dimensions; j++)
                {
                    Button ElementRepresentative = new()
                    {
                        Visibility = Visibility.Hidden,
                        Width = size * 0.3,
                        Height = size * 0.3,
                        BorderThickness = new Thickness(2),
                        Background = unselected
                    },
                    Element = new()
                    {
                        BorderThickness = new(1),
                        Background = Colors[2][0],
                        BorderBrush = Colors[2][1],
                        Width = Math.Ceiling(size),
                        Height = Math.Ceiling(size),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Style = dictionary["noHover"] as Style,
                        Tag = new int[] { i, j },
                        Content = ElementRepresentative
                    };
                    Element.Click += changeElementStance;
                    GameGrid.Children.Add(Element);
                    Grid.SetColumn(Element, i);
                    Grid.SetRow(Element, j);
                    gameMatrix[i].Add(ElementRepresentative);
                    states[i].Add(-1);
                }
            }
        }
        protected void updateLabels()
        {
            
            big_P1.Foreground = medium_P1.Foreground = Colors[0][1];
            big_P2.Foreground = medium_P2.Foreground = Colors[1][1];
            turn.Foreground = Colors[player][0];
            turn.Content = "P" + (player + 1);
        }
        protected void getPucks(int sqrt)
        {
            pucks[0][0] = pucks[1][0] = sqrt;
            pucks[0][1] = pucks[1][1] = sqrt / 2;
            big_P1.Content = big_P2.Content = pucks[0][1];
            medium_P1.Content = medium_P2.Content = pucks[0][0];
            puckLabels[0][0] = medium_P1;
            puckLabels[0][1] = big_P1;
            puckLabels[1][0] = medium_P2;
            puckLabels[1][1] = big_P2;
        }
        public MainWindow()
        {
            InitializeComponent();
            dictionary.Source = new Uri("pack://application:,,,/dict.xaml");
            Menu.ShowDialog();
            dimensions = Int32.Parse(Menu.dimensions.Text.ToString());
            if (dimensions > 20)
                dimensions = 20;
            createGrid();
            int sqrt = (dimensions < (int)Math.Ceiling(Math.Sqrt(dimensions))) ? (int)Math.Ceiling(Math.Sqrt(dimensions)) : dimensions;
            getPucks(sqrt);
            Colors[0][0] = (SolidColorBrush)Menu.P1.Background;
            Colors[0][1] = (SolidColorBrush)Menu.P1.BorderBrush;
            Colors[1][0] = (SolidColorBrush)Menu.P2.Background;
            Colors[1][1] = (SolidColorBrush)Menu.P2.BorderBrush;
            updateLabels();
            timeCounter.Interval = 1000;
            timeCounter.Elapsed += trackTime;
            timeCounter.Enabled = true;
        }
        protected void trackTime(object? sender, ElapsedEventArgs? e)
        {
            Dispatcher.Invoke(() =>
            {
                time[1]++;
                if (time[1] == 60)
                {
                    time[1] = 0;
                    time[0]++;
                }
                timeLabel.Content = "Czas: " + time[0] + "m " + time[1] + "s";
            });
        }
        protected void elementStyleAdjustment(Button target, int weight)
        {
            target.Visibility = Visibility.Visible;
            target.Width = size * styleConstraints[weight];
            target.Height = size * styleConstraints[weight];
            target.Background = Colors[player][0];
            target.BorderBrush = Colors[player][1];
            target.Style = dictionary[weightClasses[weight] + "-rotation"] as Style;
        }
        protected void win()
        {
            timeCounter.Enabled = false;
            createGrid();
            getPucks((dimensions < (int)Math.Ceiling(Math.Sqrt(dimensions))) ? (int)Math.Ceiling(Math.Sqrt(dimensions)) : dimensions);
            updateLabels();
        }
        protected void changeElementStance(object sender, RoutedEventArgs e)
        {
            int[] coords = ((Button)sender).Tag as int[];
            Button target = gameMatrix[coords[0]][coords[1]];
            if ((currentWeight == 0 || pucks[player][currentWeight - 1] > 0) && currentWeight > states[coords[0]][coords[1]] && ((SolidColorBrush)gameMatrix[coords[0]][coords[1]].Background).Color != ((SolidColorBrush)Colors[player][0]).Color)
            {
                elementStyleAdjustment(target, currentWeight);
                if (currentWeight != 0)
                {
                    pucks[player][currentWeight - 1]--;
                    puckLabels[player][currentWeight - 1].Content = pucks[player][currentWeight - 1];
                }
                states[coords[0]][coords[1]] = currentWeight;
                player = (player + 1) % 2;
                turn.Foreground = Colors[player][1];
                turn.Content = "P" + (player + 1);
            }
            for (int i = 0; i < dimensions; i++)
            {
                Color background = ((SolidColorBrush)gameMatrix[i][0].Background).Color;
                if (background == unselected.Color)
                    return;
                bool flag = true;
                for (int j = 1; j < dimensions; j++)
                    if(((SolidColorBrush)gameMatrix[i][0].Background).Color != background)
                    {
                        flag = false;
                        break;
                    }
                if (flag)
                {
                    win();
                    return;
                }
            }
            Color background1 = ((SolidColorBrush)gameMatrix[0][0].Background).Color;
            if (background1 == unselected.Color)
                return;
            bool flag2 = true;
            for (int i = 1; i < dimensions; i++)
                if (((SolidColorBrush)gameMatrix[i][i].Background).Color != background1)
                    flag2 = false;
            if (flag2)
            {
                win();
                return;
            }
            flag2 = true;
            background1 = ((SolidColorBrush)gameMatrix[gameMatrix.Count - 1][0].Background).Color;
            if (background1 == unselected.Color)
                return;
            for (int i = 1; i > dimensions; i++)
                if (((SolidColorBrush)gameMatrix[gameMatrix.Count - 1 - i][i].Background).Color != background1)
                    flag2 = false;
            if (flag2)
            {
                win();
                return;
            }
        }
        protected void changePreviewButton(int type = 0)
        {
            previewButton.Style = dictionary["S-" + weightClasses[type]] as Style;
            currentWeight = type;
        }
        protected void changePreviewButtonSmall (object sender, RoutedEventArgs e)
        {
            changePreviewButton();
        }
        protected void changePreviewButtonMedium(object sender, RoutedEventArgs e)
        {
            changePreviewButton(1);
        }
        protected void changePreviewButtonBig(object sender, RoutedEventArgs e)
        {
            changePreviewButton(2);
        }
        protected void saveSave(object sender, RoutedEventArgs e)
        {
            OpenFileDialog file = new()
            {
                FileName = "save",
                DefaultExt = ".txt",
                Filter = "Text documents (.txt)|*.txt"
            };
            bool? result = file.ShowDialog();
            if (result == true)
            {
                String toFile = $"{dimensions}\n";
                for (int i = 0; i < 2; i++)
                    for (int j = 0; j < 2; j++) { 
                        Color color = Colors[i][j].Color;
                        toFile += $"{color.R} {color.G} {color.B} ";
                    }
                toFile += "\n";
                for (int i = 0; i < states.Count; i++)
                {
                    for (int j = 0; j < states[i].Count; j++)
                    {
                        if (states[i][j] == -1)
                            toFile += "3 ";
                        else
                            toFile += $"{states[i][j]} ";
                    }
                    toFile += "\n";
                }
                for (int i = 0; i < gameMatrix.Count; i++)
                {
                    for (int j = 0; j < gameMatrix[0].Count; j++)
                    {
                        Color color = ((SolidColorBrush)gameMatrix[i][j].Background).Color;
                        if (color == Colors[0][0].Color)
                            toFile += "0 ";
                        else if (color == Colors[1][0].Color)
                            toFile += "1 ";
                        else
                            toFile += "2 ";
                    }
                    toFile += "\n";
                }
                toFile += $"{time[0]} {time[1]}\n{player}\n";
                for (int i = 0; i < 2; i++)
                    for (int j = 0; j < 2; j++)
                        toFile += $"{pucks[i][j] }";
                File.WriteAllText(file.FileName, toFile);
            }
        }
        protected void loadSave(object sender, EventArgs e)
        {
            OpenFileDialog file = new()
            {
                FileName = "save",
                DefaultExt = ".txt",
                Filter = "Text documents (.txt)|*.txt"
            };
            bool? result = file.ShowDialog();
            if (result == true)
            {
                try
                {
                    timeCounter.Enabled = false;
                    String[] fromFile = File.ReadAllLines(file.FileName);
                    dimensions = Int32.Parse(fromFile[0]);
                    size = 110 / dimensions + 10;
                    String temp = "";
                    String[] tempArray = fromFile[1].Split(' ');
                    byte[] tempColor = [0, 0, 0];
                    for (int i = 0; i < 4; i++)
                        Colors[i / 2][i % 2] = new(Color.FromRgb(byte.Parse(tempArray[0 + 3*i]), byte.Parse(tempArray[1 + 3 * i]), byte.Parse(tempArray[2 + 3 * i])));
                    GameGrid.Children.Clear();
                    GameGrid.RowDefinitions.Clear();
                    GameGrid.ColumnDefinitions.Clear();
                    createGrid();
                    states.Clear();
                    for (int i = 0; i < dimensions; i++)
                    {
                        states.Add([]);
                        tempArray = fromFile[i + 2].Split(" ");
                        for (int j = 0; j < tempArray.Length; j++)
                            if (tempArray[j] != "")
                            {
                                int value = Int32.Parse(tempArray[j]);
                                if (value == 3)
                                    states[i].Add(-1);
                                else
                                    states[i].Add(value);
                            }
                    }
                    gameMatrix.Clear();
                    for (int i = 0; i < dimensions; i++)
                    {
                        gameMatrix.Add([]);
                        for (int j = 0; j < dimensions; j++)
                        {
                            Button ElementRepresentative = new()
                            {
                                Visibility = Visibility.Hidden,
                                Width = size * 0.3,
                                Height = size * 0.3,
                                BorderThickness = new Thickness(2)
                            },
                            Element = new()
                            {
                                BorderThickness = new(1),
                                Background = Colors[2][0],
                                BorderBrush = Colors[2][1],
                                Width = Math.Ceiling(size),
                                Height = Math.Ceiling(size),
                                HorizontalAlignment = HorizontalAlignment.Center,
                                VerticalAlignment = VerticalAlignment.Center,
                                Style = dictionary["noHover"] as Style,
                                Tag = new int[] { i, j },
                                Content = ElementRepresentative
                            };
                            Element.Click += changeElementStance;
                            GameGrid.Children.Add(Element);
                            Grid.SetColumn(Element, i);
                            Grid.SetRow(Element, j);
                            if(states[i][j] != -1)
                                elementStyleAdjustment(ElementRepresentative, states[i][j]);
                            gameMatrix[i].Add(ElementRepresentative);
                        }
                        
                    }
                    for (int i = 0; i < dimensions; i++)
                    {
                        tempArray = fromFile[i + 2 + dimensions].Split(' ');
                        for (int j = 0; j < tempArray.Length; j++)
                            if (tempArray[j] != "2" && tempArray[j] != "")
                            {
                                gameMatrix[i][j].Background = Colors[Int32.Parse(tempArray[j])][0];
                                gameMatrix[i][j].BorderBrush = Colors[Int32.Parse(tempArray[j])][1];
                            }
                    }
                    tempArray = fromFile[2 + dimensions * 2].Split(' ');
                    time = [Int32.Parse(tempArray[0]), Int32.Parse(tempArray[1])];
                    player = Int32.Parse(fromFile[3 + dimensions * 2]);
                    tempArray = fromFile[4 + dimensions * 2].Split(' ');
                    for (int i = 0; i < tempArray.Length - 1; i++)
                        pucks[i / 3][i % 2] = Int32.Parse(tempArray[i]);
                    updateLabels();
                    time[1]--;
                    trackTime(null, null);
                    timeCounter.Enabled = true;
                }
                catch(Exception ex) {
                    MessageBox.Show($"An unexpected error occured:\n\n{ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}