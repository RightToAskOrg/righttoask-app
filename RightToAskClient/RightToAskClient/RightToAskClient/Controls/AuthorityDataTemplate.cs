using Xamarin.Forms;

namespace RightToAskClient.Controls
{
    public class AuthorityDataTemplate : DataTemplate
    {
        public AuthorityDataTemplate() : base()
        {
            {
                var grid = new Grid();
                var nameLabel = new Label { FontAttributes = FontAttributes.Bold };
                var selectedToggle = new Switch();

                nameLabel.SetBinding(Label.TextProperty, "TagEntity.NickName");
                selectedToggle.SetBinding(Switch.IsToggledProperty, "Selected");

                grid.Children.Add(nameLabel);
                grid.Children.Add(selectedToggle, 1, 0);
                
                // return new ViewCell { View = grid };
            }
        }
    }
}