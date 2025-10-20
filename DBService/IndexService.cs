using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace DBService
{
    public class IndexService
    {
        private readonly string _connStr;
        private readonly ErrorLog _errorLog = new ErrorLog();

        public IndexService()
        {
            _connStr = ConfigurationManager.ConnectionStrings["EPMS"].ConnectionString;
        }


        public DataTable GetRoles(int empId)
        {
            DataTable dt = new DataTable();

            try
            {
                using (var conn = new OracleConnection(_connStr))
                using (var cmd = new OracleCommand("USP_GET_ROLE_MASTER", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Input parameters (default to NULL)
                    cmd.Parameters.Add("p_RoleID", OracleDbType.Int32).Value = DBNull.Value;
                    cmd.Parameters.Add("P_RoleTypeID", OracleDbType.Int32).Value = DBNull.Value;
                    cmd.Parameters.Add("P_RoleStatus", OracleDbType.Varchar2).Value = DBNull.Value;
                    cmd.Parameters.Add("P_EmpID", OracleDbType.Int32).Value = empId;
                    cmd.Parameters.Add("P_ShopID", OracleDbType.Int32).Value = DBNull.Value;
                    cmd.Parameters.Add("P_SectionID", OracleDbType.Int32).Value = DBNull.Value;
                    cmd.Parameters.Add("P_FunctionID", OracleDbType.Int32).Value = DBNull.Value;
                    cmd.Parameters.Add("P_Status", OracleDbType.Varchar2, 50).Value = DBNull.Value;
                    cmd.Parameters.Add("P_Search", OracleDbType.Varchar2, 100).Value = DBNull.Value;

                    // Output cursor
                    cmd.Parameters.Add("p_out_cur", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    var adapter = new OracleDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                _errorLog.LogError(ex);
                //throw new Exception("Error fetching roles: " + ex.Message);
            }

            return dt;
        }
    }
}
