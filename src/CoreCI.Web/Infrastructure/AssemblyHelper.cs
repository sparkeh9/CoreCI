namespace CoreCI.Web.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.DependencyModel;

    public static class AssemblyHelper
    {
        public static Assembly GetAssembly( string nameStartsWith )
        {
            return GetAssemblies( nameStartsWith ).FirstOrDefault();
        }

        public static Assembly[] GetAssemblies( string nameStartsWith )
        {
            return DependencyContext.Default
                                    .RuntimeLibraries
                                    .Where( x => x.Name.StartsWith( nameStartsWith ) )
                                    .Select( x => Assembly.Load( new AssemblyName( x.Name ) ) )
                                    .ToArray();
        }

        public static IEnumerable<Type> GetGenericInheritors( string namespaceStartsWith, Type desiredBaseType )
        {
            var allTypes = GetAssemblies( namespaceStartsWith ).SelectMany( x => x.GetTypes() );

            return from type in allTypes
                   let typeInfo = type.GetTypeInfo()
                   let baseType = typeInfo.BaseType
                   where baseType != null && baseType.IsConstructedGenericType &&
                         baseType.GetGenericTypeDefinition() == desiredBaseType
                   select type;
        }
    }
}