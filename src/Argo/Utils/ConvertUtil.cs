using System;
using System.Collections.Generic;
using System.Text;

namespace Argo.Utils
{
    public static class ConvertUtil
    {
        public static long GetFrameValue(Span<byte> byteBuffer, int fieldOffset, int fieldlength, bool littleEndian)
        {
            var frameLength = fieldlength switch
            {
                1 => byteBuffer[0],
                2 => BitConverter.ToInt16(GetBytes(byteBuffer, fieldOffset, fieldlength, littleEndian)),
                4 => BitConverter.ToInt32(GetBytes(byteBuffer, fieldOffset, fieldlength, littleEndian)),
                8 => BitConverter.ToInt64(GetBytes(byteBuffer, fieldOffset, fieldlength, littleEndian)),
                _ => throw new Exception("unsupported fieldLength: " + fieldlength + " (expected: 1, 2, 4 or 8)"),
            };

            return frameLength;
        }

        public static Span<byte> GetFrameBuffer(long fieldValue, int fieldlength, bool littleEndian)
        {
            var biteBuffer = fieldlength switch
            {
                1 => new byte[] { (byte)fieldValue },
                2 => GetBytes(BitConverter.GetBytes((short)fieldValue), littleEndian),
                4 => GetBytes(BitConverter.GetBytes((int)fieldValue), littleEndian),
                8 => GetBytes(BitConverter.GetBytes(fieldValue), littleEndian),
                _ => throw new Exception("unsupported fieldLength: " + fieldlength + " (expected: 1, 2, 4 or 8)"),
            };

            return biteBuffer;
        }

        private static Span<byte> GetBytes(Span<byte> byteBuffer, int fieldOffset, int fieldlength, bool littleEndian)
        {
            var result = byteBuffer.Slice(fieldOffset, fieldlength);
            if (BitConverter.IsLittleEndian != littleEndian)
            {
                result.Reverse();
            }

            return result;
        }

        private static byte[] GetBytes(byte[] bytes, bool littleEndian)
        {
            if (BitConverter.IsLittleEndian != littleEndian)
            {
                Array.Reverse(bytes);
            }

            return bytes;
        }
    }
}
