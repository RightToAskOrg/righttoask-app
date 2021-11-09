using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace RightToAskClient.Controls
{
    public class AdvancedSearchControl : TableView
    {
        private String aKeyword;
        // private TableView Content;
        // public AdvancedSearchControl(string Keyword)
        public AdvancedSearchControl()
        {

        
            Root = new TableRoot
                {
                    new TableSection("Filters - click to edit")
                    {
                        new EntryCell()
                        {
                            Label = "To be answered by"+aKeyword,
                            Text = aKeyword
                        },
                        new EntryCell()
                        {
                            Label = "To be raised in Parliament by",
                        },
                        new EntryCell()
                        {
                            Label = "Keyword",
                            Text    = AKeyword ?? "" 
                        }
                    }
                };
                Intent = TableIntent.Settings;
        }

        public static readonly BindableProperty KeywordProperty =

            BindableProperty.Create(
                nameof(AKeyword),
                typeof(string),
                typeof(AdvancedSearchControl),
                null,
                BindingMode.TwoWay);

        public string AKeyword 
        {
           get => (string) GetValue(KeywordProperty);
           // set => SetValue(KeywordProperty, value);
           set
           {
              aKeyword = value;
              OnPropertyChanged("Keyword");
           }
           // set => SetValue(KeywordProperty, value);
        }  
    }
}