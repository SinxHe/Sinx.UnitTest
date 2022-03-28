#include "../Sinx.InvokeCpp.Cpp/DeliveryString.cpp"  // NOLINT(bugprone-suspicious-include)
int main(int argc, char* argv[])
{
    const auto a = new char[4]{'a', 'b', 'c'};
    ToCpp("123", a);
    return 0;
}
