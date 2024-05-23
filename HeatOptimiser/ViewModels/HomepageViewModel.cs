using System;
using System.Collections.ObjectModel;
using DynamicData;
using HeatOptimiser;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using ReactiveUI;
using SkiaSharp;

namespace UserInterface.ViewModels
{
    public class HomepageViewModel : ViewModelBase
    {
        public int _assetCount;
        public int AssetCount
        {
            get => _assetCount;
            set => this.RaiseAndSetIfChanged(ref _assetCount, value);
        }

       

        

        public HomepageViewModel()
        {
            _assetCount = AssetManager.LoadUnits().Count;

        }
    }
}


