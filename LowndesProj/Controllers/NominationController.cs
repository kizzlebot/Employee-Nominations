using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.DirectoryServices.AccountManagement;
using System.Threading;
using System.Security.Principal;
using System.Text.RegularExpressions;
using LowndesProj.Models;
using LowndesProj.Services;

namespace LowndesProj.Controllers {
    public class NominationController : Controller {
        //private EmployeeData db = new EmployeeData();
        private InMemoryCache cache = new InMemoryCache();
        private EmployeeDataEntities db = new EmployeeDataEntities();

        // Displays the HTML page
        public ActionResult Index() {
            cache.GetOrSet("employees", () => db.UltiPro);          // Load Employees into Cache


            // Get correct username from windows authenticate
            Thread.GetDomain().SetPrincipalPolicy( PrincipalPolicy.WindowsPrincipal );
            WindowsPrincipal principal = (WindowsPrincipal) User;
            string first_name;
            string last_name;
            string eecudfield;
            using( PrincipalContext pc = new PrincipalContext( ContextType.Domain ) ) {
                UserPrincipal up = UserPrincipal.FindByIdentity( pc, principal.Identity.Name );
                ViewData["UserName"] = up.Name;
                first_name = up.GivenName;
                last_name = up.Surname;
                eecudfield = Regex.Replace( User.Identity.Name, ".*\\\\(.*)", "$1", RegexOptions.None );
            }




            // Get an exact match of Nominator info from the database
            // var currUser = db.UltiPro.AsEnumerable().Select( user => user ).Where( user => user.first_name.Equals( first_name ) && user.last_name.Equals( last_name ) );
            var currUser = db.UltiPro.SingleOrDefault(user => user.eecudfield01.Contains(eecudfield));



            UltiPro us = currUser;
            // If no exact match

            //if( !currUser.Any() ) {
            //    currUser = db.UltiPro.AsEnumerable().Select( user => user ).Where( user => user.first_name.Contains( first_name ) && user.last_name.Contains( last_name ) ); // Get an approximate match
            //    us = (currUser.Any()) ? currUser.ToList<UltiPro>()[0] : null;
            //}
            //else us = currUser.ToList<UltiPro>()[0];


            WindowsIdentity id = WindowsIdentity.GetCurrent();

            ViewData["empID"] = (us != null) ? us.employee_number : "";
            ViewData["WindowsID"] = id.Name;
            ViewData["nominator_last"] = (us != null) ? us.last_name : "";
            ViewData["nominator_first"] = (us != null) ? us.first_name : "";
            ViewData["nominator_middle"] = (us != null) ? us.middle_name : "";
            ViewData["nominator_full_name"] = (us != null) ? ((us.middle_name != null) ? String.Format( "{0} {1} {2}", us.first_name, us.middle_name, us.last_name ) : String.Format( "{0} {1}", us.first_name, us.last_hire_date )) : "";
            return View();
        }
        


        [AcceptVerbs( new string[] { "Post", "Get" } )]
        public ActionResult GetEmployees( string func, string id ) {
            var data = cache.GetOrSet( "employees", () => db.UltiPro );
            
            switch( func ) {
                // GET|POST : /Nominations/GetEmployees/all[?group=<groupname>]
                case "all":
                    //if( Request.Params["group"] != null ) {
                    //    string grp = Request.Params["group"];
                    //    var q =
                    //        data.Where( user => user.org_level_1 == grp )
                    //            .AsEnumerable()
                    //            .Select( user => new {
                    //                employee_number = user.employee_number,
                    //                first_name = user.first_name,
                    //                last_name = user.last_name,
                    //                middle_name = user.middle_name,
                    //                full_name = (user.middle_name != null && user.middle_name != "") ? String.Format( "{0} {1} {2}", user.first_name, user.middle_name, user.last_name ) : String.Format( "{0} {1}", user.first_name, user.last_name ),
                    //                job = user.job,
                    //                department = String.Format( "{0} - {1}", user.org_level_2, user.org_level_3 ),
                    //                supervisor_name_last_suffix_first_mi = user.supervisor_name_last_suffix_first_mi
                    //            } );

                    //    return Json( new { data = q.ToList(), function = func }, JsonRequestBehavior.AllowGet );
                    //}
                    data = from user in db.UltiPro
                        where user.employment_status != "Terminated" && user.org_level_1 == "NON-ATTORNEY STAFF"
                        select new {
                            employee_number = user.employee_number,
                            first_name = user.first_name,
                            last_name = user.last_name,
                            middle_name = user.middle_name,
                            full_name =
                                (user.middle_name != null && user.middle_name != "")
                                    ? String.Format("{0} {1} {2}", user.first_name, user.middle_name, user.last_name)
                                    : String.Format("{0} {1}", user.first_name, user.last_name),
                            job = user.job,
                            department = String.Format("{0} - {1}", user.org_level_2, user.org_level_3),
                            supervisor_name_last_suffix_first_mi = user.supervisor_name_last_suffix_first_mi
                        };
                    return Json( new { data = data.ToList(), function = func }, JsonRequestBehavior.AllowGet );

                // GET|POST : /Nominations/GetEmployees/teams
                case "teams":
                    var query = "Select U.org_level_2 " +
                                "from UltiPro "+
                                "group by U.org_level_2 " +
                                "order by TotalCount desc ";

                    string connectionString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["EmployeeDataEntities"].ConnectionString;
                    List<string> teams = new List<string>();
                    using( SqlConnection connection = new SqlConnection( connectionString ) ) {
                        SqlCommand command = new SqlCommand( query, connection );
                        connection.Open();

                        SqlDataReader reader = command.ExecuteReader();
                        try {
                            while( reader.Read() ) {
                                teams.Add((string) reader["org_level_2"]);
                            }
                        }
                        finally {
                            reader.Close();
                            connection.Close();
                        }
                    }

                                
                    return Json( new { data = teams.ToList(), function = func }, JsonRequestBehavior.AllowGet );

                // GET|POST : /Nominations/GetEmployees/names
                case "names":
                    var data = db.UltiPro.Select( user => new {
                        first_name = user.first_name,
                        last_name = user.last_name
                    } );
                    return Json( new { data = data.ToList(), function = func }, JsonRequestBehavior.AllowGet );
                // GET|POST : /Nominations/GetEmployees/byid
                case "byid":
                    IEnumerable<UltiPro> rtn = (id != null) ? db.UltiPro.Select( user => user ).Where( user => user.employee_number == id ) : db.UltiPro.AsEnumerable();
                    return Json( new { data = rtn, function = func }, JsonRequestBehavior.AllowGet );
                case "test":
                    return Json( new { data = db.UltiPro.Select( user => user ), function = func, param = Request.Params["name"]}, JsonRequestBehavior.AllowGet );
                default:
                   return null;
            }
        }
        

        [HttpPost]
        public async Task<JsonResult> Create( [Bind( Include = "Id,Nomination_Type,Nominator_Employee_Number,Nominator_Employee_Full_Name,Nomination_Reason,Nominee_Employee_Number,Nominee_Emp_or_Team_Name,Submission_Date" )] EOM_Nomination eOM_Nominations ) {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            eOM_Nominations.Submission_Date = DateTime.Now;

            if( ModelState.IsValid && EntryInModel( eOM_Nominations ) ) {
                db.EOM_Nomination.Add( eOM_Nominations );
                await db.SaveChangesAsync();
                
                // Send Email to nominator and Josh
                handleNotification( eOM_Nominations );

                // Send success message
                dict.Add( "Message", "Success" );
                return Json( dict, JsonRequestBehavior.AllowGet );
            }
            dict.Add( "Message", "Failure" );
            return Json( dict, JsonRequestBehavior.AllowGet );
        }

        [HttpPost]
        public async Task<JsonResult> CreateTeam( EOM_Nomination[] eOM_Nominations ) {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            List<string> nominees = new List<string>();
            List<string> groups = new List<string>();


            string statMsg = String.Format( "Nominated By: {0}\nNomination Reason: {1}\n\n\n", eOM_Nominations[0].Nominator_Employee_Full_Name, eOM_Nominations[0].Nomination_Reason );

            if( ModelState.IsValid ) {
                foreach( EOM_Nomination entry in eOM_Nominations ) {
                    if( EntryInModel( entry ) ) {
                        entry.Submission_Date = DateTime.Now;
                        db.EOM_Nomination.Add( entry );
                        await db.SaveChangesAsync();

                        // If group name
                        if( entry.Nominee_Employee_Number == null ) statMsg += String.Format( "Group: {0}\n", entry.Nominee_Emp_or_Team_Name );
                        // else nominee
                        else statMsg += String.Format( "Nominee ID: {0}\nNominee Name: {1}\n\n", entry.Nominator_Employee_Number, entry.Nominee_Emp_or_Team_Name );
                    }
                }
                dict.Add( "Message", "Success" );


                var nominator = db.UltiPro.AsEnumerable().Select( user => user ).Where( user => user.employee_number == eOM_Nominations[0].Nominator_Employee_Number );
                UltiPro u = nominator.ToList()[0];
                handleNotification( eOM_Nominations[0], statMsg );

                return Json( dict, JsonRequestBehavior.AllowGet );
            }
            dict.Add( "Message", "Failure" );
            return Json( dict, JsonRequestBehavior.AllowGet );
        }


        private void handleNotification( EOM_Nomination eom, string statMsg = null ) {
            // Get nominator 
            var nominator = db.UltiPro.AsEnumerable().Select( user => user ).Where( user => user.employee_number == eom.Nominator_Employee_Number );
            UltiPro u = nominator.ToList()[0];

            
            Mail m = new Mail();
            string nominationType;

            // nominator email address
            string nominatorEmail = String.Format( "{0}.{1}@lowndes-law.com", u.first_name, u.last_name );
            string eeucid = String.Format( "{0}@lowndes-law.com", u.eecudfield01 );
            

            // Email for single nomination
            if( statMsg == null ) {
                // Send email to nomination submitter
                string messageToNominator = String.Format( "Thank you for your nomination of {0}. This email is just a confirmation that your nomination was recorded successfully", eom.Nominee_Emp_or_Team_Name );
                m.SendMessage( nominatorEmail, eeucid, "Thank you for your Applause, Applause Nomination", messageToNominator );

                // Build msg for bccRecipients
                statMsg = String.Format( "Nominated By: {0}\nNomination Reason: {1}\nNominee ID: {2}\nNominee Name: {3}\n", eom.Nominator_Employee_Full_Name, eom.Nomination_Reason, eom.Nominator_Employee_Number, eom.Nominee_Emp_or_Team_Name );
                nominationType = "New Applause, Applause Nomination";
            }
            // Email for group nomination
            else {
                string messageToNominator = String.Format( "Thank you for your teamwork award nomination. This email is a confirmation that your nomination was recorded successfully" );
                m.SendMessage( nominatorEmail, eeucid, "Thank you for your Teamwork Award Nomination", messageToNominator );

                nominationType = "New Teamwork Award Nomination";
            }

            string[] bccRecipients = new string[] { "carolyn.iannone@lowndes-law.com","vivian.happel@lowndes-law.com" };
            string mainRecipient = "carol.meyer@lowndes-law.com";
            m.SendMessage( mainRecipient, bccRecipients, eeucid, nominationType, statMsg );
        }



        public bool EntryInModel( EOM_Nomination nominee ) {
            string empID = nominee.Nominee_Employee_Number;
            string teamName = nominee.Nominee_Emp_or_Team_Name;
            // If empID supplied, assume that submission is a user
            if( empID != null && teamName != null ) {
                var usr = db.UltiPro.AsEnumerable().Select( user => user ).Where( user => user.employee_number == empID );
                return (usr.Count() == 1) ? true : false;
            }
            // If empID is null and teamName supplied, assume submission is a team
            else if( empID == null && teamName != null ) {
                var teamm = db.UltiPro.AsEnumerable().Select( team => team ).Where( user => user.org_level_1 == teamName || user.org_level_2 == teamName || user.org_level_3 == teamName );
                return (teamm.Any()) ? true : false;
            }
            else return false;
        }
    }
}