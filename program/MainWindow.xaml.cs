using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ExchangeSoft
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Storyboard _pulseStoryboard;
        private Storyboard _glitchStoryboard;
        private bool _isTransitioning;
        private readonly TimeSpan _loaderDuration = TimeSpan.FromMilliseconds(800);

        public MainWindow()
        {
            InitializeComponent();
            LoadStoryboards();
        }

        private void LoadStoryboards()
        {
            _pulseStoryboard = LoaderOverlay.Resources["PulseStoryboard"] as Storyboard;
            _glitchStoryboard = LoaderOverlay.Resources["GlitchStoryboard"] as Storyboard;
        }

        private async void ModeToggle_OnClick(object sender, RoutedEventArgs e)
        {
            if (_isTransitioning)
            {
                return;
            }

            _isTransitioning = true;
            ModeToggle.IsEnabled = false;

            var switchToCrypto = ModeToggle.IsChecked == true;
            if (DataContext is ViewModel vm)
            {
                vm.IsFiatMode = !switchToCrypto;
            }

            await FadeOutContentAsync();
            ShowLoader();
            await Task.Delay(_loaderDuration);

            var templateKey = switchToCrypto ? "CryptoTemplate" : "FiatTemplate";
            TabContent.ContentTemplate = FindResource(templateKey) as DataTemplate;

            HideLoader();
            await FadeInContentAsync();

            ModeToggle.IsEnabled = true;
            _isTransitioning = false;
        }

        private void ShowLoader()
        {
            LoaderOverlay.Visibility = Visibility.Visible;
            _pulseStoryboard?.Begin(LoaderOverlay, true);
            _glitchStoryboard?.Begin(LoaderOverlay, true);
        }

        private void HideLoader()
        {
            _pulseStoryboard?.Stop(LoaderOverlay);
            _glitchStoryboard?.Stop(LoaderOverlay);
            LoaderOverlay.Visibility = Visibility.Collapsed;
        }

        private Task FadeOutContentAsync()
        {
            return AnimateOpacityAsync(TabContent, 1, 0, TimeSpan.FromMilliseconds(250));
        }

        private Task FadeInContentAsync()
        {
            return AnimateOpacityAsync(TabContent, 0, 1, TimeSpan.FromMilliseconds(250));
        }

        private static Task AnimateOpacityAsync(UIElement element, double from, double to, TimeSpan duration)
        {
            var tcs = new TaskCompletionSource<bool>();

            var animation = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = new Duration(duration),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            animation.Completed += (_, __) => tcs.TrySetResult(true);
            element.BeginAnimation(UIElement.OpacityProperty, animation);

            return tcs.Task;
        }
    }
}
