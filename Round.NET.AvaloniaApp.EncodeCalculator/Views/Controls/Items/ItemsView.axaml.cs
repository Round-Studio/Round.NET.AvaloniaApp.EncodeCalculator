﻿using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using FluentAvalonia.UI.Controls;
using Round.NET.AvaloniaApp.EncodeCalculator.Models;
using Round.NET.AvaloniaApp.EncodeCalculator.Models.Config;
using Round.NET.AvaloniaApp.EncodeCalculator.Models.ItemManage;
using Round.NET.AvaloniaApp.EncodeCalculator.Models.ItemManage.ProjectMange;
using Round.NET.AvaloniaApp.EncodeCalculator.Models.Runner;
using Round.NET.AvaloniaApp.EncodeCalculator.Views.Controls.Items.EditItem.CompItems;
using Round.NET.AvaloniaApp.EncodeCalculator.Views.Controls.Items.ItemControls;
using Round.NET.AvaloniaApp.EncodeCalculator.Views.Pages.SubPages;
using Type = Round.NET.AvaloniaApp.EncodeCalculator.Models.Type.Type;

namespace Round.NET.AvaloniaApp.EncodeCalculator.Views.Controls;

public partial class ItemsView : UserControl
{
    public ItemsView()
    {
        InitializeComponent();
        ItemMange.ItemListBox = this.ItemListBox;
        
        Project.NewProject.NewProjectCore();
    }

    private void RunButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var res = RunCalculator.Run();
            Core.SetOutBoxText($"算式：{res.Formula}\n结果：{res.Result}");
        }
        catch
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                var Show = new ContentDialog();
                Show.Title = "运行错误";
                Show.Content = $"发生死亡性运行错误！\n请不要在你的表达式中使用死循环递归！";
                Show.CloseButtonText = "确定";
                Show.DefaultButton = ContentDialogButton.Close;
                Show.ShowAsync(Core.MainWindow);
            });
        }
    }

    private void AboutButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var Show = new ContentDialog();
            Show.Title = "关于此软件";
            Show.Content = new AboutPage();
            Show.CloseButtonText = "确定";
            Show.DefaultButton = ContentDialogButton.Close;
            Show.ShowAsync(Core.MainWindow);
        });
    }

    private void SettingButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var setpage = new SettingPage();
            setpage.Config = Config.MainConfig;
            var Show = new ContentDialog();
            Show.Title = "设置";
            Show.Content = setpage;
            Show.CloseButtonText = "确定";
            Show.PrimaryButtonText = "取消";
            Show.CloseButtonClick += (dialog, args) =>
            {
                Config.MainConfig = setpage.Config;
                Config.SaveConfig();
            };
            Show.DefaultButton = ContentDialogButton.Close;
            Show.ShowAsync(Core.MainWindow);
        });
    }
    private void AddNewItemButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var AddItems = new AddItem();
        
        ContentDialog Show = new ContentDialog();
        Show.Title = "添加新表达式项";
        Show.PrimaryButtonText = "添加";
        Show.CloseButtonText = "取消";
        Show.Content = AddItems;
        
        Show.DefaultButton = ContentDialogButton.Primary;
        Show.PrimaryButtonClick += (dialog, args) =>
        {
            if (!string.IsNullOrWhiteSpace(AddItems.ValueBox.Text))
            {
                ItemMange.AddFuncItem(new ItemMange.FuncItemConfig()
                {
                    Value = AddItems.ValueBox.Text,
                    Name = AddItems.NameBox.Text,
                    Note = AddItems.NoteBox.Text,
                    ClassicValue = AddItems.ValueBox.Text
                });

                Core.SetNowModifyTheStatus(true);
            }
        };
        
        Show.ShowAsync(Core.MainWindow);
    }
    private void AddCompButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var AddItems = new AddCompItem();
        
        ContentDialog Show = new ContentDialog();
        Show.Title = "添加新逻辑表达式项";
        Show.PrimaryButtonText = "添加";
        Show.CloseButtonText = "取消";
        Show.Content = AddItems;
        
        Show.DefaultButton = ContentDialogButton.Primary;
        Show.PrimaryButtonClick += (dialog, args) =>
        {
            ItemMange.AddCompItem(new ItemMange.CompItemConfig()
            {
                Name = AddItems.NameBox.Text,
                Note = AddItems.NoteBox.Text,
                Value2 = AddItems.ValueBox2.Text,
                Value1 = AddItems.ValueBox1.Text,
                CompareType = AddItems.CompareType
            });
        };
        
        Show.ShowAsync(Core.MainWindow);
    }
}