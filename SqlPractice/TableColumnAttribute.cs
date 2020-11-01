using System;

namespace SqlPractice
{
    public class TableColumnAttribute : Attribute
    {
        public string Name { get; }

        public TableColumnAttribute(string name)
        {
            Name = name;
        }
    }
}