using System;

namespace ReadyPlayerMe.Samples.AvatarCreator
{
    public interface ISelectable
    {
        bool IsSelected { get; }
        public void SetSelected(bool selected);
        public Action<bool> OnSelectionChanged { get; set; }
    }
}