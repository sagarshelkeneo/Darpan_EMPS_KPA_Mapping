using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web.Security;

namespace DBService
{
    public class ViewService
    {
        private readonly string _connStr;

        public ViewService()
        {
            _connStr = ConfigurationManager.ConnectionStrings["EPMS"].ConnectionString;
        }

        public DataTable GetKpaMaster(int roleId, int empId)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("KpaID");
            dt.Columns.Add("Category");
            dt.Columns.Add("KPAName");
            dt.Columns.Add("UnitOfKPA");
            dt.Columns.Add("DataSource");

            using (OracleConnection con = new OracleConnection(_connStr))
            using (OracleCommand cmd = new OracleCommand("USP_GET_KPA_MASTER", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("p_KpaID", OracleDbType.Int32).Value = DBNull.Value;
                cmd.Parameters.Add("P_KpaCategoryID", OracleDbType.Int32).Value = DBNull.Value;
                cmd.Parameters.Add("P_DataSourceID", OracleDbType.Int32).Value = DBNull.Value;
                cmd.Parameters.Add("p_RoleID ", OracleDbType.Int32).Value = roleId;
                cmd.Parameters.Add("p_EmpID", OracleDbType.Int32).Value = null;//empId;
                cmd.Parameters.Add("p_cursor", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                con.Open();
                using (OracleDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        dt.Rows.Add(
                            dr["KPAID"]?.ToString(),
                            dr["KPACategoryName"]?.ToString(),
                            dr["KPAName"]?.ToString(),
                            dr["UNITOFKPA"]?.ToString(),
                            dr["DataSourceName"]?.ToString()
                        );
                    }
                }
            }

            int rowCount = dt.Rows.Count;

            return dt;
        }

        public DataTable GetRoleById(int roleId)
        {
            DataTable dt = new DataTable();

            using (OracleConnection conn = new OracleConnection(_connStr))
            using (OracleCommand cmd = new OracleCommand("USP_GET_ROLE_BY_ROLEID", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("P_OUT_CUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("P_ROLEID", OracleDbType.Int32).Value = roleId;

                conn.Open();
                OracleDataAdapter da = new OracleDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public void SaveKpaMap(string jsonPayload)
        {
            using (OracleConnection con = new OracleConnection(_connStr))
            using (OracleCommand cmd = new OracleCommand("USP_SAVE_KPA_SIGNOFF", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new OracleParameter("p_json", OracleDbType.Clob, jsonPayload, ParameterDirection.Input));
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public DataTable GetKpaRoleMapping(int roleId, int empId, out string status)
        {
            status = "NOT STARTED";

            DataTable dt = new DataTable();
            dt.Columns.Add("KpaID");
            dt.Columns.Add("Category");
            dt.Columns.Add("KPAName");
            dt.Columns.Add("UnitOfKPA");
            dt.Columns.Add("DataSource");
            dt.Columns.Add("ImportanceID");
            dt.Columns.Add("Comments");

            using (OracleConnection con = new OracleConnection(_connStr))
            using (OracleCommand cmd = new OracleCommand("USP_GET_KPA_ROLE_MAPPING", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("P_RoleID", OracleDbType.Int32).Value = roleId;
                cmd.Parameters.Add("P_EmpID", OracleDbType.Int32).Value = empId;
                cmd.Parameters.Add("p_out_cur", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                con.Open();
                using (OracleDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.HasRows)
                        return dt;

                    while (dr.Read())
                    {
                        // get Status from first row only
                        if (status == "NOT STARTED" && dr["Status"] != DBNull.Value)
                            status = dr["Status"].ToString();

                        dt.Rows.Add(
                            dr["KPAID"]?.ToString(),
                            dr["KPACategoryName"]?.ToString(),
                            dr["KPAName"]?.ToString(),
                            dr["UnitOfKPA"]?.ToString(),
                            dr["DataSource"]?.ToString(),
                            dr["ImportanceID"]?.ToString(),
                            dr["Comments"]?.ToString()
                        );
                    }
                }
            }

            return dt;
        }
    }
}
