using DBService.Model;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;

namespace DBService
{
    public class AddKPAService
    {
        private readonly string _connStr;
        private readonly ErrorLog _errorLog = new ErrorLog();

        public AddKPAService()
        {
            _connStr = ConfigurationManager.ConnectionStrings["EPMS"].ConnectionString;
        }

        

        public DataTable GetCategories()
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(_connStr))
                using (OracleCommand cmd = new OracleCommand("USP_GET_CATEGORY_MASTER", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_KpaCategoryID", OracleDbType.Int32).Value = DBNull.Value;
                    cmd.Parameters.Add("P_OUT_CUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                _errorLog.LogError(ex);
                return null;
            }
        }

        public DataTable GetDataSources()
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(_connStr))
                using (OracleCommand cmd = new OracleCommand("USP_GET_DATA_SOURCE_MASTER", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_DataSourceID", OracleDbType.Int32).Value = DBNull.Value;
                    cmd.Parameters.Add("P_OUT_CUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                _errorLog.LogError(ex);
                return null;
            }
        }

        public DataTable GetImportance()
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(_connStr))
                using (OracleCommand cmd = new OracleCommand("USP_GET_IMPORTANCE_MASTER", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;

                    cmd.Parameters.Add("P_IMPORTANCEID", OracleDbType.Int32).Value = DBNull.Value;
                    cmd.Parameters.Add("P_OUT_CUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                _errorLog.LogError(ex);
                return null;
            }
        }

        public void InsertKpaData(List<KPAMaster> kpas, int roleId,int empID)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(_connStr))
                {
                    conn.Open();

                    foreach (var kpa in kpas)
                    {
                        int newKpaId = 0;

                        // 🔹 Insert into KPA_MASTER
                        try
                        {
                            using (OracleCommand cmd = new OracleCommand("USP_INSERT_KPA_MASTER", conn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add("p_KPACATEGORYID", OracleDbType.Int32).Value = kpa.CategoryId;
                                cmd.Parameters.Add("p_DATASOURCEID", OracleDbType.Int32).Value = kpa.DataSourceId;
                                cmd.Parameters.Add("p_KPANAME", OracleDbType.Varchar2).Value = kpa.KpaName;
                                cmd.Parameters.Add("p_UNITOFKPA", OracleDbType.Varchar2).Value = kpa.UnitOfKpa;

                                OracleParameter outParam = new OracleParameter("p_KPAID", OracleDbType.Int32, ParameterDirection.Output);
                                cmd.Parameters.Add(outParam);

                                cmd.ExecuteNonQuery();
                                newKpaId = Convert.ToInt32(outParam.Value.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            _errorLog.LogError(ex);
                            continue; // Skip this KPA and continue with next
                        }

                        // 🔹 Insert into KPA_ROLE_MAPPING
                        try
                        {
                            using (OracleCommand cmdMap = new OracleCommand("USP_INSERT_KPA_ROLE_MAPPING", conn))
                            {
                                cmdMap.CommandType = CommandType.StoredProcedure;
                                cmdMap.Parameters.Add("p_KPAID", OracleDbType.Int32).Value = newKpaId;
                                cmdMap.Parameters.Add("p_ROLEID", OracleDbType.Int32).Value = roleId;
                                cmdMap.Parameters.Add("p_EMPID", OracleDbType.Int32).Value = empID;

                                string dataSource = kpa.DataSourceText.Equals("Other", StringComparison.OrdinalIgnoreCase)
                                                    ? kpa.OtherSource
                                                    : kpa.DataSourceText;
                                cmdMap.Parameters.Add("p_DATASOURCE", OracleDbType.Varchar2).Value = dataSource;

                                OracleParameter paramComment = new OracleParameter("p_COMMENTS", OracleDbType.Varchar2);
                                paramComment.Value = string.IsNullOrWhiteSpace(kpa.Comment) ? (object)DBNull.Value : kpa.Comment;
                                cmdMap.Parameters.Add(paramComment);

                                cmdMap.Parameters.Add("p_IMPORTANCEID", OracleDbType.Int32).Value = kpa.ImportanceId;

                                cmdMap.ExecuteNonQuery();
                            }
                        }
                        catch (Exception ex)
                        {
                            _errorLog.LogError(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _errorLog.LogError(ex);
            }
        }
    }
}
