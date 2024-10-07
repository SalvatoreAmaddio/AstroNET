using Backend.Database;
using FrontEnd.Dialogs;
using FrontEnd.Utils;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AstroNET.model;

namespace AstroNET.View
{

    public class ZodiacChart : UserControl
    {
        private List<MouseStack> _mouseUpHandlers = [];

        private double _rotation;
        private double _outerRadius => Math.Min(ActualWidth, ActualHeight) / 2;
        private double _innerRadius => _outerRadius - _outerRadius * 0.15;
        private double _finalRadius => _innerRadius - _innerRadius * 0.5;
        private Canvas _canvas = new();

        public static readonly DependencyProperty SkyProperty =
        DependencyProperty.Register(nameof(Sky), typeof(AbstractSkyEvent),
        typeof(ZodiacChart), new PropertyMetadata(OnSkyChanged));

        public AbstractSkyEvent Sky
        {
            get => (AbstractSkyEvent)GetValue(SkyProperty);
            set => SetValue(SkyProperty, value);
        }

        private static void OnSkyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ZodiacChart)d).InvalidateVisual();
            ((ZodiacChart)d).UpdateLayout();
        }

        public double Rotation
        {
            get => _rotation;
            set => _rotation = -Math.Abs(value) + 180; //(-120.45) + 180;
        }

        private SkyEvent? SkyEvent => Sky as SkyEvent;
        public Point Center => new(ActualWidth / 2, ActualHeight / 2);

        public ZodiacChart()
        {
            Content = _canvas;
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Window? parentWindow = Helper.GetActiveWindow();
            parentWindow!.Closed += OnParentWindowClosed;
        }

        private void OnParentWindowClosed(object? sender, EventArgs e)
        {
            Window? parentWindow = (Window?)sender;
            foreach(MouseStack stackHandler in _mouseUpHandlers) stackHandler.Unsubscribe();
            parentWindow!.Closed -= OnParentWindowClosed;
            Loaded -= OnLoaded;
        }

        private Point CalculateEndPoint(double lineDegree, double offset = 0, bool add = false)
        {
            lineDegree += Rotation;
            double outerRadius = add ? _outerRadius + offset : _outerRadius - offset;
            double radians = lineDegree * Math.PI / 180; // Convert degrees to radians
            double x = Center.X + outerRadius * Math.Cos(radians);
            double y = Center.Y - outerRadius * Math.Sin(radians); // Subtract because the y-axis is inverted in screen coordinates
            return new(x, y);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            _canvas.Children.Clear();
            
            if (Sky == null) return;

            try 
            {
                Rotation = Sky.Houses[0].EclipticLongitude;
            }
            catch 
            { 
            
            }

            drawingContext.DrawEllipse(Brushes.White, Factory.PencilCase.BlackPen2, Center, _outerRadius, _outerRadius);

            SplitCircle(drawingContext);

            drawingContext.DrawEllipse(Brushes.White, Factory.PencilCase.BlackPen1, Center, _innerRadius, _innerRadius);

            drawingContext.DrawEllipse(Brushes.White, Factory.PencilCase.BlackPen1, Center, _finalRadius, _finalRadius);

            AddSigns(drawingContext);

            if (SkyEvent != null && SkyEvent.Horoscope != null)
                AddHouses(drawingContext);
            else
                AddHouses(drawingContext);

            AddAspects(drawingContext, SkyEvent != null && SkyEvent.Horoscope != null);

            AddPlanets(drawingContext);

            if (SkyEvent != null && SkyEvent.Horoscope != null)
                AddPlanets(drawingContext, true);
        }

        private void SplitCircle(DrawingContext drawingContext)
        {
            double degree = 0;
            for (int i = 0; i <= 12; i++)
            {
                drawingContext.DrawLine(Factory.PencilCase.RedPen, Center, CalculateEndPoint(degree));
                degree += 30;
            }

            for (int i = 0; i <= 360; i++)
            {
                degree = i;
                if (Factory.IsMultipleOfFive(degree))
                {
                    drawingContext.DrawLine(Factory.PencilCase.BluePen1, Center, CalculateEndPoint(degree, 30));
                }
                else
                {
                    drawingContext.DrawLine(Factory.PencilCase.RedPen, Center, CalculateEndPoint(degree, 35));
                }
            }
        }


        #region Drawing
        private (Point start, Point end) PrepareAspectDrawing(ref Aspect aspect, double offset)
        {
            Point startPoint = CalculateEndPoint(aspect.PointA.EclipticLongitude, offset);
            Point endPoint = CalculateEndPoint(aspect.PointB.EclipticLongitude, offset);
            aspect.BuildBrush();
            return (startPoint, endPoint);
        }
        private void DrawCurvedAspect(DrawingContext drawingContext, Aspect aspect, double offset)
        {
            var (startPoint, endPoint) = PrepareAspectDrawing(ref aspect, offset);

            Pen pen = PencilCase.MakePen(aspect.Brush,5);

            PathFigure pathFigure = new ()
            {
                StartPoint = startPoint,
                IsClosed = false
            };

            ArcSegment arcSegment = new ()
            {
                Point = endPoint,
                Size = new Size(_innerRadius, _innerRadius), // Adjust the size to control the curvature
                SweepDirection = SweepDirection.Counterclockwise, // You can set this to Counterclockwise if needed
                IsLargeArc = false // Set to true if the angle should be greater than 180 degrees
            };

            pathFigure.Segments.Add(arcSegment);

            PathGeometry pathGeometry = new ();
            pathGeometry.Figures.Add(pathFigure);

            drawingContext.DrawGeometry(null, pen, pathGeometry);

            Line _overlayLine = Factory.MakeOverlayLine(startPoint, endPoint, aspect.Info);

            _canvas.Children.Add(_overlayLine);
        }
        private void DrawAspect(DrawingContext drawingContext, Aspect aspect, double offset)
        {
            var (startPoint, endPoint) = PrepareAspectDrawing(ref aspect, offset);
            Pen pen = PencilCase.MakePen(aspect.Brush,1, aspect.IsDashed);

            drawingContext.DrawLine(pen, startPoint, endPoint);

            Line _overlayLine = Factory.MakeOverlayLine(startPoint, endPoint, aspect.Info);

            _canvas.Children.Add(_overlayLine);
        }

        private void DrawHouse(DrawingContext drawingContext, House house)
        {
            if (house.IsAngular)
                drawingContext.DrawLine(Factory.PencilCase.BlackPen2, Center, CalculateEndPoint(house.EclipticLongitude));
            else
                drawingContext.DrawLine(Factory.PencilCase.BlackPenForHouse, Center, CalculateEndPoint(house.EclipticLongitude));

            Factory.PlaceHouse(_canvas, house, CalculateEndPoint(house.EclipticLongitude, _outerRadius / 12, true));
        }

        private void DrawPlanet(DrawingContext drawingContext, IStar star, Point endPoint, bool isHoroscope = false)
        {
            StackPanel stack =
                Factory.CreateStackPanel(
                    (! (SkyEvent != null && SkyEvent.ShowHouses))
                    ? $"{star} at {star.Position.DegreeAndMinutes}° {(star.IsRetrograde ? " R" : "")}" 
                    : $"{star} at {star.Position.DegreeAndMinutes}° in {star.House} {(star.IsRetrograde ? " R" : "")}"
                    );

            stack.Children.Add(Factory.MakePlanetImage(star.URI));
            stack.Children.Add(Factory.CreateDegreeLabel($"{star.Position.DegreeAndMinutes}°{(star.IsRetrograde ? "R" : "")}"));

            _mouseUpHandlers.Add(new(MakeMouseButtonEventHandler(isHoroscope, star), stack));

            Point adjustedEndPoint = new(endPoint.X, endPoint.Y + 10);
            Factory.PlaceStackPanel(_canvas, stack, adjustedEndPoint);

            Point shortenPoint = ShortenLineFromCenter(Center, endPoint, (!isHoroscope) ? - 20 : 75);
            drawingContext.DrawLine(Factory.PencilCase.BlackPen05, shortenPoint, endPoint);
        }

        private MouseButtonEventHandler MakeMouseButtonEventHandler(bool isHoroscope, IStar star) 
        {
            return (o, e) =>
            {
                string title = "Radix Aspects";
                StringBuilder sb = new();

                IEnumerable<IAspect> aspects = Sky.RadixAspects.Where(s => s.PointA.PointId == star.PointId);

                if (isHoroscope)
                {
                    aspects = SkyEvent?.Horoscope?.RadixAspects.Where(s => s.PointA.PointId == star.PointId)!;
                    title = "Transit Aspects";
                }

                if (aspects.Count() == 0)
                    sb.Append("NO GIVING ASPECTS");

                foreach (Aspect aspect in aspects)
                {
                    sb.Append(aspect.Info);
                    sb.Append('\n');
                }

                MessageBox.Show(sb.ToString(), title);
            };
        }

        Point ShortenLineFromCenter(Point center, Point endPoint, double shortenBy)
        {
            // Calculate the vector from center to endPoint
            double dx = endPoint.X - center.X;
            double dy = endPoint.Y - center.Y;

            // Calculate the current distance (length) of the line
            double distance = Math.Sqrt(dx * dx + dy * dy);

            // Calculate the scale factor to move the start point away from the center
            double scale = (distance - shortenBy) / distance;

            // Calculate the new start point closer to the end point, moving outward from the center
            double newX = center.X + dx * scale;
            double newY = center.Y + dy * scale;

            // Return the new start point
            return new Point(newX, newY);
        }

        #endregion

        private void AddAspects(DrawingContext drawingContext, bool isHoroscope = false)
        {
            double offset = 0;
            double conjOffset = 0;
            IEnumerable<IAspect> Aspects;

            if (isHoroscope) 
            {
                offset = _finalRadius / 3;
                conjOffset = 1;
                Aspects = SkyEvent?.Horoscope?.RadixAspects!;
            }
            else 
            {
                offset = _finalRadius / .74;
                conjOffset = 3;
                Aspects = Sky.RadixAspects;
            }

            foreach (Aspect aspect in Aspects)
            {
                if (aspect.Orbit == 0) 
                    DrawCurvedAspect(drawingContext, aspect, offset - conjOffset);
                else 
                    DrawAspect(drawingContext, aspect, offset);
            }
        }
        private void AddPlanets(DrawingContext drawingContext, bool isHoroscope = false)
        {
            double diff = 0;
            double offset = 0;

            List<IStar> prev_stars = [];

            List<IStar> stars = (isHoroscope) ? new(SkyEvent?.Horoscope?.Stars!) : new(Sky.Stars);
            
            for (int i = 0; i <= stars.Count - 1; i++) 
            {
                offset = (isHoroscope) ? (_outerRadius / 1000) - 40 : _outerRadius / 4.5;
                diff = 4.5;
                prev_stars.Add(stars[i]);

                if (i > 0)
                { 
                    foreach (IStar star in prev_stars)
                    {
                        if (stars[i].PointId == star.PointId) continue;
                        double distance = Math.Abs(PositionCalculator.CalculateDistance(stars[i].EclipticLongitude, star.EclipticLongitude));
//                        IAspect? conj = PositionCalculator.IsConjunction(distance, 10);
                        if (!isHoroscope) 
                        {
                            if (distance <= 10)
                            {
                                diff = diff - .8;
                                offset = _outerRadius / diff;
                            }
                        }
                    }
                }

                DrawPlanet(drawingContext, stars[i], CalculateEndPoint(stars[i].EclipticLongitude, offset), isHoroscope);
            }
        }
        private void AddHouses(DrawingContext drawingContext)
        {
            foreach (House house in Sky.Houses)
                DrawHouse(drawingContext, house);
        }
        private void AddSigns(DrawingContext drawingContext)
        {
            double less = _outerRadius / 12.5;
            List<Sign> signs = DatabaseManager.Find<Sign>()!.MasterSource.Cast<Sign>().ToList();

            foreach (Sign sign in signs) 
                Factory.PlaceImage(sign.URI, CalculateEndPoint(sign.Start + 15, less), _outerRadius, drawingContext);
        }

        public void Screenshot(string filePath, double cropWidth = 650)
        {
            double width = this.RenderSize.Width;
            double height = this.RenderSize.Height + this.Margin.Top + this.Margin.Bottom + 60;

            if (width <= 0 || height <= 0)
            {
                MessageBox.Show("The control's width or height is invalid for capturing the screenshot.");
                return;
            }

            double targetWidth = cropWidth > 0 && cropWidth < width ? cropWidth : width;

            RenderTargetBitmap renderBitmap = new(
                (int)width, (int)height, 96d, 96d, PixelFormats.Pbgra32);

            // Render the control's current visual tree directly into the bitmap
            renderBitmap.Render(this);

            // Calculate the horizontal offset to center the crop (if needed)
            double horizontalOffset = (width - targetWidth) / 2;

            // Now create a cropped version of the RenderTargetBitmap
            CroppedBitmap croppedBitmap = new(renderBitmap,
                new Int32Rect((int)horizontalOffset, 0, (int)targetWidth, (int)height));

            // Now create a new RenderTargetBitmap with the cropped width but same height
            RenderTargetBitmap finalBitmap = new(
                (int)targetWidth, (int)height, 96d, 96d, PixelFormats.Pbgra32);

            // Create a DrawingVisual to draw the white background and overlay the cropped control
            DrawingVisual visual = new();
            using (DrawingContext context = visual.RenderOpen())
            {
                // Draw a white rectangle as the background
                context.DrawRectangle(Brushes.White, null, new Rect(0, 0, targetWidth, height));

                // Draw the cropped portion of the control
                context.DrawImage(croppedBitmap, new Rect(0, 0, targetWidth, height));
            }

            // Render the visual (white background + cropped control's content) into the final bitmap
            finalBitmap.Render(visual);

            // Create a PNG encoder to save the image
            PngBitmapEncoder encoder = new();
            encoder.Frames.Add(BitmapFrame.Create(finalBitmap));

            // Save the image to a file
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                encoder.Save(fileStream);
            }

            SuccessDialog.Display("Chart saved successfully!");
        }

        //public void Screenshot(string filePath)
        //{
        //    // Get the control's rendered size
        //    double width = this.RenderSize.Width;
        //    double height = this.RenderSize.Height + this.Margin.Top + this.Margin.Bottom + 30; // Correct margin handling

        //    if (width <= 0 || height <= 0)
        //    {
        //        MessageBox.Show("The control's width or height is invalid for capturing the screenshot.");
        //        return;
        //    }

        //    // Create a RenderTargetBitmap to render the control's content
        //    RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
        //        (int)width, (int)height, 96d, 96d, PixelFormats.Pbgra32);

        //    // Render the control's current visual tree directly into the bitmap (this preserves the correct sizes)
        //    renderBitmap.Render(this);

        //    // Now create a new RenderTargetBitmap with the same size but including a white background
        //    RenderTargetBitmap finalBitmap = new RenderTargetBitmap(
        //        (int)width, (int)height, 96d, 96d, PixelFormats.Pbgra32);

        //    // Create a DrawingVisual to draw the white background and overlay the rendered control
        //    DrawingVisual visual = new DrawingVisual();
            
        //    using (DrawingContext context = visual.RenderOpen())
        //    {
        //        // Draw a white rectangle as the background (this won't affect the control's size)
        //        context.DrawRectangle(Brushes.White, null, new Rect(0, 0, width, height));

        //        // Draw the control's rendered bitmap on top of the white background
        //        context.DrawImage(renderBitmap, new Rect(0, 0, width, height));
        //    }

        //    // Render the visual (white background + control's content) into the final bitmap
        //    finalBitmap.Render(visual);

        //    // Create a PNG encoder to save the image
        //    PngBitmapEncoder encoder = new PngBitmapEncoder();
        //    encoder.Frames.Add(BitmapFrame.Create(finalBitmap));

        //    // Save the image to a file
        //    using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
        //    {
        //        encoder.Save(fileStream);
        //    }

        //    MessageBox.Show("Screenshot with white background saved successfully.");
        //}

        private class MouseStack
        {
            private MouseButtonEventHandler _handler;
            private StackPanel _panel;

            public MouseStack(MouseButtonEventHandler handler, StackPanel panel)
            {
                this._handler = handler;
                this._panel = panel;
                this._panel.MouseUp += this._handler;
            }

            public void Unsubscribe() => _panel.MouseUp -= _handler;
        }
    }
}