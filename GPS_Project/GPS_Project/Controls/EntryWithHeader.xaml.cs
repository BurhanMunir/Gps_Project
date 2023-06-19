using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GPS_Project.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EntryWithHeader : StackLayout
    {
        public static readonly BindableProperty TextProperty =
 BindableProperty.Create("Text", typeof(string), typeof(EntryWithHeader), null,
           defaultBindingMode: BindingMode.TwoWay);
        public EntryWithHeader()
        {
            InitializeComponent();
        }
       
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public string Heading
        {
            get => lblHeader.Text;
            set => lblHeader.Text = value;
        }
        public string PlaceHolder
        {
            get => txt.Placeholder;
            set => txt.Placeholder = value;
        }
    }
}