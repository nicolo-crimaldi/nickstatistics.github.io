using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Media;
using static System.Windows.Forms.AxHost;


public class FrequencyChart
{
    public int chartWidth; // Larghezza del grafico.
    public int chartHeight; // Altezza del grafico.
    public int chartX; // Posizione X del grafico all'interno del rettangolo 
    public int chartY; // Posizione Y del grafico all'interno del rettangolo 
    private float scaleX = 100; // Fattore di scala per adattare il grafico alle dimensioni del rettangolo.
    private float scaleY = 100;
    private PictureBox pictureBox; // Riferimento al PictureBox su cui disegnare il grafico.
    private Bitmap chartbitmap;
    private ResizableRect rect;
    private Region clipRegion;
    private List<List<Tuple<float, float>>> frequencyDrawingData;
    private List<List<Tuple<float, float>>> relFrequencyDrawingData;
    private List<List<Tuple<float, float>>> normalFrequencyDrawingData;
    private List<List<Tuple<int, int>>> drawingData;
    private float cartesianZoom = 1.0f;
    public List<Color> systemColors;
    private int numberOfAttacks;
    private int systemsCount;

    public FrequencyChart(int chartWidth, int chartHeight, int chartX, int chartY, int numberOfAttacks, Bitmap chartbitmap, ResizableRect rect,
        int systemsCount, List<List<Tuple<int, int>>> systemScores, List<Color> systemColors)
    {
        this.chartWidth = chartWidth;
        this.chartHeight = chartHeight;
        this.chartX = chartX;
        this.chartY = chartY;
        this.chartbitmap = chartbitmap;
        this.rect = rect;
        this.numberOfAttacks = numberOfAttacks;
        this.systemsCount = systemsCount;
        this.systemColors = systemColors;

        frequencyDrawingData = new List<List<Tuple<float, float>>>();
        relFrequencyDrawingData = new List<List<Tuple<float, float>>>();
        normalFrequencyDrawingData = new List<List<Tuple<float, float>>>();

        List<List<Tuple<float, float>>> drawData = CalculateCumulativeFrequency(systemScores);
        CalculateRelativeFrequency(drawData);
        CalculateNormalizedFrequency(drawData);
    }


    public void SetPictureBox(PictureBox pictureBox)
    {
        this.pictureBox = pictureBox;
    }

    public PictureBox GetPictureBox()
    {
        return this.pictureBox;
    }


    public List<List<Tuple<float, float>>> CalculateCumulativeFrequency(List<List<Tuple<int, int>>> systemScores)
    {
        frequencyDrawingData.Clear();

        for (int currentSystem = 0; currentSystem < this.systemsCount; currentSystem++)
        {
            List<Tuple<float, float>> systemDataF = new List<Tuple<float, float>>();
            systemDataF.Add(new Tuple<float, float>(0, 4));
            frequencyDrawingData.Add(systemDataF);
            for (int attackNo = 1; attackNo <= this.numberOfAttacks; attackNo++)
            {
                if (systemScores[currentSystem][attackNo].Item2 == 1)
                {
                    float precY = this.frequencyDrawingData[currentSystem].Last().Item2;
                    this.frequencyDrawingData[currentSystem].Add(new Tuple<float, float>(attackNo, precY));
                }
                else
                {
                    float precY = this.frequencyDrawingData[currentSystem].Last().Item2;
                    this.frequencyDrawingData[currentSystem].Add(new Tuple<float, float>(attackNo, precY - 1));
                }
            }
        }
        return frequencyDrawingData;
    }

    public void CalculateRelativeFrequency(List<List<Tuple<float, float>>> frequencyDrawingData)
    {
        relFrequencyDrawingData.Clear();
        for (int currentSystem = 0; currentSystem < this.systemsCount; currentSystem++)
        {
            List<Tuple<float, float>> systemDataF = new List<Tuple<float, float>>();
            systemDataF.Add(new Tuple<float, float>(0, 4));
            relFrequencyDrawingData.Add(systemDataF);
            for (int attackNo = 1; attackNo <= this.numberOfAttacks; attackNo++)
            {
                float precY = this.relFrequencyDrawingData[currentSystem].Last().Item2;
                this.relFrequencyDrawingData[currentSystem].Add(new Tuple<float, float>(attackNo, (float)((precY + frequencyDrawingData[currentSystem][attackNo].Item2)) / numberOfAttacks));
            }
        }
    }

    public void CalculateNormalizedFrequency(List<List<Tuple<float, float>>> frequencyDrawingData)
    {
        normalFrequencyDrawingData.Clear();
        for (int currentSystem = 0; currentSystem < this.systemsCount; currentSystem++)
        {
            List<Tuple<float, float>> systemDataF = new List<Tuple<float, float>>();
            systemDataF.Add(new Tuple<float, float>(0, 4));
            normalFrequencyDrawingData.Add(systemDataF);
            for (int attackNo = 1; attackNo <= numberOfAttacks; attackNo++)
            {
                float precY = this.normalFrequencyDrawingData[currentSystem].Last().Item2;
                this.normalFrequencyDrawingData[currentSystem].Add(new Tuple<float, float>(attackNo, (float)((precY + frequencyDrawingData[currentSystem][attackNo].Item2) / Math.Sqrt(numberOfAttacks))));
            }
        }
    }

    public void DrawLine(Graphics graphics, ResizableRect rect)
    {
        // Distanza tra i punti sulle ascisse e le ordinate
        float stepX = 1 * scaleX * cartesianZoom; // Distanza tra i punti sull'asse x
        float stepY = 1 * scaleY * cartesianZoom; // Distanza tra i punti sull'asse y

        // Calcola l'origine del piano in base al fattore di zoom
        float originX = chartX + 0;  // Origine x (non spostato)
        float originY = chartY + 6 * scaleY * cartesianZoom; // Origine y (spostato di 6 e considerando lo zoom)

        // Calcola il numero di punti lungo le ascisse e le ordinate in base alle dimensioni del rettangolo rosso
        int pointsCountX = (int)(rect.Rectangle.Width / stepX);
        int pointsCountY = (int)(rect.Rectangle.Height / stepY);

        // Disegna la griglia

        for (int i = -pointsCountX; i <= pointsCountX; i++)
        {
            float x = i * stepX + originX;
            int textX = (int)x;
            int textY = (int)rect.Rectangle.Bottom + 2;

            string numberX = (i).ToString();  // Usa i come numero progressivo
            SizeF textSizeX = graphics.MeasureString(numberX, SystemFonts.DefaultFont);
            System.Drawing.PointF textLocationX = new System.Drawing.PointF(x, chartY + 4 * scaleY * cartesianZoom);

            graphics.DrawLine(new Pen(Color.Gray, 1), new Point(textX, (int)rect.Rectangle.Top), new Point(textX, (int)rect.Rectangle.Bottom));
            graphics.DrawString(numberX, SystemFonts.DefaultFont, Brushes.Black, textLocationX);
        }

        for (int i = -pointsCountY; i <= pointsCountY; i++)
        {
            float y = i * stepY + originY;

            // Linee orizzontali
            graphics.DrawLine(new Pen(Color.Gray, 1), new Point((int)rect.Rectangle.Left, (int)y), new Point((int)rect.Rectangle.Right, (int)y));
        }
    }

    public void DrawFrequency(Graphics graphics, List<List<Tuple<float, float>>> systemDrawing)
    {
        float startX = 0;
        float startY = 0;
        float endX = 0;
        float endY = 0;
        for (int system = 0; system < systemsCount; system++)
        {

            for (int point = 0; point < systemDrawing[system].Count - 1; point++)
            {

                startX = (systemDrawing[system][point].Item1 * scaleX * cartesianZoom) + chartX;
                startY = (systemDrawing[system][point].Item2 * scaleY * cartesianZoom) + chartY;
                endX = (systemDrawing[system][point + 1].Item1 * scaleX * cartesianZoom) + chartX;
                endY = (systemDrawing[system][point + 1].Item2 * scaleY * cartesianZoom) + chartY;

                // Disegna solo se il grafico è visibile
                graphics.DrawEllipse(new Pen(Color.Green, 2), startX, startY, 4, 4);
                graphics.DrawEllipse(new Pen(Color.Green, 2), endX, endY, 4, 4);
                DrawFloatLine(graphics, new Pen(systemColors[system], 3), new PointF(startX, startY), new PointF(endX, endY));
            }
        }
    }

    public struct PointF
    {
        public float X { get; set; }
        public float Y { get; set; }

        public PointF(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
    // Disegna una linea approssimata tra due punti con coordinate float.
    private void DrawFloatLine(Graphics g, Pen pen, PointF startPoint, PointF endPoint)
    {
        float step = 1.0f / Math.Max(Math.Abs(endPoint.X - startPoint.X), Math.Abs(endPoint.Y - startPoint.Y));

        for (float t = 0; t <= 1; t += step)
        {
            float x = startPoint.X + t * (endPoint.X - startPoint.X);
            float y = startPoint.Y + t * (endPoint.Y - startPoint.Y);
            g.DrawRectangle(pen, x, y, 1, 1); // Disegna un pixel
        }
    }

    public void DrawBlueRectFrequency(Bitmap bitmap)
    {
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            clipRegion = new Region(rect.Rectangle);
            graphics.SetClip(clipRegion, CombineMode.Intersect);
            chartX = rect.Rectangle.X;
            chartY = rect.Rectangle.Y;
            int centerBlueY = rect.Rectangle.Top + rect.Rectangle.Height / 2;
            chartY = centerBlueY - chartHeight / 2;
            DrawLine(graphics, rect);
            DrawFrequency(graphics, frequencyDrawingData);
            DrawHistograms(graphics, frequencyDrawingData);
            graphics.ResetClip();
        }
        pictureBox.Invalidate();
    }

    public void DrawGreenRectFrequency(Bitmap bitmap)
    {
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            clipRegion = new Region(rect.Rectangle);
            graphics.SetClip(clipRegion, CombineMode.Intersect);
            chartX = rect.Rectangle.X;
            chartY = rect.Rectangle.Y;
            int centerGreenY = rect.Rectangle.Top + rect.Rectangle.Height / 2;
            chartY = centerGreenY - chartHeight / 2;
            DrawLine(graphics, rect);
            DrawFrequency(graphics, relFrequencyDrawingData);
            DrawHistograms(graphics, relFrequencyDrawingData);
            graphics.ResetClip();
        }
        pictureBox.Invalidate();
    }

    public void DrawOrangeRectFrequency(Bitmap bitmap)
    {
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            clipRegion = new Region(rect.Rectangle);
            graphics.SetClip(clipRegion, CombineMode.Intersect);
            chartX = rect.Rectangle.X;
            chartY = rect.Rectangle.Y;
            int centerOrangeY = rect.Rectangle.Top + rect.Rectangle.Height / 2;
            chartY = centerOrangeY - chartHeight / 2;
            DrawLine(graphics, rect);
            DrawFrequency(graphics, normalFrequencyDrawingData);
            DrawHistograms(graphics, normalFrequencyDrawingData);
            graphics.ResetClip();
        }
        pictureBox.Invalidate();
    }

    public void DrawHistograms(Graphics graphics, List<List<Tuple<float, float>>> drawingData)
    {
        float barWidth = 1.0f / systemsCount * scaleX * cartesianZoom;

        // Ottieni le coordinate X delle linee verticali da utilizzare come ancoraggio
        float anchorXFirstSeries = (numberOfAttacks / 2.0f) * scaleX * cartesianZoom + chartX;
        float anchorXSecondSeries = numberOfAttacks * scaleX * cartesianZoom + chartX;

        // Calcola la posizione iniziale della prima serie (ancorata a "numero di attacchi / 2")
        float startXFirstSeries = anchorXFirstSeries - (barWidth * systemsCount / 2);

        // Calcola la posizione iniziale della seconda serie (ancorata a "numero di attacchi")
        float startXSecondSeries = anchorXSecondSeries - (barWidth * systemsCount / 2);

        float initialY = chartY + 4 * scaleY * cartesianZoom;  // Posizione iniziale delle barre

        for (int systemIndex = 0; systemIndex < systemsCount; systemIndex++)
        {
            float xFirstSeries = startXFirstSeries + systemIndex * barWidth;
            float xSecondSeries = startXSecondSeries + systemIndex * barWidth;

            float firstScore = drawingData[systemIndex][numberOfAttacks / 2].Item2;
            float secondScore = drawingData[systemIndex][numberOfAttacks].Item2;

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



    internal Image GetChartBitmap()
    {
        return this.chartbitmap;
    }

    public void setDrawingData(List<List<Tuple<int, int>>> drawingData)
    {
        this.drawingData = drawingData;
    }

    public void Zoom(float zoomFactor)
    {
        // Aggiorna il fattore di scala
        scaleX *= zoomFactor;
        scaleY *= zoomFactor;

        SetCartesianZoom(cartesianZoom * zoomFactor);
    }

    public void SetCartesianZoom(float zoomFactor)
    {
        cartesianZoom = zoomFactor;
    }

    public void SetSystemsCount(int systemsCount)
    {
        this.systemsCount = systemsCount;
    }

    public void SetNumberOfAttacks(int numberOfAttacks)
    {
        this.numberOfAttacks = numberOfAttacks;
    }

    public void SetSystemColors(List<Color> getSystemColors)
    {
        this.systemColors = systemColors;
    }
}