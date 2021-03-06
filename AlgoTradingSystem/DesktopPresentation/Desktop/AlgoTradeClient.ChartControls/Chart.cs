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
using System.Collections.ObjectModel;
using System.Windows;

namespace AlgoTradeClient.ChartControls
{
    public class Chart : ItemsControl
    {
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ChartItem;
        }

        protected override System.Windows.DependencyObject GetContainerForItemOverride()
        {
            return new ChartItem();
        }

        public object Title
        {
            get { return (object)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(object), typeof(Chart), new UIPropertyMetadata(null));

        public DataTemplate TitleTemplate
        {
            get { return (DataTemplate)GetValue(TitleTemplateProperty); }
            set { SetValue(TitleTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleTemplateProperty =
            DependencyProperty.Register("TitleTemplate", typeof(DataTemplate), typeof(Chart), new UIPropertyMetadata(null));



        public DataTemplateSelector TitleTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(TitleTemplateSelectorProperty); }
            set { SetValue(TitleTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleTemplateSelectorProperty =
            DependencyProperty.Register("TitleTemplateSelector", typeof(DataTemplateSelector), typeof(Chart), new UIPropertyMetadata(null));
    }
}