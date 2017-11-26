using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Tars.Csharp.Network.Hosting
{
    public class LoggingHandler : ChannelHandlerAdapter
    {
        private ILogger<LoggingHandler> logger;

        public LoggingHandler(ILogger<LoggingHandler> logger)
        {
            this.logger = logger;
        }

        public override void ChannelRegistered(IChannelHandlerContext context)
        {
            logger.LogTrace("REGISTERED", context);
            context.FireChannelRegistered();
        }

        public override void ChannelUnregistered(IChannelHandlerContext context)
        {
            logger.LogTrace("UNREGISTERED", context);
            context.FireChannelUnregistered();
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            logger.LogTrace("ACTIVE", context);
            context.FireChannelActive();
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            logger.LogTrace("INACTIVE", context);
            context.FireChannelInactive();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception cause)
        {
            logger.LogError(cause.Message, context, cause);
            context.FireExceptionCaught(cause);
        }

        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            logger.LogTrace("USER_EVENT", context, evt);
            context.FireUserEventTriggered(evt);
        }

        public override Task BindAsync(IChannelHandlerContext context, EndPoint localAddress)
        {
            logger.LogTrace("BIND", context, localAddress);
            return context.BindAsync(localAddress);
        }

        public override Task ConnectAsync(IChannelHandlerContext context, EndPoint remoteAddress, EndPoint localAddress)
        {
            logger.LogTrace("CONNECT", context, remoteAddress, localAddress);
            return context.ConnectAsync(remoteAddress, localAddress);
        }

        public override Task DisconnectAsync(IChannelHandlerContext context)
        {
            logger.LogTrace("DISCONNECT", context);
            return context.DisconnectAsync();
        }

        public override Task CloseAsync(IChannelHandlerContext context)
        {
            logger.LogTrace("CLOSE", context);
            return context.CloseAsync();
        }

        public override Task DeregisterAsync(IChannelHandlerContext context)
        {
            logger.LogTrace("DEREGISTER", context);
            return context.DeregisterAsync();
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            logger.LogDebug("RECEIVED", context, message);
            context.FireChannelRead(message);
        }

        public override Task WriteAsync(IChannelHandlerContext context, object message)
        {
            logger.LogDebug("WRITE", context, message);
            return context.WriteAsync(message);
        }

        public override void Flush(IChannelHandlerContext context)
        {
            logger.LogDebug("FLUSH", context);
            context.Flush();
        }
    }
}