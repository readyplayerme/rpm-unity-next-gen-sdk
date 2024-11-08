using System;

namespace ReadyPlayerMe.Samples.AvatarCreator
{
    public interface ISelectable
    {
        bool IsSelected { get; }
        public Action<bool> OnSelectionChanged { get; set; }
    }
}