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
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System;
using System.Windows.Controls;
using System.Collections.Specialized;
using AlgoTrade.Common.Entities;

namespace AlgoTradeClient.ChartControls
{
    public class ValueExtractor : Freezable
    {
        protected override Freezable CreateInstanceCore()
         {
            return new ValueExtractor();
        }

        private static void OnValuePathChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ValueExtractor v = sender as ValueExtractor;
            if (v != null && v.ValuePath != null && v.Items != null)
            {
                v.GenerateValueList();
            }
        }

        private static void OnItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ValueExtractor v = sender as ValueExtractor;
            ItemCollection oldItems = args.OldValue as ItemCollection;
            ItemCollection newItems = args.NewValue as ItemCollection;
            if (oldItems != null)
                ((INotifyCollectionChanged)oldItems).CollectionChanged -= new NotifyCollectionChangedEventHandler(v.OnItemsCollectionChanged);
                
            if (v != null && v.Items != null)
            {
                ((INotifyCollectionChanged)v.Items).CollectionChanged += new NotifyCollectionChangedEventHandler(v.OnItemsCollectionChanged);
                 if(v.ValuePath!=null)
                    v.GenerateValueList();
            }
        }

        private void  OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action.Equals(NotifyCollectionChangedAction.Reset))
            {
                GenerateValueList();
            }
            else if(e.Action.Equals(NotifyCollectionChangedAction.Remove))
            {
                for(int i=0; i<e.OldItems.Count; i++)
                {
                    Values.RemoveAt(e.OldStartingIndex);
                }
            }
            else if (e.Action.Equals(NotifyCollectionChangedAction.Move))
            {
                Values.Move(e.OldStartingIndex, e.NewStartingIndex);
            }
            else
            {
                for (int i = 0; i < e.NewItems.Count; i++)
                {
                    CreateInternalBinding(Items[e.NewStartingIndex+i]);
                    if (e.Action.Equals(NotifyCollectionChangedAction.Add))
                        Values.Insert(e.NewStartingIndex + i, ValueHolder);
                    else
                        Values[e.NewStartingIndex + i] = ValueHolder;
                }
            }
        }

        private void GenerateValueList()
        {
            SetValue(ValuesKey, new ObservableCollection<TradingData>());
            ObservableCollection<TradingData> tempValues = Values;
            foreach (Object o in Items)
            {
                CreateInternalBinding(o);
                tempValues.Add(ValueHolder);
            }
        }

        private void CreateInternalBinding(Object source)
        {
            Binding b = new Binding();
            b.Source = source;
            if (IsXmlNodeHelper(source))
                b.XPath = ValuePath.Path;
            else
                b.Path = ValuePath;
            BindingOperations.SetBinding(this, ValueExtractor.ValueHolderProperty, b);
        }

        private static bool IsXmlNodeHelper(object item)
        {
            return item is System.Xml.XmlNode;
        }

        public ItemCollection Items
        {
            get { return (ItemCollection)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Items.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(ItemCollection), typeof(ValueExtractor), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnItemsChanged)));


        public PropertyPath ValuePath
        {
            get { return (PropertyPath)GetValue(ValuePathProperty); }
            set { SetValue(ValuePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValuePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValuePathProperty =
            DependencyProperty.Register("ValuePath", typeof(PropertyPath), typeof(ValueExtractor), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnValuePathChanged)));


        public ObservableCollection<TradingData> Values
        {
            get { return (ObservableCollection<TradingData>)GetValue(ValuesProperty); }
        }

        // Using a DependencyProperty as the backing store for Values.  This enables animation, styling, binding, etc...
        private static readonly DependencyPropertyKey ValuesKey =
            DependencyProperty.RegisterReadOnly("Values", typeof(ObservableCollection<TradingData>), typeof(ValueExtractor), new UIPropertyMetadata(null));

        public static readonly DependencyProperty ValuesProperty = ValuesKey.DependencyProperty;

        private TradingData ValueHolder
        {
            get { return (TradingData)GetValue(ValueHolderProperty); }
            set { SetValue(ValueHolderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValueHolder.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty ValueHolderProperty =
            DependencyProperty.Register("ValueHolder", typeof(TradingData), typeof(ValueExtractor), new UIPropertyMetadata(null));

    }
}