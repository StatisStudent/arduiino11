using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture.Frames;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows;
using System.Windows;
using Windows.UI.Popups;
using System;
using System.Collections.Generic;
using System.Linq;
using fourInRow;
using FourInRow2._3;
using Microsoft.WindowsAzure.MobileServices;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    
    public sealed partial class MainPage : Page
    {

        private IMobileServiceTable<TodoItem> todoTable = App.MobileService.GetTable<TodoItem>();

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void country_input_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        async void button_Click(object sender, RoutedEventArgs e)
        {
            
            var name = name_input.Text;
            string country = country_input.SelectionBoxItem.ToString();
            var msg = System.String.Format(" name = {0}\n country ={1}\n", name, country);
            // insert_country_to_table_and_add_1_to_amount(country);
            var dig = new MessageDialog(msg);
            var items = await todoTable
                    .Where(todoItem2 => todoItem2.Complete == true && todoItem2.Country == country)
                    .ToCollectionAsync();
            TodoItem todoItem;
            if (items.Count == 0)
            {
                todoItem = new TodoItem {Amount = 1, Complete = true, Country = country};

            }
            else
            {
                 todoItem = new TodoItem { Amount = items[0].Amount + 1, Complete = true, Country = country };
                items[0].Complete = false;
                //await todoTable.DeleteAsync(items[0]);
                await todoTable.UpdateAsync(items[0]);
            }
            await todoTable.InsertAsync(todoItem);
            //items.Add(todoItem);



            await dig.ShowAsync();
            
            this.Frame.Navigate(typeof(page2));
        }

        private async void Statistics_Click(object sender, RoutedEventArgs e)
        {
            var items = await todoTable
                .Where(todoItem2 => todoItem2.Complete)
                .ToCollectionAsync();

            ListItems.ItemsSource = items;

        }
        private void ListItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void CheckBoxComplete_Checked(object sender, RoutedEventArgs e)
        {
//            CheckBox cb = (CheckBox)sender;
//            TodoItem item = cb.DataContext as TodoItem;
//            await UpdateCheckedTodoItem(item);
        }


    }
}
