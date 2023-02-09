using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PublicPrivateLabel : StackLayout
    {
        public static BindableProperty IsPublicProperty =
            BindableProperty.Create(
                nameof(IsPublic),
                typeof(Boolean),
                typeof(PublicPrivateLabel),
                false,
                propertyChanged: OnIsPublicChanged
            );

        public Boolean IsPublic
        {
            get => (bool)GetValue(IsPublicProperty);
            set => SetValue(IsPublicProperty, value);
        }

        public PublicPrivateLabel()
        {
            InitializeComponent();
        }

        static void OnIsPublicChanged(BindableObject bindable, object oldValue, object newValue)
        {
            PublicPrivateLabel label = (PublicPrivateLabel)bindable;
            Boolean isPublic = (Boolean)newValue;
            label.PublicLabel.IsVisible = isPublic;
            label.PrivateLabel.IsVisible = !isPublic;
        }
    }
}