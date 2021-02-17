// Stationeers.Addons (c) 2018-2021 Damian 'Erdroy' Korczowski & Contributors

#include "Proxy.h"

#include "Config.h"
#include "Utilities.h"

#include <Windows.h>

#define INIT_FUNCTION(name) p##name = reinterpret_cast<F##name>(GetProcAddress(module, #name))
#define DECL_FUNCTION(name, returnType, ...) typedef returnType(WINAPI* F##name)(__VA_ARGS__); F##name p##name
//#define DEF_FUNCTION(name, returnType, ...)\
//returnType WINAPI name(__VA_ARGS__) {\
//    return (p##name)(???);\  /*I have no idea how to strip types or do something similar.*/
//}

DECL_FUNCTION(VerFindFileA, DWORD, DWORD, LPCSTR, LPCSTR, LPCSTR, LPSTR, PUINT, LPSTR, PUINT);
DECL_FUNCTION(VerFindFileW, DWORD, DWORD, LPCWSTR, LPCWSTR, LPCWSTR, LPWSTR, PUINT, LPWSTR, PUINT);
DECL_FUNCTION(VerInstallFileA, DWORD, DWORD, LPCSTR, LPCSTR, LPCSTR, LPCSTR, LPCSTR, LPSTR, PUINT);
DECL_FUNCTION(VerInstallFileW, DWORD, DWORD, LPCWSTR, LPCWSTR, LPCWSTR, LPCWSTR, LPCWSTR, LPWSTR, PUINT);
DECL_FUNCTION(GetFileVersionInfoSizeA, DWORD, LPCSTR, LPDWORD);
DECL_FUNCTION(GetFileVersionInfoSizeW, DWORD, LPCWSTR, LPDWORD);
DECL_FUNCTION(GetFileVersionInfoA, BOOL, LPCSTR, DWORD, DWORD, LPVOID);
DECL_FUNCTION(GetFileVersionInfoW, BOOL, LPCWSTR, DWORD, DWORD, LPVOID);
DECL_FUNCTION(GetFileVersionInfoSizeExA, DWORD, DWORD, LPCSTR, LPDWORD);
DECL_FUNCTION(GetFileVersionInfoSizeExW, DWORD, DWORD, LPCWSTR, LPDWORD);
DECL_FUNCTION(GetFileVersionInfoExA, BOOL, DWORD, LPCSTR, DWORD, DWORD, LPVOID);
DECL_FUNCTION(GetFileVersionInfoExW, BOOL, DWORD, LPCWSTR, DWORD, DWORD, LPVOID);
DECL_FUNCTION(VerLanguageNameA, DWORD, DWORD, LPSTR, DWORD);
DECL_FUNCTION(VerLanguageNameW, DWORD, DWORD, LPWSTR, DWORD);
DECL_FUNCTION(VerQueryValueW, BOOL, LPCVOID, LPCWSTR, LPVOID*, PUINT);
DECL_FUNCTION(VerQueryValueA, BOOL, LPCVOID, LPCSTR, LPVOID*, PUINT);
DECL_FUNCTION(GetFileVersionInfoByHandle, BOOL, LPCSTR, DWORD, DWORD, LPVOID);

DWORD WINAPI VerFindFileA(DWORD uFlags, LPCSTR szFileName, LPCSTR szWinDir, LPCSTR szAppDir, LPSTR szCurDir, PUINT puCurDirLen, LPSTR szDestDir, PUINT puDestDirLen)
{
    return (pVerFindFileA)(uFlags, szFileName, szWinDir, szAppDir, szCurDir, puCurDirLen, szDestDir, puDestDirLen);
}

DWORD WINAPI VerFindFileW(DWORD uFlags, LPCWSTR szFileName, LPCWSTR szWinDir, LPCWSTR szAppDir, LPWSTR szCurDir, PUINT puCurDirLen, LPWSTR szDestDir, PUINT puDestDirLen)
{
    return (pVerFindFileW)(uFlags, szFileName, szWinDir, szAppDir, szCurDir, puCurDirLen, szDestDir, puDestDirLen);
}

DWORD WINAPI VerInstallFileA(DWORD uFlags, LPCSTR szSrcFileName, LPCSTR szDestFileName, LPCSTR szSrcDir, LPCSTR szDestDir, LPCSTR szCurDir, LPSTR szTmpFile, PUINT puTmpFileLen)
{
    return (pVerInstallFileA)(uFlags, szSrcFileName, szDestFileName, szSrcDir, szDestDir, szCurDir, szTmpFile, puTmpFileLen);
}

DWORD WINAPI VerInstallFileW(DWORD uFlags, LPCWSTR szSrcFileName, LPCWSTR szDestFileName, LPCWSTR szSrcDir, LPCWSTR szDestDir, LPCWSTR szCurDir, LPWSTR szTmpFile, PUINT puTmpFileLen)
{
    return (pVerInstallFileW)(uFlags, szSrcFileName, szDestFileName, szSrcDir, szDestDir, szCurDir, szTmpFile, puTmpFileLen);
}

DWORD WINAPI GetFileVersionInfoSizeA(LPCSTR lptstrFilename, LPDWORD lpdwHandle)
{
    return (pGetFileVersionInfoSizeA)(lptstrFilename, lpdwHandle);
}

DWORD WINAPI GetFileVersionInfoSizeW(LPCWSTR lptstrFilename, LPDWORD lpdwHandle)
{
    return (pGetFileVersionInfoSizeW)(lptstrFilename, lpdwHandle);
}

BOOL WINAPI GetFileVersionInfoA(LPCSTR lptstrFilename, DWORD dwHandle, DWORD dwLen, LPVOID lpData)
{
    return (pGetFileVersionInfoA)(lptstrFilename, dwHandle, dwLen, lpData);
}

BOOL WINAPI GetFileVersionInfoW(LPCWSTR lptstrFilename, DWORD dwHandle, DWORD dwLen, LPVOID lpData)
{
    return (pGetFileVersionInfoW)(lptstrFilename, dwHandle, dwLen, lpData);
}

DWORD WINAPI GetFileVersionInfoSizeExA(DWORD dwFlags, LPCSTR lpwstrFilename, LPDWORD lpdwHandle)
{
    return (pGetFileVersionInfoSizeExA)(dwFlags, lpwstrFilename, lpdwHandle);
}

DWORD WINAPI GetFileVersionInfoSizeExW(DWORD dwFlags, LPCWSTR lpwstrFilename, LPDWORD lpdwHandle)
{
    return (pGetFileVersionInfoSizeExW)(dwFlags, lpwstrFilename, lpdwHandle);
}

BOOL WINAPI GetFileVersionInfoExA(DWORD dwFlags, LPCSTR lpwstrFilename, DWORD dwHandle, DWORD dwLen, LPVOID lpData)
{
    return (pGetFileVersionInfoExA)(dwFlags, lpwstrFilename, dwHandle, dwLen, lpData);
}

BOOL WINAPI GetFileVersionInfoExW(DWORD dwFlags, LPCWSTR lpwstrFilename, DWORD dwHandle, DWORD dwLen, LPVOID lpData)
{
    return (pGetFileVersionInfoExW)(dwFlags, lpwstrFilename, dwHandle, dwLen, lpData);
}

DWORD WINAPI VerLanguageNameA(DWORD wLang, LPSTR szLang, DWORD cchLang)
{
    return (pVerLanguageNameA)(wLang, szLang, cchLang);
}

DWORD WINAPI VerLanguageNameW(DWORD wLang, LPWSTR szLang, DWORD cchLang)
{
    return (pVerLanguageNameW)(wLang, szLang, cchLang);
}

BOOL WINAPI VerQueryValueA(LPCVOID pBlock, LPCSTR lpSubBlock, LPVOID* lplpBuffer, PUINT puLen)
{
    return (pVerQueryValueA)(pBlock, lpSubBlock, lplpBuffer, puLen);
}

BOOL WINAPI VerQueryValueW(LPCVOID pBlock, LPCWSTR lpSubBlock, LPVOID* lplpBuffer, PUINT puLen)
{
    return (pVerQueryValueW)(pBlock, lpSubBlock, lplpBuffer, puLen);
}

BOOL WINAPI GetFileVersionInfoByHandle(LPCSTR lptstrFilename, DWORD dwHandle, DWORD dwLen, LPVOID lpData)
{
    return (pGetFileVersionInfoByHandle)(lptstrFilename, dwHandle, dwLen, lpData);
}

bool Proxy::IsGameProcess()
{
    TCHAR szFileName[MAX_PATH];
    GetModuleFileName(nullptr, szFileName, MAX_PATH);

    if (string_ends_with(szFileName, TARGET_GAME_NAME))
        return true;

    return false;
}

void Proxy::Initialize()
{
    auto* module = LoadLibrary(L"C:/Windows/system32/version.dll");

    if (module == nullptr)
    {
        MessageBox(
            nullptr,
            L"Stationeers.Addons",
            L"Failed to load 'version.dll'! Please to report this error to Erdroy#0001 or Stationeers.Addons server on Discord.",
            MB_OK
        );
        return;
    }

    INIT_FUNCTION(VerFindFileA);
    INIT_FUNCTION(VerFindFileW);
    INIT_FUNCTION(VerFindFileW);
    INIT_FUNCTION(VerInstallFileA);
    INIT_FUNCTION(VerInstallFileW);
    INIT_FUNCTION(GetFileVersionInfoA);
    INIT_FUNCTION(GetFileVersionInfoW);
    INIT_FUNCTION(GetFileVersionInfoSizeA);
    INIT_FUNCTION(GetFileVersionInfoSizeW);
    INIT_FUNCTION(GetFileVersionInfoExA);
    INIT_FUNCTION(GetFileVersionInfoExW);
    INIT_FUNCTION(GetFileVersionInfoSizeExA);
    INIT_FUNCTION(GetFileVersionInfoSizeExW);
    INIT_FUNCTION(VerLanguageNameA);
    INIT_FUNCTION(VerLanguageNameW);
    INIT_FUNCTION(VerQueryValueA);
    INIT_FUNCTION(VerQueryValueW);
    INIT_FUNCTION(GetFileVersionInfoByHandle);
}