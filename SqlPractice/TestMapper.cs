using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace SqlPractice
{

    public class TestMapper<T>
        where T : class
    {
        private static Dictionary<Type, Dictionary<string, PropertyAccessor>> _EntityColumnCaches = new Dictionary<Type, Dictionary<string, PropertyAccessor>>();

        private static Dictionary<string, PropertyAccessor> GetColumnInfos(Type entityType)
        {
            lock (_EntityColumnCaches)
            {
                Dictionary<string, PropertyAccessor> entityColumnInfos = null;

                if (_EntityColumnCaches.TryGetValue(entityType, out entityColumnInfos) == false)
                {
                    entityColumnInfos = new Dictionary<string, PropertyAccessor>();

                    foreach (var property in entityType.GetProperties())
                    {
                        var attr = property.GetCustomAttribute<TableColumnAttribute>();
                        var propertyName = (attr != null) ? attr.Name : property.Name;

                        entityColumnInfos.Add(propertyName, new PropertyAccessor(entityType, property.Name));
                    }

                    _EntityColumnCaches.Add(entityType, entityColumnInfos);
                }

                return entityColumnInfos;
            }
        }

        private Type _EntityType;

        private List<string> _ReaderCache;

        private bool _IsInitialReader = false;

        private Dictionary<string, PropertyAccessor> _ColInfos;


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

                    PropertyAccessor entityProperty;
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