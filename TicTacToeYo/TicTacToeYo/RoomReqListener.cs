using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.command;
using com.shephertz.app42.gaming.multiplayer.client.events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace TicTacToeYo
{
    public class RoomReqListener : com.shephertz.app42.gaming.multiplayer.client.listener.RoomRequestListener
    {
        private MainPage _page;
        public RoomReqListener(MainPage page)
        {
            _page = page;
        }
        public void onSubscribeRoomDone(RoomEvent eventObj)
        {
            if (eventObj.getResult() == WarpResponseResultCode.SUCCESS)
            {
                WarpClient.GetInstance().GetLiveRoomInfo(GlobalContext.GameRoomId);
            }
        }
        public void onUnSubscribeRoomDone(RoomEvent eventObj)
        {
            if (eventObj.getResult() == WarpResponseResultCode.SUCCESS)
            {
                _page.showResult("Yay! UnSubscribe room :)");
            }
        }
        public void onJoinRoomDone(RoomEvent eventObj)
        {
            if (eventObj.getResult() == WarpResponseResultCode.SUCCESS)
            {
                WarpClient.GetInstance().SubscribeRoom(GlobalContext.GameRoomId);
            }
            else
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("There are already 2 users playing.");
                });
            }
        }
        public void onLeaveRoomDone(RoomEvent eventObj)
        {
            if (eventObj.getResult() == WarpResponseResultCode.SUCCESS)
            {
                _page.showResult("Yay! Leave room :)");
            }
        }

        public void onGetLiveRoomInfoDone(LiveRoomInfoEvent eventObj)
        {
            if (eventObj.getResult() == WarpResponseResultCode.SUCCESS && (eventObj.getJoinedUsers() != null))
            {
                if (eventObj.getJoinedUsers().Length == 1)
                {
                    GlobalContext.PlayerIsFirst = true;
                }
                else
                {
                    GlobalContext.PlayerIsFirst = false;
                }
                // navigate to game play screen  
                _page.moveToGame();
            }
        }
        public void onSetCustomRoomDataDone(LiveRoomInfoEvent eventObj)
        {
        }
        public void onUpdatePropertyDone(LiveRoomInfoEvent lifeLiveRoomInfoEvent)
        {
        }
        public void onLockPropertiesDone(byte result)
        {
            throw new NotImplementedException();
        }

        public void onUnlockPropertiesDone(byte result)
        {
            throw new NotImplementedException();
        }
    }
}
