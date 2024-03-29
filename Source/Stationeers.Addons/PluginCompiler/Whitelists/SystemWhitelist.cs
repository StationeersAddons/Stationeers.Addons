// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

// Note: We do not use using keyword here, to keep things clean

// TODO: Make a list of allowed types etc.

namespace Stationeers.Addons.PluginCompiler.Whitelists
{
    internal sealed class SystemWhitelist : IWhitelistRegistry
    {
        public void Register(PluginWhitelist whitelist)
        {
            whitelist.WhitelistTypes(
                typeof(object),
                typeof(string),
                typeof(bool),
                typeof(char),
                typeof(byte),
                typeof(sbyte),
                typeof(short),
                typeof(ushort),
                typeof(int),
                typeof(uint),
                typeof(long),
                typeof(ulong),
                typeof(float),
                typeof(double),
                typeof(decimal),
                
                // These IO types are safe.
                typeof(System.IO.Stream),
                typeof(System.IO.TextWriter),
                typeof(System.IO.TextReader),
                typeof(System.IO.BinaryReader),
                typeof(System.IO.BinaryWriter),
                typeof(System.IO.StreamReader),
                typeof(System.IO.StringReader),
                typeof(System.IO.StringWriter),
                typeof(System.IO.MemoryStream), // Should be safe

                // System.* types
                typeof(System.DateTime),
                typeof(System.TimeSpan),
                typeof(System.Array),
                typeof(System.IDisposable),
                typeof(System.StringComparison),
                typeof(System.Math),
                typeof(System.Enum),
                typeof(System.Nullable<>),
                typeof(System.IEquatable<>),
                typeof(System.IComparable),
                typeof(System.IComparable<>),
                typeof(System.BitConverter),
                typeof(System.FlagsAttribute),
                typeof(System.Random),
                typeof(System.Convert),
                typeof(System.Guid),
                typeof(System.SerializableAttribute),
                
                typeof(System.Globalization.CultureInfo),
                typeof(System.ComponentModel.DefaultValueAttribute),
                
                // AssemblyInfo.cs types
                typeof(System.Reflection.AssemblyTitleAttribute),
                typeof(System.Reflection.AssemblyDescriptionAttribute),
                typeof(System.Reflection.AssemblyConfigurationAttribute),
                typeof(System.Reflection.AssemblyCompanyAttribute),
                typeof(System.Reflection.AssemblyProductAttribute),
                typeof(System.Reflection.AssemblyCopyrightAttribute),
                typeof(System.Reflection.AssemblyTrademarkAttribute),
                typeof(System.Reflection.AssemblyCultureAttribute),
                typeof(System.Runtime.InteropServices.ComVisibleAttribute), // Not sure about these two...
                typeof(System.Runtime.InteropServices.GuidAttribute),
                typeof(System.Reflection.AssemblyVersionAttribute),
                typeof(System.Reflection.AssemblyFileVersionAttribute),

                // System.* Exceptions
                typeof(System.Exception),
                typeof(System.DivideByZeroException),
                typeof(System.NotImplementedException),
                typeof(System.InvalidCastException),
                typeof(System.ArgumentException),
                typeof(System.ArgumentNullException),
                typeof(System.NullReferenceException),
                typeof(System.InvalidOperationException),
                typeof(System.NotSupportedException)
            );
            
            whitelist.WhitelistMembers(
                ReflectionUtilities.GetMethodInfo<System.ValueType>("ToString"),
                ReflectionUtilities.GetMethodInfo<System.ValueType>("GetHashCode"),
                ReflectionUtilities.GetMethodInfo<System.ValueType>("Equals", new[] { typeof(System.Type) }),
                ReflectionUtilities.GetMethodInfo<System.ValueType>("Equals", new[] { typeof(System.Object) }),
                
                ReflectionUtilities.GetMethodInfo<System.Type>("GetHashCode"),
                ReflectionUtilities.GetMethodInfo<System.Type>("Equals", new[] { typeof(System.Type) }),
                ReflectionUtilities.GetMethodInfo<System.Type>("Equals", new[] { typeof(System.Object) }),
                ReflectionUtilities.GetMethodInfo<System.Type>("op_Equality"),
                ReflectionUtilities.GetMethodInfo<System.Type>("GetFields", new[] { typeof(System.Reflection.BindingFlags) }),
                
                ReflectionUtilities.GetPropertyInfo<System.Type>("FullName"),
                ReflectionUtilities.GetPropertyInfo(typeof(System.Environment), "NewLine")
                
            );
            
            whitelist.WhitelistMembers(ReflectionUtilities.GetMethodInfos<System.Reflection.FieldInfo>("SetValue")); // This might not be safe :/
            whitelist.WhitelistMembers(ReflectionUtilities.GetMethodInfos<System.Reflection.FieldInfo>("GetValue"));
            whitelist.WhitelistMembers(ReflectionUtilities.GetMethodInfos<System.Type>("ToString"));
            whitelist.WhitelistMembers(ReflectionUtilities.GetMethodInfos<System.Type>("GetType"));
            //whitelist.WhitelistMembers(ReflectionUtilities.GetMethodInfos(typeof(System.IO.Path), "Combine")); // Just for testing, not really needed
            
            whitelist.WhitelistTypesNamespaces(
                // We no more allow the System.* namespace, but instead define all the types that are allowed above
                typeof(System.Collections.IEnumerator),
                typeof(System.Collections.Generic.List<>),
                typeof(System.Collections.Concurrent.Partitioner<>),
                typeof(System.Collections.Immutable.ImmutableArray),
                typeof(System.Text.StringBuilder),
                typeof(System.Text.RegularExpressions.Match),
                typeof(System.Timers.Timer),
                typeof(System.Linq.Enumerable),
                typeof(System.Xml.XmlDocument)
            );
        }
    }
}