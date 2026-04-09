using System.Data;
using System.Text;

namespace Crudspa.Framework.Core.Server.Extensions;

public static class SqlCommandEx
{
    private const Int32 TimeoutSeconds = 120;

    extension(SqlCommand command)
    {
        private void SetTimeout(Int32? timeout)
        {
            var timeoutSeconds = timeout ?? TimeoutSeconds;

            if (timeoutSeconds <= 0)
                timeoutSeconds = TimeoutSeconds;

            command.CommandTimeout = timeoutSeconds;
        }

        public async Task Execute(String connectionString, Int32? timeout = null, CommandType commandType = CommandType.StoredProcedure)
        {
            command.CommandType = commandType;
            command.SetTimeout(timeout);

            await using var connection = new SqlConnection(connectionString);
            command.Connection = connection;
            await connection.OpenAsync();

            await command.ExecuteNonQueryAsync();
        }

        public async Task Execute(SqlConnection connection, SqlTransaction? transaction, Int32? timeout = null, CommandType commandType = CommandType.StoredProcedure)
        {
            command.CommandType = commandType;
            command.Connection = connection;
            command.Transaction = transaction;
            command.SetTimeout(timeout);

            await command.ExecuteNonQueryAsync();
        }

        public async Task<Int32> ExecuteScalarInt(String connectionString, Int32? timeout = null, CommandType commandType = CommandType.StoredProcedure)
        {
            command.CommandType = commandType;
            command.SetTimeout(timeout);

            await using var connection = new SqlConnection(connectionString);
            command.Connection = connection;
            await connection.OpenAsync();

            return (Int32)(await command.ExecuteScalarAsync() ?? 0);
        }

        public async Task<Int32> ExecuteScalarInt(SqlConnection connection, SqlTransaction? transaction, Int32? timeout = null, CommandType commandType = CommandType.StoredProcedure)
        {
            command.CommandType = commandType;
            command.Connection = connection;
            command.Transaction = transaction;
            command.SetTimeout(timeout);

            return (Int32)(await command.ExecuteScalarAsync() ?? 0);
        }

        public async Task<Boolean> ExecuteBoolean(String connectionString, String outputParameterName, Int32? timeout = null, CommandType commandType = CommandType.StoredProcedure)
        {
            var outputParameter = command.AddOutputParameter(outputParameterName, SqlDbType.Bit);

            command.CommandType = commandType;
            command.SetTimeout(timeout);

            await using var connection = new SqlConnection(connectionString);
            command.Connection = connection;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return outputParameter.Value is Boolean result ? result : Convert.ToBoolean((Boolean)(outputParameter.Value ?? false));
        }

        public async Task<Boolean> ExecuteBoolean(SqlConnection connection, SqlTransaction? transaction, String outputParameterName, Int32? timeout = null, CommandType commandType = CommandType.StoredProcedure)
        {
            var outputParameter = command.AddOutputParameter(outputParameterName, SqlDbType.Bit);

            command.CommandType = commandType;
            command.Connection = connection;
            command.Transaction = transaction;
            command.SetTimeout(timeout);

            await command.ExecuteNonQueryAsync();

            if (outputParameter.Value is Boolean typed)
                return typed;

            return Convert.ToBoolean(outputParameter.Value ?? false);
        }

        public async Task<T> ExecuteQuery<T>(String connectionString, Func<SqlDataReader, Task<T>> func, Int32? timeout = null, CommandType commandType = CommandType.StoredProcedure)
        {
            command.CommandType = commandType;
            command.SetTimeout(timeout);

            await using var connection = new SqlConnection(connectionString);
            command.Connection = connection;
            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            return await func.Invoke(reader);
        }

        public async Task<T> ExecuteQuery<T>(SqlConnection connection, SqlTransaction? transaction, Func<SqlDataReader, Task<T>> func, Int32? timeout = null, CommandType commandType = CommandType.StoredProcedure)
        {
            command.CommandType = commandType;
            command.Connection = connection;
            command.Transaction = transaction;
            command.SetTimeout(timeout);

            await using var reader = await command.ExecuteReaderAsync();
            return await func.Invoke(reader);
        }

        public async Task<T?> ReadSingle<T>(String connectionString, Func<SqlDataReader, T?> func, Int32? timeout = null, CommandType commandType = CommandType.StoredProcedure)
        {
            command.CommandType = commandType;
            command.SetTimeout(timeout);

            await using var connection = new SqlConnection(connectionString);
            command.Connection = connection;
            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection | CommandBehavior.SingleRow);
            if (!await reader.ReadAsync()) return default;
            return func.Invoke(reader);
        }

        public async Task<T?> ReadSingle<T>(SqlConnection connection, SqlTransaction? transaction, Func<SqlDataReader, T?> func, Int32? timeout = null, CommandType commandType = CommandType.StoredProcedure)
        {
            command.CommandType = commandType;
            command.Connection = connection;
            command.Transaction = transaction;
            command.SetTimeout(timeout);

            await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);
            if (!await reader.ReadAsync())
                return default;

            return func.Invoke(reader);
        }

        public async Task<IList<T>> ReadAll<T>(String connectionString, Func<SqlDataReader, T> func, Int32? timeout = null, CommandType commandType = CommandType.StoredProcedure)
        {
            command.CommandType = commandType;
            command.SetTimeout(timeout);

            await using var connection = new SqlConnection(connectionString);
            command.Connection = connection;
            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            var ret = new List<T>();

            while (await reader.ReadAsync())
                ret.Add(func.Invoke(reader));

            return ret;
        }

        public async Task<IList<T>> ReadAll<T>(SqlConnection connection, SqlTransaction? transaction, Func<SqlDataReader, T> func, Int32? timeout = null, CommandType commandType = CommandType.StoredProcedure)
        {
            command.CommandType = commandType;
            command.Connection = connection;
            command.Transaction = transaction;
            command.SetTimeout(timeout);

            await using var reader = await command.ExecuteReaderAsync();
            var ret = new List<T>();

            while (await reader.ReadAsync())
                ret.Add(func.Invoke(reader));

            return ret;
        }

        public async Task<IList<Named>> ReadNameds(String connectionString)
        {
            return await command.ReadAll(connectionString, reader => new Named
            {
                Id = reader.ReadGuid(0),
                Name = reader.ReadString(1),
            });
        }

        public async Task<IList<Named>> ReadNameds(SqlConnection connection, SqlTransaction? transaction)
        {
            return await command.ReadAll(
                connection,
                transaction,
                reader => new Named
                {
                    Id = reader.ReadGuid(0),
                    Name = reader.ReadString(1),
                });
        }

        public async Task<IList<Selectable>> ReadSelectables(String connectionString)
        {
            return await command.ReadAll(connectionString, reader => new Selectable
            {
                RootId = reader.ReadGuid(0),
                Id = reader.ReadGuid(1),
                Name = reader.ReadString(2),
                Selected = reader.ReadBoolean(3),
            });
        }

        public async Task<IList<Selectable>> ReadSelectables(SqlConnection connection, SqlTransaction? transaction)
        {
            return await command.ReadAll(connection, transaction, reader => new Selectable
            {
                RootId = reader.ReadGuid(0),
                Id = reader.ReadGuid(1),
                Name = reader.ReadString(2),
                Selected = reader.ReadBoolean(3),
            });
        }

        public async Task<IList<Orderable>> ReadOrderables(String connectionString, Boolean includeParentId = false)
        {
            return await command.ReadAll(connectionString, reader => new Orderable
            {
                Id = reader.ReadGuid(0),
                Name = reader.ReadString(1),
                Ordinal = reader.ReadInt32(2),
                ParentId = includeParentId ? reader.ReadGuid(3) : null,
            });
        }

        public async Task<IList<Orderable>> ReadOrderables(SqlConnection connection, SqlTransaction? transaction, Boolean includeParentId = false)
        {
            return await command.ReadAll(connection, transaction, reader => new Orderable
            {
                Id = reader.ReadGuid(0),
                Name = reader.ReadString(1),
                Ordinal = reader.ReadInt32(2),
                ParentId = includeParentId ? reader.ReadGuid(3) : null,
            });
        }

        public async Task<IList<NamedCssClass>> ReadNamedCssClasses(String connectionString)
        {
            return await command.ReadAll(connectionString, reader => new NamedCssClass
            {
                Id = reader.ReadGuid(0),
                Name = reader.ReadString(1),
                CssClass = reader.ReadString(2),
            });
        }

        public async Task<IList<OrderableCssClass>> ReadOrderableCssClasses(String connectionString)
        {
            return await command.ReadAll(connectionString, reader => new OrderableCssClass
            {
                Id = reader.ReadGuid(0),
                Name = reader.ReadString(1),
                Ordinal = reader.ReadInt32(2),
                CssClass = reader.ReadString(3),
            });
        }

        public async Task<IList<T>> ExecuteAndReadAll<T>(SqlConnection connection, SqlTransaction? transaction, Func<SqlDataReader, T> func, Int32? timeout = null, CommandType commandType = CommandType.StoredProcedure)
        {
            command.CommandType = commandType;
            command.Connection = connection;
            command.Transaction = transaction;
            command.SetTimeout(timeout);

            await using var reader = await command.ExecuteReaderAsync();
            var ret = new List<T>();

            while (await reader.ReadAsync())
                ret.Add(func.Invoke(reader));

            return ret;
        }

        public void AddParameter(String name, Boolean? value) =>
            command.Parameters.Add(new(name, SqlDbType.Bit)).Value = value as Object ?? DBNull.Value;

        public void AddParameter(String name, Byte[]? value) =>
            command.Parameters.Add(new(name, SqlDbType.VarBinary, -1)).Value = value as Object ?? DBNull.Value;

        public void AddParameter(String name, Int32 length, Byte[]? value) =>
            command.Parameters.Add(new(name, SqlDbType.Binary, length)).Value = value as Object ?? DBNull.Value;

        public void AddParameter(String name, DateOnly? value) =>
            command.Parameters.Add(new(name, SqlDbType.Date)).Value = value as Object ?? DBNull.Value;

        public void AddParameter(String name, DateTimeOffset? value) =>
            command.Parameters.Add(new(name, SqlDbType.DateTimeOffset)).Value = value as Object ?? DBNull.Value;

        public void AddParameter(String name, Decimal? value) =>
            command.Parameters.Add(new(name, SqlDbType.Money)).Value = value as Object ?? DBNull.Value;

        public void AddParameter(String name, Byte precision, Byte scale, Decimal? value)
        {
            var parameter = command.Parameters.Add(new(name, SqlDbType.Decimal));
            parameter.Precision = precision;
            parameter.Scale = scale;
            parameter.Value = value as Object ?? DBNull.Value;
        }

        public void AddParameter(String name, Guid? value) =>
            command.Parameters.Add(new(name, SqlDbType.UniqueIdentifier)).Value = value as Object ?? DBNull.Value;

        public void AddParameter(String name, Double? value) =>
            command.Parameters.Add(new(name, SqlDbType.Float)).Value = value as Object ?? DBNull.Value;

        public void AddParameter(String name, Int64? value) =>
            command.Parameters.Add(new(name, SqlDbType.BigInt)).Value = value as Object ?? DBNull.Value;

        public void AddParameter(String name, Single? value) =>
            command.Parameters.Add(new(name, SqlDbType.Real)).Value = value as Object ?? DBNull.Value;

        public void AddParameter(String name, TimeSpan? value) =>
            command.Parameters.Add(new(name, SqlDbType.Time)).Value = value as Object ?? DBNull.Value;

        public void AddParameter(String name, Int32 length, String? value) =>
            command.Parameters.Add(new(name, SqlDbType.NVarChar, length)).Value = value as Object ?? DBNull.Value;

        public void AddParameter(String name, String? value) =>
            command.Parameters.Add(new(name, SqlDbType.NVarChar, -1)).Value = value as Object ?? DBNull.Value;

        public void AddParameter(String name, Int32 length, Uri? value) =>
            command.Parameters.Add(new(name, SqlDbType.NVarChar, length)).Value = value is null ? DBNull.Value : value.ToString();

        public void AddParameter<TEnum>(String name, TEnum value)
            where TEnum : struct, Enum
        {
            var sqlValue = (Int32)(Object)value;
            command.Parameters.Add(new(name, SqlDbType.Int)).Value = sqlValue;
        }

        public void AddParameter(String name, IEnumerable<Guid?> ids) =>
            command.AddStructuredParameter(name, "[Framework].[IdList]", ids.ToIdTable());

        public void AddParameter(String name, IEnumerable<IOrderable> orderables) =>
            command.AddStructuredParameter(name, "[Framework].[OrderedIdList]", orderables.ToIdTable());

        public void AddParameter(String name, IEnumerable<Selectable> selectables) =>
            command.AddStructuredParameter(name, "[Framework].[IdList]", selectables.ToIdTable());

        public SqlParameter AddInputOutputParameter(String name, Guid? value, SqlDbType type = SqlDbType.UniqueIdentifier)
        {
            var parameter = command.Parameters.Add(new(name, type));
            parameter.Direction = ParameterDirection.InputOutput;
            parameter.Value = value as Object ?? DBNull.Value;
            return parameter;
        }

        public SqlParameter AddOutputParameter(String name, SqlDbType type = SqlDbType.UniqueIdentifier)
        {
            var parameter = command.Parameters.Add(new(name, type));
            parameter.Direction = ParameterDirection.Output;
            return parameter;
        }

        public SqlParameter AddStringOutputParameter(String name, Int32 length)
        {
            var parameter = command.Parameters.Add(new(name, SqlDbType.NVarChar, length));
            parameter.Direction = ParameterDirection.Output;
            return parameter;
        }

        public SqlParameter AddStructuredParameter(String name, String typeName, DataTable table)
        {
            var parameter = command.Parameters.Add(new(name, SqlDbType.Structured));
            parameter.TypeName = typeName;
            parameter.Value = table;
            return parameter;
        }

        public String ParametersToSqlComments()
        {
            if (command.Parameters.Count == 0) return String.Empty;

            var builder = new StringBuilder();

            foreach (SqlParameter parameter in command.Parameters)
            {
                var sqlType = parameter.SqlDbType.ToString().ToLower();

                String formattedValue;

                if (parameter.Value == DBNull.Value || parameter.Value is null)
                    formattedValue = "null";
                else
                {
                    var value = parameter.Value;

                    switch (parameter.SqlDbType)
                    {
                        case SqlDbType.UniqueIdentifier:
                            formattedValue = $"'{value}'";
                            break;

                        case SqlDbType.NVarChar:
                        case SqlDbType.VarChar:
                        case SqlDbType.Char:
                        case SqlDbType.NChar:
                        case SqlDbType.Text:
                        case SqlDbType.NText:
                            formattedValue = $"'{value.ToString()?.Replace("'", "''")}'";
                            break;

                        case SqlDbType.Date:
                        case SqlDbType.DateTime:
                        case SqlDbType.SmallDateTime:
                        case SqlDbType.DateTime2:
                            formattedValue = $"'{(DateTime)value:yyyy-MM-ddTHH:mm:ss.fff}'";
                            break;

                        case SqlDbType.DateTimeOffset:
                            formattedValue = $"'{(DateTimeOffset)value:yyyy-MM-ddTHH:mm:ss.fffK}'";
                            break;

                        case SqlDbType.Bit:
                            formattedValue = (Boolean)value ? "1" : "0";
                            break;

                        case SqlDbType.Decimal:
                        case SqlDbType.Money:
                        case SqlDbType.SmallMoney:
                            formattedValue = Convert.ToDecimal(value).ToString(System.Globalization.CultureInfo.InvariantCulture);
                            break;

                        case SqlDbType.Float:
                        case SqlDbType.Real:
                            formattedValue = Convert.ToDouble(value).ToString(System.Globalization.CultureInfo.InvariantCulture);
                            break;

                        case SqlDbType.Int:
                        case SqlDbType.SmallInt:
                        case SqlDbType.TinyInt:
                        case SqlDbType.BigInt:
                            formattedValue = Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture) ?? "null";
                            break;

                        case SqlDbType.Binary:
                        case SqlDbType.Image:
                        case SqlDbType.Timestamp:
                        case SqlDbType.VarBinary:
                        case SqlDbType.Variant:
                        case SqlDbType.Xml:
                        case SqlDbType.Udt:
                        case SqlDbType.Structured:
                        case SqlDbType.Time:
                        case SqlDbType.Json:
                        default:
                            formattedValue = "-- UNSUPPORTED DATA TYPE";
                            break;
                    }
                }

                builder.Append("-- declare ")
                    .Append(parameter.ParameterName)
                    .Append(' ')
                    .Append(sqlType);

                if (formattedValue != "null")
                    builder.Append(" = ").Append(formattedValue);

                builder.AppendLine();
            }

            return builder.ToString();
        }
    }

    extension(IEnumerable<Guid?> ids)
    {
        public DataTable ToIdTable()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Id", typeof(Guid));

            foreach (var id in ids)
                dataTable.Rows.Add(id);

            return dataTable;
        }
    }

    extension(IEnumerable<Selectable> selectables)
    {
        public DataTable ToIdTable()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Id", typeof(Guid));

            if (!selectables.HasItems()) return dataTable;

            foreach (var selectable in selectables.Where(x => x.Selected == true))
                dataTable.Rows.Add(selectable.Id);

            return dataTable;
        }
    }

    extension(IEnumerable<IOrderable> orderables)
    {
        public DataTable ToIdTable()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Id", typeof(Guid));
            dataTable.Columns.Add("Ordinal", typeof(Int32));

            foreach (var orderable in orderables)
                dataTable.Rows.Add(orderable.Id, orderable.Ordinal);

            return dataTable;
        }
    }
}