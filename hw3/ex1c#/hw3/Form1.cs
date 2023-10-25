namespace hw3
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Collections.Generic;

    public partial class Form1 : Form
    {
        private Bitmap canvas;
        private Bitmap chartBitmap;
        private Bitmap blueChartBitmap;
        private Bitmap greenChartBitmap;
        private Bitmap orangeChartBitmap;
        private List<ResizableRect> resizableRectangles = new List<ResizableRect>();
        private List<List<Tuple<int, int>>> systemScores;
        private List<Color> systemColors;
        private int maxAttacks;
        private SecurityScoreChart scoreChart;
        private FrequencyChart freqChart;
        private FrequencyChart relFreqChart;
        private FrequencyChart normFreqChart;
        private int currentSystem;
        private int systemsCount = 10;
        private int numberOfAttacks = 16;
        private TextBox textBoxAttacks;
        private TextBox textBoxSystems;

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            pictureBox1.MouseDown += pictureBox1_MouseDown;
            pictureBox1.MouseMove += pictureBox1_MouseMove;
            pictureBox1.MouseUp += pictureBox1_MouseUp;
            pictureBox1.Paint += pictureBox1_Paint;
            pictureBox1.MouseWheel += pictureBox1_MouseWheel;

            pictureBox1.Dock = DockStyle.Fill;
            this.WindowState = FormWindowState.Maximized;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Inizializza la bitmap con la dimensione del PictureBox
            int canvasWidth = pictureBox1.Width;
            int canvasHeight = pictureBox1.Height;

            canvas = new Bitmap(canvasWidth, canvasHeight);

            int rectWidth = canvasWidth / 2 - 20;
            int rectHeight = canvasHeight / 2 - 20;
            chartBitmap = new Bitmap(canvasWidth, canvasHeight);

            ResizableRect redRect = new ResizableRect(new Rectangle(10, 10, rectWidth, rectHeight), Color.Red, canvas);
            ResizableRect blueRect = new ResizableRect(new Rectangle(rectWidth + 20, 10, rectWidth, rectHeight), Color.Blue, canvas);
            ResizableRect greenRect = new ResizableRect(new Rectangle(10, rectHeight + 20, rectWidth, rectHeight), Color.Green, canvas);
            ResizableRect orangeRect = new ResizableRect(new Rectangle(rectWidth + 20, rectHeight + 20, rectWidth, rectHeight), Color.Orange, canvas);

            resizableRectangles.Add(redRect);
            resizableRectangles.Add(blueRect);
            resizableRectangles.Add(greenRect);
            resizableRectangles.Add(orangeRect);

            systemColors = new List<Color>();
            for (int i = 0; i < systemsCount; i++)
            {
                systemColors.Add(GetRandomColor());
            }

            scoreChart = new SecurityScoreChart(numberOfAttacks, rectWidth, rectHeight, 500, canvasHeight - rectHeight, systemsCount, chartBitmap, redRect, systemColors);
            scoreChart.SetPictureBox(pictureBox1);

            this.systemScores = scoreChart.GetSystemScores();
            blueChartBitmap = new Bitmap(canvasWidth, canvasHeight);
            greenChartBitmap = new Bitmap(canvasWidth, canvasHeight);
            orangeChartBitmap = new Bitmap(canvasWidth, canvasHeight);
            freqChart = new FrequencyChart(rectWidth, rectHeight, 0, 0, numberOfAttacks, blueChartBitmap, blueRect, systemsCount, systemScores, systemColors);
            freqChart.SetPictureBox(pictureBox1);
            relFreqChart = new FrequencyChart(rectWidth, rectHeight, 0, 0, numberOfAttacks, greenChartBitmap, greenRect, systemsCount, systemScores, systemColors);
            relFreqChart.SetPictureBox(pictureBox1);
            normFreqChart = new FrequencyChart(rectWidth, rectHeight, 0, 0, numberOfAttacks, orangeChartBitmap, orangeRect, systemsCount, systemScores, systemColors);
            normFreqChart.SetPictureBox(pictureBox1);

            using (Graphics graphics = Graphics.FromImage(canvas))
            {
                DrawRectanglesOnCanvas(graphics);
            }

            pictureBox1.Image = canvas;
        }

        private void DrawRectanglesOnCanvas(Graphics graphics)
        {
            graphics.Clear(Color.White);

            foreach (var rect in resizableRectangles)
            {
                rect.Draw(graphics);
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            foreach (var rect in resizableRectangles)
            {
                rect.MouseDown(e.Location);
            }

            using (Graphics graphics = Graphics.FromImage(canvas))
            {
                DrawRectanglesOnCanvas(graphics);
            }
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            foreach (var rect in resizableRectangles)
            {
                rect.MouseMove(e.Location);
            }

            using (Graphics graphics = Graphics.FromImage(canvas))
            {
                DrawRectanglesOnCanvas(graphics);
            }
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            foreach (var rect in resizableRectangles)
            {
                rect.MouseUp();
            }

            using (Graphics graphics = Graphics.FromImage(canvas))
            {
                DrawRectanglesOnCanvas(graphics);
            }
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            scoreChart.chartX = resizableRectangles[0].Rectangle.X;
            scoreChart.chartY = resizableRectangles[0].Rectangle.Y;

            freqChart.chartX = resizableRectangles[1].Rectangle.X;
            freqChart.chartY = resizableRectangles[1].Rectangle.Y;

            relFreqChart.chartX = resizableRectangles[2].Rectangle.X;
            relFreqChart.chartY = resizableRectangles[2].Rectangle.Y;

            normFreqChart.chartX = resizableRectangles[3].Rectangle.X;
            normFreqChart.chartY = resizableRectangles[3].Rectangle.Y;

            using (Graphics graphics = Graphics.FromImage(scoreChart.GetChartBitmap()))
            {
                graphics.Clear(Color.Transparent);
            }

            using (Graphics graphics = Graphics.FromImage(blueChartBitmap))
            {
                graphics.Clear(Color.Transparent);
            }

            using (Graphics graphics = Graphics.FromImage(greenChartBitmap))
            {
                graphics.Clear(Color.Transparent);
            }

            using (Graphics graphics = Graphics.FromImage(orangeChartBitmap))
            {
                graphics.Clear(Color.Transparent);
            }

            scoreChart.Draw();
            e.Graphics.DrawImage(scoreChart.GetChartBitmap(), 0, 0);

            freqChart.DrawBlueRectFrequency(blueChartBitmap);
            relFreqChart.DrawGreenRectFrequency(greenChartBitmap);
            normFreqChart.DrawOrangeRectFrequency(orangeChartBitmap);
            e.Graphics.DrawImage(freqChart.GetChartBitmap(), 0, 0);
            e.Graphics.DrawImage(relFreqChart.GetChartBitmap(), 0, 0);
            e.Graphics.DrawImage(normFreqChart.GetChartBitmap(), 0, 0);


        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            foreach (var rect in resizableRectangles)
            {
                if (rect.Rectangle.Contains(e.Location))
                {
                    float zoomFactor = e.Delta > 0 ? 1.1f : 0.9f;

                    if (rect == resizableRectangles[0])
                    {
                        scoreChart.Zoom(zoomFactor);
                        using (Graphics graphics = Graphics.FromImage(chartBitmap))
                        {
                            graphics.Clear(Color.Transparent);
                        }
                    }
                    else if (rect == resizableRectangles[1])
                    {
                        freqChart.Zoom(zoomFactor);
                        using (Graphics graphics = Graphics.FromImage(freqChart.GetChartBitmap()))
                        {
                            graphics.Clear(Color.Transparent);
                        }
                    }
                    else if (rect == resizableRectangles[2])
                    {
                        relFreqChart.Zoom(zoomFactor);
                        using (Graphics graphics = Graphics.FromImage(relFreqChart.GetChartBitmap()))
                        {
                            graphics.Clear(Color.Transparent);
                        }
                    }
                    else if (rect == resizableRectangles[3])
                    {
                        normFreqChart.Zoom(zoomFactor);
                        using (Graphics graphics = Graphics.FromImage(normFreqChart.GetChartBitmap()))
                        {
                            graphics.Clear(Color.Transparent);
                        }
                    }

                    pictureBox1.Invalidate();
                    break;
                }
            }
        }

        private void buttonCalculate_Click(object sender, EventArgs e)
        {
            if (int.TryParse(nAttacks.Text, out numberOfAttacks) && int.TryParse(nSystems.Text, out systemsCount))
            {
                scoreChart.SetAttacksCount(numberOfAttacks);
                freqChart.SetNumberOfAttacks(numberOfAttacks);
                relFreqChart.SetNumberOfAttacks(numberOfAttacks);
                normFreqChart.SetNumberOfAttacks(numberOfAttacks);

                scoreChart.SetSystemsCount(systemsCount);
                freqChart.SetSystemsCount(systemsCount);
                relFreqChart.SetSystemsCount(systemsCount);
                normFreqChart.SetSystemsCount(systemsCount);
                for (int i = 0; i < systemsCount; i++)
                {
                    systemColors.Add(GetRandomColor());
                }
                scoreChart.SetSystemColor(systemColors);
                freqChart.SetSystemColors(systemColors);
                relFreqChart.SetSystemColors(systemColors);
                normFreqChart.SetSystemColors(systemColors);
                UpdateUI();
            }
            else
            {
                MessageBox.Show("Inserisci valori numerici validi per numberOfAttacks e systemsCount.");
            }
        }

        public static Color GetRandomColor()
        {
            Random random = new Random();
            return Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
        }

        private void UpdateUI()
        {
            scoreChart.CalculateScores();
            this.systemScores = scoreChart.GetSystemScores();
            List<List<Tuple<float, float>>> drawData = freqChart.CalculateCumulativeFrequency(systemScores);
            relFreqChart.CalculateRelativeFrequency(drawData);
            normFreqChart.CalculateNormalizedFrequency(drawData);
            pictureBox1.Invalidate();
        }
    }

}