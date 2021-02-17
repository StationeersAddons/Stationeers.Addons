// Stationeers.Addons (c) 2018-2021 Damian 'Erdroy' Korczowski & Contributors

#pragma once

#ifndef _WIN64
// kek
# error "This DLL wrapper only works on Windows!"
#endif

class Proxy
{
public:
    static bool IsGameProcess();
    static void Initialize();
};