using System;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace RightToAskClient.Maui.Views.Controls
{
    public class Accessibility
    {
        [Flags]
        public enum AccessibilityTrait
        {
            None = 0, // remove UIKit.AccessibilityTrait.NotEnabled
            Header = 1, // add UIKit.AccessibilityTrait.Header
            Disabled = 2, // add UIKit.AccessibilityTrait.NotEnabled
        }

        public static readonly BindableProperty AccessibilityTraitsProperty =
            BindableProperty.CreateAttached("AccessibilityTraits", typeof(AccessibilityTrait), typeof(Accessibility),
                AccessibilityTrait.None, propertyChanged: OnAccessibilityTraitsChanged);

        public static AccessibilityTrait GetAccessibilityTraits(BindableObject view)
        {
            return (AccessibilityTrait)view.GetValue(AccessibilityTraitsProperty);
        }

        public static void SetAccessibilityTraits(BindableObject view, AccessibilityTrait value)
        {
            view.SetValue(AccessibilityTraitsProperty, value);
        }

        static void OnAccessibilityTraitsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is View view))
            {
                return;
            }

            var newTraits = (AccessibilityTrait)newValue;
            var hasTraits = newTraits != AccessibilityTrait.None;

            if (hasTraits)
            {
                if (!view.Effects.OfType<AccessibilityTraitEffect>().Any())
                {
                    view.Effects.Add(new AccessibilityTraitEffect());
                }
            }
            else
            {
                var accessibilityTrait = view.Effects.OfType<AccessibilityTraitEffect>().FirstOrDefault();
                if (accessibilityTrait != null)
                {
                    view.Effects.Remove(accessibilityTrait);
                }
            }
        }

        public class AccessibilityTraitEffect : RoutingEffect
        {
            public AccessibilityTraitEffect() : base("RightToAskClient.AccessibilityTraitEffect")
            {
            }
        }
    }
}