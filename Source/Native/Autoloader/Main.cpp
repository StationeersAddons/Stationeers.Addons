// Stationeers.Addons (c) 2018-2021 Damian 'Erdroy' Korczowski & Contributors

#include "Proxy.h"
#include "Config.h"
#include <Windows.h>
#include <thread>
#include <detours.h>

#include "HelperMarcros.h"
#include "Utilities.h"

struct MonoDomain;
struct MonoImage;
struct MonoClass;
struct MonoAssembly;
struct MonoMethod;
struct MonoObject;

// Declare needed Mono calls
MAKE_FUNCTION_DECL(mono_get_domain, MonoDomain*,);
MAKE_FUNCTION_DECL(mono_jit_init_version, void*, const char*, const char*);
MAKE_FUNCTION_DECL(mono_get_root_domain, MonoDomain*,);
MAKE_FUNCTION_DECL(mono_domain_assembly_open, MonoAssembly*, MonoDomain*, const char*);
MAKE_FUNCTION_DECL(mono_runtime_invoke, MonoObject*, MonoMethod*, void*, void**, MonoObject**);
MAKE_FUNCTION_DECL(mono_method_get_name, const char*, const void*);

MAKE_FUNCTION_DECL(mono_assembly_get_image, MonoImage*, MonoAssembly*);
MAKE_FUNCTION_DECL(mono_class_from_name, MonoClass*, MonoImage*, const char*, const char*);
MAKE_FUNCTION_DECL(mono_class_get_method_from_name, MonoMethod*, MonoClass*, const char*, int);

DETOUR_FUNC(mono_runtime_invoke, MonoObject*, MonoMethod* method, void* obj, void** params, MonoObject** exc)
{
    const char* methodName = mono_method_get_name(method);

    if (string_ends_with(methodName, "Start")) {
        MonoDomain* domain = mono_get_root_domain();
        MonoAssembly* assembly = mono_domain_assembly_open(domain, "AddonManager/Stationeers.Addons.dll"); // TODO: Clean this up
        MonoImage* image = mono_assembly_get_image(assembly);
        MonoClass* getClass = mono_class_from_name(image, "Stationeers.Addons", "Loader");
        MonoMethod* loaderMethod = mono_class_get_method_from_name(getClass, "Load", 0);
        p_mono_runtime_invoke(loaderMethod, obj, params, exc); // If I set 'obj' to NULL, it fails. WTF?

        // Call the original method and unhook this detour
        MonoObject* result = p_mono_runtime_invoke(method, obj, params, exc);
        UNHOOK_FUNC(mono_runtime_invoke);
        return result;
    }

    // Call the original function
    return p_mono_runtime_invoke(method, obj, params, exc);
}

DETOUR_STDFUNC(LoadLibraryW, HMODULE, LPCWSTR lpLibFileName)
{
    HMODULE library = p_LoadLibraryW(lpLibFileName);

    if(wcsstr(lpLibFileName, MONO_ASSEMBLY))
    {
        // Bind required mono calls
        BIND_FUNCTION(library, mono_runtime_invoke);
        BIND_FUNCTION(library, mono_method_get_name);
        BIND_FUNCTION(library, mono_domain_assembly_open);
        BIND_FUNCTION(library, mono_get_root_domain);
        BIND_FUNCTION(library, mono_jit_init_version);
        BIND_FUNCTION(library, mono_get_domain);
        BIND_FUNCTION(library, mono_assembly_get_image);
        BIND_FUNCTION(library, mono_class_from_name);
        BIND_FUNCTION(library, mono_class_get_method_from_name);

        // Hook mono_runtime_invoke and check if it is Awake or Start or OnEnable call and finally load our Addons assembly and execute!
        // Unhook everything when not needed anymore
        HOOK_FUNC(mono_runtime_invoke);
        UNHOOK_FUNC(LoadLibraryW);
    }

    return library;
}


BOOL WINAPI DllMain(
    HINSTANCE /*hInstance*/,
    DWORD fdwReason,
    LPVOID /*lpReserved*/)
{
    // Check for process attachment
    if (fdwReason == DLL_PROCESS_ATTACH)
    {
        // We have to initialize anyway,
        // otherwise we will crash the host process if we skip this.
        Proxy::Initialize();

        // Skip if we're not running inside the game's or server's process
        // (crash handler is also trying to use our proxy DLL, so we have to say NO to it).
        if (!Proxy::IsGameProcess() || !Proxy::IsServerProcess())
            return TRUE;

        DetourRestoreAfterWith(); // ?

        // Hook LoadLibraryW
        HOOK_FUNC(LoadLibraryW);
    }

    return TRUE;
}
