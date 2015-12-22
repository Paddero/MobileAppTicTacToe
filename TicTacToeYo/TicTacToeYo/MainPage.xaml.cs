using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using TicTacToeYo.Resources;
using com.shephertz.app42.gaming.multiplayer.client;

namespace TicTacToeYo
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            WarpClient.initialize(GlobalContext.API_KEY, GlobalContext.SECRET_KEY);  
            GlobalContext.warpClient = WarpClient.GetInstance();
        }

        private void btnJoin_Click(object sender, RoutedEventArgs e)
        {
            string username = tbName.Text;
            if (String.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Please Specifiy user name");
            }
            else
            {
                // Initiate the connection  
                // Create and add listener objects to receive callback events for the APIs used  
                GlobalContext.conListenObj = new ConnectionListener(this);
                GlobalContext.roomReqListenerObj = new RoomReqListener(this);
                GlobalContext.warpClient.AddConnectionRequestListener(GlobalContext.conListenObj);
                GlobalContext.warpClient.AddRoomRequestListener(GlobalContext.roomReqListenerObj);
                GlobalContext.localUsername = tbName.Text;
                WarpClient.GetInstance().Connect(GlobalContext.localUsername);
                Visibility = Visibility.Collapsed;
            }
            
        }

        internal void moveToGame()
        {
            Dispatcher.BeginInvoke(() =>
            NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.RelativeOrAbsolute)));
        }

        public void showResult(String result)
        {
            //MessageBox.Show(result.ToString());
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                MessageBox.Show(result.ToString());
            });
        }

        private void tbName_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            tbName.Text = "";
        }
    }
}