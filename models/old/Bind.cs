using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using ensoCom;

namespace wstcp
{
	/// <summary>
	/// Summary description for Bind.
	/// </summary>
	/// 
 
	[Serializable]
	public abstract class Bind
	{
        public static string SqlExistConditionByMasterObject(string linkedFieldTitle, sObject MasterObject, string metatdb_childs)
        {
            return linkedFieldTitle + " in (select TrgID from BIND where Source='" + MasterObject.Meta_TDB + "' and SrcId=" + MasterObject.ID + " and Target='" + metatdb_childs + "')";
        }
        
        public static bool CheckExist(sObject first, sObject second)
        {
            string str = "select Source, SrcID as SourceID, Target, TrgID as TargetID, BindDate, Corrector from BIND where (Source='" + first.Meta_TDB + "' and SrcID=" + first.ID + " and Target='" + second.Meta_TDB + "' and TrgID=" + second.ID + ") or (Source='" + second.Meta_TDB + "' and SrcID=" + second.ID + " and Target='" + first.Meta_TDB + "' and TrgID=" + first.ID + ")";
            return (db.GetDbTable(str).Rows.Count>0);
        }
		private static DataTable getTable(string filter)
		{
			string str = "select Source, SrcID as SourceID, Target, TrgID as TargetID, BindDate, Corrector from BIND "+((filter!="")?" where "+filter:"");
			return db.GetDbTable(str);
		}
		
		public static DataTable GetDataTableByMaster(sObject master,string eTypeChilds, IAM current_user) 
		{
			string str = "select Source, SrcID as SourceID, Target, TrgID as TargetID, BindDate, Corrector from BIND where Source='"+master.Meta_TDB+"' and SrcID="+master.ID+((eTypeChilds!="")?" and Target='"+eTypeChilds+"'":"")+" order by Target";
			return db.GetDbTable(str);
		}
        public static List<sObject> GetArray_ByMaster(sObject master, string eTypeChilds, IAM current_user)
        {
            DataTable dt = GetDataTableByMaster(master, eTypeChilds, current_user);
			List<sObject> s = new List<sObject>();
			if(dt.Rows.Count>0)
			{
				//int i=0;
				foreach(DataRow r in dt.Rows)
				{
					s.Add(new sObject(Convert.ToInt32(r["TargetID"]),""+r["Target"]));					
				}
			}
			return s;
        }
		public static sObject[] GetArrayByMaster(sObject master, string eTypeChilds, IAM current_user) 
		{
            //string str = "select Source, SrcID as SourceID, Target, TrgID as TargetID, BindDate, Corrector from BIND where Source='"+master.Meta_TDB+"' and SrcID="+master.ID+((eTypeChilds!="")?" and Target='"+eTypeChilds+"'":"")+" order by Target";
            //DataTable dt = db.GetDbTable(str);
            DataTable dt = GetDataTableByMaster(master, eTypeChilds, current_user);
			sObject[] s = new sObject[dt.Rows.Count];
			if(s.Length>0)
			{
				int i=0;
				foreach(DataRow r in dt.Rows)
				{
					s[i++] = new sObject(Convert.ToInt32(r["TargetID"]),""+r["Target"]);					
				}
			}
			return s;
		}

        public static sObject[] GetArrayByMasterTDB(string master_TDB, string eTypeChilds, IAM current_user)
        {
            DataTable dt = GetTableByMasterTDB(master_TDB, eTypeChilds, current_user);
            sObject[] s = new sObject[dt.Rows.Count];
            if (s.Length > 0)
            {
                int i = 0;
                foreach (DataRow r in dt.Rows)
                {
                    s[i++] = new sObject(Convert.ToInt32(r["TargetID"]), "" + r["Target"]);
                }
            }
            return s;
        }
       public static DataTable GetTableByMasterTDB(string master_TDB, string eTypeChilds, IAM current_user)
        {
            string str = "select  destinct Target, TrgID as TargetID from BIND where Source='" + master_TDB + "' " +  ((eTypeChilds != "") ? " and Target='" + eTypeChilds + "'" : "") + " order by Target";
            return db.GetDbTable(str);
        }

		
		public static DataTable GetDataTableByChild(string eTypeMasters,sObject child, IAM current_user) 
		{
			string str = "select Source, SrcID as SourceID, Target, TrgID as TargetID, BindDate, Corrector from BIND where Target='"+child.Meta_TDB+"' and TrgID="+child.ID+((eTypeMasters!="")?" and Source='"+eTypeMasters+"'":"")+" order by Source";
			return db.GetDbTable(str);
		}
		public static sObject[] GetArrayByChild(string eTypeMasters, sObject child, IAM current_user) 
		{
            //string str = "select Source, SrcID as SourceID, Target, TrgID as TargetID, BindDate, Corrector from BIND where Target='"+child.Meta_TDB+"' and TrgID="+child.ID+((eTypeMasters!="")?" and Source='"+eTypeMasters+"'":"")+" order by Source";
            //DataTable dt = db.GetDbTable(str);
            DataTable dt = GetDataTableByChild(eTypeMasters, child, current_user);
			sObject[] s = new sObject[dt.Rows.Count];
			if(s.Length>0)
			{
				int i=0;
				foreach(DataRow r in dt.Rows)
				{
					s[i++] = new sObject(Convert.ToInt32(r["SourceID"]),""+r["Source"]);					
				}
			}
			return s;			
		}


		public static void Save(sObject master, sObject child, IAM current_user)
		{
            if (!master.IsEmpty && !child.IsEmpty && current_user.ID>0) 
			{
                SqlConnection cn = new SqlConnection(db.DefaultCnString);
				cn.Open();
				SqlCommand cmd = cn.CreateCommand();
				try 
				{
					cmd.CommandText = "select count(*) from BIND where Source='"+master.Meta_TDB+"' and SrcID="+master.ID+" and Target='"+child.Meta_TDB+"' and TrgID="+child.ID;
					if (cmd.ExecuteScalar().ToString()=="0")
					{
						cmd.CommandText = "insert into BIND (Source,SrcID,Target,TrgID,Corrector)"+
                            " values ('" + master.Meta_TDB + "'," + master.ID + ",'" + child.Meta_TDB + "'," + child.ID + ",'" + current_user.Name+ "')";
						cmd.ExecuteNonQuery();
					}
				}
				catch(Exception e1)
				{
				}
				finally 
				{
					cn.Close();
				}
			}	
		}	
		
		
		public static void DeleteBySource(sObject master, string eTypeChilds, IAM current_user)
		{
            if (!master.IsEmpty && current_user.ID > 0) 
			{
                SqlConnection cn = new SqlConnection(db.DefaultCnString);
				cn.Open();
				SqlCommand cmd = cn.CreateCommand();
				try 
				{
					cmd.CommandText = "delete from BIND where Source='"+master.Meta_TDB+"' and SrcID="+master.ID+((eTypeChilds!="")?" and Target='"+eTypeChilds:"")+"'";
					cmd.ExecuteNonQuery();
				}
				catch(Exception e1)
				{
				}
				finally 
				{
					cn.Close();
				}
			}	
		}	
		public static void DeleteByTarget (string eTypeMasters, sObject child, IAM current_user)
		{
            if (!child.IsEmpty && current_user.ID > 0) 
			{
                SqlConnection cn = new SqlConnection(db.DefaultCnString);
				cn.Open();
				SqlCommand cmd = cn.CreateCommand();
				try 
				{
					cmd.CommandText = "delete from BIND where Source='"+eTypeMasters+"' and Target='"+child.Meta_TDB+"' and TrgID="+child.ID+" ";
					cmd.ExecuteNonQuery();
				}
				catch(Exception e1)
				{
				}
				finally 
				{
					cn.Close();
				}
			}	
		}	
		public static void DeleteByTarget (sObject child, IAM current_user)
		{
            if (!child.IsEmpty && current_user.ID > 0) 
			{
                SqlConnection cn = new SqlConnection(db.DefaultCnString);
				cn.Open();
				SqlCommand cmd = cn.CreateCommand();
				try 
				{
					cmd.CommandText = "delete from BIND where Target='"+child.Meta_TDB+"' and TrgID="+child.ID+" ";
					cmd.ExecuteNonQuery();
				}
				catch(Exception e1)
				{
				}
				finally 
				{
					cn.Close();
				}
			}	
		}	
		public static void Delete(sObject master,sObject child, IAM current_user)
		{
            if (!master.IsEmpty && !child.IsEmpty && current_user.ID > 0) 
			{
                SqlConnection cn = new SqlConnection(db.DefaultCnString);
				cn.Open();
				SqlCommand cmd = cn.CreateCommand();
				cmd.Connection = cn;
				try 
				{
					cmd.CommandText = "delete from BIND where Source='"+master.Meta_TDB+"' and SrcID="+master.ID+" and Target='"+child.Meta_TDB+"' and TrgID="+child.ID;
					cmd.ExecuteNonQuery();
				}
				catch(Exception e1)
				{
				}
				finally 
				{
					cn.Close();
				}
			}	
		}	



	}
}
