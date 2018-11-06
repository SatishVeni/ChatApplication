using System;
using Matrix;
using Matrix.Xmpp.Client;
using Matrix.Xmpp.Sasl;

namespace SampleChat.MatrixSvc
{
    public class MatrixService
    {
        private ChatHub hub  = new ChatHub();
        XmppClient xmppClient = new XmppClient();
        string jidExtension = "@im-lp-594.innomindshyd.com";
        string loginMsg = string.Empty;
        bool loginStatus = false;
        public MatrixService()
        {
            this.xmppClient.Compression = false;
            this.xmppClient.ProxyPort = 1080;
            this.xmppClient.ProxyType = Matrix.Net.Proxy.ProxyType.None;
            this.xmppClient.ResolveSrvRecords = true;
            this.xmppClient.Status = "";
            this.xmppClient.StreamManagement = false;
            this.xmppClient.Transport = Matrix.Net.Transport.Socket;
            xmppClient.OnLogin += xmppClient_OnLogin;
            xmppClient.OnAuthError += xmppClient_OnAuthError;
            xmppClient.OnRosterEnd += XmppClient_OnRosterEnd;
            xmppClient.SetXmppDomain("im-lp-369.innomindshyd.com");
        }

        private void XmppClient_OnRosterEnd(object sender, Matrix.EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void xmppClient_OnAuthError(object sender, SaslEventArgs e)
        {
            loginMsg = e.Error.Text;
            loginStatus = true;
        }

        private void xmppClient_OnLogin(object sender, Matrix.EventArgs e)
        {
            loginMsg = "Success";
            loginStatus = true;
        }

        public string Login(string userName, string password)
        {
            xmppClient.SetUsername(userName);
            xmppClient.Password = password;

            xmppClient.Status = "Available";
            xmppClient.Show = Matrix.Xmpp.Show.Chat;
            
            xmppClient.Open();
            var rosterMgr = new RosterManager(xmppClient);

            while(!loginStatus)
            {
                
            }
            return loginMsg;

        }

        private void xmppClient_OnRosterItem(object sender, Matrix.Xmpp.Roster.RosterEventArgs e)
        {
            //DisplayEvent(string.Format("OnRosterItem\t{0}\t{1}", e.RosterItem.Jid, e.RosterItem.Name));

            hub.SendRoster(e.RosterItem.Jid, e.RosterItem.Name);
        }

        
    }
}