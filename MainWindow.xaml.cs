using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
using WpfTutorialSamples.Dialogs;
using VkApi.Wrapper;
using VkApi.Wrapper.Auth;
using Windows.UI.Xaml.Navigation;
using Windows.Web.UI;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using Newtonsoft.Json;

namespace WpfApp1
{

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Vkontakte vk = new Vkontakte(7655717);
        FrameworkElement ElementMainScene, ElementSettingsScene, ElementFriendsScene, NavPanel;
        StackPanel Friends;
        public MainWindow()
        {
            InitializeComponent();
            ElementMainScene = (FrameworkElement)FindName("MainScene");
            ElementSettingsScene = (FrameworkElement)FindName("SettingsScene");
            ElementFriendsScene = (FrameworkElement)FindName("FriendsScene");
            NavPanel = (FrameworkElement)FindName("Navigation_panel");
            Friends = (StackPanel)FindName("FriendsHolder");
            Image img = new Image();
            img.Source = (new ImageSourceConverter()).ConvertFromString("C:\\Users\\Пользователь\\source\\repos\\WpfApp1\\img\\eofffriends.png") as ImageSource;
            CreateFriend(img,"Test","Status",1);
            SetAuthScene(null, null);
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public async void ReLoginAsync(object sender, RoutedEventArgs e) {
            await BrowserView.EnsureCoreWebView2Async();
            if (BrowserView != null && BrowserView.CoreWebView2 != null)
            {
                var url = vk.OAuth.GetAuthUrl(ScopeSettings.CanAccessFriends | ScopeSettings.CanAccessPhotos | ScopeSettings.CanAccessOffline, AuthDisplayType.Mobile);
                BrowserView.CoreWebView2.Navigate(url.AbsoluteUri);
                SetMainScene(null,null);
            }
            
            
        }


        private void OnNavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs args)

        {  BrowserView.Visibility = Visibility.Collapsed;
            string json = File.ReadAllText("C:\\Users\\Пользователь\\source\\repos\\WpfApp1\\store\\access_token.json",
                Encoding.UTF8);
            AccessToken at;
            if ((json == null) || (json.Length == 0)) at = null;
            else at = JsonConvert.DeserializeObject<AccessToken>(json);
            if (at == null || at.HasExpired)
            {
                BrowserView.Visibility = Visibility.Visible;
                OAuthResult result = vk.OAuth.ParseResponseUrl(new Uri(args.Uri));
                if (result?.IsSuccess == true)
                {
                    // We are authenticated! Let's save the token.
                    vk.AccessToken = result.AccessToken;
                    BrowserView.Visibility = Visibility.Collapsed;
                    File.WriteAllText("C:\\Users\\Пользователь\\source\\repos\\WpfApp1\\store\\access_token.json",
                        JsonConvert.SerializeObject(vk.AccessToken) ,
                        Encoding.UTF8);
                }
            }
            else {
                vk.AccessToken = at;
            }

        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Navigation_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton pressed = (RadioButton)sender;
            MessageBox.Show(pressed.Content.ToString());
        }
        private void Empty(object sender, RoutedEventArgs e) {
            MessageBox.Show("Push");
            return;
        }
        private void Empty(object sender, DependencyPropertyChangedEventArgs e) {
            MessageBox.Show("Push");
            return;
        }
        private void NavHoverStart(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            btn.Background = new SolidColorBrush(Color.FromArgb(12,12,12,0));
            DependencyObject child = VisualTreeHelper.GetChild(btn, 0);
            child = VisualTreeHelper.GetChild(child, 0);
            child = VisualTreeHelper.GetChild(child, 0);
            Image i1 = VisualTreeHelper.GetChild(child, 0) as Image;
            Image i2 = VisualTreeHelper.GetChild(child, 1) as Image;
            i1.Visibility = Visibility.Visible;
            i2.Visibility = Visibility.Collapsed;
            Label l = VisualTreeHelper.GetChild(child, 2) as Label;
            l.Foreground = Brushes.Indigo;
            return;
        }
        private void NavHoverEnd(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            btn.Background = Brushes.Transparent;
            DependencyObject child = VisualTreeHelper.GetChild(btn, 0);
            child = VisualTreeHelper.GetChild(child, 0);
            child = VisualTreeHelper.GetChild(child, 0);
            Image i1 = VisualTreeHelper.GetChild(child, 0) as Image;
            Image i2 = VisualTreeHelper.GetChild(child, 1) as Image;
            i2.Visibility = Visibility.Visible;
            i1.Visibility = Visibility.Collapsed;
            Label l = VisualTreeHelper.GetChild(child, 2) as Label;
            l.Foreground = Brushes.Black;
            return;
        }

        private void SetMainScene(object sender, RoutedEventArgs e)
        {
            AuthScene.Visibility = Visibility.Collapsed;
            NavPanel.Visibility = Visibility.Visible;
            ElementFriendsScene.Visibility = Visibility.Collapsed;
            ElementMainScene.Visibility = Visibility.Visible;
            ElementSettingsScene.Visibility = Visibility.Collapsed;
        }
        private void SetSettingsScene(object sender, RoutedEventArgs e)
        {
            ElementFriendsScene.Visibility = Visibility.Collapsed;
            ElementMainScene.Visibility = Visibility.Collapsed;
            ElementSettingsScene.Visibility = Visibility.Visible;
        }
        private void SetFriendsScene(object sender, RoutedEventArgs e)
        {
            ElementFriendsScene.Visibility = Visibility.Visible;
            ElementMainScene.Visibility = Visibility.Collapsed;
            ElementSettingsScene.Visibility = Visibility.Collapsed;
        }
        private void CreateFriend(Image img, string name, string status, int index) {
            Grid toAdd = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = Brushes.White,
                Margin = new Thickness(5),
                Visibility = Visibility.Visible
            };
            toAdd.Name = "Friend" + index.ToString();
            /////////////////////////////////////////////////////////////////
            ColumnDefinition tmp = new ColumnDefinition();
            tmp.Width = new GridLength(10, GridUnitType.Pixel);
            toAdd.ColumnDefinitions.Add(tmp);
            tmp = new ColumnDefinition();
            tmp.Width = new GridLength(80, GridUnitType.Pixel);
            toAdd.ColumnDefinitions.Add(tmp);
            tmp = new ColumnDefinition();
            tmp.Width = new GridLength(10, GridUnitType.Pixel);
            toAdd.ColumnDefinitions.Add(tmp);
            tmp = new ColumnDefinition();
            tmp.Width = new GridLength(1, GridUnitType.Star);
            toAdd.ColumnDefinitions.Add(tmp);
            tmp = new ColumnDefinition();
            tmp.Width = new GridLength(10, GridUnitType.Pixel);
            toAdd.ColumnDefinitions.Add(tmp);
            tmp = new ColumnDefinition();
            tmp.Width = new GridLength(25, GridUnitType.Pixel);
            toAdd.ColumnDefinitions.Add(tmp);
            tmp = new ColumnDefinition();
            tmp.Width = new GridLength(10, GridUnitType.Pixel);
            toAdd.ColumnDefinitions.Add(tmp);
            /////////////////////////////////////////////////////////////////
            RowDefinition tmp1 = new RowDefinition();
            tmp1.Height = new GridLength(10, GridUnitType.Pixel);
            toAdd.RowDefinitions.Add(tmp1);
            tmp1 = new RowDefinition();
            tmp1.MinHeight = 80;
            toAdd.RowDefinitions.Add(tmp1);
            tmp1 = new RowDefinition();
            tmp1.Height = new GridLength(10, GridUnitType.Pixel);
            toAdd.RowDefinitions.Add(tmp1);
            /////////////////////////////////////////////////////////////////
            Button btn = new Button();
            btn.Click += new RoutedEventHandler(Empty);  /////////////// add a friend orig photo shower event handler //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            btn.SetValue(Grid.ColumnProperty,1);
            btn.SetValue(Grid.RowProperty, 1);
            img.Width = 80;
            img.Clip = new EllipseGeometry { Center = new Point(40, 40), RadiusX = 40, RadiusY = 40 };
            btn.Content = img;
            /////////////////////////////////////////////////////////////////
            toAdd.Children.Add(btn);
            Grid grid = new Grid {VerticalAlignment = VerticalAlignment.Top};
            grid.SetValue(Grid.ColumnProperty, 3);
            grid.SetValue(Grid.RowProperty, 1);
            /////////////////////////////////////////////////////////////////
            tmp1 = new RowDefinition();
            tmp1.Height = new GridLength(1, GridUnitType.Star);
            grid.RowDefinitions.Add(tmp1);
            tmp1 = new RowDefinition();
            tmp1.Height = new GridLength(1, GridUnitType.Star);
            grid.RowDefinitions.Add(tmp1);
            tmp1 = new RowDefinition();
            tmp1.Height = new GridLength(1, GridUnitType.Star);
            grid.RowDefinitions.Add(tmp1);
            /////////////////////////////////////////////////////////////////
            Label l1 = new Label(), l2 = new Label();
            l1.SetValue(Grid.RowProperty, 0);
            l2.SetValue(Grid.RowProperty, 1);
            l1.Content = name;
            l2.Content = status;
            grid.Children.Add(l1);
            grid.Children.Add(l2);
            toAdd.Children.Add(grid);
            Friends.Children.Add(toAdd);
        }

        private void SetAuthScene(object sender, RoutedEventArgs e)
        {
            AuthScene.Visibility = Visibility.Visible;
            NavPanel.Visibility = Visibility.Collapsed;
            ElementFriendsScene.Visibility = Visibility.Collapsed;
            ElementMainScene.Visibility = Visibility.Collapsed;
            ElementSettingsScene.Visibility = Visibility.Collapsed;
        }
    }
}
