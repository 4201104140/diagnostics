# Copyright (c) .NET Foundation and contributors. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.

# Verify minimum required version
cmake_minimum_required (VERSION 3.8)

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

set(ROOT_DIR ${CMAKE_CURRENT_SOURCE_DIR})

# Where the version source file for xplat is generated
set(VERSION_FILE_PATH "${CMAKE_BINARY_DIR}/version.cpp")

# Where _version.h for Windows is generated
if (WIN32)
include_directories("${CMAKE_BINARY_DIR}")
endif (WIN32)

set(CORECLR_SET_RPATH ON)
if(CORECLR_SET_RPATH)
  # Enable @rpath support for shared libraries.
  set(MACOSX_RPATH ON)
endif(CORECLR_SET_RPATH)

OPTION(CLR_CMAKE_ENABLE_CODE_COVERAGE "Enable code coverage" OFF)
OPTION(CLR_CMAKE_WARNINGS_ARE_ERRORS "Warnings are errors" ON)


#-----------------------------------------
# Native Projects
#-----------------------------------------
add_subdirectory(src)
