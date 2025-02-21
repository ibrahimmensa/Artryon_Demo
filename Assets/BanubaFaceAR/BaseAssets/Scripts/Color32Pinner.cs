using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BNB
{
    public class Pinner<T>
    {
        private GCHandle _pinnedArray;

        public Pinner(T obj)
        {
            _pinnedArray = GCHandle.Alloc(obj, GCHandleType.Pinned);
        }

        ~Pinner()
        {
            _pinnedArray.Free();
        }

        public static implicit operator IntPtr(Pinner<T> ap)
        {
            return ap._pinnedArray.AddrOfPinnedObject();
        }
    }

}