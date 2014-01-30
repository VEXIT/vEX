/*
 * Author:			Vex Tatarevic
 * Date Created:	2013-04-23
 * Copyright:       VEX IT Pty Ltd 2013 - www.vexit.com
 * 
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vEX.Entity
{
    public enum StateType { Deleted = 0, Active, Inactive, Pending }
    public enum ActionType { Add = 1, Edit, Delete, Activate, Deactivate }

}
