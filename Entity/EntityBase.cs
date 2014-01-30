/*
 * Author:			Vex Tatarevic
 * Date Created:	2013-04-23
 * Copyright:       VEX IT Pty Ltd 2013 - www.vexit.com
 * 
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace vEX.Entity
{
    [DataContract]
    public abstract class EntityBase
    {
        /// <summary>
        ///  Keeps track of entity state changes on client side
        ///  Used effectively with vEX.Entity Save stored procedures and javascript vEX.Grid to create easy table administration interface
        /// </summary>
        [DataMember]
        public int ClientAction { get; set; }

        [IgnoreDataMemberAttribute]
        public bool IsClientAdded { get { return ClientAction == (int)ActionType.Add; } }
        [IgnoreDataMemberAttribute]
        public bool IsClientEdited { get { return ClientAction == (int)ActionType.Edit; } }
        [IgnoreDataMemberAttribute]
        public bool IsClientDeleted { get { return ClientAction == (int)ActionType.Delete; } }
        [IgnoreDataMemberAttribute]
        public bool IsClientActivated { get { return ClientAction == (int)ActionType.Activate; } }
        [IgnoreDataMemberAttribute]
        public bool IsClientDeactivated { get { return ClientAction == (int)ActionType.Deactivate; } }

        public void SetClientAction(int action) { ClientAction = action; }
        public void SetClientAdded() { ClientAction = (int)ActionType.Add; }
        public void SetClientEdited() { ClientAction = (int)ActionType.Edit; }
        public void SetClientDeleted() { ClientAction = (int)ActionType.Delete; }
        public void SetActive() { ClientAction = (int)ActionType.Activate; }
        public void SetInactive() { ClientAction = (int)ActionType.Deactivate; }
    }
}
