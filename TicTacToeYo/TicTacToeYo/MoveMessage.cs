using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TicTacToeYo
{
    public class MoveMessage
    {
        public String sender;
        public String ImageName;
        public String piece;
        public String type;

        //Read the Message
        public static MoveMessage buildMessage(byte[] update)
        {
            JObject jsonObj = JObject.Parse(System.Text.Encoding.UTF8.GetString(update, 0, update.Length));
            MoveMessage msg = new MoveMessage();
            msg.sender = jsonObj["sender"].ToString();
            msg.type = jsonObj["type"].ToString();
            if (msg.type == "move")
            {
                msg.ImageName = jsonObj["ImageName"].ToString();
                msg.piece = jsonObj["piece"].ToString();              
            }
            return msg;
        }

        //Build Message
        public static byte[] buildMessageBytes(Image image, String piece)
        {
            // Create JSON object, add imageName, username, the piece (which is the source for an image X or O) and the type which is move.
            JObject moveObj = new JObject();
            moveObj.Add("ImageName", image.Name);
            moveObj.Add("sender", GlobalContext.localUsername);
            moveObj.Add("piece", piece);
            moveObj.Add("type", "move");
            return System.Text.Encoding.UTF8.GetBytes(moveObj.ToString());
        }

        //Build NEW game message to let everybody know that new game has been initiliased.
        public static byte[] buildNewGameMessageBytes()
        {
            JObject moveObj = new JObject();
            moveObj.Add("sender", GlobalContext.localUsername);
            moveObj.Add("type", "new");
            return System.Text.Encoding.UTF8.GetBytes(moveObj.ToString());
        }

    }
}
