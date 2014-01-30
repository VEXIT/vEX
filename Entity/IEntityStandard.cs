/*
 * Author:			Vex Tatarevic
 * Date Created:	2013-04-23
 * Copyright:       VEX IT Pty Ltd 2013 - www.vexit.com
 * 
 */

using System;

namespace vEX.Entity
{
    /// <summary>
    ///  Defines a standard entity properties structure that most tables in database should implement
    ///  This class can be implemented in Domain DataAccess class in order to define common operations for all entities
    ///  such as Get(long id) Add, Update, Delete .
    ///  Common  Data Access class that makes use of this  is further implemented in VEXIT ModMVC Web framework
    /// </summary>
    public interface IEntityStandard
    {
        int ID { get; set; }
        long CreatorID { get; set; }
        long UpdaterID { get; set; }
        DateTime DateCreated { get; set; }
        DateTime DateUpdated { get; set; }
        int State { get; set; } // --- vEX.Entity.StateType //........Deleted = 0, Active, Inactive, Pending
    }

    public interface IEntityStandardLong
    {
        long ID { get; set; }
        long CreatorID { get; set; }
        long UpdaterID { get; set; }
        DateTime DateCreated { get; set; }
        DateTime DateUpdated { get; set; }
        int State { get; set; } // --- vEX.Entity.StateType //........Deleted = 0, Active, Inactive, Pending
    }
}
