// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

using System;
using System.Linq;
using System.Reflection;

namespace Stationeers.Addons.PluginCompiler
{
    internal static class ReflectionUtilities
    {
        public static MemberInfo GetMethodInfo<TType>(string methodName)
        {
            return GetMethodInfo(typeof(TType), methodName);
        }

        public static MemberInfo GetMethodInfo<TType>(string methodName, Type[] parameters)
        {
            return GetMethodInfo(typeof(TType), methodName, parameters);
        }

        public static MemberInfo GetMethodInfo(Type type, string methodName)
        {
            return type.GetMethod(methodName);
        }

        public static MemberInfo GetMethodInfo(Type type, string methodName, Type[] parameters)
        {
            return type.GetMethod(methodName, parameters);
        }

        public static MemberInfo[] GetMethodInfos<TType>(string methodName)
        {
            return GetMethodInfos(typeof(TType), methodName);
        }

        public static MemberInfo[] GetMethodInfos(Type type, string methodName)
        {
            return type.GetMethods().Where(x => x.Name == methodName).Select(x => (MemberInfo)x).ToArray();
        }

        public static MemberInfo GetPropertyInfo<TType>(string propertyName)
        {
            return GetPropertyInfo(typeof(TType), propertyName);
        }

        public static MemberInfo GetPropertyInfo(Type type, string propertyName)
        {
            return type.GetProperty(propertyName);
        }
    }
}