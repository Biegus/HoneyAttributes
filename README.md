
# HoneyAttributes

Unity plugin that adds some useful attributes. Most of them will work with any editor. For those few who use custom one, you need to explicitly make the class use HoneyEditor inheritor.

## Types of attributes
Types of attributes are shown by their prefixes

 - Name -> doesn't require anything
 - **H**Name -> requires **HoneRun** or **EHoneyRun** attribute to be added as well
 - **E**Name -> requires the editor to be the **HoneyEditor**
 - **EH**Name -> requires **EHoneyRun** attribute and the editor to be the **HoneyEditor**

To enable context in which attributes with "E" prefix work add:
```csharp

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof($ClassName$))]
[UnityEditor.CanEditMultipleObjects]
public class $ClassName$Editor : BaseTool.Honey.Editor.HoneyEditor
#endif
```
## Meta and control attributes
####	HShowIf

Restricts  whether a field can be shown/editable.
Name can point to a static/instance method/field/property or to a simple **BoolExpression**

```csharp
public bool showX;
[HoneyRun]
[HShowIf(nameof(showX))]
public int x;

public int a;  
public int b;
[HoneyRun]  
[HShowIf("CheckSum(a,b)")]
public float secretField=42;

public bool allowToEditY;  
[HoneyRun] [HShowIf(nameof(allowToEditY),HoneyPropertyState.Disabled)]  
public string y="abcd";

private bool CheckSum(int x,int y)  
{  
  return x + y == 5;  
}
```
![Unity_y90jCkp2ym](https://user-images.githubusercontent.com/48364457/176658383-8c8b9ee8-353b-44ba-9746-b26f6020c872.gif)

#### [HReadOnly]
Blocks editing the variable

![image](https://user-images.githubusercontent.com/48364457/176658854-b1bf62a4-cdc7-4010-9d3a-4df8a721ff26.png)


#### [HHeaders]
Headers with greater capabilities than unity headers.
```csharp
[DefineStyleOverride("big_right_header",Alignment = TextAnchor.MiddleRight,  
  ColorExpression = "red",FontSize = 30)]  
[DefineStyleOverride("small_left_header",Alignment = TextAnchor.MiddleLeft,  
  ColorExpression = "RGB(51,184,100)",FontSize = 15, FontStyle = FontStyle.Italic )]  
public class HeaderDemo: MonoBehaviour  
{  
 [HHeader("Simple header")]  
 [HHeader("Big right bold text", style: "big_right_header")]  
 [HHeader("small left italic text", style: "small_left_header")]  
 [HoneyRun]  
 public int someValue;
```
![Unity_B9YNZFhcW5](https://user-images.githubusercontent.com/48364457/176658638-c1887e3e-a20b-4d65-946a-3497ff4eca2d.png)

#### [HIndentBefore]
Forces field to have bigger indent
```csharp
[HIndentBefore(5)][HoneyRun]  
public int indentation = 3;
```
![image](https://user-images.githubusercontent.com/48364457/176658973-faac4d7f-ee41-4eec-a55a-7fabd2da31b9.png)


#### [ColorSeparator]
Draws line in color of **Color Expression**
```csharp
[ColorSeparator("white")]
public int a;
[ColorSeparator("RGB(51,184,100)")]
public int b;
```

![image](https://user-images.githubusercontent.com/48364457/176659124-1d12cb07-a616-44b8-ac33-bc62f78167ca.png)


#### [HShortPrefix]
Makes prefix as short as possible

![image](https://user-images.githubusercontent.com/48364457/176659269-ab1b5ee0-5edf-4cc2-9f98-19679e9eb16c.png)

#### [HHidePrefix]
Hides prefix

![image](https://user-images.githubusercontent.com/48364457/176659522-01c5cedb-38cc-413a-96da-2de48839b7a7.png)


#### [HCustomLabel]
Changes the label

![image](https://user-images.githubusercontent.com/48364457/176659556-fbbd5d8e-ed4b-472b-9fa7-bb3e08042207.png)


#### [HDynamicContent]
Defines the label dynamically.  Method should be of the exact structure as in the example. *string METHOD(string name, object value)*
```csharp
[HDynamicContent(nameof(GetAlphabetElement))] [HoneyRun]  
public List<int> differentIndexNames = new List<int>() {42, 123,55};  
  
public string GetAlphabetElement(string name, object value)  
{  
  return ((char)(int.Parse(name.Replace("Element", "")) + 'a')).ToString();  
}
```

![image](https://user-images.githubusercontent.com/48364457/176659601-2fdc5732-2cac-439d-aa84-777e64d5ff49.png)

#### HelpBox
Draws simple helpbox
```csharp
[HelpBox("Hello how are u?")]   
public int someValue;
```
#### [HHelpBoxAttribut]
More powerful version of helpbox. Reference **Value Expression** by *&*. *Space* or "*,*" marks the end of reference.
```csharp
[HoneyRun] 
[HHelpBox("x=&x y=&y when you add them up you get &x+y")]  
public int x = 3;  
public int y = 5;
```

![image](https://user-images.githubusercontent.com/48364457/176659686-aaadef83-ac24-4054-ba70-cc20c6762cd3.png)


#### [HShortArrayIndex]
instead of Element 0, Element 1 will display 0,1,2,..

![image](https://user-images.githubusercontent.com/48364457/176659730-198dd050-2024-4c7f-a3c4-2a9ca68cac99.png)


#### [HJustProgressBar]
Draws progress bar.
```csharp

[HJustProgressBar("hp","0","maxHp" ,style: ProgressBarStyle.Red, Name = "HP",Size = 2)]  
[HJustProgressBar("mana","0","maxMana",style: ProgressBarStyle.Blue, Name = "Mana", Size = 2)]  
[HoneyRun]  
public string someVariable;
```

![image](https://user-images.githubusercontent.com/48364457/176659798-9d41103e-969c-4a01-859b-446b4b1458b4.png)


## Custom drawers/additional drawers
	
#### [HDropdownButton]
Additional button that allows for selection of predefined values. Works only with primitive types.
```csharp
[HDropdownButton(1,2,3)][HoneyRun]  
public int fastAccessTo123;
```
#### [Dropdown]
Makes dropdown with predefined values the only way of changing value of the field.

#### [HDynamicDropdownButton]
Additional button that allows for selection of dynamic values.
```csharp
[HoneyRun]  
[HDynamicDropdownButton(nameof(GetGuids))]
public string fastAccessToRandomGuids;
private IEnumerable<string> GetGuids()  
{  
  yield return GUID.Generate().ToString();  
  yield return GUID.Generate().ToString();  
}
```

#### [DynamicDropdown]
Makes dropdown with dynamic values the only way of changing value of the field.

#### SerializeReferenceHelper
Allows to select type for serialize references. 
Type can be restricted by namespace
```csharp
[SerializeReference] [SerializeReferenceHelper]  
public Animal animal = null;
[SerializeReference] [SerializeReferenceHelper("UnityEngine")]  
public object unityNamespaceObject = null;
```

#### [EHPreviev]
Draws editor for a field
```csharp
[EHoneyRun]  
[EHPreview]  
public Transform withPreview;
```

#### [ResetValue]
Adds a button that allows for reseting the value. Value has to be a primitive.
```csharp
[HoneyRun]  
[HResetValue(42)]  
public int resetable=42;
```

#### [Tag]
Draws dropdown to select unity tag

#### [Layer]
Draws dropdown to select unity layer

#### [SearchableEnum]
Replaces enum dropdown with searchable window

#### [Slider]
Similar to unity **[Range]**, but allows for dynamic limits and alternative appearance.
```csharp
public float maxHp;
public float maxMana;
[Slider("0","maxHp")]  
public float hp  = 0;
[Slider("0","maxMana",SliderMode.ProgressBar)]  
public float mana = 0;
```
#### [EHConstSizeAttribute]
Makes the array size constant and draws array expanded
```csharp
[EHoneyRun]
[EHConstSizeAttribute(3)]  
public string[] someElements= new string[3];
```
#### [HGetComponentButton]
Allows for easier way to set component field
```csharp
[HoneRun]
[HGetComponentButtonAttribute]
public SpriteRenderer renderer;
[HoneRun]
[HGetComponentButton(ComponentBFlags.AddAutomaticallyIfNotPresent)]
public SpriteRenderer rendererGetOrAdd;
```
Flags:
IncludeChildren -> allows for grabbing from children
AddAutomaticallyIfNotPresent -> If cannot grab, it adds the component
Aggressive -> Button presses itself on its own


## Restrains
#### [HMin],[HMax]
Defines min and max value for number field. May be dynamic.

(There is already unity attributes min, there's no max though)

#### CurveBound

Restricts curve range
```csharp
[CurveBound(0, 1, 0, 1)] public AnimationCurve curve01;
```
#### AssetsOnly
Disallows scene references.

#### [HValidate]
Soft restrain that marks field in red if requirements are not met.
Works with methods returning bool, bool fields and properties and simple **Bool expressions**
```csharp
[HoneyRun]
[HValidate("itself!=3")] public int not3;  
[HoneyRun]
[HValidate("Do(itself)!=2")]public int some;

private int Do(int v)  
{  
  return v / 5;  
}
```

####  [HNotNull]
Soft restrain that marks field in red if field is null




## Groups
#### General
All groups require HoneyEditor. Groups can be nested (use "/")

**[EGroup]** marks field as belonging to the group
**[EDefVerticalGroup]**,**[EDefHorizontalGroup]**, **[EDefTabGroup]**-define the type of a group


```csharp

[EDefVerticalGroup("BasicFolders",BaseGroupAppearance.Empty,true)]  
[EDefVerticalGroup("BasicFolders/a",BaseGroupAppearance.Empty,true)]  
[EDefVerticalGroup("GroupBox",BaseGroupAppearance.BoxGroup,false)]  
[EDefVerticalGroup("GroupBoxFolder",BaseGroupAppearance.BoxGroup,true)]  
[EDefVerticalGroup("GroupBoxFolder/a",BaseGroupAppearance.BoxGroup,true)]  
  
public class GroupDemo : MonoBehaviour  
{  
 [EGroup("BasicFolders")] public string x;  
 [EGroup("BasicFolders")] public string o;  
 [EGroup("BasicFolders/a")] public string y;  
 [EGroup("BasicFolders/a")] public string n;  
 [EGroup("BasicFolders/a")] public string s;  
 [Space]  
 [EGroup("GroupBoxFolder")] public string d;  
 [EGroup("GroupBoxFolder")] public string g;  
 [EGroup("GroupBoxFolder/a")] public string k;  
 [EGroup("GroupBoxFolder/a")] public string a;  
 }
  
  ```
#### EDefTabGroup

Defines groups that consists of 2 or more tabs.
In the example below group name is empty string(master group) which forces every element to be in that group.
```csharp
[EDefTabGroup("","Main","Other")]
public class TabDemo : MonoBehaviour
{
	[ETabElement("Main")] public int a;  
	[ETabElement("Main")] public string b;
	[ETabElement("Other")] public string x;
	[ETabElement("Other")] public string y;
	[ETabElement("Other")] public string z;
}
```
In order to assign tab to group  use [EAssignGroupToTab] on the class.

#### EDefHorizontalGroup
Allows to position elements in horizontal manner.
Remember that you can always use [HShortPrefix] or [HHidePrefix]
```csharp
[Serializable]  
public struct HorizontalDemoElement  
{  
  public string Descr;  
  public int Value;  
}  
  
[EDefHorizontalGroup("container",BaseGroupAppearance.Empty)]  
[EDefVerticalGroup("container/left",BaseGroupAppearance.Box)]  
[EDefVerticalGroup("container/right",BaseGroupAppearance.Box)]  
[EDefHorizontalGroup("a",BaseGroupAppearance.Empty,ShowLabel = HorizontalLabelMode.Hide)]  
public class HorizontalDemo : MonoBehaviour  
{  
 [EGroup("container/left")] public int a;  
 [EGroup("container/left")] public int b;  
 [EGroup("container/left")] public int c;  
 [EGroup("container/left")] public int d;  
 [EGroup("container/right")] public int x;  
 [EGroup("container/right")] public int y;  
 [EGroup("container/right")] public int z;  
 [EGroup("container/right")] public HorizontalDemoElement someStruct;
 [EGroup("a")][HShortPrefix][HoneyRun]public int i;  
 [EGroup("a")][HShortPrefix][HoneyRun] public int j;
 ```
 ## Non serializable attributes
 - They can be grouped just like fields.
#### [EMethodButton]
Draws button that runs parameterless method.
```csharp
[EMethodButton(ButtonSize:2)]  
public void ClickMe()=>Debug.Log("thx");
```
#### [EShowAsString]
Shows any property, non serialized field, or parameterless method as a string from .ToString()
```csharp
[EShowAsString]  
private int secret = 2152;  
[EShowAsString]  
public float TheTime => Time.time;
```
