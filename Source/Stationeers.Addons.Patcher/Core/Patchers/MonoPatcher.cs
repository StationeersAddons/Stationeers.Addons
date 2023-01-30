// Stationeers.Addons (c) 2018-2022 Damian 'Erdroy' Korczowski & Contributors

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using FieldAttributes = Mono.Cecil.FieldAttributes;
using MethodAttributes = Mono.Cecil.MethodAttributes;
using MethodBody = Mono.Cecil.Cil.MethodBody;

namespace Stationeers.Addons.Patcher.Core.Patchers
{
    /// <summary>
    ///     MonoPatcher - for games which run on mono.
    /// </summary>
    public class MonoPatcher : IGamePatcher
    {
        public const string AssemblyDir = "Managed";
        public const string AssemblyName = "Assembly-CSharp.dll";

        private const string TargetType = "Assets.Scripts.GameManager"; // BUG: Not called on server builds, we have to find better place for this - some bootstrap class or something.
        private const string TargetFunction = "OnEnable";

        private const string Signature = "StationeersModLoader";

        private AssemblyDefinition _assembly;
        private ModuleDefinition _module;
        private TypeDefinition _type;
        private string _installResourcesDir;

        public string AssemblyFileName;
        public string TemporaryAssemblyFileName;

        /// <inheritdoc />
        public void Load(string instanceExe)
        {
            GetInstanceAssemblies(instanceExe);

            if (!File.Exists(AssemblyFileName))
                Logger.Current.LogFatal($"Could not find game/server assembly '{AssemblyFileName}'.");

            // Copy the assembly into temporary file
            File.Copy(AssemblyFileName, TemporaryAssemblyFileName, true);

            Logger.Current.Log($"Copied game/server assembly into temporary file '{TemporaryAssemblyFileName}'");

            // Read the original, temporary assembly
            _assembly = AssemblyDefinition.ReadAssembly(TemporaryAssemblyFileName);

            if (_assembly == null)
            {
                Logger.Current.LogFatal($"Could not read game/server assembly '{TemporaryAssemblyFileName}'.");
                return;
            }

            // Select main module
            _module = _assembly.MainModule;

            if (_module == null)
            {
                Logger.Current.LogFatal($"Could not read game assembly (MainModule not found) '{TemporaryAssemblyFileName}'.");
                return;
            }

            Logger.Current.Log($"Found module: {_module.FileName}");

            // Find target to inject into
            _type = _module.Types.FirstOrDefault(x => x.FullName == TargetType);

            if (_type == null)
                Logger.Current.LogFatal($"Could not find target type '{TargetType}'. " +
                                        "Please make sure that you have the latest version of Stationeers.ModLoader!");
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // Dispose the assembly if loaded
            _assembly?.Dispose();

            // Delete the temporary assembly file
            if (File.Exists(TemporaryAssemblyFileName))
            {
                Logger.Current.Log($"Deleting temporary assembly file '{TemporaryAssemblyFileName}'");
                File.Delete(TemporaryAssemblyFileName);
            }
        }

        /// <inheritdoc />
        public void Patch()
        {
            // Check if game is already patched
            if (IsPatched())
            {
                Logger.Current.Log("Game is already patched.");
                Console.ReadLine();
                return;
            }

            // Backup the assembly first
            Backup();

            // Copy dlls to the Managed folder, required for linux to be auto loaded
            CopyRequiredAssemblies();

            // We are clear here, go ahead and patch the game
            Inject();
        }

        /// <inheritdoc />
        public bool IsPatched()
        {
            // Check if the target type has our signature
            var signature = _type.Fields.FirstOrDefault(x => x.Name == Signature);

            if (signature != null)
                return true; // Patched

            return false; // Not patched
        }

        /// <inheritdoc />
        private void GetInstanceAssemblies(string installInstance)
        {
            // This is kind of verbose, might need to be rewritten in a more concise manner
            if (installInstance == Constants.GameExe)
            {
                _installResourcesDir = Constants.GameResourcesDir;
            }
            else if (installInstance == Constants.ServerExe)
            {
                _installResourcesDir = Constants.ServerResourcesDir;
            }
            System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(_installResourcesDir), "Invalid install dir!");

            AssemblyFileName = Path.Combine(Environment.CurrentDirectory, _installResourcesDir, AssemblyDir, AssemblyName);
            TemporaryAssemblyFileName = Path.Combine(Environment.CurrentDirectory, _installResourcesDir, AssemblyDir, AssemblyName + ".temp.dll");
        }

        private void Backup()
        {
            // Backup assembly file, overwrite if already exists
            File.Copy(AssemblyFileName, AssemblyFileName + ".backup", true);

            if (!File.Exists(AssemblyFileName + ".original"))
                File.Copy(AssemblyFileName, AssemblyFileName + ".original", false);

            // BUG: This may be an issue, when 'IsPatched' fails.
            // But steam file integrity will fix when we fucc something up here (or the .original file).
            // So... not a big problem right now.
        }

        private void Inject()
        {
            Logger.Current.Log("Injecting...");

            var voidType = _module.ImportReference(typeof(void));

            Logger.Current.Log("Creating method definition");
            var method = new MethodDefinition(TargetFunction, MethodAttributes.Private, voidType);

            // Setup new method body
            method.Body = new MethodBody(method);
            method.Body.Instructions.Clear();

            var methodBody = method.Body;

            // Get ILProcessor for current method
            var processor = methodBody.GetILProcessor();

            // Create method body
            CreateLoaderMethodBody(ref processor, ref method);

            // Add this method
            _type.Methods.Add(method);

            // Add signature
            Logger.Current.Log("Creating loader signature");
            _type.Fields.Add(new FieldDefinition(Signature, FieldAttributes.Private,
                _module.ImportReference(typeof(int))));

            var cd = Environment.CurrentDirectory;
            Environment.CurrentDirectory = Path.GetDirectoryName(AssemblyFileName);

            // Write modified assembly
            Logger.Current.Log("Saving modified assembly");
            _assembly.Write(AssemblyName); // AssemblyName because we've changed the current directory

            Environment.CurrentDirectory = cd;

            Logger.Current.Log("Successfully patched!");
        }

        private void CreateLoaderMethodBody(ref ILProcessor processor, ref MethodDefinition method)
        {
            Logger.Current.Log("Creating loader method body");

            // Get all needed core-lib methods
            var loadFile = _module.ImportReference(typeof(Assembly).GetMethod("LoadFile", new[]
            {
                typeof(string)
            }));

            var getType = _module.ImportReference(typeof(Assembly).GetMethod("GetType", new[]
            {
                typeof(string)
            }));

            var createInstance = _module.ImportReference(typeof(Activator).GetMethod("CreateInstance", new[]
            {
                typeof(Type)
            }));

            // Get Environment.CurrentDirectory in the runtime
            var curDir = _module.ImportReference(typeof(Environment).GetMethod("get_CurrentDirectory"));
            // Path combine for joining the assembly full path
            var pathCombine = _module.ImportReference(typeof(Path).GetMethod("Combine", new[]
            {
                typeof(string),
                typeof(string)
            }));

            var invokeMember = _module.ImportReference(typeof(Type).GetMethod("InvokeMember", new[]
            {
                typeof(string),
                typeof(BindingFlags),
                typeof(Binder),
                typeof(object),
                typeof(object[])
            }));

            // And here the actual patching begins...

            // Add needed local variables
            method.Body.Variables.Add(new VariableDefinition(_module.ImportReference(typeof(Type))));
            method.Body.Variables.Add(new VariableDefinition(_module.ImportReference(typeof(object))));

            // Create instructions
            processor.Append(processor.Create(OpCodes.Nop));
            processor.Append(processor.Create(OpCodes.Call, curDir));                               // load current dir
            processor.Append(processor.Create(OpCodes.Ldstr, Constants.LoaderAssemblyFileName));    // load assembly path
            processor.Append(processor.Create(OpCodes.Call, pathCombine));                          // call Path.Combine
            processor.Append(processor.Create(OpCodes.Call, loadFile));                             // call Assembly.LoadFile(resultingPath)

            processor.Append(processor.Create(OpCodes.Ldstr, Constants.LoaderTypeName));
            processor.Append(processor.Create(OpCodes.Callvirt, getType));
            processor.Append(processor.Create(OpCodes.Stloc_0));
            processor.Append(processor.Create(OpCodes.Ldloc_0));

            processor.Append(processor.Create(OpCodes.Call, createInstance));
            processor.Append(processor.Create(OpCodes.Stloc_1));
            processor.Append(processor.Create(OpCodes.Ldloc_0));

            processor.Append(processor.Create(OpCodes.Ldstr, Constants.LoaderFunctionName));
            processor.Append(processor.Create(OpCodes.Ldc_I4, (int)BindingFlags.InvokeMethod));

            processor.Append(processor.Create(OpCodes.Ldnull));
            processor.Append(processor.Create(OpCodes.Ldloc_1));
            processor.Append(processor.Create(OpCodes.Ldnull));

            processor.Append(processor.Create(OpCodes.Callvirt, invokeMember));

            processor.Append(processor.Create(OpCodes.Pop));
            processor.Append(processor.Create(OpCodes.Ret));
        }

        private void CopyRequiredAssemblies()
        {
            // This was taken from Stationeers.Addons csproj
            // Perhaps it could be "cecilled" in a PreBuild Script?
            // Copies required runtime assemblies to the Managed folder which seems to load them
            // (without doing this, you get filenotfoundexceptions in the game runtime console for 0harmony, etc)
            var assemblies = new[]
            {
                "0Harmony",
                "Microsoft.CodeAnalysis",
                "Microsoft.CodeAnalysis.CSharp",
                "System.Buffers",
                "System.Collections.Immutable",
                "System.Memory",
                "System.Numerics.Vectors",
                "System.Reflection.Metadata",
                "System.Runtime.CompilerServices.Unsafe",
                "System.Text.Encoding.CodePages",
                "System.Threading.Tasks.Extensions"
            };
            foreach (var assemblyFile in assemblies.Select(i => i + ".dll"))
            {
                var from = Path.Combine(Environment.CurrentDirectory, assemblyFile);
                var to = Path.Combine(Environment.CurrentDirectory, _installResourcesDir, AssemblyDir, assemblyFile);
                File.Copy(from, to, true);
            }
        }
    }
}
