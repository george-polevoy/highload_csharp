using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace zero_alloc.benchmark
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Ws4 : IEquatable<Ws4>
    {
        [FieldOffset(0)] public char c0;
        [FieldOffset(2)] public char c1;
        [FieldOffset(4)] public char c2;
        [FieldOffset(6)] public char c3;

        [FieldOffset(0)] public uint ui32_0;
        [FieldOffset(4)] public uint ui32_1;
        
        [FieldOffset(0)] public ulong ui64_0;

        public bool Equals(Ws4 other)
        {
            return ui64_0 == other.ui64_0;
        }

        public override bool Equals(object obj)
        {
            return obj is Ws4 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return ui64_0.GetHashCode();
        }

        public static bool operator ==(Ws4 left, Ws4 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Ws4 left, Ws4 right)
        {
            return !left.Equals(right);
        }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct Ws8 : IEquatable<Ws8>
    {
        public Ws4 s0;
        public Ws4 s1;

        public bool Equals(Ws8 other)
        {
            return s0.Equals(other.s0) && s1.Equals(other.s1);
        }

        public override bool Equals(object obj)
        {
            return obj is Ws8 other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (s0.GetHashCode() * 397) ^ s1.GetHashCode();
            }
        }

        public static bool operator ==(Ws8 left, Ws8 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Ws8 left, Ws8 right)
        {
            return !left.Equals(right);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int AvxLength()
        {
            fixed (char* p = &s0.c0)
            {
                var a = Sse2.LoadVector128((short*) p);
                var cmp = Sse2.CompareGreaterThan(a, Vector128<short>.Zero);
                var mask = Sse2.MoveMask(cmp.AsByte());
                return (int)Popcnt.PopCount((uint) mask) / 2;
            }
        }

        
        struct Value
        {
        }
        
        void DontCallObjectEqualsEverAgain()
        {
            new Value().Equals(new Value());
        }
    }
    
    
    
    
    [StructLayout(LayoutKind.Sequential)]
    public struct Ws16 : IEquatable<Ws16>
    {
        public Ws8 s0;
        public Ws8 s1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<char> Span()
        {
            return MemoryMarshal.CreateSpan(ref s0.s0.c0, 16);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<char> ReadOnlySpan()
        {
            return MemoryMarshal.CreateReadOnlySpan(ref s0.s0.c0, 16);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Length()
        {
            return ((ReadOnlySpan<char>)this).NullTerminatedLength();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CustomLength()
        {
            if (s0.s1.ui64_0 == 0)
            {
                return ReadOnlySpan().NullTerminatedLength();
            }
            
            if (s1.s0.ui64_0 == 0)
            {
                return 4 + ReadOnlySpan().Slice(4).NullTerminatedLength();
            }
            
            if (s1.s1.ui64_0 == 0)
            {
                return 8 + ReadOnlySpan().Slice(8).NullTerminatedLength();
            }
            
            return 12 + ReadOnlySpan().Slice(12).NullTerminatedLength();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int AvxLength()
        {
            return s0.AvxLength() + s1.AvxLength();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int AvxLength2()
        {
            fixed (char* p = &s1.s0.c0)
            {
                var a1 = Sse2.LoadVector128((short*) p);
                var cmp1 = Sse2.CompareGreaterThan(a1, Vector128<short>.Zero);
                var mask1 = Sse2.MoveMask(cmp1.AsByte());
                var r1 = Popcnt.PopCount((uint) mask1);
                if (r1 == 0)
                {
                    var a0 = Sse2.LoadVector128((short*) p);
                    var cmp0 = Sse2.CompareGreaterThan(a0, Vector128<short>.Zero);
                    var mask0 = Sse2.MoveMask(cmp0.AsByte());
                    var r0 = Popcnt.PopCount((uint) mask0);
                    return (int)r0 >> 1;
                }

                return 8 + (int)r1 >> 1;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int AvxLength3()
        {
            fixed (char* p = &s1.s0.c0)
            {
                var a = Avx.LoadVector256((short*)p);
                var cmp = Avx2.CompareGreaterThan(a,Vector256<short>.Zero);
                var mask = Avx2.MoveMask(cmp.AsByte());
                var popCount = (int)Popcnt.PopCount((uint)mask);
                return popCount / 2;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Span<char>(Ws16 rhs)
        {
            return rhs.Span();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ReadOnlySpan<char>(Ws16 rhs)
        {
            return rhs.Span();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ws16 LoadFrom(ReadOnlySpan<char> span)
        {
            var x = new Ws16();
            span.CopyTo(x.Span());
            return x;
        }
        
        public bool Equals(Ws16 other)
        {
            return s0.Equals(other.s0) && s1.Equals(other.s1);
        }

        public override bool Equals(object obj)
        {
            return obj is Ws16 other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (s0.GetHashCode() * 397) ^ s1.GetHashCode();
            }
        }

        public static bool operator ==(Ws16 left, Ws16 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Ws16 left, Ws16 right)
        {
            return !left.Equals(right);
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 32)]
    public struct Ws32
    {
        public Ws16 a0;
        public Ws16 a1;
    }

    [StructLayout(LayoutKind.Sequential, Size = 64)]
    public struct Ws64
    {
        public Ws32 a0;
        public Ws32 a1;
    }
    
    public struct Ws128
    {
        public Ws64 a0;
        public Ws64 a1;
    }
    
    public struct Ws256
    {
        public Ws128 a0;
        public Ws128 a1;
    }

    public static class SpanExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NullTerminatedLength(this ReadOnlySpan<char> s)
        {
            var indexOf = s.IndexOf('\0');
            return indexOf < 0 ? s.Length : indexOf;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NullTerminatedLength(this string s)
        {
            var indexOf = s.IndexOf('\0');
            return indexOf < 0 ? s.Length : indexOf;
        }
    }
}
