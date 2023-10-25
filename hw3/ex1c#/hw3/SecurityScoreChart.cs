using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Media;
using System.Windows.Forms;
using static System.Formats.Asn1.AsnWriter;
using Timer = System.Windows.Forms.Timer;

public class SecurityScoreChart
{

    private int attacksCount; // Numero di attacchi.
    public int chartWidth; // Larghezza del grafico.
    public int chartHeight; // Altezza del grafico.
    public int chartX; // Posizione X del grafico all'interno del rettangolo rosso.
    public int chartY; // Posizione Y del grafico all'interno del rettangolo rosso.
    private float scaleX = 100; // Fattore di scala per adattare il grafico alle dimensioni del rettangolo rosso.
    private float scaleY = 100;
    private PictureBox pictureBox; // Riferimento al PictureBox su cui disegnare il grafico.
    private int systemsCount;
    private Bitmap chartbitmap;
    private ResizableRect redRect;
    private Region clipRegion;
    List<List<Tuple<int, int>>> drawingData;
    List<Tuple<int, int>> systemData;
    List<List<Tuple<int, int>>> systemScores;
    private int redRectX;
    private int redRectY;
    public int redRectTop;
    private int redRectHeight;
    private float cartesianZoom = 1.0f;
    public List<Color> systemColors;





    public SecurityScoreChart(int attacksCount, int chartWidth, int chartHeight, int chartX, int chartY, int systemsCount, Bitmap chartbitmap, ResizableRect redRect, List<Color> systemColors)
    {
        this.attacksCount = attacksCount;
        this.chartWidth = chartWidth;
        this.chartHeight = chartHeight;
        this.chartX = chartX;
        this.chartY = chartY;
        this.chartbitmap = chartbitmap;
        this.systemsCount = systemsCount; // Inizializza il numero di sistemi.
        this.redRect = redRect;
        this.systemColors = systemColors;
        redRectX = redRect.Rectangle.X;
        redRectY = redRect.Rectangle.Y;
        redRectHeight = redRect.Rectangle.Height;

        CalculateScores();

    }



    public void SetPictureBox(PictureBox pictureBox)
    {
        this.pictureBox = pictureBox;
    }

    public PictureBox GetPictureBox()
    {
        return this.pictureBox;
    }

    public void CalculateScores()
    {
        Random random = new Random();
        systemScores = new List<List<Tuple<int, int>>>(systemsCount);
        drawingData = new List<List<Tuple<int, int>>>(systemsCount);

        for (int currentSystem = 0; currentSystem < systemsCount; currentSystem++)
        {
            systemData = new List<Tuple<int, int>>();
            systemData.Add(new Tuple<int, int>(0, 4));
            drawingData.Add(systemData);

            systemData = new List<Tuple<int, int>>();
            systemData.Add(new Tuple<int, int>(0, 0));
            systemScores.Add(systemData);

            for (int attack = 1; attack <= attacksCount; attack++)
            {
                double successProbability = random.NextDouble();

                // Calcola l'esito dell'attacco e aggiorna il punteggio.
                if (successProbability > 0.5)
                {
                    this.systemScores[currentSystem].Add(new Tuple<int, int>(attack, 1));
                    int precY = drawingData[currentSystem].Last().Item2;
                    drawingData[currentSystem].Add(new Tuple<int, int>(attack, precY + 1));
                }
                else
                {
                    this.systemScores[currentSystem].Add(new Tuple<int, int>(attack, -1));
                    int precY = drawingData[currentSystem].Last().Item2;
                    drawingData[currentSystem].Add(new Tuple<int, int>(attack, precY - 1));
                }
            }
        }
    }

    private void SetSystemScores(List<List<Tuple<int, int>>> systemScores)
    {
        this.systemScores = systemScores;
    }

    public void DrawLines(Graphics graphics)
    {
        int startX;
        int startY;
        int endX;
        int endY;
        int thickness = systemsCount;

        // Distanza tra i punti sulle ascisse e le ordinate
        float stepX = 1 * scaleX * cartesianZoom; // Distanza tra i punti sull'asse x
        float stepY = 1 * scaleY * cartesianZoom; // Distanza tra i punti sull'asse y

        // Calcola l'origine del piano in base al fattore di zoom
        float originX = chartX + 0;  // Origine x (non spostato)
        float originY = chartY + 6 * scaleY * cartesianZoom; // Origine y (spostato di 6 e considerando lo zoom)

        // Calcola il numero di punti lungo le ascisse e le ordinate in base alle dimensioni del rettangolo rosso
        int pointsCountX = (int)(redRect.Rectangle.Width / stepX);
        int pointsCountY = (int)(redRect.Rectangle.Height / stepY);

        // Disegna la griglia

        for (int i = -pointsCountX; i <= pointsCountX; i++)
        {
            float x = i * stepX + originX;
            int textX = (int)x;
            int textY = (int)redRect.Rectangle.Bottom + 2;

            string numberX = (i).ToString();  // Usa i come numero progressivo
            SizeF textSizeX = graphics.MeasureString(numberX, SystemFonts.DefaultFont);
            PointF textLocationX = new PointF(x, chartY + 4 * scaleY * cartesianZoom);

            graphics.DrawLine(new Pen(Color.Gray, 1), new Point((int)x, (int)redRect.Rectangle.Top), new Point((int)x, (int)redRect.Rectangle.Bottom));
            graphics.DrawString(numberX, SystemFonts.DefaultFont, Brushes.Black, textLocationX);
        }

        for (int i = -pointsCountY; i <= pointsCountY; i++)
        {
            float y = i * stepY + originY;

            // Linee orizzontali
            graphics.DrawLine(new Pen(Color.Gray, 1), new Point((int)redRect.Rectangle.Left, (int)y), new Point((int)redRect.Rectangle.Right, (int)y));
        }

        for (int system = 0; system < systemsCount; system++)
        {
            for (int point = 0; point < drawingData[system].Count - 1; point++)
            {

                startX = (int)(drawingData[system][point].Item1 * scaleX * cartesianZoom) + chartX;
                startY = (int)(drawingData[system][point].Item2 * scaleY * cartesianZoom) + chartY;
                endX = (int)(drawingData[system][point + 1].Item1 * scaleX * cartesianZoom) + chartX;
                endY = (int)(drawingData[system][point + 1].Item2 * scaleY * cartesianZoom) + chartY;

                // Disegna solo se il grafico è visibile
                graphics.DrawEllipse(new Pen(Color.Green, 2), startX, startY, 4, 4);
                graphics.DrawEllipse(new Pen(Color.Green, 2), endX, endY, 4, 4);
                graphics.DrawLine(new Pen(systemColors[system], 3), new Point(startX, startY), new Point(endX, endY));
            }
        }
    }

    public void DrawHistograms(Graphics graphics)
    {
        float barWidth = 1.0f / systemsCount * scaleX * cartesianZoom;

        // Ottieni le coordinate X delle linee verticali da utilizzare come ancoraggio
        float anchorXFirstSeries = (attacksCount / 2.0f) * scaleX * cartesianZoom + chartX;
        float anchorXSecondSeries = attacksCount * scaleX * cartesianZoom + chartX;

        // Calcola la posizione iniziale della prima serie (ancorata a "numero di attacchi / 2")
        float startXFirstSeries = anchorXFirstSeries - (barWidth * systemsCount / 2);

        // Calcola la posizione iniziale della seconda serie (ancorata a "numero di attacchi")
        float startXSecondSeries = anchorXSecondSeries - (barWidth * systemsCount / 2);

        float initialY = chartY + 4 * scaleY * cartesianZoom;  // Posizione iniziale delle barre

        for (int systemIndex = 0; systemIndex < systemsCount; systemIndex++)
        {
            float xFirstSeries = startXFirstSeries + systemIndex * barWidth;
            float xSecondSeries = startXSecondSeries + systemIndex * barWidth;

            int firstScore = drawingData[systemIndex][attacksCount / 2].Item2;
            int secondScore = drawingData[systemIndex][attacksCount].Item2;

            float normalizedFirstScore = (float)firstScore / 2.0f;
            float normalizedSecondScore = (float)secondScore / 2.0f;

            float initialHeight = 4 * scaleY * cartesianZoom;  // Altezza iniziale delle barre
            float firstBarHeight = normalizedFirstScore * scaleY * cartesianZoom;
            float secondBarHeight = normalizedSecondScore * scaleY * cartesianZoom;

            float yFirstSeries = initialY - initialHeight + firstBarHeight;  // Calcolo della nuova posizione Y delle barre
            float ySecondSeries = initialY - initialHeight + secondBarHeight;

            RectangleF barFirstSeries = new RectangleF(xFirstSeries, yFirstSeries, barWidth, initialHeight - firstBarHeight);
            RectangleF barSecondSeries = new RectangleF(xSecondSeries, ySecondSeries, barWidth, initialHeight - secondBarHeight);

            graphics.FillRectangle(new SolidBrush(systemColors[systemIndex]), barFirstSeries);
            graphics.FillRectangle(new SolidBrush(systemColors[systemIndex]), barSecondSeries);
        }
    }



    public void Draw()
    {
        using (Graphics graphics = Graphics.FromImage(chartbitmap))
        {
            // Calcola la regione di clipping basata sul resizableRect rosso
            clipRegion = new Region(redRect.Rectangle);
            graphics.SetClip(clipRegion, CombineMode.Intersect);

            // Calcola la posizione Y del grafico in modo che sia al centro dell'altezza del resizableRect rosso
            int centerY = redRect.Rectangle.Top + redRect.Rectangle.Height / 2;
            chartY = centerY - chartHeight / 2;
            DrawHistograms(graphics);
            DrawLines(graphics);

        }
        pictureBox.Invalidate();
    }



    internal Image GetChartBitmap()
    {
        return this.chartbitmap;
    }

    public void SetCartesianZoom(float zoomFactor)
    {
        cartesianZoom = zoomFactor;
    }

    public float GetCartesianZoom()
    {
        return this.cartesianZoom;
    }

    public void Zoom(float zoomFactor)
    {
        // Aggiorna il fattore di scala
        scaleX *= zoomFactor;
        scaleY *= zoomFactor;

        SetCartesianZoom(cartesianZoom * zoomFactor);

        // Ricalcola il disegno del grafico
        Draw();
    }

    public static Color GetRandomColor()
    {
        Random random = new Random();
        return Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
    }

    public List<List<Tuple<int, int>>> GetDrawingData()
    {
        return this.drawingData;
    }

    public List<List<Tuple<int, int>>> GetSystemScores()
    {
        return this.systemScores;
    }

    public List<Color> GetSystemColors()
    {
        return this.systemColors;
    }

    public void SetSystemsCount(int systemsCount)
    {
        this.systemsCount = systemsCount;
    }

    public void SetAttacksCount(int numberOfAttacks)
    {
        this.attacksCount = numberOfAttacks;
    }

    public void SetSystemColor(List<Color> systemColors)
    {
        this.systemColors = systemColors;
    }
}