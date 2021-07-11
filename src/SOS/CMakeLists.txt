if(CLR_CMAKE_PLATFORM_UNIX)
    include_directories(${ROOT_DIR}/src/pal/inc)
    include_directories(${ROOT_DIR}/src/pal/inc/rt)

    add_subdirectory(lldbplugin)
endif(CLR_CMAKE_PLATFORM_UNIX)

# lldbplugin doesn't build with these options
if(WIN32)
  message(STATUS "CMAKE_VS_WINDOWS_TARGET_PLATFORM_VERSION: ${CMAKE_VS_WINDOWS_TARGET_PLATFORM_VERSION}")

endif(WIN32)