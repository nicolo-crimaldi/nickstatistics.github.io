namespace hw3
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Collections.Generic;
    using System.Media;
    using static System.Windows.Forms.LinkLabel;
    using System.Runtime.ConstrainedExecution;

    public partial class Form1 : Form
    {
        private Bitmap canvas;
        private Bitmap chartBitmap;
        private List<ResizableRect> resizableRectangles = new List<ResizableRect>();
        private int maxAttacks;
        private SecurityScoreChart scoreChart;
        private int currentSystem;
        private int systemsCount = 10;

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
            int canvasWidth = pictureBox1.Width;
            int canvasHeight = pictureBox1.Height;


            // Inizializza la bitmap con la dimensione del PictureBox
            canvas = new Bitmap(canvasWidth, canvasHeight);
            // Nel metodo Form1_Load, passa la bitmap ai costruttori dei rettangoli
            int rectWidth = canvasWidth / 2 - 20;  // Larghezza del rettangolo
            int rectHeight = canvasHeight / 2 - 20; // Altezza del rettangolo
            chartBitmap = new Bitmap(canvasWidth, canvasHeight);


            ResizableRect redRect = new ResizableRect(new Rectangle(10, 10, rectWidth, rectHeight), Color.Red, canvas);
            ResizableRect blueRect = new ResizableRect(new Rectangle(rectWidth + 20, 10, rectWidth, rectHeight), Color.Blue, canvas);
            ResizableRect greenRect = new ResizableRect(new Rectangle(10, rectHeight + 20, rectWidth, rectHeight), Color.Green, canvas);
            ResizableRect orangeRect = new ResizableRect(new Rectangle(rectWidth + 20, rectHeight + 20, rectWidth, rectHeight), Color.Orange, canvas);
            // Aggiungi 4 rettangoli con colori diversi alla lista al caricamento del form
            resizableRectangles.Add(redRect);
            resizableRectangles.Add(blueRect);
            resizableRectangles.Add(greenRect);
            resizableRectangles.Add(orangeRect);

            // Crea un'istanza di SecurityScoreChart
            scoreChart = new SecurityScoreChart(20, rectWidth, rectHeight, 500, canvasHeight - rectHeight, 10, chartBitmap, redRect);
            // Imposta il PictureBox su cui disegnare il grafico
            scoreChart.SetPictureBox(pictureBox1);


            using (Graphics graphics = Graphics.FromImage(canvas))
            {
                DrawRectanglesOnCanvas(graphics);
            }

            // Visualizza la bitmap nel PictureBox
            pictureBox1.Image = canvas;
        }

        private void DrawRectanglesOnCanvas(Graphics graphics)
        {
            graphics.Clear(Color.White); // Cancella la bitmap con uno sfondo bianco

            foreach (var rect in resizableRectangles)
            {
                rect.Draw(graphics);
            }

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            // Verifica se il mouse è sopra uno dei rettangoli
            foreach (var rect in resizableRectangles)
            {
                rect.MouseDown(e.Location);
            }


            // Aggiorna la bitmap con i rettangoli
            using (Graphics graphics = Graphics.FromImage(canvas))
            {
                DrawRectanglesOnCanvas(graphics);
            }
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            // Gestisci il movimento dei rettangoli
            foreach (var rect in resizableRectangles)
            {
                rect.MouseMove(e.Location);
            }

            // Aggiorna la bitmap con i rettangoli
            using (Graphics graphics = Graphics.FromImage(canvas))
            {
                DrawRectanglesOnCanvas(graphics);
            }
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            // Rilascia il mouse per tutti i rettangoli
            foreach (var rect in resizableRectangles)
            {
                rect.MouseUp();
            }

            // Aggiorna la bitmap con i rettangoli
            using (Graphics graphics = Graphics.FromImage(canvas))
            {
                DrawRectanglesOnCanvas(graphics);
            }
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            // Assicurati che il grafico sia ancorato al resizableRect rosso
            scoreChart.chartX = resizableRectangles[0].Rectangle.X;
            scoreChart.chartY = resizableRectangles[0].Rectangle.Y;

            using (Graphics graphics = Graphics.FromImage(scoreChart.GetChartBitmap()))
            {
                graphics.Clear(Color.Transparent); // Puoi utilizzare Color.Transparent per cancellare il disegno precedente
            }

            // Disegna il grafico

            scoreChart.Draw();
            e.Graphics.DrawImage(scoreChart.GetChartBitmap(), 0, 0);
        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            // Verifica se il puntatore del mouse è all'interno del rettangolo rosso
            if (resizableRectangles[0].Rectangle.Contains(e.Location))
            {
                // Calcola il fattore di zoom in base alla rotella del mouse
                float zoomFactor = e.Delta > 0 ? 1.1f : 0.9f; // Zoom in o zoom out

                // Esegui lo zoom del grafico nel SecurityScoreChart
                scoreChart.Zoom(zoomFactor);


                // Ricalcola il disegno del grafico nel PictureBox
                using (Graphics graphics = Graphics.FromImage(chartBitmap))
                {
                    graphics.Clear(Color.Transparent); // Cancella il disegno precedente
                }

                pictureBox1.Invalidate();
            }
        }



    }
}