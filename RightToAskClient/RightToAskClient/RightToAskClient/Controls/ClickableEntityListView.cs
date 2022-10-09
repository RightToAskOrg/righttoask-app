using System;
using System.Collections.ObjectModel;
using System.Linq;
using RightToAskClient.Models;
using Xamarin.Forms;

namespace RightToAskClient.Controls
{
    public class ClickableEntityListView<T> : StackLayout where T:Entity 
    {
        private TapGestureRecognizer _tapGestureRecognizer = new TapGestureRecognizer();
        private Label _title = new Label();
        private Label _entityList = new Label();
        
        public ClickableEntityListView()
        {
            GestureRecognizers.Add(_tapGestureRecognizer);
            Children.Add(_title);
            Children.Add(
                new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    VerticalOptions = LayoutOptions.Start,
                    Children =
                    {
                        _entityList
                    },
                }
            );
        }

        public string ClickableListLabel
        {
            get => (string)GetValue(clickableListLabelProperty);
            set => SetValue(clickableListLabelProperty, value);
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
            get => (ObservableCollection<T>)GetValue(clickableListContentsProperty);
            set => SetValue(clickableListContentsProperty, value);
        }

        private readonly BindableProperty clickableListContentsProperty = BindableProperty.Create(
            propertyName: "ClickableListContents",
            returnType: typeof(ObservableCollection<T>),
            declaringType: typeof(ClickableEntityListView<T>),
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: OnClickableListContentsPropertyChanged
        );

        /* This allows the UI list to update when either the list itself changes (i.e. the object changes)
         * or its contents changes. 
         */
        private static void OnClickableListContentsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ClickableEntityListView<T> control)
            {
                if (newValue is ObservableCollection<T> entities)
                {
                    entities.CollectionChanged += 
                        
                        (s, e) =>
                    {
                        control._entityList.Text = string.Join(", ", entities.Select(a => a.ShortestName));
                    };
                    
                    // Writing the list the first time.
                    control._entityList.Text = string.Join(", ", entities.Select(a => a.ShortestName));
                }
                control._title.TextColor = Color.Black;
                control._entityList.TextColor = Color.Black;
            }
        }

        public EventHandler UpdateAction
        {
            get => (EventHandler)GetValue(updateActionProperty);
            set => SetValue(updateActionProperty, value);
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
        }
    }
}