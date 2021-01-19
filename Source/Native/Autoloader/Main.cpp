// Stationeers.Addons (c) 2018-2021 Damian 'Erdroy' Korczowski & Contributors

#include "Proxy.h"
#include "Config.h"
#include <Windows.h>

#include <thread>

#define MAKE_FUNCTION_DECL(name, returnType, ...)\
    typedef returnType (*name##_t)(__VA_ARGS__);\
    name##_t name

std::thread g_mono_awaiter_thread;

MAKE_FUNCTION_DECL(mono_get_domain, void*,);
MAKE_FUNCTION_DECL(mono_get_root_domain, void*,);

void monoAwaiterWorker()
{
#define BIND_FUNCTION(name) \
    name = reinterpret_cast<name##_t>(GetProcAddress(mono_handle, _STRINGIZE(name)))

    HMODULE mono_handle;

    // Wait for mono to be loaded
    do
    {
        mono_handle = GetModuleHandle(MONO_ASSEMBLY);
        Sleep(250);
    } while (mono_handle == nullptr);

    // Grab mono_domain_get function
    BIND_FUNCTION(mono_get_root_domain);

    // Wait for mono to be initialized
    void* root_domain = nullptr;
    do
    {
        root_domain = mono_get_root_domain();
        Sleep(250);
    } while (root_domain == nullptr);

    // TODO: Signal the main thread
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

        // Skip if we're not running inside game's process (i.e. crash handler is also using our proxy DLL)
        if (!Proxy::IsGameProcess())
            return TRUE;

        g_mono_awaiter_thread = std::thread(monoAwaiterWorker);

        // TODO: Hook some method that will give us some access to main thread and run, when signaled by the awaiter:

        /*  
        assembly = mono_domain_assembly_open(domain, "Stationeers.Addons.dll");
        MonoImage* image = mono_assembly_get_image(assembly);
        MonoClass* getClass = mono_class_from_name(image, "Stationeers.Addons", "Loader");
        MonoMethod* method = mono_class_get_method_from_name(getClass, "Load", 0);
        void* params[1] = { NULL };
        MonoObject* returnVal = mono_runtime_invoke(method, NULL, params, NULL);
         */
        // Thread has exited, now, we can try to run mono calls
        // TODO: Load assembly and execute
    }

    return TRUE;
}
