using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static System.Net.Mime.MediaTypeNames;

namespace GPS_Project.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LabelWithHeader : StackLayout
    {
        public static readonly BindableProperty TextProperty =
BindableProperty.Create("Text", typeof(string), typeof(LabelWithHeader), null,
          defaultBindingMode: BindingMode.TwoWay);
        public LabelWithHeader()
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
        public Color TextColor
        {
            get => txt.TextColor;
            set => txt.TextColor = value;
        }
        public double FontSize
        {
            get => txt.FontSize; set => txt.FontSize = value;
        }
        
    }
}