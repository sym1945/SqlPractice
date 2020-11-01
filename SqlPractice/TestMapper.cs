using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace SqlPractice
{

    public class TestMapper<T>
        where T : class
    {
        private static Dictionary<Type, Dictionary<string, PropertyInfo>> _EntityColumnCaches = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

        private static Dictionary<string, PropertyInfo> GetColumnInfos(Type entityType)
        {
            lock (_EntityColumnCaches)
            {
                Dictionary<string, PropertyInfo> entityColumnInfos = null;

                if (_EntityColumnCaches.TryGetValue(entityType, out entityColumnInfos) == false)
                {
                    entityColumnInfos = new Dictionary<string, PropertyInfo>();

                    foreach (var property in entityType.GetProperties())
                    {
                        var attr = property.GetCustomAttribute<TableColumnAttribute>();
                        var propertyName = (attr != null) ? attr.Name : property.Name;

                        entityColumnInfos.Add(attr.Name, property);
                    }
                }

                return entityColumnInfos;
            }
        }


        public T MapRow(IDataReader reader)
        {
            var entityType = typeof(T);
            var entityColumnInfos = GetColumnInfos(entityType);

            var count = reader.FieldCount;
            var entity = (T)Activator.CreateInstance(entityType);

            for (int i = 0; i < count; i++)
            {
                var columnName = reader.GetName(i);

                PropertyInfo entityProperty;
                if (entityColumnInfos.TryGetValue(columnName, out entityProperty))
                {
                    entityProperty.SetValue(entity, reader[i]);
                }
            }

            return entity;
        }
    }
}