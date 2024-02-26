﻿using Avalonia.NodeEditor.Core;
using Avalonia.NodeEditor.Core.Mvvm.Extensions;
using Avalonia.NodeEditor.Mvvm;
using BfevLibrary.Core;
using CafeEventEditor.Views.Nodes;

namespace CafeEventEditor.ViewModels.Nodes;

public partial class EntryPointNode : ObservableNode, INodeTemplateProvider
{
    private const string DEFAULT_NAME = "Entry Point";

    public static INodeTemplate Template { get; } = new ObservableNodeTemplate(() => new EntryPointNode()) {
        Preview = new EntryPointNode(DEFAULT_NAME),
        Title = DEFAULT_NAME
    };

    public EntryPointNode() : this(DEFAULT_NAME)
    {
    }

    public EntryPointNode(string name)
    {
        Name = name;
        EntryPointNodeView view = new() {
            DataContext = this
        };

        Content = view;
        Width = view.Width;
        Height = view.Height;

        this.AddPin(Width / 2, Height - view.Padding.Bottom, 10, 10, PinAlignment.Bottom, "Output");
    }

    public EntryPoint GetEntryPoint()
    {
        throw new NotImplementedException();
    }
}