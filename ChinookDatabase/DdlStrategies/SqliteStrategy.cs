﻿using Microsoft.Data.Entity.Design.DatabaseGeneration;
using System;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

namespace ChinookDatabase.DdlStrategies
{
    public class SqliteStrategy : AbstractDdlStrategy
    {
        public SqliteStrategy()
        {
            Name = "Sqlite";
            DatabaseFileExtension = "sqlite";
            Identity = "PRIMARY KEY AUTOINCREMENT";
            ForeignKeyDef = KeyDefinition.OnCreateTableBottom;
            CommandLineFormat = "if exist {0}ite del {0}ite\nsqlite3 -init {0} {0}ite";
        }

        public override string FormatStringValue(string value)
        {
            return string.Format("'{0}'", value.Replace("'", "''"));
        }

        public override string FormatDateValue(string value)
        {
            var date = Convert.ToDateTime(value);
            return string.Format("'{0}'", date.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        public override string GetFullyQualifiedName(string schema, string name)
        {
            return FormatName(name);
        }

        public override string GetStoreType(EdmProperty property)
        {
            if (property.TypeUsage.EdmType.Name == "int")
                return "INTEGER";

            return base.GetStoreType(property);
        }

        public override string WriteCreateColumn(DataColumn column)
        {
            var notnull = (column.AllowDBNull ? "" : "NOT NULL");
            var identity = IsIdentityEnabled ? Identity : String.Empty;
            return string.Format("{0} {1} {2} {3}",
                                 FormatName(column.ColumnName),
                                 GetStoreType(column),
                                 identity, notnull).Trim();
        }

        public override string WriteDropTable(string tableName)
        {
            return string.Format("DROP TABLE IF EXISTS {0};", FormatName(tableName));
        }

        public override string WriteDropForeignKey(string tableName, string columnName)
        {
            return string.Empty;
        }
    }
}
