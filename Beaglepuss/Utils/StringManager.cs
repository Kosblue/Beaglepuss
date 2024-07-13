using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Beaglepuss;

public unsafe class StringManager<T> : IDisposable where T : Enum
{
    private readonly Dictionary<T, nint> Strings = new();

    public bool TryGetString(T identifier, out byte* ptr)
    {
        if (Strings.TryGetValue(identifier, out nint intPtr))
        {
            ptr = (byte*)intPtr;
            return true;
        }

        ptr = null;
        return false;
    }

    public void SetString(T identifier, string text)
    {
        Services.Log.Verbose($"Setting string {identifier} to \"{text}\"");
        RemoveString(identifier);
        Strings[identifier] = AllocateString(text);
    }

    private void RemoveString(T identifier)
    {
        if (Strings.TryGetValue(identifier, out nint ptr))
        {
            Services.Log.Verbose($"Removing string {identifier}");
            Marshal.FreeHGlobal(ptr);
            Strings.Remove(identifier);
        }
    }

    private nint AllocateString(string text)
    {
        Services.Log.Verbose($"Allocating string \"{text}\"");
        byte[] textBytes = Encoding.UTF8.GetBytes(text);
        nint ptr = Marshal.AllocHGlobal(textBytes.Length + 1);
        Marshal.Copy(textBytes, 0, ptr, textBytes.Length);
        Marshal.WriteByte(ptr, textBytes.Length, 0);
        return ptr;
    }

    public void Dispose()
    {
        Services.Log.Verbose("Disposing StringManager");
        GC.SuppressFinalize(this);
        foreach (nint ptr in Strings.Values)
        {
            Marshal.FreeHGlobal(ptr);
        }
        Strings.Clear();
    }
}