﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mongo.Migration.Extensions;
using Mongo.Migration.Migrations.Adapters;

namespace Mongo.Migration.Migrations.Locators
{
    internal class TypeMigrationDependencyLocator<TMigrationType> : MigrationLocator<TMigrationType>
        where TMigrationType : class, IMigration
    {
        private readonly IContainerProvider _containerProvider;

        public TypeMigrationDependencyLocator(IContainerProvider containerProvider)
        {
            _containerProvider = containerProvider;
        }

        public override void Locate()
        {
            var migrationTypes =
                (from assembly in Assemblies
                 from type in assembly.GetTypes()
                 where typeof(TMigrationType).IsAssignableFrom(type) && !type.IsAbstract
                 select type).Distinct(new TypeComparer());

            Migrations = migrationTypes.Select(GetMigrationInstance).ToMigrationDictionary();
        }

        private TMigrationType GetMigrationInstance(Type type)
        {
            var constructor = type.GetConstructors()[0];

            var args = constructor
                .GetParameters()
                .Select(o => o.ParameterType)
                .Select(o => _containerProvider.GetInstance(o))
                .ToArray();

            return Activator.CreateInstance(type, args) as TMigrationType;
        }

        private class TypeComparer : IEqualityComparer<Type>
        {
            public bool Equals(Type x, Type y)
            {
                return x.AssemblyQualifiedName == y.AssemblyQualifiedName;
            }

            public int GetHashCode(Type obj)
            {
                return obj.AssemblyQualifiedName.GetHashCode();
            }
        }
    }
}