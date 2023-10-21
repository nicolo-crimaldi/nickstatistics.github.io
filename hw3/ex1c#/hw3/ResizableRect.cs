using System;
using System.Drawing;
using System.Collections.Generic;

public class ResizableRect
{
    private Rectangle rectangle;
    private bool isResizing;
    private bool isMoving;
    private Point resizeStart;
    private bool isResizingTop;
    private Point resizeStartTop;
    private Point moveStart;
    private Color rectColor;
    private Random random;
    private Bitmap canvas;

    public ResizableRect(Rectangle rect, Color color, Bitmap canvas)
    {
        rectangle = rect;
        rectColor = color;
        random = new Random();
        this.canvas = canvas;
    }

    public Rectangle Rectangle
    {
        get { return rectangle; }
    }

    public Color RectColor
    {
        get { return rectColor; }
        set { rectColor = value; }
    }

    public void Draw(Graphics graphics)
    {
        using (Pen pen = new Pen(rectColor, 2))
        {
            graphics.DrawRectangle(pen, rectangle);
        }
    }

    public void MouseDown(Point location)
    {
        if (rectangle.Contains(location))
        {
            if (location.X > rectangle.Left + 8 && location.X < rectangle.Right - 8)
            {
                isMoving = true;
                moveStart = location;
            }
            else if (location.Y < rectangle.Top + 8)
            {
                isResizingTop = true;
                resizeStartTop = location;
            }
            else
            {
                isResizing = true;
                resizeStart = location;
            }
        }
    }

    public void MouseMove(Point location)
    {
        if (isResizing)
        {
            int newWidth = rectangle.Width + location.X - resizeStart.X;
            int newHeight = rectangle.Height + location.Y - resizeStart.Y;
            int newLeft = rectangle.Left;
            int newTop = rectangle.Top;

            if (newWidth < 0)
            {
                newLeft = rectangle.Left + newWidth;
                newWidth = Math.Abs(newWidth);
            }

            if (newHeight < 0)
            {
                newTop = rectangle.Top + newHeight;
                newHeight = Math.Abs(newHeight);
            }

            rectangle = new Rectangle(newLeft, newTop, newWidth, newHeight);
            resizeStart = location;
        }
        else if (isMoving)
        {
            int deltaX = location.X - moveStart.X;
            int deltaY = location.Y - moveStart.Y;
            int newLeft = rectangle.Left + deltaX;
            int newTop = rectangle.Top + deltaY;
            int newWidth = rectangle.Width;
            int newHeight = rectangle.Height;
            rectangle = new Rectangle(newLeft, newTop, newWidth, newHeight);
            moveStart = location;
        }
        else if (isResizingTop)
        {
            int newHeight = rectangle.Height + rectangle.Top - location.Y;
            int newTop = location.Y;

            if (newHeight < 0)
            {
                newTop = rectangle.Top + newHeight;
                newHeight = Math.Abs(newHeight);
            }

            rectangle = new Rectangle(rectangle.Left, newTop, rectangle.Width, newHeight);
            resizeStartTop = location;
        }
    }

    public void MouseUp()
    {
        isResizing = false;
        isMoving = false;
        isResizingTop = false;
    }
}