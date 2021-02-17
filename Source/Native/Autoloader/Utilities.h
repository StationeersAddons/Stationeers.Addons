// Stationeers.Addons (c) 2018-2021 Damian 'Erdroy' Korczowski & Contributors

#pragma once

#include <string>

inline int string_ends_with(const wchar_t* string, const wchar_t* postfix)
{
    const auto string_length = wcslen(string);
    const auto end_string_length = wcslen(postfix);
    const auto string_offset = string_length - end_string_length;

    // If the string is smaller than the endString, we will not fit, cancel.
    if (string_length < end_string_length) return false;

    // Offset the string, to possibly match the endString
    const auto* target_string = string + string_offset;

    // Final compare, if the strings are the same, it's a match!
    return wcscmp(target_string, postfix) == 0;
}

inline int string_ends_with(const char* string, const char* postfix)
{
    const auto string_length = strlen(string);
    const auto end_string_length = strlen(postfix);
    const auto string_offset = string_length - end_string_length;

    // If the string is smaller than the endString, we will not fit, cancel.
    if (string_length < end_string_length) return false;

    // Offset the string, to possibly match the endString
    const auto* target_string = string + string_offset;

    // Final compare, if the strings are the same, it's a match!
    return strcmp(target_string, postfix) == 0;
}
