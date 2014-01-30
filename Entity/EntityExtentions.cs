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
    public static class EntityExtentions
    {
        //public static bool IsDeleted<T>(this T e) where T : IEntityStandard { return e.State == (int)StateType.Deleted; }
        //public static bool IsActive<T>(this T e) where T : IEntityStandard { return e.State == (int)StateType.Active; }
        //public static bool IsInactive<T>(this T e) where T : IEntityStandard { return e.State == (int)StateType.Inactive; }
        //public static void SetDeleted<T>(this T e) where T : IEntityStandard { e.State = (int)StateType.Deleted; }
        //public static void SetActive<T>(this T e) where T : IEntityStandard { e.State = (int)StateType.Active; }
        //public static void SetInactive<T>(this T e) where T : IEntityStandard { e.State = (int)StateType.Inactive; }

        public static bool IsDeleted<T>(this T e) where T : IEntityStandardLong { return e.State == (int)StateType.Deleted; }
        public static bool IsActive<T>(this T e) where T : IEntityStandardLong { return e.State == (int)StateType.Active; }
        public static bool IsInactive<T>(this T e) where T : IEntityStandardLong { return e.State == (int)StateType.Inactive; }
        public static bool IsPending<T>(this T e) where T : IEntityStandardLong { return e.State == (int)StateType.Pending; }
        public static void SetDeleted<T>(this T e) where T : IEntityStandardLong { e.State = (int)StateType.Deleted; }
        public static void SetActive<T>(this T e) where T : IEntityStandardLong { e.State = (int)StateType.Active; }
        public static void SetInactive<T>(this T e) where T : IEntityStandardLong { e.State = (int)StateType.Inactive; }
        public static void SetPending<T>(this T e) where T : IEntityStandardLong { e.State = (int)StateType.Pending; }

    }
}
