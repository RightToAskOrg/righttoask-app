using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using RightToAskClient.Models;
using Xamarin.Forms;

namespace RightToAskClient.Controls
{
    //public class Tag<T> where T:Entity , INotifyPropertyChanged
    public class ClickableEntityListView<T> : StackLayout where T:Entity 
    {
        private TapGestureRecognizer _tapGestureRecognizer = new TapGestureRecognizer();
        private Label _title = new Label();
        private Label _title2 = new Label();
        private Label _authorityList = new Label()
        {
            Text = "This is a test"
        };

        // private ObservableCollection<T> _entities = new ObservableCollection<T>();
        private Label _entityList = new Label();
        
        /*
    Label authorityList = new Label()
    {
        Text = String.Join(",", _filterContext.SelectedAuthorities.Select(a => a.ShortestName))
    };
            
    tapGestureRecognizer.Tapped += OnMoreButtonClicked;
    */
        public ClickableEntityListView(string title)
        {
            GestureRecognizers.Add(_tapGestureRecognizer);

            //VerticalOptions = LayoutOptions.Start;
            //HorizontalOptions = LayoutOptions.FillAndExpand;
            Children.Add(_title);
            Children.Add(
                new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    VerticalOptions = LayoutOptions.Start,
                    Children =
                    {
                        _entityList
                        // _authorityList
                    }
                }
            );
        }

        public string ClickableListLabel
        {
            get { return (string)GetValue(clickableListLabelProperty); }
            set { SetValue(clickableListLabelProperty, value); }
        }
        
        private static readonly BindableProperty clickableListLabelProperty = BindableProperty.Create(
            propertyName: "ClickableListLabel",
            returnType: typeof(string),
            declaringType: typeof(ClickableEntityListView<T>),
            defaultValue: "",
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged: OnClickableListLabelPropertyChanged
        );

        private static void OnClickableListLabelPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (ClickableEntityListView<T>)bindable;
            control._title.Text = newValue.ToString();
        }
        
        public ObservableCollection<T> ClickableListContents
        {
            get { return (ObservableCollection<T>)GetValue(clickableListContentsProperty); }
            set { SetValue(clickableListContentsProperty, value); }
        }

        private static readonly BindableProperty clickableListContentsProperty = BindableProperty.Create(
            propertyName: "ClickableListContents",
            returnType: typeof(ObservableCollection<T>),
            declaringType: typeof(ClickableEntityListView<T>),
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: OnClickableListContentsPropertyChanged
        );

        private static void OnClickableListContentsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (ClickableEntityListView<T>)bindable;
            var _entities = newValue as ObservableCollection<T> ?? new ObservableCollection<T>();
            control._entityList.Text = String.Join(", ", _entities.Select(a => a.ShortestName)); 
        }
    }
}