using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class ComputeBufferExt
{

}

public static class ComputeBufferUtils
{
    public static ComputeBuffer Alloc<ty>(int count, ComputeBufferType cbt = ComputeBufferType.Default)
    {
        return new ComputeBuffer(count, Marshal.SizeOf(typeof(ty)), cbt);
    }
    public static ComputeBuffer Alloc<ty>(ty[] data, ComputeBufferType cbt = ComputeBufferType.Default)
    {
        ComputeBuffer buff = Alloc<ty>(data.Length,cbt);
        buff.SetData(data);
        return buff;
    }
    public static ComputeBuffer Alloc<ty>(List<ty> data, ComputeBufferType cbt = ComputeBufferType.Default)
    {
        ComputeBuffer buff = Alloc<ty>(data.Count,cbt);
        buff.SetData(data);
        return buff;
    }

    public static void Free(ref ComputeBuffer buff)
    {
        if (buff != null)
            buff.Release();
        buff = null;
    }

}