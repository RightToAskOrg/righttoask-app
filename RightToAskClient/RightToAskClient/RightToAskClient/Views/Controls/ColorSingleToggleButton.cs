using System;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

// Losely based on https://learn.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/button
// Implements the single version of a toggle button, i.e. one that can be selected once but (at the moment) not
// turned off. When it is turned on, it turns a bright colour.
namespace RightToAskClient.Views.Controls
{
    public class ColorSingleToggleButton : ImageButton
    {
        private event EventHandler<ToggledEventArgs> Brightened;

        public static BindableProperty IsBrightenedProperty =
            BindableProperty.Create("IsBrightened", typeof(bool), typeof(ColorSingleToggleButton),
                false, BindingMode.OneWay, propertyChanged: OnIsBrightenedChanged);

        public bool IsBrightened
        {
            set { SetValue(IsBrightenedProperty, value); }
        }
        
        // Set to NotBrightened state at init
        protected override void OnParentSet()
        {
            base.OnParentSet();
            VisualStateManager.GoToState(this, "NotBrightened");
        }

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