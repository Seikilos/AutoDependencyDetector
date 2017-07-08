// DependencyA.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "DependencyA.h"

DependencyA::DependencyA()
{

}

std::string DependencyA::WhoAmI()
{
    return "I am Dependency A";
}
