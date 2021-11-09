using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RightToAskClient.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExploringPageWithSearchAndPreSelections  : ExploringPageWithSearch 
    {
		public ExploringPageWithSearchAndPreSelections(ObservableCollection<Entity> allEntities, 
			ObservableCollection<Entity> selectedEntities, string message=null) : base (allEntities, selectedEntities, message)
        {
            Label testInsert = new Label() 
                { 
                    Text = "Already selected",
                };
                
            MainLayout.Children.Insert(1, testInsert);

            listPriorSelections();
        }

        private void listPriorSelections()
        {
            ListView selections = new ListView()
            {
                
                ItemTemplate=(DataTemplate)Application.Current.Resources["SelectableDataTemplate"],
                ItemsSource =wrapInTags(selectedEntities,selectedEntities)
                // ItemTemplate = StaticResource SelectableDataTemplate,
            };
            MainLayout.Children.Insert(2,selections);
        }
        
    }
}