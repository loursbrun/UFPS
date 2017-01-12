using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace AltSystems.AltTrees
{
    public class AltUtilities
    {
        static float _x, _y, _z;
        static public float fastDistanceSqrt(ref Vector3 lhs, ref Vector3 rhs)
        {
            _x = lhs.x - rhs.x;
            _y = lhs.y - rhs.y;
            _z = lhs.z - rhs.z;
            return (_x * _x + _y * _y + _z * _z);
        }
        static public float fastDistanceSqrt(ref Vector3 lhs, Vector3 rhs)
        {
            _x = lhs.x - rhs.x;
            _y = lhs.y - rhs.y;
            _z = lhs.z - rhs.z;
            return (_x * _x + _y * _y + _z * _z);
        }
        static public float fastDistanceSqrt(Vector3 lhs, Vector3 rhs)
        {
            _x = lhs.x - rhs.x;
            _y = lhs.y - rhs.y;
            _z = lhs.z - rhs.z;
            return (_x * _x + _y * _y + _z * _z);
        }

        static public float fastDistance(ref Vector3 lhs, ref Vector3 rhs)
        {
            _x = lhs.x - rhs.x;
            _y = lhs.y - rhs.y;
            _z = lhs.z - rhs.z;
            return Mathf.Sqrt(_x * _x + _y * _y + _z * _z);
        }
        static public float fastDistanceSqrt2D(ref Vector2 lhs, ref Vector2 rhs)
        {
            _x = lhs.x - rhs.x;
            _y = lhs.y - rhs.y;
            return (_x * _x + _y * _y);
        }
        static public float fastDistanceSqrt2D(ref Vector2 lhs, Vector3 rhs)
        {
            _x = lhs.x - rhs.x;
            _y = lhs.y - rhs.z;
            return (_x * _x + _y * _y);
        }

        static public float fastDistance2D(ref Vector2 lhs, ref Vector2 rhs)
        {
            _x = lhs.x - rhs.x;
            _y = lhs.y - rhs.y;
            return Mathf.Sqrt(_x * _x + _y * _y);
        }

        static public float fastDistance2D(ref Vector2 lhs, Vector2 rhs)
        {
            _x = lhs.x - rhs.x;
            _y = lhs.y - rhs.y;
            return Mathf.Sqrt(_x * _x + _y * _y);
        }

        static public void WriteBytes(int value, byte[] buffer, int offset)
        {
            if (offset + 4 > buffer.Length)
                Debug.LogException(new System.IndexOutOfRangeException());

            if (BitConverter.IsLittleEndian)
            {
                buffer[offset + 3] = (byte)(value >> 24);
                buffer[offset + 2] = (byte)(value >> 16);
                buffer[offset + 1] = (byte)(value >> 8);
                buffer[offset] = (byte)value;
            }
            else
            {
                buffer[offset] = (byte)(value >> 24);
                buffer[offset + 1] = (byte)(value >> 16);
                buffer[offset + 2] = (byte)(value >> 8);
                buffer[offset + 3] = (byte)value;
            }
        }

        static public int ReadBytesInt(byte[] buffer, int offset)
        {
            if (offset + 4 > buffer.Length)
                Debug.LogException(new System.IndexOutOfRangeException());

            int value = 0;

            if (BitConverter.IsLittleEndian)
            {
                value = (value & ~(0xFF << 24)) | (buffer[offset + 3] << 24);
                value = (value & ~(0xFF << 16)) | (buffer[offset + 2] << 16);
                value = (value & ~(0xFF << 8)) | (buffer[offset + 1] << 8);
                value = (value & ~(0xFF << 0)) | (buffer[offset] << 0);
            }
            else
            {
                value = (value & ~(0xFF << 24)) | (buffer[offset] << 24);
                value = (value & ~(0xFF << 16)) | (buffer[offset + 1] << 16);
                value = (value & ~(0xFF << 8)) | (buffer[offset + 2] << 8);
                value = (value & ~(0xFF << 0)) | (buffer[offset + 3] << 0);
            }
            return value;
        }

        static public void WriteBytes(short value, byte[] buffer, int offset)
        {
            if (offset + 2 > buffer.Length)
                Debug.LogException(new System.IndexOutOfRangeException());

            if (BitConverter.IsLittleEndian)
            {
                buffer[offset + 1] = (byte)(value >> 8);
                buffer[offset] = (byte)value;
            }
            else
            {
                buffer[offset] = (byte)(value >> 8);
                buffer[offset + 1] = (byte)value;
            }
        }

        static public short ReadBytesShort(byte[] buffer, int offset)
        {
            if (offset + 4 > buffer.Length)
                Debug.LogException(new System.IndexOutOfRangeException());

            int value = 0;

            if (BitConverter.IsLittleEndian)
            {
                value = (value & ~(0xFF << 8)) | (buffer[offset + 1] << 8);
                value = (value & ~(0xFF << 0)) | (buffer[offset] << 0);
            }
            else
            {
                value = (value & ~(0xFF << 24)) | (buffer[offset] << 8);
                value = (value & ~(0xFF << 16)) | (buffer[offset + 1] << 0);
            }
            return (short)value;
        }



        static public void WriteBytes(float value, byte[] buffer, int offset)
        {
            WriteBytes(SingleToInt32Bits(value), buffer, offset);
        }

        static public float ReadBytesFloat(byte[] buffer, int offset)
        {
            return Int32ToSingleBits(ReadBytesInt(buffer, offset));
        }

        static public void WriteBytes(Vector3 value, byte[] buffer, int offset)
        {
            WriteBytes(SingleToInt32Bits(value.x), buffer, offset);
            WriteBytes(SingleToInt32Bits(value.y), buffer, offset + 4);
            WriteBytes(SingleToInt32Bits(value.z), buffer, offset + 8);
        }

        static public Vector3 ReadBytesVector3(byte[] buffer, int offset)
        {
            return new Vector3(ReadBytesFloat(buffer, offset), ReadBytesFloat(buffer, offset + 4), ReadBytesFloat(buffer, offset + 8));
        }

        static public void WriteBytes(Color value, byte[] buffer, int offset)
        {
            WriteBytes(SingleToInt32Bits(value.r), buffer, offset);
            WriteBytes(SingleToInt32Bits(value.g), buffer, offset + 4);
            WriteBytes(SingleToInt32Bits(value.b), buffer, offset + 8);
            WriteBytes(SingleToInt32Bits(value.a), buffer, offset + 12);
        }

        static public Color ReadBytesColor(byte[] buffer, int offset)
        {
            return new Color(ReadBytesFloat(buffer, offset), ReadBytesFloat(buffer, offset + 4), ReadBytesFloat(buffer, offset + 8), ReadBytesFloat(buffer, offset + 12));
        }

        static public void WriteBytes(bool value, byte[] buffer, int offset)
        {
            if (offset + 1 > buffer.Length)
                Debug.LogException(new System.IndexOutOfRangeException());

            buffer[offset] = BitConverter.GetBytes(value)[0];
        }

        static public bool ReadBytesBool(byte[] buffer, int offset)
        {
            return BitConverter.ToBoolean(buffer, offset);
        }



        [StructLayout(LayoutKind.Explicit)]
        struct Int32SingleUnion
        {
            [FieldOffset(0)]
            int i;
            [FieldOffset(0)]
            float f;
        
            internal Int32SingleUnion(int i)
            {
                this.f = 0;
                this.i = i;
            }
        
            internal Int32SingleUnion(float f)
            {
                this.i = 0;
                this.f = f;
            }
        
            internal int AsInt32
            {
                get { return i; }
            }
        
            internal float AsSingle
            {
                get { return f; }
            }
        }

        static int SingleToInt32Bits(float value)
        {
            return new Int32SingleUnion(value).AsInt32;
        }
        static float Int32ToSingleBits(int value)
        {
            return new Int32SingleUnion(value).AsSingle;
        }
    }
}
