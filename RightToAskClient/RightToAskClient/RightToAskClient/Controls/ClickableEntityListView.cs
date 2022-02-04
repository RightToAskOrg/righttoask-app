using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Org.BouncyCastle.Tls;
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
        public ClickableEntityListView()
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
        
        private readonly BindableProperty clickableListLabelProperty = BindableProperty.Create(
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

        private readonly BindableProperty clickableListContentsProperty = BindableProperty.Create(
            propertyName: "ClickableListContents",
            returnType: typeof(ObservableCollection<T>),
            declaringType: typeof(ClickableEntityListView<T>),
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: OnClickableListContentsPropertyChanged
        );

        /*
        private static void OnClickableListContentsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (ClickableEntityListView<T>)bindable;
            var _entities = newValue as ObservableCollection<T> ?? new ObservableCollection<T>();
            control._entityList.Text = String.Join(", ", _entities.Select(a => a.ShortestName)); 
        }
        */
        
        
        /* This allows the UI list to update when either the list itself changes (i.e. the object changes)
         * or its contents changes. I'm not certain that all the redrawing here is necessary, but it 
         * doesn't hurt to cover all possibilities.
         */
        private static void OnClickableListContentsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ClickableEntityListView<T> control)
            {
                // var control = (ClickableEntityListView<T>)bindable;
                if (newValue is ObservableCollection<T> entities)
                {
                    // entities.CollectionChanged += ItemsSourceCollectionChanged;
                    entities.CollectionChanged += (s, e) =>
                    {
                        control._entityList.Text = String.Join(", ", entities.Select(a => a.ShortestName));
                    };
                    entities.CollectionChanged -= (s, e) =>
                    {
                        control._entityList.Text = String.Join(", ", entities.Select(a => a.ShortestName));
                    };
                    // entities.CollectionChanged -= ItemsSourceCollectionChanged;
                    // var _entities = newValue as ObservableCollection<T> ?? new ObservableCollection<T>();
                    control._entityList.Text = String.Join(", ", entities.Select(a => a.ShortestName));
                }
            }
        }

        /*
        private static void ItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems is ObservableCollection<T> entities)
            {
                _entityList.Text = String.Join(", ", entities.Select(a => a.ShortestName)); 
            }
        }
        // This code allows the list view to update when items are added or removed from the list
        // of selected entities.
        
        protected override void OnPropertyChanged(string propertyName)
        {
            if(propertyName == clickableListContentsProperty.PropertyName)
            {
                ***WRONG if (_entityList != null && _entityList is INotifyCollectionChanged collection)
                // if (ItemsSource != null && ItemsSource is INotifyCollectionChanged collection)
                {
                    collection.CollectionChanged -= ItemsSourceCollectionChanged;
                    collection.CollectionChanged += ItemsSourceCollectionChanged;
                }

            }

            base.OnPropertyChanged(propertyName);
        }
        */

        public EventHandler UpdateAction
        {
            get { return (EventHandler)GetValue(updateActionProperty); }
            set { SetValue(updateActionProperty, value); }
        }

        private readonly BindableProperty updateActionProperty = BindableProperty.Create(
            propertyName: "UpdateAction",
            returnType: typeof(EventHandler),
            declaringType: typeof(ClickableEntityListView<T>),
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged: OnUpdateActionPropertyChanged
        );

        private static void OnUpdateActionPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (ClickableEntityListView<T>)bindable;
            control._tapGestureRecognizer.Tapped += newValue as EventHandler;
            // control._tapGestureRecognizer.Tapped += newValue as TapGestureRecognizer;
        }
    }
}