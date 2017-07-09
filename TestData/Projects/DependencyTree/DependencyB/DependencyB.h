#pragma once

#ifdef DependencyB_EXPORTS
#   define DependencyB_API __declspec(dllexport)
#else
#   define DependencyB_API __declspec(dllimport)
#   pragma comment(lib, "DependencyB.lib")
#endif

#include <string>


class DependencyB_API DependencyB {
public:
    DependencyB();

    std::string WhoAmI();

};
