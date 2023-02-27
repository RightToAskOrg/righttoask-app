using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("RightToAskClient")]
[assembly: ExportEffect(typeof(RightToAskClient.iOS.AccessibilityTraitEffect), "AccessibilityTraitEffect")]

namespace RightToAskClient.iOS
{
    public class AccessibilityTraitEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            AddAccessibilityTraits();
        }

        protected override void OnDetached()
        {
        }

        protected override void OnElementPropertyChanged(System.ComponentModel.PropertyChangedEventArgs args)
        {
            if (args.PropertyName == Views.Controls.Accessibility.AccessibilityTraitsProperty.PropertyName)
            {
                AddAccessibilityTraits();
            }
            else
            {
                base.OnElementPropertyChanged(args);
            }
        }

        void AddAccessibilityTraits()
        {
            var traits = Control.AccessibilityTraits;

            var newTraits = Views.Controls.Accessibility.GetAccessibilityTraits(Element);

            if ((newTraits & Views.Controls.Accessibility.AccessibilityTrait.Header) > 0)
                traits |= UIAccessibilityTrait.Header;
            else if ((newTraits & Views.Controls.Accessibility.AccessibilityTrait.Disabled) > 0)
                traits |= UIAccessibilityTrait.NotEnabled;
            else
                traits &= ~UIAccessibilityTrait.NotEnabled;

            Control.AccessibilityTraits = traits;
        }
    }
}