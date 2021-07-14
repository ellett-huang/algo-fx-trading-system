using AlgoTrade.Common.Entities;
using AlgoTrade.DAL.Provider;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace AlgoTradeClient.ChartControls
{
    public class ChartLines : FrameworkElement
    {

      

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (VerticalAxisTickPositions != null)
            {
                if (DrawVerticalAxisReferenceLines)
                {
                    for (int i = 0; i < VerticalAxisTickPositions.Count; i++)
                    {
                        drawingContext.DrawLine(new Pen(Brushes.Gray, 0.5), new Point(2, VerticalAxisTickPositions[i].Location.X), new Point(RenderSize.Width, VerticalAxisTickPositions[i].Location.X));
                    }
                }
                else if(DrawVerticalAxisTickMarks)
                {
                    for (int i = 0; i < VerticalAxisTickPositions.Count; i++)
                    {
                        drawingContext.DrawLine(ReferenceLinePen, new Point(VerticalAxis - TickMarksLength / 2, VerticalAxisTickPositions[i].Location.Y), new Point(VerticalAxis + TickMarksLength / 2, VerticalAxisTickPositions[i].Location.Y));
                     
                    }
                }
            }
            drawingContext.DrawLine(ReferenceLinePen, new Point(VerticalAxis, 0), new Point(VerticalAxis, RenderSize.Height));
            if (HorizontalAxisTickPositions != null)
            {
                if (DrawHorizontalAxisReferenceLines)
                {
                    foreach(ChartPosition  HorizontalAxisTickPosition in HorizontalAxisTickPositions)
                    {
                        if ((HorizontalAxisTickPosition.TradingDataInfo.TradeDateTime.Hour == 0 ||
                             HorizontalAxisTickPosition.TradingDataInfo.TradeDateTime.Hour == 12
                            ) )
                        {
                            drawingContext.DrawLine(new Pen(Brushes.Gray, 0.5), new Point(HorizontalAxisTickPosition.Location.X, TickMarksLength / 2), new Point(HorizontalAxisTickPosition.Location.X, RenderSize.Height - TickMarksLength-20 ));
                           if(HorizontalAxisTickPosition.TradingDataInfo.TradeDateTime.Hour == 0 && HorizontalAxisTickPositions.IndexOf(HorizontalAxisTickPosition)>2
                               && HorizontalAxisTickPositions.IndexOf(HorizontalAxisTickPosition)<HorizontalAxisTickPositions.Count-2)
                                drawingContext.DrawText(new FormattedText(HorizontalAxisTickPosition.TradingDataInfo.TradeDateTime.ToString("MM-dd"), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 10, Brushes.Black), new Point(HorizontalAxisTickPosition.Location.X-12, RenderSize.Height - TickMarksLength-10 ));
                        }
                    }
                }
                else if (DrawHorizontalAxisTickMarks)
                {
                    foreach (ChartPosition HorizontalAxisTickPosition in HorizontalAxisTickPositions)
                    {
                        if (HorizontalAxisTickPosition.TradingDataInfo.TradeDateTime.Minute == 0 ||
                             HorizontalAxisTickPosition.TradingDataInfo.TradeDateTime.Minute == 30
                            )
                            drawingContext.DrawLine(new Pen(Brushes.Gray, 0.5), new Point(HorizontalAxisTickPosition.Location.X, HorizontalAxis - TickMarksLength / 2), new Point(HorizontalAxisTickPosition.Location.X, HorizontalAxis + TickMarksLength / 2));
                       
                    }
                }
            }
            drawingContext.DrawLine(ReferenceLinePen, new Point(2, HorizontalAxis), new Point(RenderSize.Width, HorizontalAxis));
        }

        public static void OnVerticalAxisTickValuesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ChartLines obj = sender as ChartLines;
            if (obj != null && obj.VerticalAxisTickPositions != null)
            {
                obj.InvalidateVisual();
                obj.VerticalAxisTickPositions.CollectionChanged += new NotifyCollectionChangedEventHandler(obj.VerticalAxisTickPositions_CollectionChanged);
            }
        }

        public void VerticalAxisTickPositions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            InvalidateVisual();                        
        }

        public Pen ReferenceLinePen
        {
            get { return (Pen)GetValue(ReferenceLinePenProperty); }
            set { SetValue(ReferenceLinePenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ReferenceLinePen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReferenceLinePenProperty =
            DependencyProperty.Register("ReferenceLinePen", typeof(Pen), typeof(ChartLines), new UIPropertyMetadata(new Pen(Brushes.Gray,0.5)));


        public ObservableCollection<ChartPosition> VerticalAxisTickPositions
        {
            get { return (ObservableCollection<ChartPosition>)GetValue(VerticalAxisTickPositionsProperty); }
            set { SetValue(VerticalAxisTickPositionsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerticalAxisTickPositions.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalAxisTickPositionsProperty =
            DependencyProperty.Register("VerticalAxisTickPositions", typeof(ObservableCollection<ChartPosition>), typeof(ChartLines), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnVerticalAxisTickValuesChanged)));

        public ObservableCollection<ChartPosition> HorizontalAxisTickPositions
        {
            get { return (ObservableCollection<ChartPosition>)GetValue(HorizontalAxisTickPositionsProperty); }
            set { SetValue(HorizontalAxisTickPositionsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorizontalAxisTickPositions.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalAxisTickPositionsProperty =
            DependencyProperty.Register("HorizontalAxisTickPositions", typeof(ObservableCollection<ChartPosition>), typeof(ChartLines), new UIPropertyMetadata(null));

        public double TickMarksLength
        {
            get { return (double)GetValue(TickMarksLengthProperty); }
            set { SetValue(TickMarksLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TickMarksLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TickMarksLengthProperty =
            DependencyProperty.Register("TickMarksLength", typeof(double), typeof(ChartLines), new UIPropertyMetadata(8.0));


        public bool DrawVerticalAxisTickMarks
        {
            get { return (bool)GetValue(DrawVerticalAxisTickMarksProperty); }
            set { SetValue(DrawVerticalAxisTickMarksProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DrawVerticalAxisTickMarks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DrawVerticalAxisTickMarksProperty =
            DependencyProperty.Register("DrawVerticalAxisTickMarks", typeof(bool), typeof(ChartLines), new UIPropertyMetadata(true));


        public bool DrawVerticalAxisReferenceLines
        {
            get { return (bool)GetValue(DrawVerticalAxisReferenceLinesProperty); }
            set { SetValue(DrawVerticalAxisReferenceLinesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DrawVerticalAxisReferenceLines.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DrawVerticalAxisReferenceLinesProperty =
            DependencyProperty.Register("DrawVerticalAxisReferenceLines", typeof(bool), typeof(ChartLines), new UIPropertyMetadata(true));


        public bool DrawHorizontalAxisTickMarks
        {
            get { return (bool)GetValue(DrawHorizontalAxisTickMarksProperty); }
            set { SetValue(DrawHorizontalAxisTickMarksProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DrawHorizontalAxisTickMarks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DrawHorizontalAxisTickMarksProperty =
            DependencyProperty.Register("DrawHorizontalAxisTickMarks", typeof(bool), typeof(ChartLines), new UIPropertyMetadata(true));


        public bool DrawHorizontalAxisReferenceLines
        {
            get { return (bool)GetValue(DrawHorizontalAxisReferenceLinesProperty); }
            set { SetValue(DrawHorizontalAxisReferenceLinesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DrawHorizontalAxisReferenceLines.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DrawHorizontalAxisReferenceLinesProperty =
            DependencyProperty.Register("DrawHorizontalAxisReferenceLines", typeof(bool), typeof(ChartLines), new UIPropertyMetadata(true));


        public double HorizontalAxis
        {
            get { return (double)GetValue(HorizontalAxisProperty); }
            set { SetValue(HorizontalAxisProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorizontalAxis.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalAxisProperty =
            DependencyProperty.Register("HorizontalAxis", typeof(double), typeof(ChartLines), new UIPropertyMetadata(0.0));


        public double VerticalAxis
        {
            get { return (double)GetValue(VerticalAxisProperty); }
            set { SetValue(VerticalAxisProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerticalAxis.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalAxisProperty =
            DependencyProperty.Register("VerticalAxis", typeof(double), typeof(ChartLines), new UIPropertyMetadata(0.0));

    }
}