using Klava.WPF.ViewModels;

namespace Klava.WPF.Services;

public interface INavigationService
{
    ViewModelBase? CurrentViewModel { get; }
    event Action? OnNavigated;
    void NavigateTo<TViewModel>() where TViewModel : ViewModelBase;
    void NavigateTo<TViewModel>(object parameter) where TViewModel : ViewModelBase;
}
