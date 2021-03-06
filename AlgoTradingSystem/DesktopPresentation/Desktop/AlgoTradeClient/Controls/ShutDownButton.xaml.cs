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

namespace AlgoTradeClient.Controls
{
    /// <summary>
    /// Interaction logic for AddWatchControl.xaml
    /// </summary>
    [Export(typeof(ShutDownButton))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ShutDownButton : UserControl
    {
        public ShutDownButton()
        {
            InitializeComponent();
            this.DataContext = new ShutdownButtonViewModel();
        }

        /// <summary>
        /// Sets the ViewModel.
        /// </summary>
        /// <remarks>
        /// This set-only property is annotated with the <see cref="ImportAttribute"/> so it is injected by MEF with
        /// the appropriate view model.
        /// </remarks>
        [Import]
        ShutdownButtonViewModel ViewModel
        {          
            set
            {
                this.DataContext = value;
            }
        }
    }
}
