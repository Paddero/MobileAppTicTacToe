using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using com.shephertz.app42.gaming.multiplayer.client;

namespace TicTacToeYo
{
    public partial class GamePage : PhoneApplicationPage
    {
        internal Dictionary<string, string> _pieceMap;
        private int moveCount = 0;
        public GamePage()
        {
            InitializeComponent();
            //Initialising Listener Objects to listen to any calls from users in this room.
            GlobalContext.notificationListenerObj = new NotificationListener(this);
            GlobalContext.warpClient.AddNotificationListener(GlobalContext.notificationListenerObj);

            //When user joins then decide whether he is waiting for opponent or waiting for opponents move.
            if (GlobalContext.PlayerIsFirst)
            {
                InfoBox.Text = "Waiting on second user to join. You are assigned X's.";
                GlobalContext.myPiece = "/Images/x.png";
            }
            else
            {
                InfoBox.Text = "Waiting for opponents move... You are assigned O's.";
                GlobalContext.myPiece = "/Images/o.png";
            }
            LockSquares();           
        }

        //Initialize the board so we know if it is filled with X's and O's so we can check at later stages of the game
        private void _initBoard()
        {
            _pieceMap = new Dictionary<string, string>();
            moveCount = 0;

            foreach (Image image in ContentPanel.Children.OfType<Image>())
            {
                _pieceMap.Add(image.Name, string.Empty);
                image.Source = new BitmapImage(new Uri(@GlobalContext.isBlank, UriKind.RelativeOrAbsolute));
            }

            //Unlock the squares so the users can tap on squares.
            UnLockSquares();
        }

        //Encapsulate the method initboard as we need dispatcher to call it
        internal void InitializeBoard()
        {
            if (System.Windows.Deployment.Current.Dispatcher.CheckAccess())
            {
                _initBoard();
            }
            else
            {
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    _initBoard();

                });
            }

        }

        //Just new game, if its called in any way.
        internal void NewGame()
        {
            ClearImgPieces();
            AssignPieces();
            InitializeBoard();
        }

        //Clearing all global img pieces so they are clear
        public void ClearImgPieces()
        {
            GlobalContext.img1Piece = "";
            GlobalContext.img2Piece = "";
            GlobalContext.img3Piece = "";
            GlobalContext.img4Piece = "";
            GlobalContext.img5Piece = "";
            GlobalContext.img6Piece = "";
            GlobalContext.img7Piece = "";
            GlobalContext.img8Piece = "";
            GlobalContext.img9Piece = "";
        }

        //Assigning who is first, who has X's and who has O's
        public void AssignPieces()
        {
            if (GlobalContext.PlayerIsFirst == true)
            {
                InfoBox.Text = "You are first player, you are assigned X's.";
                GlobalContext.myPiece = "/Images/x.png";
            }
            else
            {
                InfoBox.Text = "You are second player, you are assigned O's.";
                GlobalContext.myPiece = "/Images/o.png";
            }
        }

        //Creating the new game button at the bottom of the page
        private void appbarNewGame_Click(object sender, EventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("You really want to start a new game?", "Are you sure?", MessageBoxButton.OKCancel);
            //If its OK then start new game, otherwise do nothing.
            if (result == MessageBoxResult.OK)
            {
                //Sending a message to everyone in the room about new game.
                GlobalContext.warpClient.SendUpdatePeers(MoveMessage.buildNewGameMessageBytes());
            }
            
        }

        //Creating the endgame button even at the bottom of the page
        private void appbarEndGame_Click(object sender, EventArgs e)
        {
            // Clean up listeners and unsubscribe to stop receiving further events
            GlobalContext.warpClient.UnsubscribeRoom(GlobalContext.GameRoomId);
            GlobalContext.warpClient.LeaveRoom(GlobalContext.GameRoomId);
            GlobalContext.warpClient.RemoveNotificationListener(GlobalContext.notificationListenerObj);
            GlobalContext.warpClient.RemoveRoomRequestListener(GlobalContext.roomReqListenerObj);
            GlobalContext.warpClient.RemoveConnectionRequestListener(GlobalContext.conListenObj);
            GlobalContext.warpClient.Disconnect();
            //Go back to the main page
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
            //Clear all imgPieces
            ClearImgPieces();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("You really want to leave the game?", "Are you sure?", MessageBoxButton.OKCancel);
            //If its OK then start new game, otherwise do nothing.
            if (result == MessageBoxResult.OK)
            {
                // Clean up listeners and unsubscribe to stop receiving further events
                GlobalContext.warpClient.UnsubscribeRoom(GlobalContext.GameRoomId);
                GlobalContext.warpClient.LeaveRoom(GlobalContext.GameRoomId);
                GlobalContext.warpClient.RemoveNotificationListener(GlobalContext.notificationListenerObj);
                GlobalContext.warpClient.RemoveRoomRequestListener(GlobalContext.roomReqListenerObj);
                GlobalContext.warpClient.RemoveConnectionRequestListener(GlobalContext.conListenObj);
                GlobalContext.warpClient.Disconnect();
                //Go back to the main page
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
                //Clear all imgPieces
                ClearImgPieces();
            }
        }

        //Tap event for every square that is tapped
        private void Tap_Img(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //All squares are locked when waiting on opponent, but incase new game has been initiliased, this statement check where its your move or opponents.
            if ((moveCount % 2 == 0 && !GlobalContext.PlayerIsFirst) || (moveCount % 2 != 0 && GlobalContext.PlayerIsFirst))
            {
                MessageBox.Show("Waiting on Opponent!");
            }
            else
            {
                // create tapped object from sender.
                Image tapped = (Image)sender;

                //Checking if the the square is blank by comparing the source to the global context source for blank square
                if(tapped.Source.GetType() == typeof(BitmapImage) && (tapped.Source as BitmapImage).UriSource.Equals(GlobalContext.isBlank))
                {   
                    //If its blank, send a move (JSON) to everyone in the room with the tapped image and the source for an image for users piece.
                    //Then lock all the squares to this user can't tap on any while waiting for opponent and updating the status
                    WarpClient.GetInstance().SendUpdatePeers(MoveMessage.buildMessageBytes(tapped, GlobalContext.myPiece));
                    LockSquares();
                    UpdateStatus("Opponent's Turn. Please wait.");
                }
                else
                {
                    //If its taken then simply display a messagebox saying its allrdy taken
                    MessageBox.Show("This Square is taken.");
                } 
            }     
        }

        //Encapsulate the method updateStatus as we need dispatcher to call it
        internal void UpdateStatus(String status)
        {
            if (System.Windows.Deployment.Current.Dispatcher.CheckAccess())
            {
                _updateStatus(status);
            }
            else
            {
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    _updateStatus(status);

                });
            }
        }

        //The real updateStatus method
        private void _updateStatus(string status)
        {
            InfoBox.Text = status;
        }

        //Encapsulate the method leaveRoom as we need dispatcher to call it
        internal void LeaveRoom(String reason)
        {
            if (System.Windows.Deployment.Current.Dispatcher.CheckAccess())
            {
                _LeaveRoom(reason);
            }
            else
            {
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    _LeaveRoom(reason);

                });
            }
        }

        //The real leave room method, show message what's the reason
        private void _LeaveRoom(string reason)
        {
            MessageBox.Show(reason);
            // Clean up listeners and unsubscribe to stop receiving further events
            GlobalContext.warpClient.UnsubscribeRoom(GlobalContext.GameRoomId);
            GlobalContext.warpClient.LeaveRoom(GlobalContext.GameRoomId);
            GlobalContext.warpClient.RemoveNotificationListener(GlobalContext.notificationListenerObj);
            GlobalContext.warpClient.RemoveRoomRequestListener(GlobalContext.roomReqListenerObj);
            GlobalContext.warpClient.RemoveConnectionRequestListener(GlobalContext.conListenObj);
            GlobalContext.warpClient.Disconnect();
            //Go back to main page
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
            //Clear all imgPieces
            ClearImgPieces();
        }

        //Encapsulate the method showMessageBox as we need dispatcher to call it
        internal void showMessageBox(String information)
        {
            if (System.Windows.Deployment.Current.Dispatcher.CheckAccess())
            {
                _showMessageBox(information);
            }
            else
            {
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    _showMessageBox(information);
                });
            }
        }

        //The real show message method
        private void _showMessageBox(string information)
        {
            MessageBox.Show(information);
        }

        //This is how the game is played
        private void PlayGame(Image image, MoveMessage msg)
        {
            //By receiving the image object, check which image was being tapped
            //Then check if the piece added to that square was an image source for an X
            //If so, change global img1piece to x, otherwise to an o
            if (image.Name == "img1")
            {
                if (msg.piece == "/Images/x.png")
                    GlobalContext.img1Piece = "x";
                else
                    GlobalContext.img1Piece = "o";
            }
            else if (image.Name == "img2")
            {
                if (msg.piece == "/Images/x.png")
                    GlobalContext.img2Piece = "x";
                else
                    GlobalContext.img2Piece = "o";
            }
            else if (image.Name == "img3")
            {
                if (msg.piece == "/Images/x.png")
                    GlobalContext.img3Piece = "x";
                else
                    GlobalContext.img3Piece = "o";
            }
            else if (image.Name == "img4")
            {
                if (msg.piece == "/Images/x.png")
                    GlobalContext.img4Piece = "x";
                else
                    GlobalContext.img4Piece = "o";
            }
            else if (image.Name == "img5")
            {
                if (msg.piece == "/Images/x.png")
                    GlobalContext.img5Piece = "x";
                else
                    GlobalContext.img5Piece = "o";
            }
            else if (image.Name == "img6")
            {
                if (msg.piece == "/Images/x.png")
                    GlobalContext.img6Piece = "x";
                else
                    GlobalContext.img6Piece = "o";
            }
            else if (image.Name == "img7")
            {
                if (msg.piece == "/Images/x.png")
                    GlobalContext.img7Piece = "x";
                else
                    GlobalContext.img7Piece = "o";
            }
            else if (image.Name == "img8")
            {
                if (msg.piece == "/Images/x.png")
                    GlobalContext.img8Piece = "x";
                else
                    GlobalContext.img8Piece = "o";
            }
            else if (image.Name == "img9")
            {
                if (msg.piece == "/Images/x.png")
                    GlobalContext.img9Piece = "x";
                else
                    GlobalContext.img9Piece = "o";
            }
          
            // The user can tap on the gameboard without clicking "New Game". In this case we may 
            // need to initialize the board if it has not been initialized already.
            // Initialization is signified by the _pieceMap not being null.
            if (_pieceMap == null)
                InitializeBoard();
         
            // Change the source of an image received to the source of a piece it was sent.
            image.Source = new BitmapImage(new Uri(@msg.piece, UriKind.RelativeOrAbsolute));
            

            // Update the piecemap so we know a piece has been added for a image
            _pieceMap[image.Name] = msg.piece;

            // Check if there is a winner, if so, call gameOver method.
            if (isThereWinner)
            {
                GameOver();
            }
            else
            {
                // If there is no winner, check if there are still any square left to play
                if (anyPiecesLeft())
                {
                    //If the message sender isnt the user, notify that it is their turn
                    //And unlock all the images/squares
                    if (msg.sender != GlobalContext.localUsername)
                    {
                        MessageBox.Show("Your turn!");
                        UpdateStatus("Your Turn!");
                        UnLockSquares();                       
                    }
                }
                else
                {
                    // Nobody wins - end the game
                    MessageBox.Show("Nobody Won - It's a draw!");
                    UpdateStatus("Nobody Won - It's a draw!");
                    GameOver();                  
                }

            }        
        }

        //Check if there are any piece left
        //by checking the value for each value in the map
        private bool anyPiecesLeft()
        {
            bool anyPiecesAvailable = false;
            foreach (string value in _pieceMap.Values)
            {
                if (String.IsNullOrEmpty(value))
                {
                    anyPiecesAvailable = true;
                    break;
                }
            }
            return anyPiecesAvailable;
        }

        // Check if there is a winner
        internal bool isThereWinner
        {
            get
            {
                //Get the string x or o from checkWinningSquares method
                string winnersName = checkWinningSquares();

                if (!String.IsNullOrEmpty(winnersName))
                {
                    // If user is first and string is x
                    // OR player isnt first and string is o
                    // It means user has won.
                    if(GlobalContext.PlayerIsFirst && winnersName.ToLower() == "x" || !GlobalContext.PlayerIsFirst && winnersName.ToLower() == "o")
                    {
                        //Notify user that he/she won
                        MessageBox.Show("You Won!");
                        UpdateStatus("You Won!");
                        return true;
                    }
                    else
                    {
                        //Notify others that they lost
                        MessageBox.Show("You Lost :(!");
                        UpdateStatus("You Lost :(!");
                        return true;
                    }
                }
                return false;
            }
        }

        //Check all the images for winning line
        private string checkWinningSquares()
        {
            //Checking Top Horizontal line
            if (!String.IsNullOrWhiteSpace(GlobalContext.img1Piece) && GlobalContext.img1Piece == GlobalContext.img2Piece && GlobalContext.img2Piece == GlobalContext.img3Piece)
            {
                return GlobalContext.img1Piece;
            }

            //Checking Middle Horizontal line
            if (!String.IsNullOrWhiteSpace(GlobalContext.img4Piece) && GlobalContext.img4Piece == GlobalContext.img5Piece && GlobalContext.img5Piece == GlobalContext.img6Piece)
            {
                return GlobalContext.img4Piece;
            }

            //Checking Bottom Horizontal line
            if (!String.IsNullOrWhiteSpace(GlobalContext.img7Piece) && GlobalContext.img7Piece == GlobalContext.img8Piece && GlobalContext.img8Piece == GlobalContext.img9Piece)
            {
                return GlobalContext.img7Piece;
            }

            //Checking Left Vertical line
            if (!String.IsNullOrWhiteSpace(GlobalContext.img1Piece) && GlobalContext.img1Piece == GlobalContext.img4Piece && GlobalContext.img4Piece == GlobalContext.img7Piece)
            {
                return GlobalContext.img1Piece;
            }

            //Checking Middle Vertical line
            if (!String.IsNullOrWhiteSpace(GlobalContext.img2Piece) && GlobalContext.img2Piece == GlobalContext.img5Piece && GlobalContext.img5Piece == GlobalContext.img8Piece)
            {
                return GlobalContext.img2Piece;
            }

            //Checking Right Vertical line
            if (!String.IsNullOrWhiteSpace(GlobalContext.img3Piece) && GlobalContext.img3Piece == GlobalContext.img6Piece && GlobalContext.img6Piece == GlobalContext.img9Piece)
            {
                return GlobalContext.img3Piece;
            }

            //Checking Top Left To Bottom Right line
            if (!String.IsNullOrWhiteSpace(GlobalContext.img1Piece) && GlobalContext.img1Piece == GlobalContext.img5Piece && GlobalContext.img5Piece == GlobalContext.img9Piece)
            {
                return GlobalContext.img1Piece;
            }

            //Checking Bottom Left To Top Right line
            if (!String.IsNullOrWhiteSpace(GlobalContext.img7Piece) && GlobalContext.img7Piece == GlobalContext.img5Piece && GlobalContext.img5Piece == GlobalContext.img3Piece)
            {
                return GlobalContext.img7Piece;
            }

            //Else nobody won yet
            return String.Empty;
        }
        
        //Game Over, lock all image/squares
        internal void GameOver()
        {
            LockSquares();
        }

        //Lock all images/squares
        internal void LockSquares()
        {
            ContentPanel.IsHitTestVisible = false;
        }

        //Unlock all images/squares
        internal void UnLockSquares()
        {
            ContentPanel.IsHitTestVisible = true;
        }

        //Update everybody on the move
        internal void UpdateMove(MoveMessage msg)
        {
            //Create image object same as the one that is passed in the message and pass it on to playGame
            //Increment moveCount.
            Image image = (Image)ContentPanel.FindName(msg.ImageName);
            PlayGame(image, msg);
            moveCount++;
        }
    }
}