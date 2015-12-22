using com.shephertz.app42.gaming.multiplayer.client.events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace TicTacToeYo
{
    public class NotificationListener : com.shephertz.app42.gaming.multiplayer.client.listener.NotifyListener
    {
        private GamePage _page;

        public NotificationListener(GamePage page)
        {
            _page = page;
        }

        public void onRoomCreated(RoomData eventObj)
        {

        }

        public void onRoomDestroyed(RoomData eventObj)
        {

        }

        public void onUserLeftRoom(RoomData eventObj, String username)
        {
            //If user left the room, use dispatcher to tell everybody
            //
            //Call Game over
            if (GlobalContext.localUsername != username)
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    _page.LeaveRoom("Opponent left the game, you won. Please rejoin the game.");
                    _page.GameOver();                  
                });
        }

        public void onUserJoinedRoom(RoomData eventObj, String username)
        {
            //When user joins it will notify first user
            //Unlock all image/squares and update info to click on any square.
            _page.showMessageBox(username + " joined the game. You may begin.");
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                _page.UnLockSquares();
                _page.NewGame();
                _page.UpdateStatus("Click on any square to start the game.");               
            });

        }

        public void onUserLeftLobby(LobbyData eventObj, String username)
        {

        }

        public void onUserJoinedLobby(LobbyData eventObj, String username)
        {

        }

        public void onChatReceived(ChatEvent eventObj)
        {

        }

        public void onUpdatePeersReceived(UpdateEvent eventObj)
        {
            //Read the message
            //Create message Object, check the type
            //If new, notify that new game has been initiated
            //Otherwise update everybody with the move
            MoveMessage msg = MoveMessage.buildMessage(eventObj.getUpdate());
            if (msg.type == "new")
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    _page.showMessageBox("New game has been initiated.");
                    _page.NewGame();
                });
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {                
                    _page.UpdateMove(msg);
                });
            }
        }

        public void onUserChangeRoomProperty(RoomData roomData, string sender, Dictionary<string, object> properties)
        {

        }


        public void onUserChangeRoomProperty(RoomData roomData, string sender, Dictionary<string, object> properties, Dictionary<string, string> lockedPropertiesTable)
        {
            throw new NotImplementedException();
        }

        public void onPrivateChatReceived(string sender, string message)
        {
            throw new NotImplementedException();
        }

        public void onMoveCompleted(MoveEvent moveEvent)
        {
            throw new NotImplementedException();
        }

        public void onUserPaused(string locid, bool isLobby, string username)
        {
            throw new NotImplementedException();
        }

        public void onUserResumed(string locid, bool isLobby, string username)
        {
            throw new NotImplementedException();
        }

        public void onGameStarted(string sender, string roomId, string nextTurn)
        {
            throw new NotImplementedException();
        }

        public void onGameStopped(string sender, string roomId)
        {
            throw new NotImplementedException();
        }

        public void onPrivateUpdateReceived(string sender, byte[] update, bool fromUdp)
        {
            throw new NotImplementedException();
        }


        public void onNextTurnRequest(string lastTurn)
        {
            throw new NotImplementedException();
        }
    }
}
