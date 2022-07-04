#if UNITY_EDITOR
using System;

namespace Honey.Editor
{
    public interface IHoneyGroupDrawer
    {
        void DrawLayout(string groupPath,string name,HoneyEditorHandler editor, Attribute attribute, Group group);
        void OnEditorDisable(HoneyEditorHandler editor);

    }
}

#endif