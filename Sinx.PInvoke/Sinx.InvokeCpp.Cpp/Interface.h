#pragma once
#if _MSC_VER    // TRUE for Microsoft compiler.
#define DllExport  __declspec(dllexport)
#else
#define DllExport
#endif


