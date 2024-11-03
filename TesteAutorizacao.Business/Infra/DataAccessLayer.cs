using System.Data;
using System.Data.SqlClient;
using System.Reflection;

public class DataAccessLayer
    {
        private enum SqlConnectionOwnership
        {
            Internal,
            External
        }
        private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (commandParameters != null)
            {
                foreach (SqlParameter p in commandParameters)
                {
                    if (p != null)
                    {
                        if ((p.Direction == ParameterDirection.InputOutput ||
                            p.Direction == ParameterDirection.Input) &&
                            (p.Value == null))
                        {
                            p.Value = DBNull.Value;
                        }
                        command.Parameters.Add(p);
                    }
                }
            }
        }
        private static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, out bool mustCloseConnection)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");
            if (connection.State != ConnectionState.Open)
            {
                mustCloseConnection = true;
                connection.Open();
            }
            else
            {
                mustCloseConnection = false;
            }

            command.Connection = connection;
            command.CommandText = commandText;
            command.CommandTimeout = connection.ConnectionTimeout;
            if (transaction != null)
            {
                if (transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                command.Transaction = transaction;
            }
            command.CommandType = commandType;
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }
            return;
        }
        private static SqlParameter[] AssignParameterValues(object valor)
        {
            //
            if (valor == null)
                return null;
            //
            SqlParameter[] parametros;
            int numParametros = 0, posParametro = 0;
            PropertyInfo[] propriedades = valor.GetType().GetProperties();
            //
            for (int a = 0; a < propriedades.Length; a++)
            {
                if ((valor.GetType().GetProperty(propriedades[a].Name).GetValue(valor, null) != null) && !propriedades[a].PropertyType.Namespace.Equals("System.Collections.Generic"))
                {
                    numParametros++;
                }
            }
            //
            if (numParametros.Equals(0))
                return null;
            //
            parametros = new SqlParameter[numParametros];
            //
            for (int a = 0; a < propriedades.Length; a++)
            {
                //
                object objeto = valor.GetType().GetProperty(propriedades[a].Name).GetValue(valor, null);
                //
                if ((objeto != null) && !propriedades[a].PropertyType.Namespace.Equals("System.Collections.Generic"))
                {
                    parametros[posParametro] = new SqlParameter("@" + propriedades[a].Name, objeto);
                    posParametro++;
                }
            }
            return parametros;
        }
        private static T SetObject<T>(SqlDataReader dr) where T : new()
         {
            T item = new T();
            var Properties = item.GetType().GetProperties();
            for (int a = 0; a < dr.VisibleFieldCount; a++)
             {
                if (!dr.IsDBNull(a))
                {
                    var result = (from i in Properties where i.Name.ToLower() == dr.GetName(a).ToLower() select i.Name).ToList();
                    if(result.Count != 0)
                        item.GetType().GetProperty(result[0]).SetValue(item, dr[a]);
                }           
            }
            return item;
        }
        private static List<TR> ExecuteReader<TR>(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, object valor, SqlConnectionOwnership connectionOwnership) where TR : new()
        {
            List<TR> ret = new List<TR>();
            if (connection == null) throw new ArgumentNullException("connection");
            bool mustCloseConnection = false;
            SqlCommand cmd = new SqlCommand();
            try
            {
                PrepareCommand(cmd, connection, transaction, commandType, commandText, AssignParameterValues(valor), out mustCloseConnection);
                SqlDataReader dataReader;
                if (connectionOwnership == SqlConnectionOwnership.External)
                {
                    dataReader = cmd.ExecuteReader();
                }
                else
                {
                    dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
                bool canClear = true;
                foreach (SqlParameter commandParameter in cmd.Parameters)
                {
                    if (commandParameter.Direction != ParameterDirection.Input)
                        canClear = false;
                }
                if (canClear)
                {
                    cmd.Parameters.Clear();
                }
                while (dataReader.Read())
                {
                    ret.Add(SetObject<TR>(dataReader));
                }
                return ret;
            }
            catch
            {
                if (mustCloseConnection)
                    connection.Close();
                throw;
            }
        }
        public static List<T> ExecuteReader<T>(string connectionString, CommandType commandType, string commandText) where T : new()
        {
            return ExecuteReader<T>(connectionString, commandType, commandText, null);
        }
        public static List<T> ExecuteReader<T>(string connectionString, CommandType commandType, string commandText, object valor) where T : new()
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
                return ExecuteReader<T>(connection, null, commandType, commandText, valor, SqlConnectionOwnership.Internal);
            }
            catch
            {
                if (connection != null) connection.Close();
                throw;
            }

        }
        public static List<T> ExecuteReader<T>(string connectionString, string spName, object valor) where T : new()
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            if (valor != null)
            {
                return ExecuteReader<T>(connectionString, CommandType.StoredProcedure, spName, valor);
            }
            else
            {
                return ExecuteReader<T>(connectionString, CommandType.StoredProcedure, spName);
            }
        }
        public static List<T> ExecuteReader<T>(SqlConnection connection, CommandType commandType, string commandText) where T : new()
        {
            return ExecuteReader<T>(connection, commandType, commandText, null);
        }
        public static List<T> ExecuteReader<T>(SqlConnection connection, CommandType commandType, string commandText, object valor) where T : new()
        {
            return ExecuteReader<T>(connection, null, commandType, commandText, valor, SqlConnectionOwnership.External);
        }
        public static List<T> ExecuteReader<T>(SqlConnection connection, string spName, object valor) where T : new()
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            if (valor != null)
            {
                return ExecuteReader<T>(connection, CommandType.StoredProcedure, spName, AssignParameterValues(valor));
            }
            else
            {
                return ExecuteReader<T>(connection, CommandType.StoredProcedure, spName);
            }
        }
        public static List<T> ExecuteReader<T>(SqlTransaction transaction, CommandType commandType, string commandText) where T : new()
        {
            return ExecuteReader<T>(transaction, commandType, commandText, (SqlParameter[])null);
        }
        public static List<T> ExecuteReader<T>(SqlTransaction transaction, CommandType commandType, string commandText, object valor) where T : new()
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            return ExecuteReader<T>(transaction.Connection, transaction, commandType, commandText, valor, SqlConnectionOwnership.External);
        }
        public static List<T> ExecuteReader<T>(SqlTransaction transaction, string spName, object valor) where T : new()
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            if (valor != null)
            {

                return ExecuteReader<T>(transaction, CommandType.StoredProcedure, spName, valor);
            }
            else
            {
                return ExecuteReader<T>(transaction, CommandType.StoredProcedure, spName);
            }
        }
        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteScalar(connectionString, commandType, commandText, null);
        }
        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, object valor)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return ExecuteScalar(connection, commandType, commandText, valor);
            }
        }
        public static object ExecuteScalar(string connectionString, string spName, object valor)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            if (valor != null)
            {
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, AssignParameterValues(valor));
            }
            else
            {
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
            }
        }
        public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteScalar(connection, commandType, commandText, null);
        }
        public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText, object valor)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, null, commandType, commandText, AssignParameterValues(valor), out mustCloseConnection);
            object retval = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            if (mustCloseConnection)
                connection.Close();
            return retval;
        }
        public static object ExecuteScalar(SqlConnection connection, string spName, object valor)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            if (valor != null)
            {
                return ExecuteScalar(connection, CommandType.StoredProcedure, spName, AssignParameterValues(valor));
            }
            else
            {
                return ExecuteScalar(connection, CommandType.StoredProcedure, spName);
            }
        }
        public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteScalar(transaction, commandType, commandText, null);
        }
        public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText, object valor)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, AssignParameterValues(valor), out mustCloseConnection);
            object retval = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return retval;
        }
        public static object ExecuteScalar(SqlTransaction transaction, string spName, object valor)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            if (valor != null)
            {
                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName, AssignParameterValues(valor));
            }
            else
            {
                return ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
            }
        }
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(connectionString, commandType, commandText, null);
        }
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, object valor)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return ExecuteNonQuery(connection, commandType, commandText, valor);
            }
        }
        public static int ExecuteNonQuery(string connectionString, string spName, object valor)
        {
            if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            if (valor != null)
            {
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, AssignParameterValues(valor));
            }
            else
            {
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
            }
        }
        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(connection, commandType, commandText, null);
        }
        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText, object valor)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, null, commandType, commandText, AssignParameterValues(valor), out mustCloseConnection);
            int retval = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            if (mustCloseConnection)
                connection.Close();
            return retval;
        }
        public static int ExecuteNonQuery(SqlConnection connection, string spName, object valor)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            if (valor != null)
            {
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, AssignParameterValues(valor));
            }
            else
            {
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
            }
        }
        public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(transaction, commandType, commandText, null);
        }
        public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText, object valor)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, AssignParameterValues(valor), out mustCloseConnection);
            int retval = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return retval;
        }
        public static int ExecuteNonQuery(SqlTransaction transaction, string spName, object valor)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");
            if (valor != null)
            {
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, AssignParameterValues(valor));
            }
            else
            {
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
            }
        }
    }