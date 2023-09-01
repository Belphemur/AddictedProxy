using System.Runtime.InteropServices;

namespace Compressor.Utils;

public static class ByteCompare
{
#if _WINDOWS
    [DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
    private static extern int memcmp(byte[]? firstArray, byte[]? secondArray, ulong size);
#else
    [DllImport("libc", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
    private static extern int memcmp(byte[] firstArray, byte[] secondArray, ulong size);
#endif
    public static bool CompareWith(this byte[]? firstArray, byte[]? secondArray)
    {
        if (firstArray == null && secondArray == null)
        {
            return true;
        }

        if (firstArray == null || secondArray == null || firstArray.Length != secondArray.Length)
        {
            return false;
        }

        return memcmp(firstArray, secondArray, (ulong)firstArray.Length) == 0;
    }
}