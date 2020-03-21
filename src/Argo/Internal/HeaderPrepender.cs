using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;

namespace Argo
{
    internal class HeaderPrepender : MessageToMessageEncoder<MessagePacket>
    {
        private readonly ByteOrder _byteOrder;
        private readonly int _lengthFieldLength;

        public HeaderPrepender()
        {
            _byteOrder = ByteOrder.LittleEndian;
            _lengthFieldLength = 4;
        }

        protected override void Encode(IChannelHandlerContext context, MessagePacket message, List<object> output)
        {
            int length = message.Body.Length; //message.ReadableBytes + this.lengthAdjustment;
            //if (this.lengthFieldIncludesLengthFieldLength)
            //{
            //    length += this.lengthFieldLength;
            //}

            if (length < 0)
            {
                throw new ArgumentException($"Adjusted frame length ({ length }) is less than zero");
            }

            output.Add(this._byteOrder == ByteOrder.BigEndian
                       ? context.Allocator.Buffer(4).WriteInt((int)message.CommandId)
                       : context.Allocator.Buffer(4).WriteIntLE((int)message.CommandId));

            switch (this._lengthFieldLength)
            {
                case 1:
                    if (length >= 256)
                    {
                        throw new ArgumentException("length of object does not fit into one byte: " + length);
                    }

                    output.Add(context.Allocator.Buffer(1).WriteByte((byte)length));
                    break;
                case 2:
                    if (length >= 65536)
                    {
                        throw new ArgumentException("length of object does not fit into a short integer: " + length);
                    }

                    output.Add(this._byteOrder == ByteOrder.BigEndian
                        ? context.Allocator.Buffer(2).WriteShort((short)length)
                        : context.Allocator.Buffer(2).WriteShortLE((short)length));
                    break;
                case 4:
                    output.Add(this._byteOrder == ByteOrder.BigEndian
                        ? context.Allocator.Buffer(4).WriteInt((short)length)
                        : context.Allocator.Buffer(4).WriteIntLE((short)length));
                    break;
                case 8:
                    output.Add(this._byteOrder == ByteOrder.BigEndian
                        ? context.Allocator.Buffer(8).WriteLong((short)length)
                        : context.Allocator.Buffer(8).WriteLongLE((short)length));
                    break;
                default:
                    throw new Exception("Unknown length field length");
            }

            output.Add(this._byteOrder == ByteOrder.BigEndian
                    ? context.Allocator.Buffer(4).WriteInt((int)message.SequenceId)
                    : context.Allocator.Buffer(4).WriteIntLE((int)message.SequenceId));

            IByteBuffer byteBuffer = Unpooled.CopiedBuffer(message.Body);
            output.Add(byteBuffer.Retain());
        }
    }
}