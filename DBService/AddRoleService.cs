using DBService.Model;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Web.UI.WebControls;

namespace DBService
{
    public class AddRoleService
    {
        private readonly string _connStr;
        private readonly ErrorLog _errorLog = new ErrorLog();

        public AddRoleService()
        {
            _connStr = ConfigurationManager.ConnectionStrings["EPMS"].ConnectionString;
        }

       

        #region Data Fetching

        public DataTable GetWorksData()
        {
            try
            {
                using (OracleConnection con = new OracleConnection(_connStr))
                using (OracleCommand cmd = new OracleCommand("USP_GET_ROLE_WORKS_TYPE_MASTER", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("P_OUT_CUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    DataTable dt = new DataTable();
                    new OracleDataAdapter(cmd).Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                _errorLog.LogError(ex);
                return null;
            }
        }

        public DataTable GetRoleTypeData(int roleWorksId)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(_connStr))
                using (OracleCommand cmd = new OracleCommand("USP_GET_ROLE_TYPE_MASTER", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_RoleTypeID", OracleDbType.Int32).Value = DBNull.Value;
                    cmd.Parameters.Add("P_RoleWorksID", OracleDbType.Int32).Value = roleWorksId;
                    cmd.Parameters.Add("P_OUT_CUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    DataTable dt = new DataTable();
                    new OracleDataAdapter(cmd).Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                _errorLog.LogError(ex);
                return null;
            }
        }

        public DataTable GetShopData(int? roleWorksId = null)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(_connStr))
                using (OracleCommand cmd = new OracleCommand("USP_GET_SHOP_MASTER", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_ShopID", OracleDbType.Int32).Value = DBNull.Value;
                    cmd.Parameters.Add("p_RoleWorksID", OracleDbType.Int32).Value = roleWorksId.HasValue ? (object)roleWorksId.Value : DBNull.Value;
                    cmd.Parameters.Add("p_out_cur", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    DataTable dt = new DataTable();
                    new OracleDataAdapter(cmd).Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                _errorLog.LogError(ex);
                return null;
            }
        }

        public DataTable GetFunctionData(int? functionId = null)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(_connStr))
                using (OracleCommand cmd = new OracleCommand("USP_GET_FUNCTION_MASTER", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("P_OUT_CUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("P_FUNCTIONID", OracleDbType.Int32).Value = functionId.HasValue ? (object)functionId.Value : DBNull.Value;

                    DataTable dt = new DataTable();
                    new OracleDataAdapter(cmd).Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                _errorLog.LogError(ex);
                return null;
            }
        }

        public DataTable GetSectionData(int shopId)
        {
            try
            {
                using (OracleConnection con = new OracleConnection(_connStr))
                using (OracleCommand cmd = new OracleCommand("USP_GET_SECTION_MASTER", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("P_SECTIONID", OracleDbType.Int32).Value = DBNull.Value;
                   // cmd.Parameters.Add("P_ROLETYPEID", OracleDbType.Int32).Value = DBNull.Value;
                    cmd.Parameters.Add("P_SHOPID", OracleDbType.Int32).Value = shopId;
                    cmd.Parameters.Add("P_OUT_CUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    DataTable dt = new DataTable();
                    new OracleDataAdapter(cmd).Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                _errorLog.LogError(ex);
                return null;
            }
        }

        #endregion

        public RoleFlags FetchRoleFlags(int roleTypeId)
        {
            try
            {
                RoleFlags flags = null;

                using (OracleConnection conn = new OracleConnection(_connStr))
                using (OracleCommand cmd = new OracleCommand("USP_GET_ROLE_TYPE_MASTER", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("p_RoleTypeID", OracleDbType.Int32).Value = roleTypeId;
                    cmd.Parameters.Add("P_RoleWorksID", OracleDbType.Int32).Value = DBNull.Value;
                    cmd.Parameters.Add("P_OUT_CUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            flags = new RoleFlags
                            {
                                IsFunctionApplicable = reader["ISFUNCTIONAPPLICABLE"].ToString(),
                                IsSectionApplicable = reader["ISSECTIONAPPLICABLE"].ToString()
                            };
                        }
                    }
                }

                return flags;
            }
            catch (Exception ex)
            {
                _errorLog.LogError(ex);
                return null;
            }
        }

        public int InsertRole(RoleMaster role)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(_connStr))
                using (OracleCommand cmd = new OracleCommand("USP_INSERT_ROLE_MASTER", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("P_ROLEID", OracleDbType.Int32).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("P_ROLETYPEID", OracleDbType.Int32).Value = role.RoleTypeId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_ROLENAME", OracleDbType.Varchar2).Value = role.RoleName;
                    cmd.Parameters.Add("P_ROLECODE", OracleDbType.Varchar2).Value = role.RoleCode;
                    cmd.Parameters.Add("P_ROLEWORKSID", OracleDbType.Int32).Value = role.WorkId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_SHOPID", OracleDbType.Int32).Value = role.ShopId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_SECTIONID", OracleDbType.Int32).Value = role.SectionId ?? (object)DBNull.Value;
                    cmd.Parameters.Add("P_FUNCTIONID", OracleDbType.Int32).Value = role.FunctionId ?? (object)DBNull.Value;

                    cmd.Parameters.Add("P_FUNCTIONNAME", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(role.FunctionName) ? (object)DBNull.Value : role.FunctionName;
                    cmd.Parameters.Add("P_SECTIONNAME", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(role.SectionName) ? (object)DBNull.Value : role.SectionName;
                    cmd.Parameters.Add("P_SHOPNAME", OracleDbType.Varchar2).Value = string.IsNullOrEmpty(role.ShopName) ? (object)DBNull.Value : role.ShopName;

                    conn.Open();


                    // ✅ Build a debug string of all parameters
                    var parameterDetails = new StringBuilder();
                    parameterDetails.AppendLine($"Executing Stored Procedure: {cmd.CommandText}");
                    foreach (OracleParameter p in cmd.Parameters)
                    {
                        string direction = p.Direction.ToString();
                        string value = (p.Value == null || p.Value == DBNull.Value) ? "NULL" : p.Value.ToString();
                        parameterDetails.AppendLine($"{p.ParameterName} ({direction}) = {value}");
                    }

                    // ✅ Example: log it or debug it
                    string ss = parameterDetails.ToString();

                    cmd.ExecuteNonQuery();

                    return ((Oracle.ManagedDataAccess.Types.OracleDecimal)cmd.Parameters["P_ROLEID"].Value).ToInt32();
                }
            }
            catch (Exception ex)
            {
                string mes = ex.ToString();
                _errorLog.LogError(ex);
                return -1; // Return -1 to indicate failure
            }
        }
    }
}
