using System.Net;

namespace Crudspa.Framework.Core.Server.Extensions;

public static class SqlDataReaderEx
{
    extension(SqlDataReader reader)
    {
        public Boolean? ReadBoolean(String fieldName)
        {
            var ordinal = reader.GetOrdinalOrThrow(fieldName);
            return reader.ReadBoolean(ordinal);
        }

        public Boolean? ReadBoolean(Int32 ordinal)
        {
            return reader.ReadNullable(ordinal, static (r, o) => r.GetBoolean(o));
        }

        public Byte? ReadByte(String fieldName)
        {
            var ordinal = reader.GetOrdinalOrThrow(fieldName);
            return reader.ReadByte(ordinal);
        }

        public Byte? ReadByte(Int32 ordinal)
        {
            return reader.ReadNullable(ordinal, static (r, o) => r.GetByte(o));
        }

        public Byte[]? ReadByteArray(String fieldName)
        {
            var ordinal = reader.GetOrdinalOrThrow(fieldName);
            return reader.ReadByteArray(ordinal);
        }

        public Byte[]? ReadByteArray(Int32 ordinal)
        {
            return reader.ReadRef(ordinal, static (r, o) => r.GetFieldValue<Byte[]>(o));
        }

        public DateOnly? ReadDateOnly(String fieldName)
        {
            var ordinal = reader.GetOrdinalOrThrow(fieldName);
            return reader.ReadDateOnly(ordinal);
        }

        public DateOnly? ReadDateOnly(Int32 ordinal)
        {
            try
            {
                return reader.IsDBNull(ordinal) ? null : reader.GetDateTime(ordinal).ToDateOnly();
            }
            catch (InvalidCastException ex)
            {
                throw new InvalidCastException($"Error reading DateOnly. {reader.GetFieldInfo(ordinal)}", ex);
            }
        }

        public DateTime? ReadDateTime(String fieldName)
        {
            var ordinal = reader.GetOrdinalOrThrow(fieldName);
            return reader.ReadDateTime(ordinal);
        }

        public DateTime? ReadDateTime(Int32 ordinal)
        {
            return reader.ReadNullable(ordinal, static (r, o) => r.GetDateTime(o));
        }

        public DateTimeOffset? ReadDateTimeOffset(String fieldName)
        {
            var ordinal = reader.GetOrdinalOrThrow(fieldName);
            return reader.ReadDateTimeOffset(ordinal);
        }

        public DateTimeOffset? ReadDateTimeOffset(Int32 ordinal)
        {
            return reader.ReadNullable(ordinal, static (r, o) => r.GetDateTimeOffset(o));
        }

        public Decimal? ReadDecimal(String fieldName)
        {
            var ordinal = reader.GetOrdinalOrThrow(fieldName);
            return reader.ReadDecimal(ordinal);
        }

        public Decimal? ReadDecimal(Int32 ordinal)
        {
            return reader.ReadNullable(ordinal, static (r, o) => r.GetDecimal(o));
        }

        public Double? ReadDouble(String fieldName)
        {
            var ordinal = reader.GetOrdinalOrThrow(fieldName);
            return reader.ReadDouble(ordinal);
        }

        public Double? ReadDouble(Int32 ordinal)
        {
            return reader.ReadNullable(ordinal, static (r, o) => r.GetDouble(o));
        }

        public Guid? ReadGuid(String fieldName)
        {
            var ordinal = reader.GetOrdinalOrThrow(fieldName);
            return reader.ReadGuid(ordinal);
        }

        public Guid? ReadGuid(Int32 ordinal)
        {
            return reader.ReadNullable(ordinal, static (r, o) => r.GetGuid(o));
        }

        public Int16? ReadInt16(String fieldName)
        {
            var ordinal = reader.GetOrdinalOrThrow(fieldName);
            return reader.ReadInt16(ordinal);
        }

        public Int16? ReadInt16(Int32 ordinal)
        {
            return reader.ReadNullable(ordinal, static (r, o) => r.GetInt16(o));
        }

        public Int32? ReadInt32(String fieldName)
        {
            var ordinal = reader.GetOrdinalOrThrow(fieldName);
            return reader.ReadInt32(ordinal);
        }

        public Int32? ReadInt32(Int32 ordinal)
        {
            return reader.ReadNullable(ordinal, static (r, o) => r.GetInt32(o));
        }

        public Int64? ReadInt64(String fieldName)
        {
            var ordinal = reader.GetOrdinalOrThrow(fieldName);
            return reader.ReadInt64(ordinal);
        }

        public Int64? ReadInt64(Int32 ordinal)
        {
            return reader.ReadNullable(ordinal, static (r, o) => r.GetInt64(o));
        }

        public TEnum ReadEnum<TEnum>(String fieldName)
            where TEnum : struct, Enum
        {
            var ordinal = reader.GetOrdinalOrThrow(fieldName);
            return reader.ReadEnum<TEnum>(ordinal);
        }

        public TEnum ReadEnum<TEnum>(Int32 ordinal)
            where TEnum : struct, Enum
        {
            try
            {
                if (reader.IsDBNull(ordinal))
                    return default;

                var value = reader.GetInt32(ordinal);
                return (TEnum)Enum.ToObject(typeof(TEnum), value);
            }
            catch (Exception ex)
            {
                throw new($"Error reading Enum of type '{typeof(TEnum).FullName}'. {reader.GetFieldInfo(ordinal)}", ex);
            }
        }

        public Single? ReadSingle(String fieldName)
        {
            var ordinal = reader.GetOrdinalOrThrow(fieldName);
            return reader.ReadSingle(ordinal);
        }

        public Single? ReadSingle(Int32 ordinal)
        {
            return reader.ReadNullable(ordinal, static (r, o) => r.GetFloat(o));
        }

        public String? ReadString(String fieldName)
        {
            var ordinal = reader.GetOrdinalOrThrow(fieldName);
            return reader.ReadString(ordinal);
        }

        public String? ReadString(Int32 ordinal)
        {
            return reader.ReadRef(ordinal, static (r, o) => r.GetString(o));
        }

        public TimeOnly? ReadTimeOnly(String fieldName)
        {
            var ordinal = reader.GetOrdinalOrThrow(fieldName);
            return reader.ReadTimeOnly(ordinal);
        }

        public TimeOnly? ReadTimeOnly(Int32 ordinal)
        {
            try
            {
                return reader.IsDBNull(ordinal) ? null : reader.GetTimeSpan(ordinal).ToTimeOnly();
            }
            catch (InvalidCastException ex)
            {
                throw new InvalidCastException($"Error reading TimeOnly. {reader.GetFieldInfo(ordinal)}", ex);
            }
        }

        public TimeSpan? ReadTimeSpan(String fieldName)
        {
            var ordinal = reader.GetOrdinalOrThrow(fieldName);
            return reader.ReadTimeSpan(ordinal);
        }

        public TimeSpan? ReadTimeSpan(Int32 ordinal)
        {
            return reader.ReadNullable(ordinal, static (r, o) => r.GetTimeSpan(o));
        }

        public String? ReadIpAddress(String fieldName)
        {
            var ordinal = reader.GetOrdinalOrThrow(fieldName);
            return reader.ReadIpAddress(ordinal);
        }

        public String? ReadIpAddress(Int32 ordinal)
        {
            try
            {
                return reader.IsDBNull(ordinal) ? null : new IPAddress(reader.GetFieldValue<Byte[]>(ordinal)).ToString();
            }
            catch (InvalidCastException ex)
            {
                throw new InvalidCastException($"Error reading IPAddress. {reader.GetFieldInfo(ordinal)}", ex);
            }
        }

        public Uri? ReadUri(String fieldName)
        {
            var ordinal = reader.GetOrdinalOrThrow(fieldName);
            return reader.ReadUri(ordinal);
        }

        public Uri? ReadUri(Int32 ordinal)
        {
            try
            {
                return reader.IsDBNull(ordinal) ? null : new Uri(reader.GetString(ordinal));
            }
            catch (InvalidCastException ex)
            {
                throw new InvalidCastException($"Error reading Uri. {reader.GetFieldInfo(ordinal)}", ex);
            }
        }

        private T? ReadNullable<T>(Int32 ordinal, Func<SqlDataReader, Int32, T> getter)
            where T : struct
        {
            try
            {
                return reader.IsDBNull(ordinal) ? null : getter(reader, ordinal);
            }
            catch (InvalidCastException ex)
            {
                throw new InvalidCastException($"Error reading {typeof(T).Name}. {reader.GetFieldInfo(ordinal)}", ex);
            }
        }

        private T? ReadRef<T>(Int32 ordinal, Func<SqlDataReader, Int32, T> getter)
            where T : class
        {
            try
            {
                return reader.IsDBNull(ordinal) ? null : getter(reader, ordinal);
            }
            catch (InvalidCastException ex)
            {
                throw new InvalidCastException($"Error reading {typeof(T).Name}. {reader.GetFieldInfo(ordinal)}", ex);
            }
        }

        public String GetFieldInfo(Int32 ordinal)
        {
            try
            {
                return $"The field at ordinal '{ordinal}' is named '{reader.GetName(ordinal)}' and is of type '{reader.GetDataTypeName(ordinal)}'.";
            }
            catch (Exception ex)
            {
                return $"Error fetching field info for ordinal '{ordinal}'. {ex.Message}";
            }
        }

        public async Task<IList<Guid>> ReadGuids()
        {
            var list = new List<Guid>();

            while (await reader.ReadAsync())
            {
                var value = reader.ReadGuid(0);

                if (value.HasValue)
                    list.Add(value.Value);
            }

            return list;
        }

        public async Task<IList<Named>> ReadNameds()
        {
            var list = new List<Named>();

            while (await reader.ReadAsync())
                list.Add(reader.ReadNamed());

            return list;
        }

        public async Task<IList<NamedCssClass>> ReadNamedCssClasses()
        {
            var list = new List<NamedCssClass>();

            while (await reader.ReadAsync())
                list.Add(reader.ReadNamedCssClass());

            return list;
        }

        public async Task<IList<Linked>> ReadLinkeds()
        {
            var list = new List<Linked>();

            while (await reader.ReadAsync())
                list.Add(reader.ReadLinked());

            return list;
        }

        public Named ReadNamed()
        {
            return new()
            {
                Id = reader.ReadGuid(0),
                Name = reader.ReadString(1),
            };
        }

        public NamedCssClass ReadNamedCssClass()
        {
            return new()
            {
                Id = reader.ReadGuid(0),
                Name = reader.ReadString(1),
                CssClass = reader.ReadString(2),
            };
        }

        public OrderableCssClass ReadOrderableCssClass()
        {
            return new()
            {
                Id = reader.ReadGuid(0),
                Name = reader.ReadString(1),
                Ordinal = reader.ReadInt32(2),
                CssClass = reader.ReadString(3),
            };
        }

        public Selectable ReadSelectable()
        {
            return new()
            {
                RootId = reader.ReadGuid(0),
                Id = reader.ReadGuid(1),
                Name = reader.ReadString(2),
                Selected = reader.ReadBoolean(3),
            };
        }

        public Linked ReadLinked()
        {
            return new()
            {
                Id = reader.ReadGuid(0),
                Name = reader.ReadString(1),
                Url = reader.ReadString(2),
            };
        }

        private Int32 GetOrdinalOrThrow(String fieldName)
        {
            try
            {
                return reader.GetOrdinal(fieldName);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new($"Field '{fieldName}' not found by data reader.", ex);
            }
        }
    }
}