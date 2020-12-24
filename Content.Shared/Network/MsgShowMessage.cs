using Lidgren.Network;
using Robust.Shared.Interfaces.Network;
using Robust.Shared.Network;

namespace Content.Shared.Network
{
    public class MsgShowMessage : NetMessage
    {
        #region REQUIRED

        public const MsgGroups GROUP = MsgGroups.Command;
        public const string NAME = nameof(MsgShowMessage);
        public MsgShowMessage(INetChannel channel) : base(NAME, GROUP) { }

        #endregion

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

