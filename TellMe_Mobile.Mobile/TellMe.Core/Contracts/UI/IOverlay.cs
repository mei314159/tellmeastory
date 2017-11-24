using System;

namespace TellMe.Core.Contracts.UI
{
    public interface IOverlay
    {
        void PopUp(bool animated = true, Action popAnimationFinish = null);
        void Close(bool animated = true);
        void Dispose();
    }
}