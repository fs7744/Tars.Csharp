using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace Tars.Csharp.Codecs
{
    public static class PacketExtensions
    {
        public static IChannelPipeline AddLengthFieldHanlder(this IChannelPipeline pipeline, int maxFrameLength, int lengthFieldLength)
        {
            pipeline.AddLast(new LengthFieldPrepender(lengthFieldLength, true));
            pipeline.AddLast(new LengthFieldBasedFrameDecoder(ByteOrder.BigEndian,
                maxFrameLength, 0, lengthFieldLength, -1 * lengthFieldLength, lengthFieldLength, true));
            return pipeline;
        }
    }
}