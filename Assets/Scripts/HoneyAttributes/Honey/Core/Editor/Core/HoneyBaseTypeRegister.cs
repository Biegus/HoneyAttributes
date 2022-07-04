#if UNITY_EDITOR
using Honey;
using Honey.Editor;
using UnityEditor;

namespace  Honey.Editor
{
    [InitializeOnLoad]
    public static class HoneyBaseTypeRegister
    {
        static HoneyBaseTypeRegister()
        {
            HoneyReflectionCache h = HoneyHandler.HoneyReflectionCache;
            h.RegisterFieldAttribute(typeof(HShowIfAttribute),new HoneyTypeDrawerDefinition(new ShowIfAttributeDrawer(),null));
           
            h.RegisterFieldAttribute(typeof(HColorAttribute),new HoneyTypeDrawerDefinition(new ColorDrawer(),null));
          
            h.RegisterFieldAttribute(typeof(HCustomLabelAttribute),new HoneyTypeDrawerDefinition(new HCustomLabelDrawer(),null));
            h.RegisterFieldAttribute(typeof(HShortPrefixAttribute),new HoneyTypeDrawerDefinition(new HShortPrefixDrawer(),null));
            h.RegisterFieldAttribute(typeof(HDropdownButtonAttribute),new HoneyTypeDrawerDefinition(null,new HDropdownButtonDrawer()));
            h.RegisterFieldAttribute(typeof(HResetValueAttribute),new HoneyTypeDrawerDefinition(null,new ResetValueDrawer()));
            h.RegisterFieldAttribute(typeof(HGetComponentButtonAttribute),new HoneyTypeDrawerDefinition(null,new HGetComponentButtonDrawer()));
            h.RegisterFieldAttribute(typeof(HHidePrefixAttribute),new HoneyTypeDrawerDefinition(new HHidePrefixDrawer(),null));
            h.RegisterFieldAttribute(typeof(EHPreviewAttribute),new HoneyTypeDrawerDefinition(null,new EHPreviewDrawer()));
            h.RegisterFieldAttribute(typeof(HHeaderAttribute),new HoneyTypeDrawerDefinition(new HHeaderDrawer(),null));
            h.RegisterFieldAttribute(typeof(HJustProgressBarAttribute),new HoneyTypeDrawerDefinition(new HJustProgressBarDrawer(),null));
            h.RegisterFieldAttribute(typeof(HSpaceAttribute),new HoneyTypeDrawerDefinition(new HSpaceDrawer(),null));
            h.RegisterFieldAttribute(typeof(HFormatPreviewAttribute),new HoneyTypeDrawerDefinition(new HFormatPreviewDrawer(),null));
            h.RegisterFieldAttribute(typeof(HDynamicDropdownButtonAttribute),new HoneyTypeDrawerDefinition(null, new HDynamicDropdownButtonDrawer()));
            h.RegisterFieldAttribute(typeof(HNotNullAttribute),new HoneyTypeDrawerDefinition(new HNotNullDrawer(),null));
            
            h.RegisterFieldAttribute(typeof(HMinAttribute),new HoneyTypeDrawerDefinition(MinMaxDrawerFactory.BuildMin(),null));
            h.RegisterFieldAttribute(typeof(HMaxAttribute),new HoneyTypeDrawerDefinition(MinMaxDrawerFactory.BuildMax(),null));
            
            h.RegisterFieldAttribute(typeof(HReadOnlyAttribute), new HoneyTypeDrawerDefinition(new HReadOnlyDrawer(),null ));
            
            h.RegisterFieldAttribute(typeof(HShortArrayIndexAttribute), new HoneyTypeDrawerDefinition(new HShortArrayIndexDrawer(),null));
            h.RegisterFieldAttribute(typeof(HDynamicContentAttribute), new HoneyTypeDrawerDefinition(new HDynamicContentDrawer(),null));
            h.RegisterFieldAttribute(typeof(HValidateAttribute), new HoneyTypeDrawerDefinition(new HValidateDrawer(),null));
            h.RegisterFieldAttribute(typeof(HHelpBoxAttribute), new HoneyTypeDrawerDefinition(new HHelpBoxDrawer(),null));
            h.RegisterFieldAttribute(typeof(HIndentBeforeAttribute), new HoneyTypeDrawerDefinition(new HIndentThisDrawer(),null));
            h.RegisterFieldAttribute(typeof(HPrefabPreviewAttribute), new HoneyTypeDrawerDefinition(new HPrefabPreviewDrawer(),null));
            h.RegisterFieldAttribute(typeof(HSpritePreviewAttribute), new HoneyTypeDrawerDefinition(new SpritePreviewDrawer(),null));
            h.RegisterFieldAttribute(typeof(EHConstSizeAttributeAttribute), new HoneyTypeDrawerDefinition(null,new EHConstSizeDrawer()));
            
            
            
            h.RegisterGroupAttribute(typeof(EDefVerticalGroupAttribute), new EDefVerticalGroupDrawer());
            h.RegisterGroupAttribute(typeof(EDefHorizontalGroupAttribute), new EDefHorizontalGroupDrawer());
            h.RegisterGroupAttribute(typeof(EDefTabGroupAttribute), new EDefTabHoneyGroupDrawer());

            h.RegisterCustomMember(typeof(EMethodButtonAttribute), new MethodButtonDrawer());
            h.RegisterCustomMember(typeof(EShowAsStringAttribute),new  EShowAsStringDrawer());
        }
    }   
}
#endif