using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Web.Configuration;

namespace generic.UserClasses
{
  public class genericLogin : BasePage
    {
        genericUtility utl = new genericUtility();
        SqlCommand cmd = new SqlCommand();
        SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConnString"].ConnectionString);
        string LoginPage = "/login/index.aspx";
        string NotAuthorizedPage = "/login/notauthorized.aspx";

        ////----------------
        // essentially just a c# version of the function from the classic asp.
        ////----------------
        public void SetCookieFromSession()
        {
            HttpContext.Current.Response.Cookies["generic"]["UserID"] = Session["UserID"].ToString();
            HttpContext.Current.Response.Cookies["generic"]["Lastname"] = utl.HtmlEncodeThis(Session["Lastname"].ToString());
            HttpContext.Current.Response.Cookies["generic"]["Firstname"] = utl.HtmlEncodeThis(Session["Firstname"].ToString());
            HttpContext.Current.Response.Cookies["generic"]["Email"] = utl.HtmlEncodeThis(Session["Email"].ToString());
            //HttpContext.Current.Response.Cookies["Access"].Value = Session["Access"].ToString(); // not bothering to put the Session["Access"] into a cookie because the cookie spec says no whitespace...
            // expire the cookie after 60 minutes.
            HttpContext.Current.Response.Cookies["generic"].Expires = DateTime.Now.AddMinutes(93);
        }

        ////----------------
        // essentially just a c# version of the function from the classic asp.
        ////----------------
        public void SetSessionFromCookie()
        {
            string SessionAccess = "";
            if ((Session["UserID"] == null) && (HttpContext.Current.Request.Cookies["generic"] != null))
                {
                //check if the userid exists in the cookie...
                if ((HttpContext.Current.Request.Cookies["generic"]["UserID"] != null) && (HttpContext.Current.Request.Cookies["generic"]["UserID"].ToString() != ""))
                {
                    using (conn)
                    {
                        using (cmd)
                        {
                            //get the "permissions" of the user
                            cmd.Connection = conn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "spGetUserPermissions";
                            cmd.Parameters.AddWithValue("@RecordID", Convert.ToInt16(HttpContext.Current.Request.Cookies["generic"]["UserID"].ToString()));
                            cmd.Connection.Open();

                            using (SqlDataReader dr = cmd.ExecuteReader())
                            {
                                //because there is a userid, at this point they are at least a member
                                SessionAccess = "Member";
                                //now loop through all the permissions and add them to the SessionAccess variable
                                while (dr.Read())
                                {
                                    SessionAccess += "," + dr["contribution"].ToString();
                                }
                            }
                        }
                    }
                    //dump all the cookie variables and SessionAccess into the .net session
                    Session["UserID"] = HttpContext.Current.Request.Cookies["generic"]["UserID"];
                    string emailStr = HttpContext.Current.Request.Cookies["generic"]["Email"].ToString(); // convert it to a string first. then decode it with UrlDecode, otherwise issue with @ and . will occur.
                    string lastnameStr = HttpContext.Current.Request.Cookies["generic"]["Lastname"].ToString();
                    string firstnameStr = HttpContext.Current.Request.Cookies["generic"]["Firstname"].ToString();
                    Session["Lastname"] = HttpContext.Current.Server.UrlDecode(lastnameStr);
                    Session["Firstname"] = HttpContext.Current.Server.UrlDecode(firstnameStr);
                    Session["Email"] = HttpContext.Current.Server.UrlDecode(emailStr);
                    Session["Access"] = SessionAccess;
                    //HttpContext.Current.Response.Cookies["generic"].Expires = DateTime.Now.AddMinutes(65);
                    Session["Error"] = "SetSessionFromCookie, was hit";
                }
            }
        }

      }
}
