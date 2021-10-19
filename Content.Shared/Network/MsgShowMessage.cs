using Lidgren.Network;
using Robust.Shared.Network;

namespace Content.Shared.Network
{
    public class MsgShowMessage : NetMessage
    {
        public override string MsgName => nameof(MsgShowMessage);
        public override MsgGroups MsgGroup => MsgGroups.Command;

        public string Text { get; set; }

        public override void ReadFromBuffer(NetIncomingMessage buffer)
        {
            Text = buffer.ReadString();
        }

        public override void WriteToBuffer(NetOutgoingMessage buffer)
        {
            buffer.Write(Text);
        }
    }
}

