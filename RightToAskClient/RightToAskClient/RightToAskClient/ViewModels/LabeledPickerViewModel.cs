using System.Collections.Generic;

namespace RightToAskClient.ViewModels
{
    public class LabeledPickerViewModel:  BaseViewModel
    {
        
        private bool _showTitle;
        public bool ShowTitle
        {
            get => _showTitle;
            set => SetProperty(ref _showTitle, value);
        }
        
        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        
        private List<string> _items;
        public List<string> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        private int _selectedIndex = -1;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                SetProperty(ref _selectedIndex, value);
                OnSelectedCallback?.Invoke(value);
                ShowTitle = true;
            }
        }

        public delegate void SelectIndex(int index);
        public event SelectIndex OnSelectedCallback;
    }
}