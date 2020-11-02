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

                        entityColumnInfos.Add(propertyName, property);
                    }

                    _EntityColumnCaches.Add(entityType, entityColumnInfos);
                }

                return entityColumnInfos;
            }
        }

        private Type _EntityType;

        private List<string> _ReaderCache;

        private bool _IsInitialReader = false;

        private Dictionary<string, PropertyInfo> _ColInfos;


        public TestMapper()
        {
            _EntityType = typeof(T);
            _ColInfos = GetColumnInfos(_EntityType);
        }


        public T MapRow(IDataReader reader)
        {
            var entityColumnInfos = _ColInfos;
            var entity = (T)Activator.CreateInstance(_EntityType);

            if (_IsInitialReader == false)
            {
                var count = reader.FieldCount;
                _ReaderCache = new List<string>();

                for (int i = 0; i < count; i++)
                {
                    var columnName = reader.GetName(i);

                    PropertyInfo entityProperty;
                    if (entityColumnInfos.TryGetValue(columnName, out entityProperty))
                    {
                        entityProperty.SetValue(entity, reader[i]);
                        _ReaderCache.Add(columnName);
                    }
                }

                _IsInitialReader = true;
            }
            else
            {
                var count = _ReaderCache.Count;

                for (int i = 0; i < count; i++)
                {
                    var columnName = _ReaderCache[i];
                    entityColumnInfos[columnName].SetValue(entity, reader[i]);
                }
            }


            return entity;
        }

    }
}