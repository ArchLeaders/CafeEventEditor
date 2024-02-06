﻿using Avalonia.Controls;
using System.Collections.ObjectModel;

namespace CafeEventEditor.Builders;

// (c) NX-Editor, https://github.com/NX-Editor/NxEditor.PluginBase/blob/master/src/Components/IMenuFactory.cs

public interface IMenuFactory
{
    public ObservableCollection<Control> Items { get; set; }

    public IMenuFactory Prepend<T>() where T : class;
    public IMenuFactory Prepend(Type type);
    public IMenuFactory Append<T>() where T : class, new() => Append(new T());
    public IMenuFactory Append<T>(T source) where T : class;
    public IMenuFactory Append(object source);
}
