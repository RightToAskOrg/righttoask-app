using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

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
                false
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

    }
}