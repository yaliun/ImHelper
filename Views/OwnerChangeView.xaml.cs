using System.Windows.Controls;
using ImHelper.Services;
using ImHelper.ViewModels;

namespace ImHelper.Views;

public partial class OwnerChangeView : UserControl
{
    public OwnerChangeView()
    {
        InitializeComponent();
        DataContext = new OwnerChangeViewModel(new SampleUserDirectory());
    }
}
