
namespace wstcp
{
    public class NSImeta
    {
        public const string 
            TDB_GOODIES = "GOOD",
            TDB_PARAMETERS = "PARAMETER",
            TDB_GOODPARAMETERS = "GOODPARAMETER",
            TDB_OWNERS = "OWN",
            TDB_OWNERSGOODIES = "OWNG",
            TDB_WAREHOUSES = "WH";

        private static string _cnstring = "";
        public static bool CheckDb
        {
            get
            {
                _cnstring = System.Configuration.ConfigurationManager.AppSettings["defaultcn"];                    
                return ensoCom.db.InitDbConnection("default", _cnstring);;
            }
        }
    }
}