using AlgoTradeClient.Infrastructure;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace AlgoTradeClient.Modules.Position.TrackingOrderInput
{
    /// <summary>
    /// Interaction logic for OrderCompositeView.xaml
    /// </summary>
    [ViewExport(RegionName = RegionNames.OrdersInputRegion)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class TrackingOrderCompositeView : UserControl
    {
        public TrackingOrderCompositeView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets the ViewModel.
        /// </summary>
        /// <remarks>
        /// This set-only property is annotated with the <see cref="ImportAttribute"/> so it is injected by MEF with
        /// the appropriate view model.
        /// </remarks>
        [Import]
        TrackingOrderCompositeViewModel ViewModel
        {
            get
            {
                return (TrackingOrderCompositeViewModel)this.DataContext;
            }
            set
            {
                this.DataContext = value;
            }
        }
    }
}
