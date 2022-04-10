// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

using System;
using System.Reflection;

namespace Stationeers.Addons.Utilities
{
    /// <summary>
    ///     Utility class for reflection. Contains methods for getting information about types, fields, properties, methods, etc.
    /// </summary>
    internal static class ReflectionHelper
    {
        public static object ReadStaticField(Type classType, string fieldName)
        {
            var field = classType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (field == null)
                throw new Exception($"Field {fieldName} not found in type {classType.Name}");
            return field.GetValue(null);
        }
        
        // TODO: We might want to add more thingies here. But we don't want to expose it to the Addons API.
    }
}