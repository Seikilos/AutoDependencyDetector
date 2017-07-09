// DependencyA.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "DependencyA.h"
#include "DependencyB.h"

DependencyA::DependencyA()
{

}

std::string DependencyA::WhoAmI()
{
    DependencyB b;
    return "I am Dependency A "+ b.WhoAmI();
}
