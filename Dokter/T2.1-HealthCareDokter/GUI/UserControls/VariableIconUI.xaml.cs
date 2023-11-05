using System.Windows;
using System.Windows.Controls;

namespace T2._1_HealthCareDokter.GUI.UserControls
{
    /// <summary>
    /// Interaction logic for VariableIconUI.xaml
    /// </summary>
    public partial class VariableIconUI : UserControl
    {
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(VariableIconUI), new PropertyMetadata(string.Empty));

        public Style TextStyle
        {
            get { return (Style)GetValue(TextStyleProperty); }
            set { SetValue(TextStyleProperty, value); }
        }

        public static readonly DependencyProperty TextStyleProperty =
            DependencyProperty.Register("TextStyle", typeof(Style), typeof(VariableIconUI));


        public Style Icon
        {
            get { return (Style)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(Style), typeof(VariableIconUI));

        public VariableIconUI()
        {
            InitializeComponent();
        }
    }
}
