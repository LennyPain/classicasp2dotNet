This is a sort of brute force method to get classic asp and .Net users logged
into each of the other technology. Not the most elegant solution but it works.

The problem we had was trying to stop development of the old classic asp to newer .Net. Unfortunately .Net and classic ASP do not share a session, but can share a cookie.
Basically this whole process involves setting a cookie and putting those values into the session object of whichever technology they are moving into... If you are in classic ASP and the next page goes to an .aspx page, or vice versa, the user should not see another login screen. 
