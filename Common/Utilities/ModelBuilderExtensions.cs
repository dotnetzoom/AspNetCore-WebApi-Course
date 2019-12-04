using System;
using System.Linq;
using Pluralize.NET;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Common.Utilities
{
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// Singularizin table name like Posts to Post or People to Person
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void AddSingularizingTableNameConvention(this ModelBuilder modelBuilder)
        {
            var pluralizer = new Pluralizer();
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                entityType.SetTableName(pluralizer.Singularize(tableName));
            }
        }

        /// <summary>
        /// Pluralizing table name like Post to Posts or Person to People
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void AddPluralizingTableNameConvention(this ModelBuilder modelBuilder)
        {
            var pluralizer = new Pluralizer();
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                entityType.SetTableName(pluralizer.Singularize(tableName));
            }
        }

        /// <summary>
        /// Set NEWSEQUENTIALID() sql function for all columns named "Id"
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void AddSequentialGuidForIdConvention(this ModelBuilder modelBuilder)
        {
            modelBuilder.AddDefaultValueSqlConvention("Id", typeof(Guid), "NEWSEQUENTIALID()");
        }

        /// <summary>
        /// Set DefaultValueSql for sepecific property name and type
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="propertyName">Name of property wants to set DefaultValueSql for</param>
        /// <param name="propertyType">Type of property wants to set DefaultValueSql for </param>
        /// <param name="defaultValueSql">DefaultValueSql like "NEWSEQUENTIALID()"</param>
        public static void AddDefaultValueSqlConvention(this ModelBuilder modelBuilder, string propertyName, Type propertyType, string defaultValueSql)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var property = entityType.GetProperties().SingleOrDefault(p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
                if (property != null && property.ClrType == propertyType)
                    property.SetDefaultValueSql(defaultValueSql);
            }
        }

        /// <summary>
        /// Set DeleteBehavior.Restrict by default for relations
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void AddRestrictDeleteBehaviorConvention(this ModelBuilder modelBuilder)
        {
            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);
            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;
        }

        /// <summary>
        /// Dynamicaly load all IEntityTypeConfiguration with Reflection
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="assemblies">Assemblies contains Entities</param>
        public static void RegisterEntityTypeConfiguration(this ModelBuilder modelBuilder, params Assembly[] assemblies)
        {
            var applyGenericMethod = typeof(ModelBuilder).GetMethods().First(m => m.Name == nameof(ModelBuilder.ApplyConfiguration));

            var types = assemblies.SelectMany(a => a.GetExportedTypes())
                .Where(c => c.IsClass && !c.IsAbstract && c.IsPublic);

            foreach (var type in types)
            {
                foreach (var iface in type.GetInterfaces())
                {
                    if (iface.IsConstructedGenericType && iface.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
                    {
                        var applyConcreteMethod = applyGenericMethod.MakeGenericMethod(iface.GenericTypeArguments[0]);
                        applyConcreteMethod.Invoke(modelBuilder, new[] { Activator.CreateInstance(type) });
                    }
                }
            }
        }

        /// <summary>
        /// Dynamicaly register all Entities that inherit from specific BaseType
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="assemblies">Assemblies contains Entities</param>
        public static void RegisterAllEntities<BaseType>(this ModelBuilder modelBuilder, params Assembly[] assemblies)
        {
            var types = assemblies.SelectMany(a => a.GetExportedTypes())
                .Where(c => c.IsClass && !c.IsAbstract && c.IsPublic && typeof(BaseType).IsAssignableFrom(c));

            foreach (var type in types)
                modelBuilder.Entity(type);
        }
    }
}
