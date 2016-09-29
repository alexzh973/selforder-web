using System;


	/// <summary>
	/// Summary description for sObject.
	/// </summary>
    /// 
    [Serializable]
	public struct sObject
	{
        private int _id;
        private string _meta_tdb;

        
        public sObject(int id, string meta_tdb)
        {
            _id = id;
            _meta_tdb = meta_tdb;
        }

		public int ID
		{
			get{return _id;}
		}
		public string Meta_TDB
		{
			get{return _meta_tdb;}
		}
		public bool IsEmpty
		{
			get{return (_id<=0 || _meta_tdb=="");}
		}

	}

