using System;
using System.Data;
using System.Linq;
using System.Reflection;

namespace SqlPractice
{
    
    public class TestMapper<T>
        where T: class
    {
        public T MapRow(IDataReader reader)
        {
            var entityType = typeof(T);
            var entityProperties = entityType.GetProperties();

            var count = reader.FieldCount;
            var entity = (T)Activator.CreateInstance(entityType);

            for (int i = 0; i < count; i++)
            {
                var columnName = reader.GetName(i);
                var entityProperty = entityProperties.FirstOrDefault(d =>
                {
                    var attr = d.GetCustomAttribute<TableColumnAttribute>();
                    if (attr != null)
                    {
                        if (attr.Name == columnName)
                            return true;
                    }

                    return d.Name == columnName;
                });
                if (entityProperty != null)
                {
                    entityProperty.SetValue(entity, reader[i]);
                }
            }

            return entity;
        }
    }
}