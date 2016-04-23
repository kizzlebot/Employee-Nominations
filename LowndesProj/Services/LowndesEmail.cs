using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Threading.Tasks;

namespace LowndesProj.Services {
    public class LowndesEmail {
        public SmtpClient smtp = new SmtpClient( "orlex13" );
        public MailMessage msg = new MailMessage();

        public LowndesEmail() {
            this.msg.From = new MailAddress( "nominations@lowndes-law.com" );
        }

        public string to { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public List<string> bcc = new List<string>();
        public bool isHTML = false;
        
        public MailMessage getMailMessage(){
            this.msg.To.Add(new MailAddress(this.to));
            foreach( string r in bcc ) this.msg.Bcc.Add( new MailAddress( r ) );     // Add all bcc recipients
            this.msg.Subject = this.subject;
            this.msg.IsBodyHtml = isHTML;
            return this.msg; 
        }
        public async Task<bool> SendEmail( bool bodyIsHTML = false ) {
            if( this.to == "" || this.body == "" || this.subject == "" ) return false;

            this.msg.To.Add( new MailAddress( this.to ) );                           // Add primary "to" address
            foreach( string r in bcc ) this.msg.Bcc.Add( new MailAddress( r ) );     // Add all bcc recipients


            this.msg.Body = this.body;                                               // Add Message Body
            this.msg.Subject = this.subject;                                         // Add Subject
            this.msg.IsBodyHtml = bodyIsHTML;                                        // Content can be HTML

            this.smtp.Send( this.msg ); // Send the email

            clear();                                                                 // Reset this class's field variables
            return true;
        }

        //public static Task SendAsync( this SmtpClient client, MailMessage message ) {
        //    TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
        //    Guid sendGuid = Guid.NewGuid();

        //    SendCompletedEventHandler handler = null;
        //    handler = ( o, ea ) => {
        //        if( ea.UserState is Guid && ((Guid) ea.UserState) == sendGuid ) {
        //            client.SendCompleted -= handler;
        //            if( ea.Cancelled ) {
        //                tcs.SetCanceled();
        //            }
        //            else if( ea.Error != null ) {
        //                tcs.SetException( ea.Error );
        //            }
        //            else {
        //                tcs.SetResult( null );
        //            }
        //        }
        //    };

        //    client.SendCompleted += handler;
        //    client.SendAsync( message, sendGuid );
        //    return tcs.Task;
        //}

        public bool clear() {
            this.to = this.subject = this.body = null;
            this.bcc = new List<string>();
            this.isHTML = false; 
            return true;
        }
        protected virtual void Dispose( bool disposing ) {
            if( disposing ) {
                // dispose managed resources
                smtp.Dispose();
            }
            // free native resources
        }

        public void Dispose() {
            Dispose( true );
            GC.SuppressFinalize( this );
        }
    }



    public class NominationEmail:LowndesEmail {

        private string nominator ;
        private string[] nominated ;
        private string nominee ; 
        private string notifyAdmin;
        public NominationEmail(string nominator, string[] nominated, string notifyAdmin = "james.choi@lowndes-law.com"): base(){
            this.nominator = nominator;
            this.nominated = nominated;
            this.notifyAdmin = notifyAdmin;
        }
        public NominationEmail(string nominator, string nominee, string notifyAdmin = "james.choi@lowndes-law.com"): base(){
            this.nominator = nominator;
            this.nominated = new string[1] {nominee};
            this.notifyAdmin = notifyAdmin;
        }


        //d9edf7
        //f2dede
        //dff0d8
        private string GenerateTblHTML(string tblName, string size, string color, string[] nominated){
            string rtn = String.Format("<div class='col-xs-{0}' style='-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;position: relative;min-height: 1px;padding-right: 15px;padding-left: 15px;float: left;width: 25%;'><div class='list-group' style='-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;padding-left: 0;margin-bottom: 20px;'><div class='list-group-item list-group-item-info' style='-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;position: relative;display: block;padding: 10px 15px;margin-bottom: -1px;background-color: {1};border: 1px solid #ddd;color: #31708f;border-top-left-radius: 4px;border-top-right-radius: 4px;'><h4 class='list-group-item-heading' style='-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;font-family: inherit;font-weight: 500;line-height: 1.1;color: inherit;margin-top: 0;margin-bottom: 5px;font-size: 18px;'>{2}</h4></div>", size, color, tblName);
            foreach(string nom in nominated) rtn+= String.Format("<div class='list-group-item' style='-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;position: relative;display: block;padding: 10px 15px;margin-bottom: -1px;background-color: #fff;border: 1px solid #ddd;'>{0}</div>",nom);
            return rtn += "</div></div>";
            
        }
        private string WrapContainerHTML(string txt){
            return String.Format("<div class='container' style='-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;padding-right: 15px;padding-left: 15px;margin-right: auto;margin-left: auto;'><div class='row' style='-webkit-box-sizing: border-box;-moz-box-sizing: border-box;box-sizing: border-box;margin-right: -15px;margin-left: -15px;'>{0}</div></div>", txt);
        }
        

        public void SendNotification(){
            // Send to nominator a thank you message
            this.to = this.nominator;
            this.body = "Thank you for your nomination.  Your nomination(s) have been confirmed.";
            this.subject = "Nomination Confirmed";
            this.isHTML = false;
            this.SendEmail(false);
            
            this.clear();

            // Send to Josh the notification
            this.to = this.notifyAdmin;
            this.body = WrapContainerHTML(GenerateTblHTML("Nominated", "6", "#d9edf7", nominated)); 
            this.subject = "EOM Nomination";
            this.isHTML = true;
            this.SendEmail( true );

            this.clear();
        }

    }


}