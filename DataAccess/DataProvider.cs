/*
 * Author:				Vex Tatarevic
 * Date Created:		2007-09-09
 * Copyright:           VEX IT Pty Ltd 2013 - www.vexit.com
 *
 */


using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace vEX.DataAccess
{

    public class DataProvider
    {
        /// <summary>
        ///  CONNECTION STRING fetched from App Config
        /// </summary>         
        public static string ConnectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];

        public SqlConnection Connection;
        public SqlCommand Command;
        public IDataReader Reader;

        /// <summary>
        ///  CONSTRUCTOR
        /// </summary>
        public DataProvider() { }

        
        /// <summary>
        ///  EXECUTE READER - Used for SELECT
        /// </summary>  
        public IDataReader ExecuteReader(string storedProcedure, Hashtable parameters)
        {
            Connection = new SqlConnection(ConnectionString);
            Command = new SqlCommand();
            // Set Command and open Connection
            SetCommandOpenConnection(ref Connection, ref Command, storedProcedure, parameters);
            Reader = Command.ExecuteReader();
            return Reader;
        }


        /// <summary>
        ///  DEPRECATED !!! - Use ExecutePaged instead
        ///  EXECUTE SEARCH  - Used for SEARCH Selection - adds ItemCount Output parameter to command
        /// </summary>  
        [Obsolete("Use ExecutePaged instead")]
        public IDataReader ExecuteSearch(string storedProcedure, Hashtable parameters)
        {
            Connection = new SqlConnection(ConnectionString);
            Command = new SqlCommand();
            // Set Command and open Connection
            SetCommandOpenConnection(ref Connection, ref Command, storedProcedure, parameters);
            Command.Parameters.Add("@ItemCount", SqlDbType.Int).Direction = ParameterDirection.Output;
            Reader = Command.ExecuteReader();
            return Reader;
        }

        /// <summary>
        ///  EXECUTE PAGED  - Used for PAGED Selection - 
        ///                     Ads @TotalCount Output parameter to command
        /// </summary>  
        public IDataReader ExecutePaged(string storedProcedure, Hashtable parameters)
        {
            Connection = new SqlConnection(ConnectionString);
            Command = new SqlCommand();
            // Set Command and open Connection
            SetCommandOpenConnection(ref Connection, ref Command, storedProcedure, parameters);
            Command.Parameters.Add("@TotalCount", SqlDbType.BigInt).Direction = ParameterDirection.Output;
            Reader = Command.ExecuteReader();
            return Reader;
        }

        public void CloseReaderConnection()
        {
            if (!Reader.IsClosed)
            {
                Reader.Close();
                Reader.Dispose();
            }
            Connection.Close();
            Command.Dispose();
            Connection.Dispose();
        }

        /// <summary>
        ///     EXECUTE READER     - Used for SELECT
        /// </summary>
        public IDataReader ExecuteReader(string storedProcedure) { return ExecuteReader(storedProcedure, new Hashtable()); }


        /// <summary>
        ///  EXECUTE READER - Used for SELECT
        /// </summary>  
        /*public DataTable ExecuteReader(string storedProcedure, Hashtable parameters)
        {
            DataTable dt;
            // Open Connection
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                //Create a Command Object identifying the Stored Procedure
                using (SqlCommand command = new SqlCommand(storedProcedure, connection))
                {
                    // Set Command
                    SetCommandOpenConnection(connection, command, parameters);
                    // Open reader
                    SqlDataReader reader = command.ExecuteReader();
                    //Construct DataTable
                    dt = new DataTable();
                    DataTable metaTable = reader.GetSchemaTable();
                    foreach (DataRow row in metaTable.Rows)
                    {
                        string colName = (string)(row[0]);
                        dt.Columns.Add(colName);
                    }
                    //Get all Result Rows from reader and put them in a Table
                    while (reader.Read())
                    {
                        DataRow dr = dt.NewRow();
                        // Fill Columns with Values
                        for (int i = 0; i <= dt.Columns.Count - 1; i++)
                            dr[i] = reader.GetValue(i);
                        // Add Row to Table
                        dt.Rows.Add(dr);
                    }
                    reader.Close();
                }
            }
            return dt;
        }
        */
        /// <summary>
        ///     EXECUTE READER     - Used for SELECT
        /// </summary>
        // public DataTable ExecuteReader(string storedProcedure) { return ExecuteReader(storedProcedure, new Hashtable()); }

        /// <summary>
        ///     EXECUTE SCALAR     - Used for INSERT
        /// </summary>
        public int ExecuteScalar(string storedProcedure, Hashtable parameters)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(storedProcedure, connection))
                {
                    // Set Command
                    SetCommandOpenConnection(connection, command, parameters);
                    // Execute Command
                    object id = command.ExecuteScalar();
                    return Convert.ToInt32(id == DBNull.Value ? -1 : id);
                }
            }
        }

        public long ExecuteScalarLong(string storedProcedure, Hashtable parameters)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(storedProcedure, connection))
                {
                    // Set Command
                    SetCommandOpenConnection(connection, command, parameters);
                    // Execute Command
                    object id = command.ExecuteScalar();
                    return Convert.ToInt64(id == DBNull.Value ? -1 : id);
                }
            }
        }


        /// <summary>
        ///  EXECUTE NONQUERY    - Used for UPDATE , DELETE
        /// </summary>
        public int ExecuteNonQuery(string storedProcedure, Hashtable parameters)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(storedProcedure, connection))
                {
                    // Set Command
                    SetCommandOpenConnection(connection, command, parameters);
                    // Execute Command
                    return (int)(command.ExecuteNonQuery());
                }
            }
        }
        public int ExecuteNonQuery(string storedProcedure)
        {
            return ExecuteNonQuery(storedProcedure, new Hashtable());
        }

        /// <summary>
        ///  Opens connection , Sets command type , adds input parameters to the command
        /// </summary> 
        private void SetCommandOpenConnection(SqlConnection connection, SqlCommand command, Hashtable parameters)
        {
            // Set the Command Object so it knows to execute a Stored Procedure
            command.CommandType = CommandType.StoredProcedure;
            // Add parameters
            foreach (DictionaryEntry parameter in parameters)
                command.Parameters.Add(new SqlParameter((string)(parameter.Key), parameter.Value));
            command.Connection = connection;
            // Open Connection
            connection.Open();
        }

        /// <summary>
        ///  Opens connection , Sets command type , adds input parameters to the command
        /// </summary> 
        private void SetCommandOpenConnection(ref SqlConnection connection, ref SqlCommand command, string commandText, Hashtable parameters)
        {
            // Set the Command Object so it knows to execute a Stored Procedure
            command.CommandType = CommandType.StoredProcedure;
            // Set command text
            command.CommandText = commandText;
            // Add parameters
            foreach (DictionaryEntry parameter in parameters)
                command.Parameters.Add(new SqlParameter((string)(parameter.Key), parameter.Value));
            command.Connection = connection;
            // Open Connection
            connection.Open();
        }

        /// <summary>
        ///  Convert DB Null to code null
        /// </summary>
        public static object IsDBNull(object val)
        {
            return (object)((val == System.DBNull.Value ? val = null : val));
        }

        private DataTable GetDataTable(string sSQL)
        {
            DataTable DT = new DataTable();
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(sSQL, con);
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 600;
                SqlDataAdapter DA = new SqlDataAdapter(cmd);
                try
                {
                    DA.Fill(DT);
                }
                catch (Exception ex)
                {                   
                    DT = null;
                }
                finally
                {
                    cmd.Dispose();
                    DA.Dispose();
                    con.Close();
                    con.Dispose();
                }
            }
            return DT;
        }

    }

}
