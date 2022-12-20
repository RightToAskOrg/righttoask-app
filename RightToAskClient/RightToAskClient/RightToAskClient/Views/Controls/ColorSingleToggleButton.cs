using System;
using Xamarin.Forms;

// Losely based on https://learn.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/button
// Implements the single version of a toggle button, i.e. one that can be selected once but (at the moment) not
// turned off. When it is turned on, it turns a bright colour.
namespace RightToAskClient.Views.Controls
{
    class ColorSingleToggleButton : ImageButton 
    {
        public event EventHandler<ToggledEventArgs> Brightened;

        public static BindableProperty IsBrightenedProperty =
            BindableProperty.Create("IsBrightened", typeof(bool), typeof(ColorSingleToggleButton), 
                false, propertyChanged: OnIsBrightenedChanged);

        public static BindableProperty BrightColor =
            BindableProperty.Create("BrightColor", typeof(string), typeof(ColorSingleToggleButton), 
                "", BindingMode.OneWay);

        public static BindableProperty DullColor =
            BindableProperty.Create("DullColor", typeof(string), typeof(ColorSingleToggleButton), 
                "", BindingMode.OneWay);
    
        public ColorSingleToggleButton()
        {
            // This will turn it on if it's off, and keep it on thereafter.
            Clicked += (sender, args) => IsBrightened |= true;
        
            // Note that the original MS code really did toggle:
            // Clicked += (sender, args) => IsBrightened ^= true;
        }

        public bool IsBrightened
        {
            set { SetValue(IsBrightenedProperty, value); }
            get { return (bool)GetValue(IsBrightenedProperty); }
        }

        /* Not sure we need this.
    protected override void OnParentSet()
    {
        base.OnParentSet();
        VisualStateManager.GoToState(this, "ToggledOff");
    }
    */

        static void OnIsBrightenedChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ColorSingleToggleButton singleToggleColorButton = (ColorSingleToggleButton)bindable;
            bool isBrightened = (bool)newValue;

            // Fire event
            singleToggleColorButton.Brightened?.Invoke(singleToggleColorButton, new ToggledEventArgs(isBrightened));

            // Set the visual state
            VisualStateManager.GoToState(singleToggleColorButton, isBrightened ? "Brightened" : "NotBrightened");
        }
    }
}