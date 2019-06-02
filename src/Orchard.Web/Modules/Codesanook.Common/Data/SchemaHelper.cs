using CodeSanook.Common.Modules;
using FluentNHibernate.Mapping;
using Orchard.Data.Migration.Schema;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeSanook.Common.Data
{
    public static class SchemaHelper
    {
        public static SchemaBuilder CreateTable<TModel>(this SchemaBuilder schemaBuilder, Action<CreateTableCommand> table = null) => 
            schemaBuilder.CreateTable(typeof(TModel).Name, table);

        public static SchemaBuilder CreateTable<TParent, TChild>(
            this SchemaBuilder schemaBuilder,
            Action<CreateTableCommand> table = null)
        {
            var associatedTableName = GetAssociatedTableName<TParent, TChild>();
            return schemaBuilder.CreateTable(associatedTableName, table);
        }

        public static SchemaBuilder AlterTable<TModel>(this SchemaBuilder schemaBuilder, Action<AlterTableCommand> table) 
            => schemaBuilder.AlterTable(typeof(TModel).Name, table);

        public static SchemaBuilder AddCompositedPrimaryKey<TParent, TChild, TProperty>(
            this SchemaBuilder schemaBuilder,
            Expression<Func<TParent, TProperty>> parentPropertySelector,
            Expression<Func<TChild, TProperty>> childPropertySelector)
        {
            var tableName = GetActualTableName<TParent, TChild>();
            var parentColumnId = GetColumnName(parentPropertySelector, true);
            var childColumnId = GetColumnName(childPropertySelector, true);
            var keyName = $"PK_{parentColumnId}_{childColumnId}";
            var keyIds = $"{parentColumnId}, {childColumnId}";
            return schemaBuilder.ExecuteSql($"ALTER TABLE {tableName} ADD CONSTRAINT {keyName} PRIMARY KEY ({keyIds})");
        }

        public static CreateTableCommand Column<TModel, TProperty>(
            this CreateTableCommand command,
            Expression<Func<TModel, TProperty>> propertySelector,
            Action<CreateColumnCommand> column = null,
            bool isPrefixWithModelType = false)
        {
            var columnName = GetColumnName(propertySelector, isPrefixWithModelType);
            var propertyType = GetPropertyType<TProperty>();
            var dbType = SchemaUtils.ToDbType(propertyType);
            return command.Column(columnName, dbType, column);
        }

        public static AlterTableCommand AddColumn<TModel, TProperty>(
            this AlterTableCommand command,
            Expression<Func<TModel, TProperty>> propertySelector,
            Action<CreateColumnCommand> column = null,
            bool isPrefixWithModelType = false)
        {
            var columnName = GetColumnName(propertySelector, isPrefixWithModelType);
            var propertyType = GetPropertyType<TProperty>();
            var dbType = SchemaUtils.ToDbType(propertyType);
            command.AddColumn(columnName, dbType, column);
            return command;
        }


        //Need to get underlying type, we cannot send the generic type because it is not supported by DbType Enumeration
        private static Type GetPropertyType<TProperty>()
        {
            var propertyType = typeof(TProperty);
            //return underlying type if given type is nullable
            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return Nullable.GetUnderlyingType(propertyType);
            }
            else
            {
                return propertyType;
            }
        }

        public static ManyToManyPart<TChild> Table<TParent, TChild>(this ManyToManyPart<TChild> manyToManyPart)
        {
            var actualTableName = GetActualTableName<TParent, TChild>();
            return manyToManyPart.Table(actualTableName);
        }

        public static ManyToManyPart<TChild> ParentKeyColumn<TParent, TChild, TProperty>(
            this ManyToManyPart<TChild> manyToManyPart,
            Expression<Func<TParent, TProperty>> propertySelector)
        {
            var parentTableName = typeof(TParent).Name.RemoveRecordName();
            var columnName = GetColumnName(propertySelector);
            return manyToManyPart.ParentKeyColumn($"{parentTableName}_{columnName}");
        }

        public static ManyToManyPart<TChild> ChildKeyColumn<TParent, TChild, TProperty>(
            this ManyToManyPart<TChild> manyToManyPart,
            Expression<Func<TChild, TProperty>> propertySelector)
        {
            var childTableName = typeof(TChild).Name.RemoveRecordName();
            var columnName = GetColumnName(propertySelector);
            return manyToManyPart.ChildKeyColumn($"{childTableName}_{columnName}");
        }

        //no record keyword in column name
        public static string GetColumnName<TModel, TProperty>(
            Expression<Func<TModel, TProperty>> propertySelector,
            bool isPrefixWithModelType = false)
        {
            string path = "";
            var memberExpression = (MemberExpression)propertySelector.Body;
            var memberExpressionOrg = memberExpression;
            while (memberExpression.Expression.NodeType == ExpressionType.MemberAccess)
            {
                var propInfo = memberExpression.Expression.GetType().GetProperty("Member");
                var propValue = propInfo.GetValue(memberExpression.Expression, null) as PropertyInfo;
                path = propValue.Name + "." + path;
                memberExpression = memberExpression.Expression as MemberExpression;
            }

            path = path + memberExpressionOrg.Member.Name;
            if (isPrefixWithModelType)
            {
                var modelTypePrefix = typeof(TModel).Name;
                return $"{modelTypePrefix}_{path}".RemoveRecordName().ChangeToUnderScore();
            }

            return path.RemoveRecordName().ChangeToUnderScore();
        }

        private static string GetAssociatedTableName<TParent, TChild>()
        {
            var parentName = typeof(TParent).Name.RemoveRecordName();
            var childName = typeof(TChild).Name;
            return $"{parentName}{childName}";
        }

        private static string GetActualTableName<TParent, TChild>()
        {
            var tableName = GetAssociatedTableName<TParent, TChild>();
            var tablePrefix = ModuleHelper.GetModuleName<TParent>().ChangeToUnderScore() + "_";
            return $"{tablePrefix}{tableName}";
        }

        public static string GetTableName<TTable>( this SchemaBuilder schemaBuilder)
        {
            return string.Concat(
                ModuleHelper.GetModuleName<TTable>().ChangeToUnderScore(),
                "_",
                typeof(TTable).Name
            );
        }

        private static string RemoveRecordName(this string input) => input.Replace("Record", "");
        private static string ChangeToUnderScore(this string input) => input.Replace(".", "_");
    }
}