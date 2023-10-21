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
    private List<List<int>> systemsScores; // Una lista di liste per memorizzare i punteggi di diversi sistemi.
    private int attacksCount; // Numero di attacchi.
    public int chartWidth; // Larghezza del grafico.
    public int chartHeight; // Altezza del grafico.
    public int chartX; // Posizione X del grafico all'interno del rettangolo rosso.
    public int chartY; // Posizione Y del grafico all'interno del rettangolo rosso.
    private float scaleX = 100; // Fattore di scala per adattare il grafico alle dimensioni del rettangolo rosso.
    private float scaleY = 100;
    private PictureBox pictureBox; // Riferimento al PictureBox su cui disegnare il grafico.
    private int systemsCount;
    private int currentSystem; // Sistema attualmente selezionato per il grafico.
    private Bitmap chartbitmap;
    private ResizableRect redRect;
    private Region clipRegion;
    List<List<Tuple<int, int>>> drawingData;
    List<Tuple<int, int>> systemData;
    private int redRectX;
    private int redRectY;
    public int redRectTop;
    private int redRectHeight;
    private float cartesianZoom = 1.0f;
    public List<Color> systemColors;
    private int currentPoint = 0;




    public SecurityScoreChart(int attacksCount, int chartWidth, int chartHeight, int chartX, int chartY, int systemsCount, Bitmap chartbitmap, ResizableRect redRect)
    {
        this.attacksCount = attacksCount;
        this.chartWidth = chartWidth;
        this.chartHeight = chartHeight;
        this.chartX = chartX;
        this.chartY = chartY;
        this.chartbitmap = chartbitmap;
        this.systemsCount = systemsCount; // Inizializza il numero di sistemi.
        currentSystem = 0;
        this.redRect = redRect;
        redRectX = redRect.Rectangle.X;
        redRectY = redRect.Rectangle.Y;
        redRectHeight = redRect.Rectangle.Height;

        drawingData = new List<List<Tuple<int, int>>>();
        systemColors = new List<Color>();
        for (int i = 0; i < systemsCount; i++)
        {
            systemColors.Add(GetRandomColor());
        }

        // Aggiungi dati di disegno per ogni sistema
        for (int i = 0; i < systemsCount; i++)
        {
            systemData = new List<Tuple<int, int>>();
            systemData.Add(new Tuple<int, int>(0, 4));
            drawingData.Add(systemData);
        }



        calculateScores();


    }



    public void SetPictureBox(PictureBox pictureBox)
    {
        this.pictureBox = pictureBox;
    }

    public PictureBox GetPictureBox()
    {
        return this.pictureBox;
    }

    public void calculateScores()
    {
        Random random = new Random();

        for (int currentSystem = 0; currentSystem < systemsCount; currentSystem++)
        {
            for (int attack = 1; attack <= attacksCount; attack++)
            {
                double successProbability = random.NextDouble();

                // Calcola l'esito dell'attacco e aggiorna il punteggio.
                if (successProbability > 0.5)
                {
                    int precY = drawingData[currentSystem].Last().Item2;
                    drawingData[currentSystem].Add(new Tuple<int, int>(attack, precY + 1));
                }
                else
                {
                    int precY = drawingData[currentSystem].Last().Item2;
                    drawingData[currentSystem].Add(new Tuple<int, int>(attack, precY - 1));
                }
            }
        }
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

}