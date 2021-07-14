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
using System.Collections.ObjectModel;
using System.Windows.Data;
using System;
using AlgoTrade.Common.Entities;


namespace AlgoTradeClient.ChartControls
{
    public class DiscreteAxisPanel : Panel
    {
        public DiscreteAxisPanel()
        {
            TickPositions = new ObservableCollection<ChartPosition>();
            _largestLabelSize = new Size();
        }

        protected override void OnInitialized(System.EventArgs e)
        {
            base.OnInitialized(e);
            _parentControl = ((ItemsControl)((FrameworkElement)VisualTreeHelper.GetParent(this)).TemplatedParent);

            Binding tickBinding = new Binding();
            tickBinding.Path = new PropertyPath(DiscreteAxisPanel.TickPositionsProperty);
            tickBinding.Source = this;
            _parentControl.SetBinding(DiscreteAxis.TickPositionsProperty, tickBinding);

            Binding originBinding = new Binding();
            originBinding.Path = new PropertyPath(DiscreteAxisPanel.OriginProperty);
            originBinding.Source = this;
            _parentControl.SetBinding(DiscreteAxis.OriginProperty, originBinding);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _largestLabelSize.Height = 0.0;
            _largestLabelSize.Width = 0.0;
            UIElementCollection tempInternalChildren = InternalChildren;
            for (int i = 0; i < tempInternalChildren.Count; i++)
            {
                tempInternalChildren[i].Measure(availableSize);
                _largestLabelSize.Height = _largestLabelSize.Height > tempInternalChildren[i].DesiredSize.Height
                    ? _largestLabelSize.Height : tempInternalChildren[i].DesiredSize.Height;
                _largestLabelSize.Width = _largestLabelSize.Width > tempInternalChildren[i].DesiredSize.Width
                    ? _largestLabelSize.Width : tempInternalChildren[i].DesiredSize.Width;
            }
            if (Orientation.Equals(Orientation.Vertical))
            {
                double fitAllLabelSize = _largestLabelSize.Height * InternalChildren.Count;
                availableSize.Height = fitAllLabelSize < availableSize.Height ? fitAllLabelSize : availableSize.Height;
                availableSize.Width = _largestLabelSize.Width;
            }
            else
            {
                double fitAllLabelsSize = _largestLabelSize.Width * InternalChildren.Count;
                availableSize.Width = fitAllLabelsSize < availableSize.Width ? fitAllLabelsSize : availableSize.Width;
                availableSize.Height = _largestLabelSize.Height;
            }
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {           
            if (InternalChildren.Count > 0)
            {
                if (Orientation.Equals(Orientation.Horizontal))
                {
                    Origin = TickMarksLength / 2;
                    ArrangeHorizontalLabels(finalSize);
                }
                else
                {
                    Origin = finalSize.Height - TickMarksLength / 2;
                    ArrangeVerticalLabels(finalSize);
                }
            }
            return base.ArrangeOverride(finalSize);
        }


        private void ArrangeHorizontalLabels(Size constraint)
        {
            double rectHeight = _largestLabelSize.Height;
            double rectWidth = _largestLabelSize.Width;

            TickPositions.Clear();

            double availableWidth = constraint.Width - TickMarksLength / 2;
            int skipfactor;
            if (availableWidth < rectWidth)
                skipfactor = InternalChildren.Count + 1;
            else
                skipfactor = (int)Math.Ceiling(InternalChildren.Count/Math.Floor(availableWidth / rectWidth));
            skipfactor = Math.Min(skipfactor, (int)Math.Ceiling((double)InternalChildren.Count/2.0));
            bool canDisplayAllLabels = true;

            if (skipfactor > 1)
            {
                canDisplayAllLabels = false;
            }

            double sections = (availableWidth-40) / InternalChildren.Count;
            double startCord = TickMarksLength/2;
            ChartPosition itemPosition = new ChartPosition();
            ChartPosition itemData = (ChartPosition)_parentControl.Items[0]; 
            itemPosition.Location = new Point(startCord, 0);
            itemPosition.TradingDataInfo = new TradingData(itemPosition.TradingDataInfo);
            TickPositions.Add(itemPosition);
            for (int i = 0; i < InternalChildren.Count; i++)
            {
                ChartPosition item = (ChartPosition)_parentControl.Items[i];               
                if (TickPositions.Count - 1 <= i)
                    TickPositions.Add(new ChartPosition(){
                        Location=new Point( startCord + (i + 1) * sections,0),
                        TradingDataInfo=item.TradingDataInfo
                    });
                else
                    TickPositions[i + 1] = new ChartPosition(){
                        Location=new Point( startCord + (i + 1) * sections,0),
                         TradingDataInfo=item.TradingDataInfo
                    };                        
                
                if (canDisplayAllLabels)
                {
                    Rect r = new Rect(i * sections + sections / 2 - rectWidth / 2, 0, rectWidth, InternalChildren[i].DesiredSize.Height);
                    InternalChildren[i].Arrange(r);
                }
                else
                {
                    if((i+1)%skipfactor == 0)
                    {
                        double x = i * sections + sections / 2 - rectWidth / 2;
                        if (x < 0 || x + rectWidth > availableWidth)
                        {
                            Rect r1 = new Rect(0, 0, 0, 0);
                            InternalChildren[i].Arrange(r1);
                            continue;
                        }
                        Rect r = new Rect(i * sections + sections / 2 - rectWidth / 2, 0, rectWidth, InternalChildren[i].DesiredSize.Height);
                        InternalChildren[i].Arrange(r);
                    }
                    else
                    {
                        Rect r = new Rect(0, 0, 0, 0);
                        InternalChildren[i].Arrange(r);
                    }
                }
            }
        }

        private void ArrangeVerticalLabels(Size constraint)
        {
            double rectHeight = _largestLabelSize.Height;
            double rectWidth = _largestLabelSize.Width;

            TickPositions.Clear();
            double availableHeight = constraint.Height - TickMarksLength / 2;

            int skipfactor;
            if (availableHeight < rectHeight)
                skipfactor = InternalChildren.Count + 1;
            else
                skipfactor = (int)Math.Ceiling(InternalChildren.Count / Math.Floor(availableHeight / rectHeight));
            skipfactor = Math.Min(skipfactor, (int)Math.Ceiling((double)InternalChildren.Count / 2.0));
            bool canDisplayAllLabels = true;

            if (skipfactor > 1)
            {
                canDisplayAllLabels = false;
            }

            double sections = availableHeight / InternalChildren.Count;
            for (int i = 0; i < InternalChildren.Count; i++)
            {
                ChartPosition item = (ChartPosition)_parentControl.Items[i];
                if (TickPositions.Count <= i)
                    TickPositions.Add(new ChartPosition(){
                        Location = new Point(i * sections, 0),
                        TradingDataInfo=new TradingData(item.TradingDataInfo)
                    });
                else
                    TickPositions[i] = new ChartPosition(){
                        Location = new Point(i * sections, 0),
                        TradingDataInfo = new TradingData(item.TradingDataInfo)
                    };

                if (canDisplayAllLabels)
                {
                    Rect r = new Rect(0, i * sections + sections / 2 - rectHeight / 2, InternalChildren[i].DesiredSize.Width, InternalChildren[i].DesiredSize.Height);
                    InternalChildren[i].Arrange(r);
                }
                else
                {
                    if ((i + 1) % skipfactor == 0)
                    {
                        double x = i * sections + sections / 2 - rectHeight / 2;
                        if (x < 0 || x + rectHeight > availableHeight)
                        {
                            Rect r1 = new Rect(0, 0, 0, 0);
                            InternalChildren[i].Arrange(r1);
                            continue;
                        }
                        Rect r = new Rect(0, i * sections + sections / 2 - rectHeight / 2, InternalChildren[i].DesiredSize.Width, InternalChildren[i].DesiredSize.Height);
                        InternalChildren[i].Arrange(r);
                    }
                    else
                    {
                        Rect r = new Rect(0, 0, 0, 0);
                        InternalChildren[i].Arrange(r);
                    }
                }
                TickPositions.Add(new ChartPosition(){
                        Location = new Point(availableHeight, 0),
                        TradingDataInfo = new TradingData(item.TradingDataInfo)
                    });
            }
            
        }


        public double TickMarksLength
        {
            get { return (double)GetValue(TickMarksLengthProperty); }
            set { SetValue(TickMarksLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TickMarksLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TickMarksLengthProperty =
            DependencyProperty.Register("TickMarksLength", typeof(double), typeof(DiscreteAxisPanel), new UIPropertyMetadata(null));


        public ObservableCollection<ChartPosition> TickPositions
        {
            get { return (ObservableCollection<ChartPosition>)GetValue(TickPositionsProperty); }
            set { SetValue(TickPositionsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorizontalAxisTickPositions.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TickPositionsProperty =
            DependencyProperty.Register("TickPositions", typeof(ObservableCollection<ChartPosition>), typeof(DiscreteAxisPanel), new UIPropertyMetadata(null));



        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(DiscreteAxisPanel), new UIPropertyMetadata(Orientation.Horizontal));

        private ObservableCollection<ChartPosition> DataValues
        {
            get { return (ObservableCollection<ChartPosition>)GetValue(DataValuesProperty); }
            set { SetValue(DataValuesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DataValues.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty DataValuesProperty =
            DependencyProperty.Register("LabelDataValues", typeof(ObservableCollection<ChartPosition>), typeof(ContinuousAxisPanel), new FrameworkPropertyMetadata(null));



        public double Origin
        {
            get { return (double)GetValue(OriginProperty); }
            set { SetValue(OriginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Origin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OriginProperty =
            DependencyProperty.Register("Origin", typeof(double), typeof(DiscreteAxisPanel), new UIPropertyMetadata(0.0));


        private ItemsControl _parentControl;
        private Size _largestLabelSize;
    }
}