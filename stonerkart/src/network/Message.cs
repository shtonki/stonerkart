using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace stonerkart
{
    [Serializable]
    public abstract class Message
    {
        public int responseID { get; set; }
        public bool demandsResponse => responseID != 0;

        private static IFormatter serializer = new BinaryFormatter();

        public byte[] GetBytes()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.Serialize(ms, this);
                return ms.ToArray();
            }
        }

        public static Message FromBytes(byte[] xd)
        {
            using (MemoryStream ms = new MemoryStream(xd))
                return (Message)serializer.Deserialize(ms);
        }
    }


    [Serializable]
    public class ResponseMessage : Message
    {
        public enum Code
        {
            OK,
            FAILED,
        };
        public Code code { get; set; }
        public string message { get; set; }

        public ResponseMessage()
        {

        }

        private ResponseMessage(Code code, string message)
        {
            this.code = code;
            this.message = message;
        }

        public static ResponseMessage OK => new ResponseMessage(Code.OK, "");
        public static ResponseMessage Failed(string message) { return new ResponseMessage(Code.FAILED, message); }
    }

    public class UserQueryMessage : Message
    {
        public string username { get; set; }
    }
    [Serializable]
    public class UserQueryResponseMessage : Message
    {
        public BasicUserJist jist { get; set; }
    }

    [Serializable]
    public class GameMessage : Message
    {
        public int gameid { get; set; }
        public string message { get; set; }
    }
    [Serializable]
    public class SurrenderMessage : Message
    {
        public int gameid { get; set; }
        public GameEndStateReason reason { get; set; }
    }

    [Serializable]
    public class LoginMessage : Message
    {
        public string username { get; set; }
        public string password { get; set; }
    }
    [Serializable]
    public class LoginResponseMessage : Message
    {
        public bool Success => loggedInUserFullJist != null;
        public BasicUserJist loggedInUserFullJist { get; set; }

    }

    [Serializable]
    public class ChallengeMessage : Message
    {
        public string challengee { get; set; }
    }
    [Serializable]
    public class AcceptChallengeMessage : Message
    {
        public string challenger { get; set; }
    }

    [Serializable]
    public class FriendRequestMessage : Message
    {
        public string username { get; set; }
    }
    [Serializable]
    public class RespondToFriendRequestMessage : Message
    {
        public string requester { get; set; }
        public bool accept { get; set; }
    }
    [Serializable]
    public class UserStatusChangedMessage : Message
    {
        public string username { get; set; }
        public UserStatus newStatus { get; set; }
    }

    [Serializable]
    public class PurchaseMessage : Message
    {
        public Product product { get; set; }
    }
}