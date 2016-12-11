﻿using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace stonerkart
{
    interface SyncConnection
    {
        Message receive();
        void send(Message m);
    }

    static class Network
    {
        public static ServerConnection serverConnection;
        private const string servername = "_server";
        private static ManualResetEvent messageReceived = new ManualResetEvent(false);
        private static Message receivedMessage;

        public static bool connectToServer()
        {
            try
            {
                serverConnection = new ServerConnection();
                return true;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                return false;
            }
        }

        public static void handle(Message m)
        {
            receivedMessage = m;
            switch (m.messageType)
            {
                case Message.MessageType.RESPONSE:
                {
                    receivedMessage = m;
                    messageReceived.Set();
                } break;
            }
        }

        private static Message awaitResponseMessage()
        {
            messageReceived.WaitOne();
            Message r = receivedMessage;
            receivedMessage = null;
            messageReceived.Reset();
            return r;
        }

        public static bool login(string username, string password)
        {
            LoginBody b = new LoginBody(username, password);
            serverConnection.send(new Message(servername, Message.MessageType.LOGIN, b));
            Message m = awaitResponseMessage();
            ResponseBody rb = new ResponseBody(m.body);
            if (rb.code == ResponseBody.ResponseCode.OKWITHFRIENDS)
            {
                FriendListBody fb = new FriendListBody(rb.text);
                Controller.setFriendList(fb.friends);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool register(string username, string password)
        {
            LoginBody b = new LoginBody(username, password);
            serverConnection.send(new Message(servername, Message.MessageType.REGISTER, b));
            return false;
        }

        public static bool addFriend(string username)
        {
            AddFriendBody b = new AddFriendBody(username);
            serverConnection.send(new Message(servername, Message.MessageType.ADDFRIEND, b));
            Message m = awaitResponseMessage();
            ResponseBody rb = new ResponseBody(m.body);
            return rb.code == ResponseBody.ResponseCode.OK;
        }

        public static bool challenge(string username)
        {
            ChallengeBody b = new ChallengeBody(username);
            serverConnection.send(new Message(servername, Message.MessageType.CHALLENGE, b));
            Message m = awaitResponseMessage();
            ResponseBody rb = new ResponseBody(m.body);
            return rb.code == ResponseBody.ResponseCode.OK;
        }
    }
}
