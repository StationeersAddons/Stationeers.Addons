// Stationeers.Addons (c) 2018-2021 Damian 'Erdroy' Korczowski & Contributors

#pragma once

#define MAKE_FUNCTION_DECL(name, returnType, ...)           \
    typedef returnType (*name##_t)(__VA_ARGS__);            \
    static name##_t name

#define BIND_FUNCTION(handle, name)                         \
    name = reinterpret_cast<name##_t>(                      \
        GetProcAddress(handle, _STRINGIZE(name))            \
    )

#define DETOUR_FUNC(name, returnType, ...)                  \
    typedef returnType(*name##_t)(__VA_ARGS__);             \
    name##_t p_##name = NULL;                               \
    returnType Detour##name(__VA_ARGS__)

#define DETOUR_STDFUNC(name, returnType, ...)               \
    typedef returnType(__stdcall *name##_t)(__VA_ARGS__);   \
    name##_t p_##name = NULL;                               \
    returnType __stdcall Detour##name(__VA_ARGS__)

#define HOOK_FUNC(name)                                     \
    p_##name = name;                                        \
    DetourTransactionBegin();                               \
    DetourUpdateThread(GetCurrentThread());                 \
    DetourAttach(&(LPVOID&)p_##name, Detour##name);         \
    DetourTransactionCommit()

#define UNHOOK_FUNC(name)                                   \
    DetourTransactionBegin();                               \
    DetourUpdateThread(GetCurrentThread());                 \
    DetourDetach(&(LPVOID&)p_##name, Detour##name);         \
    DetourTransactionCommit();                              \
    p_##name = NULL
