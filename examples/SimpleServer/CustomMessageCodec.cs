using Argo.Internal;

namespace SimpleServer
{
    public class CustomMessageCodec : DefaultMessageCodec
    {
        public CustomMessageCodec() : base(commandFieldLength: 2)
        {
        }
    }
}
