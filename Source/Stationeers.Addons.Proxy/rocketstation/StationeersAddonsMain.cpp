#include "PrecompiledHeader.h"
#include <Windows.h>
#include <shellapi.h>

BOOL FileExists(LPCTSTR szPath)
{
    // source: https://stackoverflow.com/questions/3828835/how-can-we-check-if-a-file-exists-or-not-using-win32-program
    DWORD dwAttrib = GetFileAttributes(szPath);
    return (dwAttrib != INVALID_FILE_ATTRIBUTES && !(dwAttrib & FILE_ATTRIBUTE_DIRECTORY));
}

bool check_version()
{
    // TODO: Read version from ./AddonManager/version.json
    // TODO: Check version from raw-file from github
    return true;
}

bool show_new_version_dialog()
{
    const auto result = MessageBox(
        nullptr, L"New version of Stationeers.Addons is available. Do you want to download it now?",
        L"Stationeers.Addons", MB_YESNO);

    if (result == IDYES)
    {
        // Open web browser
        ShellExecute(nullptr, nullptr, L"https://github.com/Erdroy/Stationeers.Addons/releases", nullptr, nullptr, SW_SHOW);
        return false;
    }

    return true;
}

void run_patcher()
{
    if (!FileExists(L"./AddonManager/Stationeers.Addons.Patcher.exe"))
    {
        MessageBox(nullptr,
                   L"Could not find Stationeers.Addons main directory 'AddonManager'."
                    "Subscribed mods that use Addons will not work properly or will not work at all!"
                    "Please reinstall Stationeers.Addon!",
                   L"Stationeers.Addons", MB_OK);

        // Pass it through, we don't want to kill the game.
        return;
    }

    // Spawn hidden patcher console window and run it
    ShellExecute(nullptr, nullptr, L"./AddonManager/Stationeers.Addons.Patcher.exe", nullptr, L"./AddonManager/", SW_HIDE);

    // Sleep for a while
    Sleep(2000);
}

bool stationeers_addons_main()
{
    // TODO: Show Stationeers.Addons splash screen

    // Check current version
    if(!check_version())
    {
        // New version has been detected, ask the user to download it.
        if (!show_new_version_dialog())
        {
            return false;
        }
    }

    // Run patcher now
    run_patcher();

    // We're good.
    return true;
}