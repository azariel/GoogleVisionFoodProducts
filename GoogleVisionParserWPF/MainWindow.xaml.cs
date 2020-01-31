using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GoogleVisionParser.Parser.Nutrionnal;
using GoogleVisionParser.Parser.Nutrionnal.Models;

namespace GoogleVisionParserWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private SolidColorBrush GetColorFromNutritionalFoodView(NutritionalFoodView _v)
        {
            Color _Color = Color.FromArgb(128, 0, 128, 0);

            if (string.IsNullOrEmpty(_v?.Percentage) || _v.Percentage == "?")
                _Color = Color.FromArgb(128, 128, 128, 0);

            if (string.IsNullOrEmpty(_v?.Amount))
                _Color = Color.FromArgb(128, 128, 0, 0);

            if (string.IsNullOrEmpty(_v?.Name))
                _Color = Color.FromArgb(128, 200, 0, 0);

            if (_v == null)
                _Color = Color.FromArgb(128, 255, 0, 0);

            return new SolidColorBrush(_Color);
        }

        private void Initialize()
        {
            string _RawDataFromFile = File.ReadAllText("DATA.txt");
            NutritionnalParser _Parser = new NutritionnalParser();
            var _Result = _Parser.Parse(_RawDataFromFile, out var _Report);

            var gridView = new GridView();
            gridView.Columns.Add(new GridViewColumn() { Header = "Name", DisplayMemberBinding = new Binding("Name") });
            gridView.Columns.Add(new GridViewColumn() { Header = "Amount", DisplayMemberBinding = new Binding("Amount") });
            gridView.Columns.Add(new GridViewColumn() { Header = "TypeAmount", DisplayMemberBinding = new Binding("TypeAmount") });
            gridView.Columns.Add(new GridViewColumn() { Header = "Percentage", DisplayMemberBinding = new Binding("Percentage") });
            this.listView.View = gridView;

            List<NutritionalFoodView> _Unknown = new List<NutritionalFoodView>();

            foreach (var _v in _Result)
            {
                this.listView.Items.Add(new ListViewItem() { Content = _v, Background = GetColorFromNutritionalFoodView(_v) });
            }


            File.WriteAllText(@"C:\TEmp\Json.json", JsonSerializer.Serialize(_Result));
        }
    }
}
