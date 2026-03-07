#pragma once

#include <cstdint>

class lua_fix64
{
public:
    int64_t value;
    
    lua_fix64() : value(0) {}
    lua_fix64(int v) : value(v << 32) {}
    lua_fix64(int64_t v) : value(v << 32) {}
    lua_fix64(double v) : value(static_cast<int64_t>(v * 65536.0)) {}
    lua_Number to_float() const {
        return static_cast<lua_Number>(value) / 65536.0;
    }

    bool operator==(const lua_fix64& other) const
    {
        return value == other.value;
    }

    bool operator==(const int64_t other) const
    {
        // TODO
    }

    bool operator==(const int other) const
    {
        // TODO
    }

    bool operator==(const double other) const
    {
        // TODO
    }

    lua_fix64 operator+(const lua_fix64 other) const
    {
        
    }

    lua_fix64 operator-(const lua_fix64 other) const
    {
        
    }

    lua_fix64 operator*(const lua_fix64 other) const
    {
        
    }

    lua_fix64 operator/(const lua_fix64 other) const
    {
        
    }

    lua_fix64 operator%(const lua_fix64 other) const
    {
        
    }

    static lua_fix64 pow(lua_fix64 a, lua_fix64 b)
    {
        
    }

    static lua_fix64 floor(lua_fix64 value)
    {
    }
};
