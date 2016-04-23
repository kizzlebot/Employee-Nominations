using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc; 

namespace LowndesProj.Services {
    public class Mail : IDisposable {
        private SmtpClient smtp ; 
        public Mail() {
            smtp = new SmtpClient( "orlex13" );
        }

        public void SendMessage(string recipient, string fromEmail, string subject, string body){
            MailMessage mailMsg = new MailMessage();
            mailMsg.To.Add( recipient );
            mailMsg.From = new MailAddress( fromEmail );
            mailMsg.Subject = subject;
            mailMsg.Body = body;
            smtp.Send( mailMsg );
        }

        public async Task SendMessage( string toRecipient, string[] bccRecipients, string fromEmail, string subject, string body ) {
            MailMessage mailMsg = new MailMessage();
            mailMsg.To.Add( toRecipient );
            foreach (string r in bccRecipients) mailMsg.Bcc.Add( r );
            mailMsg.From = new MailAddress( fromEmail );
            mailMsg.Subject = subject;
            mailMsg.Body = body;
            smtp.Send( mailMsg );
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
}