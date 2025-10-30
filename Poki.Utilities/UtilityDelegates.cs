// ReSharper disable UnusedType.Global
namespace Poki.Utilities;

// https://github.com/media-tools/core
//
// The MIT License (MIT)
// 
// Copyright (c) 2015 media-tools
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

// async Func
public delegate Task<TR> AsyncFunc<TR> ();
public delegate Task<TR> AsyncFunc<in T1, TR> (T1 a);
public delegate Task<TR> AsyncFunc<in T1, in T2, TR> (T1 a, T2 b);
public delegate Task<TR> AsyncFunc<in T1, in T2, in T3, TR> (T1 a, T2 b, T3 c);
public delegate Task<TR> AsyncFunc<in T1, in T2, in T3, in T4, TR> (T1 a, T2 b, T3 c, T4 d);

// async Action
public delegate Task AsyncAction ();
public delegate Task AsyncAction<in T1> (T1 a);
public delegate Task AsyncAction<in T1, in T2> (T1 a, T2 b);
public delegate Task AsyncAction<in T1, in T2, in T3> (T1 a, T2 b, T3 c);
public delegate Task AsyncAction<in T1, in T2, in T3, in T4> (T1 a, T2 b, T3 c, T4 d);

// params Func
public delegate TR ParamsFunc<in TP, out TR> (params TP[] array);
public delegate TR ParamsFunc<in T1, in TP, out TR> (T1 a, params TP[] array);
public delegate TR ParamsFunc<in T1, in T2, in TP, out TR> (T1 a, T2 b, params TP[] array);
public delegate TR ParamsFunc<in T1, in T2, in T3, in TP, out TR> (T1 a, T2 b, T3 c, params TP[] array);
public delegate TR ParamsFunc<in T1, in T2, in T3, in T4, in TP, out TR> (T1 a, T2 b, T3 c, T4 d, params TP[] array);

// params Action
public delegate void ParamsAction<in TP> (params TP[] array);
public delegate void ParamsAction<in T1, in TP> (T1 a, params TP[] array);
public delegate void ParamsAction<in T1, in T2, in TP> (T1 a, T2 b, params TP[] array);
public delegate void ParamsAction<in T1, in T2, in T3, in TP> (T1 a, T2 b, T3 c, params TP[] array);
public delegate void ParamsAction<in T1, in T2, in T3, in T4, in TP> (T1 a, T2 b, T3 c, T4 d, params TP[] array);
