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
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using AlgoTradeClient.Infrastructure;

namespace AlgoTradeClient.Modules.SystemMonitor.LogDisplay
{
    /// <summary>
    /// Interaction logic for NewsReader.xaml
    /// </summary>
    [ViewExport("LogsReaderView")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class LogsReaderView : UserControl
    {
        public LogsReaderView()
        {
            InitializeComponent();
        }

        public static string Title
        {
            get
            {
                return Properties.Resources.NewsReaderViewTitle;
            }
        }

        /// <summary>
        /// Sets the ViewModel.
        /// </summary>
        /// <remarks>
        /// This set-only property is annotated with the <see cref="ImportAttribute"/> so it is injected by MEF with
        /// the appropriate view model.
        /// </remarks>
        [Import]
        [SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification = "Needs to be a property to be composed by MEF")]
        LogsReaderViewModel ViewModel
        {
            set
            {
                this.DataContext = value;
            }
        }
    }
}
