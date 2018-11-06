using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using Matrix.Xmpp.Client;
using Matrix.Xmpp;
using Matrix.Xmpp.Sasl;
using SampleChat.Models;
using Matrix.Net;

namespace SampleChat
{
    public class ChatHub : Hub
    {
        private static readonly Dictionary<string, XmppClient> XmppClients = new Dictionary<string, XmppClient>();
        private static readonly Dictionary<string, List<RosterData>> XmppContactList = new Dictionary<string, List<RosterData>>();

        public override Task OnConnected()
        {
            
            if (!XmppClients.ContainsKey(Context.ConnectionId))
            {
                var xmppClient = new XmppClient();
                XmppContactList.Add(Context.ConnectionId, new List<RosterData>());

                //xmppClient.OnReceiveXml += xmppClient_OnReceiveXml;
                //xmppClient.OnSendXml += xmppClient_OnSendXml;


                xmppClient.OnPresence += xmppClient_OnPresence;
                xmppClient.OnMessage += xmppClient_OnMessage;
                xmppClient.OnIq += xmppClient_OnIq;
                xmppClient.AutoPresence = true;
                xmppClient.AutoRoster = true;
                xmppClient.OnRosterStart += xmppClient_OnRosterStart;
                xmppClient.OnRosterItem += xmppClient_OnRosterItem;
                xmppClient.OnRosterEnd += xmppClient_OnRosterEnd;
                xmppClient.OnRosterItem += new EventHandler<Matrix.Xmpp.Roster.RosterEventArgs>(xmppClient_OnRosterItem);

                xmppClient.OnLogin += xmppClient_OnLogin;
                xmppClient.OnAuthError += xmppClient_OnAuthError;

                xmppClient.OnSendBody += xmppClient_OnSendBody;
                
                xmppClient.OnClose += xmppClient_OnClose;
                xmppClient.OnBeforeSendPresence += xmppClient_OnBeforeSendPresence;
                xmppClient.OnBeforeSasl += xmppClient_OnBeforeSasl;

                XmppClients.Add(Context.ConnectionId, xmppClient);
            }

            return Clients.All.joined(Context.ConnectionId, DateTime.Now.ToString());
        }

        private void xmppClient_OnSendBody(object sender, BodyEventArgs e)
        {
            var ts= e.Body;
        }

        private void xmppClient_OnAuthError(object sender, SaslEventArgs e)
        {
            Clients.Client(Context.ConnectionId).loggedIn(false);
        }

        public override Task OnReconnected()
        {
            return Clients.All.rejoined(Context.ConnectionId, DateTime.Now.ToString());
        }
        public void login(string userName, string password)
        {
            Connect(userName, password, "im-lp-594.innomindshyd.com");
        }

        public void sendMessage(string to, string body)
        {
            var msg = new Message
            {
                To = to,
                Body = body,
                Type = MessageType.Chat
            };
            XmppClients[Context.ConnectionId].Send(msg);
        }

        public void retrieveArchivedMessages(string from)
        {
            var list = new Matrix.Xmpp.MessageArchiving.List
            {
                With = from
            };

            var iq = new Iq
            {
                Type = IqType.Get,
                Query = list
            };
            iq.GenerateId();

            XmppClients[Context.ConnectionId].IqFilter.SendIq(iq, RetrieveArchivedMessagesCallback);
        }

        public void RetrieveArchivedMessagesCallback(object sender, IqEventArgs args)
        {

        }
        
        public void SendRoster(string jid, string name)
        {
            Clients.All.sendRoster(jid, name);
        }

        void xmppClient_OnMessage(object sender, MessageEventArgs e)
        {
            Clients.Client(Context.ConnectionId).onMessage(
                   e.Message.From.User, e.Message.Body
                   );

        }

        void xmppClient_OnIq(object sender, IqEventArgs e)
        {
            //DisplayEvent("OnIq");
        }
        

        void xmppClient_OnBeforeSasl(object sender, Matrix.Xmpp.Sasl.SaslEventArgs e)
        {
            //DisplayEvent("OnBeforeSasl");
        }

        void xmppClient_OnBeforeSendPresence(object sender, PresenceEventArgs e)
        {
            //DisplayEvent("OnBeforeSendPresence");
        }

        void xmppClient_OnClose(object sender, Matrix.EventArgs e)
        {
            //DisplayEvent("OnClose");
        }

        void xmppClient_OnLogin(object sender, Matrix.EventArgs e)
        {
            Clients.Client(Context.ConnectionId).loggedIn(true);
            
        }

        void xmppClient_OnRosterEnd(object sender, Matrix.EventArgs e)
        {
            //DisplayEvent("OnRosterEnd");
        }

        void xmppClient_OnRosterStart(object sender, Matrix.EventArgs e)
        {
            //DisplayEvent("OnRosterStart");
        }

        void xmppClient_OnPresence(object sender, PresenceEventArgs e)
        {
            //DisplayEvent("OnPresence");

            string uniqueId = Matrix.Util.Hash.Sha1HashHex(e.Presence.From.Bare);
            Clients.Client(Context.ConnectionId).onPresence(
                    uniqueId,
                    e.Presence.From.Bare,
                    e.Presence.Type == PresenceType.Unavailable ? "Offline" : ShowToString(e.Presence.Show),
                    e.Presence.Status ?? ""
                );
        }

        void xmppClient_OnRosterItem(object sender, Matrix.Xmpp.Roster.RosterEventArgs e)
        {
            //DisplayEvent("OnRosterItem");
            if (e.RosterItem.Subscription != Matrix.Xmpp.Roster.Subscription.Remove && !XmppContactList[Context.ConnectionId].Any(x => x.Bare == e.RosterItem.Jid.Bare))
            {
                string uniqueId = Matrix.Util.Hash.Sha1HashHex(e.RosterItem.Jid.Bare);
                if (XmppContactList.ContainsKey(Context.ConnectionId))
                {
                    XmppContactList[Context.ConnectionId].Add(new RosterData
                    {
                        uniqueId = uniqueId,
                        Bare = e.RosterItem.Jid.Bare,
                        Name = e.RosterItem.Name
                    }); 
                }
                else
                {
                    XmppContactList.Add(Context.ConnectionId, new List<RosterData> { new RosterData
                    {
                        uniqueId = uniqueId,
                        Bare = e.RosterItem.Jid.Bare,
                        Name = e.RosterItem.Name
                    } });
                }
            }
            else
            {
                if (XmppContactList[Context.ConnectionId].Any(x => x.Bare == e.RosterItem.Jid.Bare))
                {
                    var deletedRoster = XmppContactList[Context.ConnectionId].First(x => x.Bare == e.RosterItem.Jid.Bare);
                    XmppContactList[Context.ConnectionId].Remove(deletedRoster);
                }
            }
            Clients.Client(Context.ConnectionId).onRosterItemReceived(XmppContactList[Context.ConnectionId]);
        }
        //void DisplayEvent(string evt)
        //{
        //    Clients.Client(Context.ConnectionId).displayEvent(evt);
        //}

        public void Connect(String username, String password, String xmppDomain)
        {
            XmppClient xmppClient = XmppClients[Context.ConnectionId];
            xmppClient.SetUsername(username);
            xmppClient.SetXmppDomain(xmppDomain);
            xmppClient.Password = password;

            xmppClient.Status = "Available";
            xmppClient.Show = Show.Chat;
            xmppClient.AutoPresence = true;
            xmppClient.AutoRoster = true;

            xmppClient.Open();
        }

        public void Close()
        {
            XmppClient xmppClient = XmppClients[Context.ConnectionId];
            xmppClient.Close();
        }

        private string ShowToString(Show show)
        {
            switch (show)
            {
                case Show.None:
                    return "Online";
                case Show.Away:
                    return "Away";
                case Show.ExtendedAway:
                    return "Extended away";
                case Show.DoNotDisturb:
                    return "Do not disturb";
                case Show.Chat:
                    return "I want to chat";
            }

            return "Offline";
        }

    }
}