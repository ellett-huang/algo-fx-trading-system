//===================================================================================
// Microsoft patterns & practices
// Composite Application Guidance for Windows Presentation Foundation and Silverlight
//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===================================================================================
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//===================================================================================
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using AlgoTrade.DAL.Provider;
using System.Collections.Generic;
using AlgoTrade.Common.Entities;
using System.Globalization;
using Microsoft.Expression.Shapes;
using System.Linq;

namespace AlgoTradeClient.ChartControls
{
    public class LineChartPanel : Panel
    {
        protected override void OnInitialized(System.EventArgs e)
        {
            base.OnInitialized(e);
            _childrenPositions = new ObservableCollection<ChartPosition>();
        }

        private static void OnValuesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            LineChartPanel v = sender as LineChartPanel;
            if (v == null)
                return;
            ItemCollection oldItems = args.OldValue as ItemCollection;
            ItemCollection newItems = args.NewValue as ItemCollection;
            if (oldItems != null)
                ((INotifyCollectionChanged)oldItems).CollectionChanged -= new NotifyCollectionChangedEventHandler(v.LineChartPanel_CollectionChanged);

            if (args.Property == LineChartPanel.XValuesProperty)
            {
                if (GetXValues(v) != null)
                    GetXValues(v).CollectionChanged += new NotifyCollectionChangedEventHandler(v.LineChartPanel_CollectionChanged);
            }
            else if (args.Property == LineChartPanel.YValuesProperty)
            {
                if (GetYValues(v) != null)
                    GetYValues(v).CollectionChanged += new NotifyCollectionChangedEventHandler(v.LineChartPanel_CollectionChanged);
            }
            v.InvalidateArrange();
            v.InvalidateVisual();
        }

        private void LineChartPanel_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            InvalidateArrange();
            InvalidateVisual();
        }

        private List<Transaction> getTransactionHistory(DateTime LastDate, string Symbol)
        {
            List<Transaction> transactions = new List<Transaction>();
            DataAdapter dataAdapter = new DataAdapter();
            transactions = dataAdapter.LookupLatestTransactions(LastDate, Symbol);
            return transactions;
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            if (_childrenPositions.Count == 0)
                return;
            int i = 0;
            while (_childrenPositions[i].Location.Y == _childrenPositions[i + 10].Location.Y)
            {
                i++;
            }
            double increments = Math.Abs(_childrenPositions[i].Location.Y - _childrenPositions[i + 10].Location.Y) / Math.Abs(_childrenPositions[i].TradingDataInfo.Close - _childrenPositions[i + 10].TradingDataInfo.Close);
            foreach (ChartPosition child in _childrenPositions)
            {

                double height = increments * Math.Abs(child.TradingDataInfo.Close - child.TradingDataInfo.Open);
                double hight = increments * Math.Abs(child.TradingDataInfo.High - (child.TradingDataInfo.Open > child.TradingDataInfo.Close ? child.TradingDataInfo.Open : child.TradingDataInfo.Close));
                double low = increments * Math.Abs((child.TradingDataInfo.Open < child.TradingDataInfo.Close ? child.TradingDataInfo.Open : child.TradingDataInfo.Close) - child.TradingDataInfo.Low);

                if (child.TradingDataInfo.Open - child.TradingDataInfo.Close > 0)
                {
                    dc.DrawRectangle(System.Windows.Media.Brushes.IndianRed, new Pen(Brushes.Black, 0.5), new Rect(child.Location.X - 1, child.Location.Y - height, 2, height));
                    dc.DrawLine(new Pen(Brushes.Black, 0.5), new Point(child.Location.X, child.Location.Y - height), new Point(child.Location.X, child.Location.Y - height - hight));
                    dc.DrawLine(new Pen(Brushes.Black, 0.5), new Point(child.Location.X, child.Location.Y), new Point(child.Location.X, child.Location.Y + low));
                }
                else
                {
                    dc.DrawRectangle(System.Windows.Media.Brushes.Green, new Pen(Brushes.Black, 0.5), new Rect(child.Location.X - 1, child.Location.Y, 2, height));
                    dc.DrawLine(new Pen(Brushes.Black, 0.5), new Point(child.Location.X, child.Location.Y), new Point(child.Location.X, child.Location.Y - hight));
                    dc.DrawLine(new Pen(Brushes.Black, 0.5), new Point(child.Location.X, child.Location.Y + height), new Point(child.Location.X, child.Location.Y + height + low));
                }
            }
            List<Transaction> lastTransactions = getTransactionHistory(_childrenPositions[0].TradingDataInfo.TradeDateTime, _childrenPositions[0].TradingDataInfo.Symbol);
            BlockArrow downShapeDrawing = FindResource("SignalDownArrow") as BlockArrow;

            VisualBrush myDownBush = new VisualBrush(downShapeDrawing);
            BlockArrow upShapeDrawing = FindResource("SignalUpArrow") as BlockArrow;

            VisualBrush myUpBush = new VisualBrush(upShapeDrawing);
            int lastIndex = 0;
            foreach (Transaction lastTransaction in lastTransactions)
            {
                int index = -1;

                foreach (ChartPosition position in _childrenPositions)
                {
                    DateTime lastTransactionTime = lastTransaction.TradingDate ?? new DateTime();
                    DateTime CurrentPositionTime = position.TradingDataInfo.TradeDateTime;
                    TimeSpan timeCompare = lastTransactionTime - CurrentPositionTime;
                    if (timeCompare.TotalHours <= 1)
                    {
                        index = _childrenPositions.IndexOf(position);
                        break;
                    }
                }
                bool IsPaperTrade = lastTransaction.IsPaperTrade ?? true;
                if ((IsPaperTrade && lastIndex != 0 && index - lastIndex < 10) || index == -1)
                    continue;
                lastIndex = index;
                
                //this.Children.Add(shapeDrawing);
                double height = increments * Math.Abs(_childrenPositions[index].TradingDataInfo.Close - _childrenPositions[index].TradingDataInfo.Open);
                double hight = increments * Math.Abs(_childrenPositions[index].TradingDataInfo.High - (_childrenPositions[index].TradingDataInfo.Open > _childrenPositions[index].TradingDataInfo.Close ? _childrenPositions[index].TradingDataInfo.Open : _childrenPositions[index].TradingDataInfo.Close));
                double low = increments * Math.Abs((_childrenPositions[index].TradingDataInfo.Open < _childrenPositions[index].TradingDataInfo.Close ? _childrenPositions[index].TradingDataInfo.Open : _childrenPositions[index].TradingDataInfo.Close) - _childrenPositions[index].TradingDataInfo.Low);
                bool IsUp = _childrenPositions[index].TradingDataInfo.Open < _childrenPositions[index].TradingDataInfo.Close;
                double upShapeOffset = (IsUp == true ? height + low : low);
                double downShapeOffset = (IsUp == false ? height + hight + downShapeDrawing.Height : hight + downShapeDrawing.Height);
                if (String.Compare(lastTransaction.ActionTypeName, TradingAction.Buy.ToString(), false, CultureInfo.CurrentCulture) == 0)
                {
                    dc.DrawRectangle(myUpBush, null, new Rect(_childrenPositions[index].Location.X + 2 - upShapeDrawing.Width / 2, _childrenPositions[index].Location.Y + upShapeOffset, upShapeDrawing.Width, upShapeDrawing.Height));
                    if (!IsPaperTrade)
                        dc.DrawText(new FormattedText("Buy", CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 12, Brushes.Red), new Point(_childrenPositions[index].Location.X - 10, _childrenPositions[index].Location.Y + upShapeOffset + 30));
                }
                if (String.Compare(lastTransaction.ActionTypeName, TradingAction.Sell.ToString(), false, CultureInfo.CurrentCulture) == 0)
                {

                    dc.DrawRectangle(myDownBush, null, new Rect(_childrenPositions[index].Location.X + 2 - downShapeDrawing.Width / 2, _childrenPositions[index].Location.Y - downShapeOffset, downShapeDrawing.Width, downShapeDrawing.Height));
                    if (!IsPaperTrade)
                        dc.DrawText(new FormattedText("Sell", CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 12, Brushes.Red), new Point(_childrenPositions[index].Location.X - 10, _childrenPositions[index].Location.Y - downShapeOffset - 10));
                }
                if (String.Compare(lastTransaction.ActionTypeName, TradingAction.ShortSell.ToString(), false, CultureInfo.CurrentCulture) == 0)
                {
                    dc.DrawRectangle(myDownBush, null, new Rect(_childrenPositions[index].Location.X + 2 - downShapeDrawing.Width / 2, _childrenPositions[index].Location.Y - downShapeOffset, downShapeDrawing.Width, downShapeDrawing.Height));
                    if (!IsPaperTrade)
                        dc.DrawText(new FormattedText("Short", CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 12, Brushes.Red), new Point(_childrenPositions[index].Location.X - 10, _childrenPositions[index].Location.Y - downShapeOffset - 20));
                }
                if (String.Compare(lastTransaction.ActionTypeName, TradingAction.ShortCover.ToString(), false, CultureInfo.CurrentCulture) == 0)
                {
                    dc.DrawRectangle(myUpBush, null, new Rect(_childrenPositions[index].Location.X + 2 - upShapeDrawing.Width / 2, _childrenPositions[index].Location.Y + upShapeOffset, upShapeDrawing.Width, upShapeDrawing.Height));
                    if (!IsPaperTrade)
                        dc.DrawText(new FormattedText("Cover", CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 12, Brushes.Red), new Point(_childrenPositions[index].Location.X - 10, _childrenPositions[index].Location.Y + 30 + upShapeOffset));
                }

            }

        }

        private PathGeometry CreateLineCurveGeometry()
        {
            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure();
            pathGeometry.Figures.Add(pathFigure);
            pathFigure.StartPoint = (Point)GeometryOperation.ComputeIntersectionPoint((FrameworkElement)InternalChildren[0], (FrameworkElement)InternalChildren[1]);
            PolyLineSegment pls = new PolyLineSegment();
            for (int i = 1; i < InternalChildren.Count; i++)
            {
                pls.Points.Add(GeometryOperation.ComputeIntersectionPoint((FrameworkElement)InternalChildren[i],
                    ((FrameworkElement)InternalChildren[i - 1])));
            }
            pathFigure.Segments.Add(pls);
            if (AreaBrush != null)
            {
                pathFigure.Segments.Add(new LineSegment(new Point(_childrenPositions[_childrenPositions.Count - 1].Location.X, GetHorizontalAxis(this)), false));
                pathFigure.Segments.Add(new LineSegment(new Point(_childrenPositions[0].Location.X, GetHorizontalAxis(this)), false));
            }
            return pathGeometry;
        }

        private PathGeometry CreateAreaCurveGeometry()
        {
            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure();
            pathGeometry.Figures.Add(pathFigure);

            Point[] catmullRomPoints = new Point[_childrenPositions.Count];
            for (int i = 0; i < _childrenPositions.Count; i++)
                catmullRomPoints[i] = _childrenPositions[i].Location;
            Point[] bezierPoints = GeometryOperation.CatmullRom(catmullRomPoints);
            pathFigure.StartPoint = bezierPoints[0];
            PolyBezierSegment pbs = new PolyBezierSegment();
            for (int i = 1; i < bezierPoints.GetLength(0); i++)
                pbs.Points.Add(bezierPoints[i]);
            pathFigure.Segments.Add(pbs);
            if (AreaBrush != null)
            {
                pathFigure.Segments.Add(new LineSegment(new Point(_childrenPositions[_childrenPositions.Count - 1].Location.X, GetHorizontalAxis(this)), false));
                pathFigure.Segments.Add(new LineSegment(new Point(_childrenPositions[0].Location.X, GetHorizontalAxis(this)), false));
            }
            return pathGeometry;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            for (int i = 0; i < InternalChildren.Count; i++)
            {
                InternalChildren[i].Measure(availableSize);
            }
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (XValues == null || YValues == null)
                return finalSize;
            int count = Math.Min(XValues.Count, YValues.Count);
            _childrenPositions.Clear();

            for (int i = 0; i < count; i++)
            {
                double x = (i + 1 >= XValues.Count) ? XValues[i].Location.X : XValues[i].Location.X + XValues[i + 1].Location.X;
                Point position = new Point(x / 2, YValues[i].Volume);
                ChartPosition theChartPosition = new ChartPosition();
                theChartPosition.Location = position;
                theChartPosition.TradingDataInfo = YValues[i];
                _childrenPositions.Add(theChartPosition);
                Rect r = new Rect(position.X - InternalChildren[i].DesiredSize.Width / 2,
                    position.Y - InternalChildren[i].DesiredSize.Height / 2
                    , InternalChildren[i].DesiredSize.Width / 2, InternalChildren[i].DesiredSize.Height / 2);
                InternalChildren[i].Arrange(r);

            }
            return finalSize;
        }

        public bool IsSmoothOutline
        {
            get { return (bool)GetValue(IsSmoothOutlineProperty); }
            set { SetValue(IsSmoothOutlineProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSmoothOutline.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSmoothOutlineProperty =
            DependencyProperty.Register("IsSmoothOutline", typeof(bool), typeof(LineChartPanel), new UIPropertyMetadata(false));

        public Pen LinePen
        {
            get { return (Pen)GetValue(LinePenProperty); }
            set { SetValue(LinePenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LinePen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LinePenProperty =
            DependencyProperty.Register("LinePen", typeof(Pen), typeof(LineChartPanel), new UIPropertyMetadata(null));


        public Brush AreaBrush
        {
            get { return (Brush)GetValue(AreaBrushProperty); }
            set { SetValue(AreaBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AreaBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AreaBrushProperty =
            DependencyProperty.Register("AreaBrush", typeof(Brush), typeof(LineChartPanel), new UIPropertyMetadata(null));


        private ObservableCollection<ChartPosition> XValues
        {
            get { return (ObservableCollection<ChartPosition>)GetXValues(this); }
            set { SetXValues(this, value); }
        }

        public static ObservableCollection<ChartPosition> GetXValues(DependencyObject obj)
        {
            return (ObservableCollection<ChartPosition>)obj.GetValue(XValuesProperty);
        }

        public static void SetXValues(DependencyObject obj, ObservableCollection<ChartPosition> value)
        {
            obj.SetValue(XValuesProperty, value);
        }

        // Using a DependencyProperty as the backing store for XValues.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XValuesProperty =
            DependencyProperty.RegisterAttached("XValues", typeof(ObservableCollection<ChartPosition>), typeof(LineChartPanel), new UIPropertyMetadata(null, new PropertyChangedCallback(OnValuesChanged)));


        private ObservableCollection<TradingData> YValues
        {
            get { return (ObservableCollection<TradingData>)GetYValues(this); }
            set { SetYValues(this, value); }
        }

        public static ObservableCollection<TradingData> GetYValues(DependencyObject obj)
        {
            return (ObservableCollection<TradingData>)obj.GetValue(YValuesProperty);
        }

        public static void SetYValues(DependencyObject obj, ObservableCollection<TradingData> value)
        {
            obj.SetValue(YValuesProperty, value);
        }

        // Using a DependencyProperty as the backing store for YValues.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YValuesProperty =
            DependencyProperty.RegisterAttached("YValues", typeof(ObservableCollection<TradingData>), typeof(LineChartPanel), new UIPropertyMetadata(null, new PropertyChangedCallback(OnValuesChanged)));



        public static double GetHorizontalAxis(DependencyObject obj)
        {
            return (double)obj.GetValue(HorizontalAxisProperty);
        }

        public static void SetHorizontalAxis(DependencyObject obj, double value)
        {
            obj.SetValue(HorizontalAxisProperty, value);
        }

        // Using a DependencyProperty as the backing store for HorizontalAxis.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HorizontalAxisProperty =
            DependencyProperty.RegisterAttached("HorizontalAxis", typeof(double), typeof(LineChartPanel), new UIPropertyMetadata(0.0));


        private ObservableCollection<ChartPosition> _childrenPositions;
    }
}