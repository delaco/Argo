using System;
using System.Collections.Generic;
using System.Text;

namespace Argo.Utils
{
    public static class ConvertUtil
    {
        public static long GetFrameValue(Span<byte> byteBuffer, int fieldOffset, int fieldlength, bool littleEndian)
        {
            switch (fieldlength)
            {
                case 1:
                    return byteBuffer[0];
                case 2:
                    return BitConverter.ToInt16(GetBytes(byteBuffer, fieldOffset, fieldlength, littleEndian), 0);
                case 4:
                    return BitConverter.ToInt32(GetBytes(byteBuffer, fieldOffset, fieldlength, littleEndian), 0);
                case 8:
                    return BitConverter.ToInt64(GetBytes(byteBuffer, fieldOffset, fieldlength, littleEndian), 0);
                default:
                    throw new Exception("unsupported fieldLength: " + fieldlength + " (expected: 1, 2, 4 or 8)");
            }
        }

        public static Span<byte> GetFrameBuffer(long fieldValue, int fieldlength, bool littleEndian)
        {
            switch (fieldlength)
            {
                case 1:
                    return new byte[] { (byte)fieldValue };
                case 2:
                    return GetBytes(BitConverter.GetBytes((short)fieldValue), littleEndian);
                case 4:
                    return GetBytes(BitConverter.GetBytes((int)fieldValue), littleEndian);
                case 8:
                    return GetBytes(BitConverter.GetBytes(fieldValue), littleEndian);
                default:
                    throw new Exception("unsupported fieldLength: " + fieldlength + " (expected: 1, 2, 4 or 8)");
            }
        }

        private static byte[] GetBytes(Span<byte> byteBuffer, int fieldOffset, int fieldlength, bool littleEndian)
        {
            var result = byteBuffer.Slice(fieldOffset, fieldlength);
            if (BitConverter.IsLittleEndian != littleEndian)
            {
                result.Reverse();
            }

            return result.ToArray();
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
