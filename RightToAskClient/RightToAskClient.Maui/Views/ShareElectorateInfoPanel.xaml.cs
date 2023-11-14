using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace RightToAskClient.Maui.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShareElectorateInfoPanel : Frame
    {
        public static BindableProperty IsPublicProperty =
            BindableProperty.Create(
                nameof(IsPublic),
                typeof(bool),
                typeof(PublicPrivateLabel),
                false
            );

        public bool IsPublic
        {
            get => (bool)GetValue(IsPublicProperty);
            set => SetValue(IsPublicProperty, value);
        }

        public static BindableProperty IsAbleToFinishProperty =
            BindableProperty.Create(
                nameof(IsAbleToFinish),
                typeof(bool),
                typeof(PublicPrivateLabel),
                false
            );

        public bool IsAbleToFinish
        {
            get => (bool)GetValue(IsAbleToFinishProperty);
            set => SetValue(IsAbleToFinishProperty, value);
        }

        public static BindableProperty TitleProperty =
            BindableProperty.Create(
                nameof(Title),
                typeof(string),
                typeof(PublicPrivateLabel),
                ""
            );

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static BindableProperty ValueProperty =
            BindableProperty.Create(
                nameof(Value),
                typeof(string),
                typeof(PublicPrivateLabel),
                ""
            );

        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public ShareElectorateInfoPanel()
        {
            InitializeComponent();
        }
    }
}