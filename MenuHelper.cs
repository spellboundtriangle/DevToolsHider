using BoneLib;
using BoneLib.BoneMenu;
using MelonLoader;
using UnityEngine;
using DevToolsHider;

namespace MenuHelper;

public static class MenuHelper
{
    internal static IntElement CreateIntPref(this Page page, string name, Color color, ref MelonPreferences_Entry<int> value, int increment, int minValue, int maxValue, Action<int> callback = null, string prefName = null, int prefDefaultValue = default, string tooltip = null)
    {
        return CreateIntPref(page, name, color, ref value, DevToolsHiderMod.DevToolsHider_Category, increment, minValue, maxValue, callback, prefName, prefDefaultValue, tooltip);
    }

    internal static FloatElement CreateFloatPref(this Page page, string name, Color color, ref MelonPreferences_Entry<float> value, float increment, float minValue, float maxValue, Action<float> callback = null, string prefName = null, float prefDefaultValue = default, string tooltip = null)
    {
        return CreateFloatPref(page, name, color, ref value, DevToolsHiderMod.DevToolsHider_Category, increment, minValue, maxValue, callback, prefName, prefDefaultValue, tooltip);
    }

    internal static BoolElement CreateBoolPref(this Page page, string name, Color color, ref MelonPreferences_Entry<bool> value, Action<bool> callback = null, string prefName = null, bool prefDefaultValue = default, string tooltip = null)
    {
        return CreateBoolPref(page, name, color, ref value, DevToolsHiderMod.DevToolsHider_Category, callback, prefName, prefDefaultValue, tooltip);
    }

    internal static EnumElement CreateEnumPref<T>(this Page page, string name, Color color, ref MelonPreferences_Entry<T> value, Action<Enum> callback = null, string prefName = null, Enum prefDefaultValue = default, string tooltip = null) where T : Enum
    {
        return CreateEnumPref(page, name, color, ref value, DevToolsHiderMod.DevToolsHider_Category, callback, prefName, prefDefaultValue, tooltip);
    }

    internal static StringElement CreateStringPref(this Page page, string name, Color color, ref MelonPreferences_Entry<string> value, Action<string> callback = null, string prefName = null, string prefDefaultValue = default, string tooltip = null)
    {
        return CreateStringPref(page, name, color, ref value, DevToolsHiderMod.DevToolsHider_Category, callback, prefName, prefDefaultValue, tooltip);
    }

    public static IntElement CreateIntPref(this Page page, string name, Color color, ref MelonPreferences_Entry<int> value, MelonPreferences_Category prefCategory, int increment, int minValue, int maxValue, Action<int> callback = null, string prefName = null, int prefDefaultValue = default, string tooltip = null)
    {
        prefName ??= name;

        if (!prefCategory.HasEntry(prefName))
            value = prefCategory.CreateEntry(prefName, prefDefaultValue);

        MelonPreferences_Entry<int> val = value;
        var element = page.CreateInt(name, color, val.Value, increment, minValue, maxValue, (x) =>
        {
            val.Value = x;
            prefCategory.SaveToFile(false);
            callback?.InvokeActionSafe(x);
        });
        element.ElementTooltip = tooltip;
        return element;
    }

    public static FloatElement CreateFloatPref(this Page page, string name, Color color, ref MelonPreferences_Entry<float> value, MelonPreferences_Category prefCategory, float increment, float minValue, float maxValue, Action<float> callback = null, string prefName = null, float prefDefaultValue = default, string tooltip = null)
    {
        prefName ??= name;

        if (!prefCategory.HasEntry(prefName))
            value = prefCategory.CreateEntry(prefName, prefDefaultValue);

        MelonPreferences_Entry<float> val = value;
        var element = page.CreateFloat(name, color, val.Value, increment, minValue, maxValue, (x) =>
        {
            val.Value = x;
            prefCategory.SaveToFile(false);
            callback?.InvokeActionSafe(x);
        });
        element.ElementTooltip = tooltip;
        return element;
    }

    public static BoolElement CreateBoolPref(this Page page, string name, Color color, ref MelonPreferences_Entry<bool> value, MelonPreferences_Category prefCategory, Action<bool> callback = null, string prefName = null, bool prefDefaultValue = default, string tooltip = null)
    {
        prefName ??= name;

        if (!prefCategory.HasEntry(prefName))
            value = prefCategory.CreateEntry(prefName, prefDefaultValue);

        MelonPreferences_Entry<bool> val = value;
        var element = page.CreateBool(name, color, val.Value, (x) =>
        {
            val.Value = x;
            prefCategory.SaveToFile(false);
            callback?.InvokeActionSafe(x);
        });
        element.ElementTooltip = tooltip;
        return element;
    }

    public static EnumElement CreateEnumPref<T>(this Page page, string name, Color color, ref MelonPreferences_Entry<T> value, MelonPreferences_Category prefCategory, Action<Enum> callback = null, string prefName = null, Enum prefDefaultValue = default, string tooltip = null) where T : Enum
    {
        prefName ??= name;

        if (!prefCategory.HasEntry(prefName))
            value = prefCategory.CreateEntry(prefName, (T)prefDefaultValue);

        MelonPreferences_Entry<T> val = value;
        var element = page.CreateEnum(name, color, val.Value, (x) =>
        {
            val.Value = (T)x;
            prefCategory.SaveToFile(false);
            callback?.InvokeActionSafe(x);
        });
        element.ElementTooltip = tooltip;
        return element;
    }
    public static StringElement CreateStringPref(this Page page, string name, Color color, ref MelonPreferences_Entry<string> value, MelonPreferences_Category prefCategory, Action<string> callback = null, string prefName = null, string prefDefaultValue = default, string tooltip = null)
    {
        prefName ??= name;

        if (!prefCategory.HasEntry(prefName))
            value = prefCategory.CreateEntry(prefName, prefDefaultValue);

        MelonPreferences_Entry<string> val = value;
        var element = page.CreateString(name, color, val.Value, (x) =>
        {
            val.Value = x;
            prefCategory.SaveToFile(false);
            callback?.InvokeActionSafe(x);
        });
        element.ElementTooltip = tooltip;
        return element;
    }
}