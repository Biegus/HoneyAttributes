#if UNITY_EDITOR
using System;

namespace Honey.Editor
{
    public interface IHoneyGroupDrawer
    {
        void DrawLayout(string groupPath, string name, HoneyEditorHandler editor, Attribute attribute, Group group,
            string innerPath);
        void DisposeEditor(HoneyEditorHandler editor);

    }
}

#endif