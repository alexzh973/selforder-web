using System;
using System.Data;
using System.Collections;
using ensoCom;
using System.Data.SqlClient;

namespace wstcp
{
	/// <summary>
	/// Summary description for eObject.
	/// </summary>
	[Serializable]
	public abstract class eObject 
	{
        internal static Hashtable __editing_objects;

        internal int _id;
        protected string   _name;        
		protected string   _state;
        protected string   _meta_tdb;

	
		protected eObject(string meta_tdb, int ID)
            
		{
            _id = ID;
            _meta_tdb = meta_tdb;            
            _name=_state = "";
		}
		public int ID
		{
			get{return _id;}
            set { if (_id == 0 && value>0) _id = value; }
		}
		public sObject ThisObject
		{
			get{return new sObject(_id, _meta_tdb);}
		}
        public bool IsEmpty
        {
            get { return (_id <= 0); }
        }

		public string Name
		{
			get{return _name;}
            set { _name = (value != null) ? value.Trim().Replace("\"", "") : ""; if (_name.Length > 150)_name=_name.Substring(0, 150); }
		}

        public string Meta_TDB
		{
			get{return _meta_tdb;}
		}
		
        public string State
		{
			get{return _state;}
		}

        public override string ToString()
		{
			return Name;
		}

        public bool LockForModify(IAM current_user)
        {
            return LockForModify(this, current_user);
        }



        public static bool LockForModify(eObject obj, IAM current_user)
        {
            if (__editing_objects == null) __editing_objects = new Hashtable();
            if (obj != null && obj.ID <= 0) return true;
            if ( !__editing_objects.ContainsKey(obj.Meta_TDB + ":" + obj.ID) || cNum.cToInt(__editing_objects[obj.Meta_TDB + ":" + obj.ID]) == current_user.ID  )
            {
                __editing_objects[obj.Meta_TDB + ":" + obj.ID] = current_user.ID;
                return true;
            }
            else
            {
                return false;
            }
        }
       public static void Unlock(sObject obj, IAM current_user)
        {
            if (__editing_objects == null) __editing_objects = new Hashtable();
            if (cNum.cToInt(__editing_objects[obj.Meta_TDB + ":" + obj.ID]) == current_user.ID)
            {
                __editing_objects.Remove(obj.Meta_TDB + ":" + obj.ID);
            }
        }

        public static void Unlock(eObject obj, IAM current_user)
       {
           Unlock(obj.ThisObject, current_user);
        }
       
        public bool Can_I_Modify(IAM current_user)
        {
            return Can_I_Modify(this.ThisObject, current_user);

        }
        
        public static bool Can_I_Modify(sObject obj, IAM current_user)
        {
            return (cNum.cToInt(__editing_objects[obj.Meta_TDB + ":" + obj.ID]) == current_user.ID || !__editing_objects.ContainsKey(obj.Meta_TDB + ":" + obj.ID));

        }

        public static int GetCurrentModifierID(sObject obj)
        {
            return cNum.cToInt(__editing_objects[obj.Meta_TDB + ":" + obj.ID]);
        }

        public static void unlock_by_admin(string meta_tdb, int id)
        {
            if (__editing_objects == null) __editing_objects = new Hashtable();
            __editing_objects.Remove(meta_tdb + ":" + id);
        }




        


	}
}
