#pragma once

#ifdef DEPENDENCYA_EXPORTS
#   define DEPENDENCYA_API __declspec(dllexport)
#else
#   define DEPENDENCYA_API __declspec(dllimport)
#   pragma comment(lib, "DependencyA.lib")
#endif

#include <string>


class DEPENDENCYA_API DependencyA {
public:
    DependencyA();

    std::string WhoAmI();

};
