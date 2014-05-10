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

namespace lab_24._04._13
{
    /// <summary>
    /// Interaction logic for Message.xaml
    /// </summary>
    public partial class Message : Window
    {
        MainWindow X;

        public Message()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ;
        }

        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            String username = name.Text;

            if (username.Count()  <= 0)
                username = "John Doe";

            String t = (String)X.time.Content;
            String sh = t.Substring(5, 2);
            String sm = t.Substring(8, 2);
            String ss = t.Substring(11, 2);
            int s = Convert.ToUInt16(ss);
            int m = Convert.ToUInt16(sm);
            int h = Convert.ToUInt16(sh);
            int time = s + 60 * m + 3600 * h;

            int cnt = (int)this.Tag;

            Score newScore = new Score()
            {
                Username = username,
                Count = cnt,
                Time = time
            };

            if (MainWindow.Size == 150)
            {
                X.easyScoresList.Items.Add(newScore);

                X.easyScoresList.Items.SortDescriptions.Add
                (new System.ComponentModel.SortDescription
                    ("Count", System.ComponentModel.ListSortDirection.Descending));

                X.easyScoresList.Items.SortDescriptions.Add
                    (new System.ComponentModel.SortDescription
                        ("Time", System.ComponentModel.ListSortDirection.Ascending));
            }

            else if (MainWindow.Size == 75)
            {
                X.mediumScoresList.Items.Add(newScore);

                X.mediumScoresList.Items.SortDescriptions.Add
                (new System.ComponentModel.SortDescription
                    ("Count", System.ComponentModel.ListSortDirection.Descending));

                X.mediumScoresList.Items.SortDescriptions.Add
                    (new System.ComponentModel.SortDescription
                        ("Time", System.ComponentModel.ListSortDirection.Ascending));
            }

            else if (MainWindow.Size == 50)
            {
                X.hardScoresList.Items.Add(newScore);

                X.hardScoresList.Items.SortDescriptions.Add
                (new System.ComponentModel.SortDescription
                    ("Count", System.ComponentModel.ListSortDirection.Descending));

                X.hardScoresList.Items.SortDescriptions.Add
                    (new System.ComponentModel.SortDescription
                        ("Time", System.ComponentModel.ListSortDirection.Ascending));
            }

            X.canvasPanel.Children.Clear();
            X.time.Visibility = System.Windows.Visibility.Hidden;
            X.gameButton.Content = "Start Game";
            X.pauseButton.IsEnabled = false;
            MainWindow.dispatcherTimer.Stop();

            this.Close();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            if ((int)this.Tag == (MainWindow.imageHeight / MainWindow.Size) * (MainWindow.imageWidth / MainWindow.Size))
            {
                X.canvasPanel.Children.Clear();
                X.time.Visibility = System.Windows.Visibility.Hidden;
                X.gameButton.Content = "Start Game";
                X.pauseButton.IsEnabled = false;
                MainWindow.dispatcherTimer.Stop();
            }

            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            X = ((MainWindow)this.Owner);
            X.Owner = null;
        }
    }
}
