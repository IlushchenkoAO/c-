using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ExchangeSoft
{
    public class ViewModel : INotifyPropertyChanged
    {
        private bool _isFiatMode = true;

        public bool IsFiatMode
        {
            get => _isFiatMode;
            set
            {
                if (_isFiatMode == value)
                {
                    return;
                }

                _isFiatMode = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentModeLabel));
            }
        }

        public string CurrentModeLabel => IsFiatMode ? "Fiat" : "Crypto";

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
