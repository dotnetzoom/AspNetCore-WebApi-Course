using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Common.Utilities
{
    public static class ReflectionHelper
    {
        public static bool HasAttribute<T>(this MemberInfo type, bool inherit = false) where T : Attribute
        {
            return HasAttribute(type, typeof(T), inherit);
        }

        public static bool HasAttribute(this MemberInfo type, Type attribute, bool inherit = false)
        {
            return Attribute.IsDefined(type, attribute, inherit);
            //return type.IsDefined(attribute, inherit);
            //return type.GetCustomAttributes(attribute, inherit).Length > 0;
        }

        public static bool IsInheritFrom<T>(this Type type)
        {
            return IsInheritFrom(type, typeof(T));
        }

        public static bool IsInheritFrom(this Type type, Type parentType)
        {
            //the 'is' keyword do this too for values (new ChildClass() is ParentClass)
            return parentType.IsAssignableFrom(type);
        }

        public static bool BaseTypeIsGeneric(this Type type, Type genericType)
        {
            return type.BaseType?.IsGenericType == true && type.BaseType.GetGenericTypeDefinition() == genericType;
        }

        public static IEnumerable<Type> GetTypesAssignableFrom<T>(params Assembly[] assemblies)
        {
            return typeof(T).GetTypesAssignableFrom(assemblies);
        }

        public static IEnumerable<Type> GetTypesAssignableFrom(this Type type, params Assembly[] assemblies)
        {
            return assemblies.SelectMany(p => p.GetTypes()).Where(p => p.IsInheritFrom(type));
        }

        public static IEnumerable<Type> GetTypesHasAttribute<T>(params Assembly[] assemblies) where T : Attribute
        {
            return typeof(T).GetTypesHasAttribute(assemblies);
        }

        public static IEnumerable<Type> GetTypesHasAttribute(this Type type, params Assembly[] assemblies)
        {
            return assemblies.SelectMany(p => p.GetTypes()).Where(p => p.HasAttribute(type));
        }

        public static bool IsEnumerable(this Type type)
        {
            return type != typeof(string) && type.IsInheritFrom<IEnumerable>();
        }

        public static bool IsEnumerable<T>(this Type type)
        {
            return type != typeof(string) && type.IsInheritFrom<IEnumerable<T>>() && type.IsGenericType;
        }

        public static IEnumerable<Type> GetBaseTypesAndInterfaces(this Type type)
        {
            if ((type == null) || (type.BaseType == null))
                yield break;

            foreach (var i in type.GetInterfaces())
                yield return i;

            var currentBaseType = type.BaseType;
            while (currentBaseType != null)
            {
                yield return currentBaseType;
                currentBaseType = currentBaseType.BaseType;
            }
        }

        public static bool IsCustomType(this Type type)
        {
            //return type.Assembly.GetName().Name != "mscorlib";
            return type.IsCustomValueType() || type.IsCustomReferenceType();
        }

        public static bool IsCustomValueType(this Type type)
        {
            return type.IsValueType && !type.IsPrimitive && type.Namespace != null && !type.Namespace.StartsWith("System", StringComparison.Ordinal);
        }

        public static bool IsCustomReferenceType(this Type type)
        {
            return !type.IsValueType && !type.IsPrimitive && type.Namespace != null && !type.Namespace.StartsWith("System", StringComparison.Ordinal);
        }
    }
}
