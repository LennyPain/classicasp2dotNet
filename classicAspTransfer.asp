<%
'called to set the cookie for classic asp
sub SetCookieFromSession()
    response.Cookies("generic").Expires = DateAdd("n", 59, Now())
    response.Cookies("generic")("UserID") = Session("UserID")
    response.Cookies("generic")("Lastname") = htmlencodeThis(Session("Lastname"))
    response.Cookies("generic")("Firstname") = htmlencodeThis(Session("Firstname"))
    response.Cookies("generic")("Email") = htmlencodeThis(Session("Email"))
    response.Cookies("generic")("ChkEm") = ""
	' not adding Session("Access") cookie spec does not allow spaces.
	'use Setsession from cookie to fill in the Session("Access")
end sub

function SetSessionFromCookie()
    SetSessionFromCookie = 0
    'check if the userid exists in the cookie...
    if (not IsNull(request.Cookies("generic")("UserID"))) or (request.Cookies("generic")("UserID") <> "") then
        'get the "permissions" of the user
        cmd.CommandText = "spGetUserPermissions"
	    cmd.CommandType = 4
	    cmd.Prepared = true
	    cmd.NamedParameters = True

	    cmd.Parameters("@RecordID") = request.Cookies("generic")("UserID")

        Set rsAccess = Server.CreateObject("ADODB.Recordset")
        rsAccess.CursorLocation = 3
        rsAccess.CursorType = 3
        rsAccess.LockType = 3
        rsAccess.Open cmd

        'because there is a userid, at this point they are at least a member
	    SessionAccess = "Member"
        'now loop through all the permissions and add them to the SessionAccess variable
	    do while not rsAccess.eof
	        SessionAccess = SessionAccess & "," & rsAccess("userPermissions")
            rsAccess.MoveNext
        loop
        'dump all the cookie variables into the classic asp session
        Session("UserID") = request.Cookies("generic")("UserID")
        Session("Lastname") = request.Cookies("generic")("Lastname")
        Session("Firstname") = request.Cookies("generic")("Firstname")
	    Session("Email") = request.Cookies("generic")("Email")
	    Session("Access") = SessionAccess
	    SetSessionFromCookie = 1
	end if
end function


%>
