# Copyright (c) .NET Foundation and contributors. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.

# Verify minimum required version
cmake_minimum_required(VERSION 2.8.12)

if(CMAKE_VERSION VERSION_EQUAL 3.0 OR CMAKE_VERSION VERSION_GREATER 3.0)
  cmake_policy(SET CMP0042 NEW)
endif()

# Set the project name
project(diagnostics)

# Include cmake functions
include(functions.cmake)

if (WIN32)
  message(STATUS "VS_PLATFORM_TOOLSET is ${CMAKE_VS_PLATFORM_TOOLSET}")
  message(STATUS "VS_PLATFORM_NAME is ${CMAKE_VS_PLATFORM_NAME}")
endif (WIN32)

# Where the version source file for xplat is generated
set(VERSION_FILE_PATH "${CMAKE_BINARY_DIR}/version.cpp")

# Ensure other tools are present
if (WIN32)
    if(CLR_CMAKE_HOST_ARCH STREQUAL arm)

      # Confirm that Windows SDK is present
      if(NOT DEFINED CMAKE_VS_WINDOWS_TARGET_PLATFORM_VERSION OR CMAKE_VS_WINDOWS_TARGET_PLATFORM_VERSION STREQUAL "" )
          message(FATAL_ERROR "Windows SDK is required for the Arm32 build.")
      else()
          message("Using Windows SDK version ${CMAKE_VS_WINDOWS_TARGET_PLATFORM_VERSION}")
      endif()
    else()
      enable_language(ASM_MASM)
    endif()

    # Ensure that MC is present
    find_program(MC mc)
    if (MC STREQUAL "MC-NOTFOUND")
        message(FATAL_ERROR "MC not found")
    endif()

    if (CLR_CMAKE_HOST_ARCH STREQUAL arm64)
      # CMAKE_CXX_COMPILER will default to the compiler installed with
      # Visual studio. Overwrite it to the compiler on the path.
      # TODO, remove when cmake generator supports Arm64 as a target.
      find_program(PATH_CXX_COMPILER cl)
      set(CMAKE_CXX_COMPILER ${PATH_CXX_COMPILER})
      message("Overwriting the CMAKE_CXX_COMPILER.")
      message(CMAKE_CXX_COMPILER found:${CMAKE_CXX_COMPILER})
    endif()
endif(WIN32)

#----------------------------------------
# Detect and set platform variable names
#     - for non-windows build platform & architecture is detected using inbuilt CMAKE variables and cross target component configure
#     - for windows we use the passed in parameter to CMAKE to determine build arch
#----------------------------------------
if(CMAKE_SYSTEM_NAME STREQUAL Linux)
endif(CMAKE_SYSTEM_NAME STREQUAL Linux)

#--------------------------------------------
# This repo builds two set of binaries
# 1. binaries which execute on target arch machine
#        - for such binaries host architecture & target architecture are same
#        - eg. coreclr.dll
# 2. binaries which execute on host machine but target another architecture
#        - host architecture is different from target architecture
#        - eg. crossgen.exe - runs on x64 machine and generates nis targeting arm64
#        - for complete list of such binaries refer to file crosscomponents.cmake
#-------------------------------------------------------------
# Set HOST architecture variables
if(CLR_CMAKE_PLATFORM_UNIX_ARM)

elseif(WIN32)
    message(CLR_CMAKE_HOST_ARCH)
    message("heheheheheh")
    # CLR_CMAKE_HOST_ARCH is passed in as param to cmake
    if (CLR_CMAKE_HOST_ARCH STREQUAL x64)
        set(CLR_CMAKE_PLATFORM_ARCH_AMD64 1)
        clr_unknown_arch()
    endif()
endif()