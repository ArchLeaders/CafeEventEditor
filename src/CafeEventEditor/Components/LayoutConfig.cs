using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CafeEventEditor.Components;

public partial class LayoutConfig : ObservableObject
{
    [JsonIgnore]
    public string Name { get; private set; } = "Generic";

    public GridUnitType LeftPanelGridUnitType { get; set; } = GridUnitType.Star;
    public double LeftPanelValue { get; set; } = 1;

    public GridUnitType CenterPanelGridUnitType { get; set; } = GridUnitType.Star;
    public double CenterPanelValue { get; set; } = 1.8;

    public GridUnitType RightPanelGridUnitType { get; set; } = GridUnitType.Star;
    public double RightPanelValue { get; set; } = 1;

    [ObservableProperty]
    [property: JsonIgnore]
    private GridLength _leftPanel = GridLength.Star;

    [ObservableProperty]
    [property: JsonIgnore]
    private GridLength _centerPanel = new(1, GridUnitType.Star);

    [ObservableProperty]
    [property: JsonIgnore]
    private GridLength _rightPanel = GridLength.Star;

    public static LayoutConfig Load(string name)
    {
        string file = GetPath(name);
        if (!File.Exists(file)) {
            return new() {
                Name = name
            };
        }

        using FileStream fs = File.OpenRead(file);
        LayoutConfig result = JsonSerializer.Deserialize<LayoutConfig>(fs) ?? new();

        result.Name = name;
        result.LeftPanel = new(result.LeftPanelValue, result.LeftPanelGridUnitType);
        result.CenterPanel = new(result.CenterPanelValue, result.CenterPanelGridUnitType);
        result.RightPanel = new(result.RightPanelValue, result.RightPanelGridUnitType);

        return result;
    }

    public void Save()
    {
        using FileStream fs = File.Create(GetPath(Name));
        JsonSerializer.Serialize(fs, this);
    }

    private static string GetPath(string name)
    {
        string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CafeEventEditor", "Layouts");
        Directory.CreateDirectory(folder);
        return Path.Combine(folder, $"{name}.json");
    }

    partial void OnLeftPanelChanged(GridLength value)
    {
        LeftPanelGridUnitType = value.GridUnitType;
        LeftPanelValue = value.Value;
    }

    partial void OnCenterPanelChanged(GridLength value)
    {
        CenterPanelGridUnitType = value.GridUnitType;
        CenterPanelValue = value.Value;
    }

    partial void OnRightPanelChanged(GridLength value)
    {
        RightPanelGridUnitType = value.GridUnitType;
        RightPanelValue = value.Value;
    }
}
