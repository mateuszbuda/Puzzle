using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace lab_24._04._13
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class Score
    {
        public String Username { set; get; }
        public int Count { set; get; }
        public int Time { set; get; }
    }

    public partial class MainWindow : Window
    {
        public static System.Windows.Threading.DispatcherTimer dispatcherTimer;
        public const int hard = 2;
        public const int medium = 3;
        public const int easy = 6;
        public const int basePuzzleSize = 25;
        public static int puzzleSize;
        public static int Size;
        public const int imageHeight = 450;
        public const int imageWidth = 600;
        public const int PI = 180;
        public static List<Thumb>[,] thumbs;
        private double accuracy = 0.12;
        private static int top = 255;
        private Canvas pauseCanvas;
        private bool animationRunning = false;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
            dispatcherTimer.Stop();

            pauseButton.IsEnabled = false;
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (0 != ((String)gameButton.Content).CompareTo("Start Game") && pauseButton.IsEnabled == true)
            {
                    String t = (String)time.Content;
                    String sh = t.Substring(5, 2);
                    String sm = t.Substring(8, 2);
                    String ss = t.Substring(11, 2);
                    int s = (Convert.ToUInt16(ss) + 1) % 60;
                    int m = s != 0 ? Convert.ToUInt16(sm) : (Convert.ToUInt16(sm) + 1) % 60;
                    int h = (s != 0 || m != 0) ? Convert.ToUInt16(sh) : (Convert.ToUInt16(sh) + 1);
                    string timeContent = String.Format("Time {0}", new TimeSpan(h, m, s));
                    time.Content = timeContent;
            }
        }

        private void TabItem_GotFocus_High_Scores(object sender, RoutedEventArgs e)
        {
            ;
        }

        private void TabItem_GotFocus_Game(object sender, RoutedEventArgs e)
        {
            ;
        }

        private void Start_Game_Button_Click(object sender, RoutedEventArgs e)
        {
            if (0 == ((String)gameButton.Content).CompareTo("Start Game"))
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Filter = "Image Files|*.jpg; *.bmp; *.png"; // Filter files by extension 

                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results 
                if (result == true)
                {
                    // Open document 
                    string filename = dlg.FileName;

                    InitializeGame(filename);

                    ((Button)sender).Content = "End Game";

                    time.Content = "Time 00:00:00";
                    time.Visibility = System.Windows.Visibility.Visible;

                    dispatcherTimer.Start();

                    pauseButton.IsEnabled = true;
                }
            }

            else
            {
                int counter = getMaxConnectionValue();

                Message msg = new Message();
                msg.Owner = this;

                String tmp1 = "You connected only ";
                String cnt = Convert.ToString(counter);
                String tmp2 = " puzzle(s). Your time:";

                msg.Count.Content = tmp1 + cnt + tmp2;

                String t = (String)time.Content;
                String sh = t.Substring(5, 2);
                String sm = t.Substring(8, 2);
                String ss = t.Substring(11, 2);
                int s = Convert.ToUInt16(ss);
                int m = Convert.ToUInt16(sm);
                int h = Convert.ToUInt16(sh);
                string timeContent = String.Format("{0}", new TimeSpan(h, m, s));
                msg.time.Content = timeContent;

                msg.Tag = counter;

                msg.ShowDialog();
            }
        }

        private int getMaxConnectionValue()
        {
            int max = 1;

            for (int i = 0; i < thumbs.GetLength(0); i++)
                for (int j = 0; j < thumbs.GetLength(1); j++)
                    if (max < thumbs[i, j].Count)
                        max = thumbs[i, j].Count;

            return max;
        }

        private void InitializeGame(string filename)
        {
            this.canvasPanel.Children.Clear();

            Size = puzzleSize * basePuzzleSize;

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(filename);
            bi.DecodePixelHeight = imageHeight;
            bi.DecodePixelWidth = imageWidth;
            bi.EndInit();

            thumbs = new List<Thumb>[imageHeight / Size, imageWidth / Size];

            for (int i = 0; i < imageHeight / Size; i++)
                for (int j = 0; j < imageWidth / Size; j++)
                    thumbs[i, j] = new List<Thumb>((imageHeight + imageWidth) / Size);

            double rightBound = this.canvasPanel.ActualWidth - Size;
            double bottomBound = this.canvasPanel.ActualHeight - Size;
            Random r = new Random();

            int w = 0;
            int h = 0;
            while (w * Size < imageWidth)
            {
                while (h * Size < imageHeight)
                {
                    Thumb t = new Thumb();
                    Int32Rect rc = new Int32Rect(w * Size, h * Size, Size, Size);
                    CroppedBitmap cp = new CroppedBitmap(bi, rc);

                    t.Background = new ImageBrush(cp);
                    t.Width = Size;
                    t.Height = Size;
                    t.DragDelta += onDragDelta;
                    t.DragCompleted += onDragCompleted;
                    t.MouseRightButtonDown += onRightMouseDown;

                    t.Tag = new int[3];
                    ((int[])(t.Tag))[0] = (int)r.Next(0, 4);
                    RotateTransform rt = new RotateTransform(((int[])(t.Tag))[0] * PI / 2, 0.5, 0.5);
                    t.Background.RelativeTransform = rt;

                    double left = r.Next(0, (int)rightBound);
                    double top = r.Next(0, (int)bottomBound);
                    t.Margin = new Thickness(left, top, 0, 0);

                    ((int[])(t.Tag))[1] = h;
                    ((int[])(t.Tag))[2] = w;
                    thumbs[h, w].Add(t);

                    this.canvasPanel.Children.Add(t);

                    h++;
                }

                h = 0;
                w++;
            }
        }

        private void onRightMouseDown(object sender, MouseButtonEventArgs e)
        {
            Thumb t = ((Thumb)sender);

            int x = ((int[])t.Tag)[1];
            int y = ((int[])t.Tag)[2];

            if (thumbs[x, y].Count > 1)
                return;

            int a = ((int[])t.Tag)[0] + 1;
            ((int[])t.Tag)[0] = a;
            RotateTransform rt = new RotateTransform(a * PI / 2, 0.5, 0.5);

            t.Background.RelativeTransform = rt;

            int p = 0;
            int q = 0;
            int xChange;
            int yChange;

            for (int i = 0; i < 4; i++)
            {
                if (canBeConnected(x, y, out p, out q, out xChange, out yChange))
                    Connect(x, y, p, q);

                foreach (Thumb tt in thumbs[x, y])
                    Canvas.SetZIndex(tt, top++);

                if (p != -1 && q != -1)
                    correctPositions(thumbs[p, q][0]);
            }

            if (isWinningMove())
                showWinInfo();
        }

        private void onDragCompleted(object sender, DragCompletedEventArgs e)
        {
            Thumb tmp = (Thumb)sender;
            int x = ((int[])tmp.Tag)[1];
            int y = ((int[])tmp.Tag)[2];

            foreach (Thumb t in thumbs[x, y])
                t.Effect = null;

            int p = 0;
            int q = 0;
            int xChange;
            int yChange;

            for (int i = 0; i < 4; i++)
            {
                if (canBeConnected(x, y, out p, out q, out xChange, out yChange))
                    Connect(x, y, p, q);

                foreach (Thumb t in thumbs[x, y])
                    Canvas.SetZIndex(t, top++);

                if (p != -1 && q != -1)
                    correctPositions(thumbs[p, q][0]);
            }

            if (isWinningMove())
                showWinInfo();
        }

        private bool isWinningMove()
        {
            if (thumbs[0, 0].Count == (imageHeight / Size) * (imageWidth / Size))
                return true;

            return false;
        }

        private void showWinInfo()
        {
            Message msg = new Message();
            msg.Owner = this;

            String tmp1 = "You won! You connected all ";
            String cnt = Convert.ToString((imageHeight / Size) * (imageWidth / Size));
            String tmp2 = " puzzles. Your time:";

            msg.Count.Content = tmp1 + cnt + tmp2;

            String t = (String)time.Content;
            String sh = t.Substring(5, 2);
            String sm = t.Substring(8, 2);
            String ss = t.Substring(11, 2);
            int s = Convert.ToUInt16(ss);
            int m = Convert.ToUInt16(sm);
            int h = Convert.ToUInt16(sh);
            string timeContent = String.Format("{0}", new TimeSpan(h, m, s));
            msg.time.Content = timeContent;

            msg.Tag = (imageHeight / Size) * (imageWidth / Size);

            msg.ShowDialog();
        }

        private void correctPositions(Thumb thumb)
        {
            int x = ((int[])thumb.Tag)[1];
            int y = ((int[])thumb.Tag)[2];
            int thumbLeft = (int)thumb.Margin.Left;
            int thumbTop = (int)thumb.Margin.Top;

            foreach (Thumb t in thumbs[x, y])
            {
                int tx = ((int[])t.Tag)[1];
                int ty = ((int[])t.Tag)[2];
                if (t.Margin.Left != thumbLeft + ((ty - y) * Size) || t.Margin.Top != thumbTop + ((tx - x) * Size))
                    t.Margin = new Thickness(thumbLeft + ((ty - y) * Size), thumbTop + ((tx - x) * Size), 0, 0);
            }
        }

        private bool canBeConnected(int x, int y, out int p, out int q, out int xChange, out int yChange)
        {
            Thumb t = thumbs[x, y][0];

            if ((((int[])(t.Tag))[0] % 4) != 0)
            {
                p = q = -1;
                xChange = yChange = 0;
                return false;
            }

            Thumb tmp;

            //puzel po lewej
            if (y - 1 >= 0)
            {
                tmp = thumbs[x, y - 1][0];

                if ((((int[])tmp.Tag)[0] % 4) == 0 && !thumbs[x, y].Contains(tmp))
                {
                    int tLeft = (int)t.Margin.Left;
                    int tTop = (int)t.Margin.Top;
                    int tmpLeft = (int)tmp.Margin.Left;
                    int tmpTop = (int)tmp.Margin.Top;

                    if (Math.Abs(tmpLeft + Size - tLeft) < accuracy * Size && Math.Abs(tTop - tmpTop) < accuracy * Size)
                    {
                        xChange = (int)(tmp.Margin.Left + Size - tLeft);
                        yChange = (int)(tmp.Margin.Top - tTop);
                        p = ((int[])(tmp.Tag))[1];
                        q = ((int[])(tmp.Tag))[2];
                        return true;
                    }
                }
            }

            //puzel po prawej
            if (y + 1 < imageWidth / Size)
            {
                tmp = thumbs[x, y + 1][0];

                if ((((int[])tmp.Tag)[0] % 4) == 0 && !thumbs[x, y].Contains(tmp))
                {
                    int tLeft = (int)t.Margin.Left;
                    int tTop = (int)t.Margin.Top;
                    int tmpLeft = (int)tmp.Margin.Left;
                    int tmpTop = (int)tmp.Margin.Top;

                    if (Math.Abs(tLeft + Size - tmpLeft) < accuracy * Size && Math.Abs(tTop - tmpTop) < accuracy * Size)
                    {
                        xChange = (int)(tmp.Margin.Left - Size - tLeft);
                        yChange = (int)(tmp.Margin.Top - tTop);
                        p = ((int[])(tmp.Tag))[1];
                        q = ((int[])(tmp.Tag))[2];
                        return true;
                    }
                }
            }

            //puzel na górze
            if (x - 1 >= 0)
            {
                tmp = thumbs[x - 1, y][0];

                if ((((int[])tmp.Tag)[0] % 4) == 0 && !thumbs[x, y].Contains(tmp))
                {
                    int tLeft = (int)t.Margin.Left;
                    int tTop = (int)t.Margin.Top;
                    int tmpLeft = (int)tmp.Margin.Left;
                    int tmpTop = (int)tmp.Margin.Top;

                    if (Math.Abs(tmpLeft - tLeft) < accuracy * Size && Math.Abs(tTop - Size - tmpTop) < accuracy * Size)
                    {
                        xChange = (int)(tmp.Margin.Left - tLeft);
                        yChange = (int)(tmp.Margin.Top + Size - tTop);
                        p = ((int[])(tmp.Tag))[1];
                        q = ((int[])(tmp.Tag))[2];
                        return true;
                    }
                }
            }

            //puzel na dole
            if (x + 1 < imageHeight / Size)
            {
                tmp = thumbs[x + 1, y][0];

                if ((((int[])tmp.Tag)[0] % 4) == 0 && !thumbs[x, y].Contains(tmp))
                {
                    int tLeft = (int)t.Margin.Left;
                    int tTop = (int)t.Margin.Top;
                    int tmpLeft = (int)tmp.Margin.Left;
                    int tmpTop = (int)tmp.Margin.Top;

                    if (Math.Abs(tmpLeft - tLeft) < accuracy * Size && Math.Abs(tTop + Size - tmpTop) < accuracy * Size)
                    {
                        xChange = (int)(tmp.Margin.Left - tLeft);
                        yChange = (int)(tmp.Margin.Top - Size - tTop);
                        p = ((int[])(tmp.Tag))[1];
                        q = ((int[])(tmp.Tag))[2];
                        return true;
                    }
                }
            }

            p = q = -1;
            xChange = yChange = 0;
            return false;
        }

        private void Connect(int x, int y, int p, int q)
        {
            for (int i = 0; i < thumbs[p, q].Count; i++)
            {
                int tx = ((int[])(thumbs[p, q][i]).Tag)[1];
                int ty = ((int[])(thumbs[p, q][i]).Tag)[2];

                for (int j = 0; j < thumbs[x, y].Count; j++)
                {
                    if (!thumbs[tx, ty].Contains(thumbs[x, y][j]))
                        thumbs[tx, ty].Add(thumbs[x, y][j]);
                }
            }

            for (int i = 0; i < thumbs[x, y].Count; i++)
            {
                int tx = ((int[])(thumbs[x, y][i]).Tag)[1];
                int ty = ((int[])(thumbs[x, y][i]).Tag)[2];

                for (int j = 0; j < thumbs[p, q].Count; j++)
                {
                    if (!thumbs[tx, ty].Contains(thumbs[p, q][j]))
                        thumbs[tx, ty].Add(thumbs[p, q][j]);
                }
            }
        }

        private void onDragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb tmp = (Thumb)sender;
            int x = ((int[])tmp.Tag)[1];
            int y = ((int[])tmp.Tag)[2];

            foreach (Thumb t in thumbs[x, y])
            {
                Canvas.SetZIndex(t, int.MaxValue);

                t.Effect = new DropShadowEffect()
                {
                    Color = Colors.DimGray,
                    ShadowDepth = 21,
                    BlurRadius = 21,
                    Opacity = 0.5
                };
            }

            int minX = int.MaxValue, minY = int.MaxValue, maxX = 0, maxY = 0;

            foreach (Thumb t in thumbs[x, y])
            {
                if (((int[])(t.Tag))[2] < minX)
                    minX = ((int[])(t.Tag))[2];

                if (((int[])(t.Tag))[2] > maxX)
                    maxX = ((int[])(t.Tag))[2];

                if (((int[])(t.Tag))[1] < minY)
                    minY = ((int[])(t.Tag))[1];

                if (((int[])(t.Tag))[1] > maxY)
                    maxY = ((int[])(t.Tag))[1];
            }

            double rightBound = this.canvasPanel.ActualWidth;
            double bottomBound = this.canvasPanel.ActualHeight;

            foreach (Thumb t in thumbs[x, y])
            {
                int tX = ((int[])(t.Tag))[2];
                int tY = ((int[])(t.Tag))[1];

                rightBound -= (maxX - tX) * Size;
                bottomBound -= (maxY - tY) * Size;

                double left = t.Margin.Left + e.HorizontalChange;
                double top = t.Margin.Top + e.VerticalChange;

                if (left + t.ActualWidth > rightBound)
                    left = rightBound - t.ActualWidth;

                if (top + t.ActualHeight > bottomBound)
                    top = bottomBound - t.ActualHeight;

                t.Margin = new Thickness(left > (tX - minX) * Size ? left : (tX - minX) * Size, top > (tY - minY) * Size ? top : (tY - minY) * Size, 0, 0);

                rightBound = this.canvasPanel.ActualWidth;
                bottomBound = this.canvasPanel.ActualHeight;
            }
        }

        private void SimpleRadio_Checked(object sender, RoutedEventArgs e)
        {
            MainWindow.puzzleSize = MainWindow.easy;
        }

        private void MediumRadio_Checked(object sender, RoutedEventArgs e)
        {
            MainWindow.puzzleSize = MainWindow.medium;
        }

        private void HardRockRadio_Checked(object sender, RoutedEventArgs e)
        {
            MainWindow.puzzleSize = MainWindow.hard;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double rightBound = this.canvasPanel.ActualWidth;
            double bottomBound = this.canvasPanel.ActualHeight;

            foreach (Thumb t in canvasPanel.Children)
            {
                double left = t.Margin.Left;
                double top = t.Margin.Top;

                if (left + t.ActualWidth > rightBound)
                    left = rightBound - t.ActualWidth;

                if (top + t.ActualHeight > bottomBound)
                    top = bottomBound - t.ActualHeight;

                t.Margin = new Thickness(left > 0 ? left : 0, top > 0 ? top : 0, 0, 0);

                correctPositions(t);
            }
        }

        private void Pause_Button_Click(object sender, RoutedEventArgs e)
        {
            pauseButton.IsEnabled = false;

            pauseCanvas = new Canvas()
            {
                Name = "pauseCanvas",
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                Background = new ImageBrush()
                {
                    ImageSource = new BitmapImage(new Uri(@"Resources/pause.png", UriKind.Relative)),
                    Opacity = 0.8
                }
            };
            pauseCanvas.MouseLeftButtonDown += pauseCanvas_MouseLeftButtonDown;

            gameTabGrid.Children.Add(pauseCanvas);
        }

        void pauseCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!animationRunning)
            {
                animationRunning = true;
                DoubleAnimation da = new DoubleAnimation();
                da.From = 1.0;
                da.To = 0.0;
                da.Duration = new Duration(TimeSpan.FromSeconds(2));
                da.Completed += da_Completed;
                ((Canvas)sender).BeginAnimation(Canvas.OpacityProperty, da);
            }
        }

        void da_Completed(object sender, EventArgs e)
        {
            if (gameTabGrid.Children[gameTabGrid.Children.Count - 1] == pauseCanvas)
                gameTabGrid.Children.RemoveAt(gameTabGrid.Children.Count - 1);

            pauseButton.IsEnabled = true;
            animationRunning = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            accuracy = (accuracy == 0.12 ? 36 : 0.12);
        }

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Button_Click(null, null);
        }
    }
}
