using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.command;
using com.shephertz.app42.gaming.multiplayer.client.events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicTacToeYo
{
    public class ConnectionListener : com.shephertz.app42.gaming.multiplayer.client.listener.ConnectionRequestListener
    {
        private MainPage _page;
        public ConnectionListener(MainPage result)
        {
            _page = result;
        }
        public void onConnectDone(ConnectEvent eventObj)
        {
            if (eventObj.getResult() == WarpResponseResultCode.SUCCESS)
            {
                // Successfully connected to the server. Lets go ahead and join the room.  
                WarpClient.GetInstance().JoinRoom(GlobalContext.GameRoomId);
            }
            else
            {
                _page.showResult("Connection failed");
            }
        }
        public void onDisconnectDone(ConnectEvent eventObj)
        {
            if (eventObj.getResult() == WarpResponseResultCode.SUCCESS)
            {
                _page.showResult("Successfully disconnected.");
            }
            else
            {
                _page.showResult("Disconnection failed.");
            }
        }

        public void onInitUDPDone(byte resultCode)
        {
            throw new NotImplementedException();
        }
    }
}
