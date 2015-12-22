using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.listener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToeYo
{
    class GlobalContext
    {
        public static String localUsername;
        public static String myPiece;
        public static String img1Piece;
        public static String img2Piece;
        public static String img3Piece;
        public static String img4Piece;
        public static String img5Piece;
        public static String img6Piece;
        public static String img7Piece;
        public static String img8Piece;
        public static String img9Piece;
        public static String isBlank = "/Images/blank.png";
        public static String API_KEY = "c2c51415d3f3f69fcc0a083fb600123b149db4c77ca35f894c705e4ec972b919";
        public static String SECRET_KEY = "1ef87f70de91c035774fa5525f5faaff29f0611e1ea223f19a67e4a9b865209f";  
        public static String GameRoomId = "450615198";
        internal static bool PlayerIsFirst = false;
        public static WarpClient warpClient;
        public static ConnectionListener conListenObj;
        public static RoomReqListener roomReqListenerObj;
        public static NotificationListener notificationListenerObj;
        public static ChatRequestListener chatListenerObj;
    }
}
