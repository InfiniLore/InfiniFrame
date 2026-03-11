#pragma once

#include <string>

// AutoString: platform-specific string pointer type
#ifdef _WIN32
typedef wchar_t* AutoString;
typedef const wchar_t* AutoStringConst;
using NativeString = std::wstring;
#else
typedef char* AutoString;
typedef const char* AutoStringConst;
using NativeString = std::string;
#endif
