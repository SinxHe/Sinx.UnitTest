#include <iostream>
#include "SinxClass.cpp"
using namespace std;

int main()
{
    auto sinx = new SinxClass();
    sinx = nullptr;
	try
	{
		cout << sinx->IntValue;
	}
	catch (const std::exception&)
	{
		cout << "exception";
	}
}
