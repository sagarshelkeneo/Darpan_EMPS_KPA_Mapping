using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBService.Model
{
    public class LoginService
    {
        private readonly string _connStr;
        private readonly string _loginConnStr;
        private readonly ErrorLog _errorLog = new ErrorLog();


        public LoginService()
        {
            try
            {
                _connStr = ConfigurationManager.ConnectionStrings["EPMS"].ConnectionString;
                // _loginConnStr = ConfigurationManager.ConnectionStrings["EPMSLogin"].ConnectionString;
            }
            catch (Exception ex) { _errorLog.LogError(ex); }

        }

        //public int ValidateUser(string username, string password)
        //{
        //    int empId = 0;

        //    try
        //    {
        //        using (OracleConnection conn = new OracleConnection(_connStr))
        //        using (OracleCommand cmd = new OracleCommand("USP_EMP_LOGIN", conn))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;

        //            cmd.Parameters.Add("P_UserName", OracleDbType.Varchar2).Value = username;
        //            //
        //            cmd.Parameters.Add("P_Password", OracleDbType.Varchar2).Value = password;

        //            // output parameters
        //            cmd.Parameters.Add("p_EmpID", OracleDbType.Int32).Direction = ParameterDirection.Output;
        //            cmd.Parameters.Add("P_Result", OracleDbType.Varchar2, 1).Direction = ParameterDirection.Output;

        //            conn.Open();
        //            cmd.ExecuteNonQuery();

        //            // get outputs
        //            string result = cmd.Parameters["P_Result"].Value?.ToString();
        //            object empVal = cmd.Parameters["p_EmpID"].Value;

        //            if (result == "Y" && empVal != DBNull.Value)
        //            {
        //                // Cast to OracleDecimal first, then get Int32
        //                empId = ((Oracle.ManagedDataAccess.Types.OracleDecimal)empVal).ToInt32();
        //            }

        //        }

        //    }
        //    catch (Exception ex) { _errorLog.LogError(ex); }
        //    return empId; // null if invalid

        //}


        //public int ValidateUser(string username, string password)
        //{
        //    int empId = 0;

        //    try
        //    {
        //        using (OracleConnection conn = new OracleConnection(_connStr))
        //        using (OracleCommand cmd = new OracleCommand("USP_EMP_LOGIN", conn))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;

        //            // Add input parameters
        //            cmd.Parameters.Add("P_UserName", OracleDbType.Varchar2).Value = username;
        //            cmd.Parameters.Add("P_Password", OracleDbType.Varchar2).Value = password;
        //            cmd.Parameters.Add("P_OUT_CUR", OracleDbType.Int32).Direction = ParameterDirection.Output;


        //            // Add return value parameter
        //            var returnParam = cmd.Parameters.Add("P_OUT_CUR", OracleDbType.Int32);
        //            returnParam.Direction = ParameterDirection.ReturnValue;

        //            conn.Open();
        //            cmd.ExecuteNonQuery();

        //            // Capture return value
        //            if (returnParam.Value != DBNull.Value)
        //            {
        //                empId = Convert.ToInt32(returnParam.Value);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _errorLog.LogError(ex);
        //    }

        //    return empId;
        //}

        public int ValidateUser(string username, string password)
        {
            int empId = 0;

            try
            {
                using (OracleConnection conn = new OracleConnection(_connStr))
                using (OracleCommand cmd = new OracleCommand("USP_EMP_LOGIN", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Input parameters
                    cmd.Parameters.Add("P_UserName", OracleDbType.Varchar2).Value = username;
                    cmd.Parameters.Add("P_Password", OracleDbType.Varchar2).Value = password;

                    // Output parameter (Ref Cursor)
                    cmd.Parameters.Add("P_OUT_CUR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    conn.Open();

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string result = reader["RESULT"]?.ToString();
                            if (result == "Y")
                            {
                                empId = Convert.ToInt32(reader["EMPID"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _errorLog.LogError(ex);
            }

            return empId;
        }



        public bool CheckLogin(string username, string password)
        {
            string result = "N";
            try
            {
                using (OracleConnection conn = new OracleConnection(_connStr))
                using (OracleCommand cmd = new OracleCommand("SELECT FN_CHECK_LOGIN(:username, :password) FROM DUAL", conn))
                {
                    cmd.Parameters.Add("username", OracleDbType.Varchar2).Value = username;
                    cmd.Parameters.Add("password", OracleDbType.Varchar2).Value = password;

                    conn.Open();
                    result = cmd.ExecuteScalar()?.ToString().Trim() ?? "N";  // Trim to remove CHAR padding
                }


                //using (OracleCommand cmd = new OracleCommand("SELECT PMS.Chk_Pms_Pwd(:vsail_pno, :vpasswd) FROM DUAL", conn))

                //{

                //    cmd.Parameters.Add("vsail_pno", OracleDbType.Varchar2).Value = username;

                //    cmd.Parameters.Add("vpasswd", OracleDbType.Varchar2).Value = password;

                //    conn.Open();

                //    result = cmd.ExecuteScalar()?.ToString().Trim() ?? "N";  // Trim to remove CHAR padding

                //}


            }
            catch (Exception ex)
            {
                _errorLog.LogError(ex);
            }
            return result == "Y";
        }

    }
}
