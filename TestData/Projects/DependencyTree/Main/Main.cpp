// Main.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <iostream>

#include "DependencyA.h"


int main()
{
    DependencyA a;
    std::cout << a.WhoAmI() << std::endl;
    return 0;
}

