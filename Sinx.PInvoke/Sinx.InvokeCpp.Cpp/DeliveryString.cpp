// ReSharper disable CppClangTidyClangDiagnosticFormatNonliteral
// ReSharper disable CppClangTidyClangDiagnosticDanglingGsl
#include <cstdlib>
#include <cstring>

#include "Interface.h"
#include <memory>
#include "fmt/format.h"
using namespace std;


extern "C" {

DllExport const char* ToCpp(const char* str0, char* str1)
{
    const auto str = fmt::format("{}, {}", &str0[0], &str1[1]).c_str();
    constexpr auto size = sizeof(str);
    const auto newStr = new char[strlen(str) + 1];
    memcpy(newStr, str, size);
    return newStr;
}
    
// 整型是可以直接复制到本机结构中的类型 - blittable
DllExport int DeliveryInt(int i)
{
    return i;
}

DllExport const char* DeliveryString(const char* msg)
{
    const auto str = "abc";
    // 这里是4不是3, 因为有'/0'结尾
    constexpr auto size = sizeof(str);
    const auto ptr = malloc(size);
    memcpy(ptr, str, size);
    return static_cast<const char*>(ptr);
}

DllExport const char* GetStringNew()
{
    const auto str = "abc";
    // 这里是4不是3, 因为有'/0'结尾
    const auto size = strlen(str) + 1;
    const auto ptr = new char[size];
    strcpy_s(ptr, size, str);
    return ptr;
}
// 需要区分array和非array吧????
DllExport void Delete(const char* ptr)
{
    if (nullptr != ptr)
    {
        delete ptr;
        ptr = nullptr;
    }
}


DllExport void ChangeString(const char* in, char* out, int size)
{
    if (nullptr != in)
    {
        strcpy_s(out, size, in);
    }
}

DllExport void Free(void* ptr)
{
    if (nullptr != ptr)
    {
        free(ptr);
        ptr = nullptr;
    }
}

[[noreturn]]
DllExport void ThrowException()
{
    throw "i am a exception";
}
}
