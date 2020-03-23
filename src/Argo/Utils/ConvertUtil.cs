using System;
using System.Collections.Generic;
using System.Text;

namespace Argo.Utils
{
    public class ConvertUtil
    {
        public static long GetFrameValue(Span<byte> byteBuffer, int fieldOffset, int fieldlength)
        {
            var frameLength = fieldlength switch
            {
                1 => byteBuffer[0],
                2 => BitConverter.ToInt16(byteBuffer.Slice(fieldOffset, fieldlength)),
                4 => BitConverter.ToInt32(byteBuffer.Slice(fieldOffset, fieldlength)),
                _ => throw new Exception("unsupported fieldLength: " + fieldlength + " (expected: 1, 2, 4)"),
            };

            return frameLength;
        }

        public static Span<byte> GetFrameBuffer(int fieldValue, int fieldlength)
        {
            var biteBuffer = fieldlength switch
            {
                1 => new byte[] { (byte)fieldValue },
                2 => BitConverter.GetBytes((short)fieldValue),
                4 => BitConverter.GetBytes(fieldValue),
                _ => throw new Exception("unsupported fieldLength: " + fieldlength + " (expected: 1, 2, 4)"),
            };

            return biteBuffer;
        }
    }
}
