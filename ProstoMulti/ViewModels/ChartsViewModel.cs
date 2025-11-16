using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProstoMulti.Services;

namespace ProstoMulti.ViewModels
{
    public class ChartsViewModel : BaseViewModel
    {
        private readonly ApiService _apiService = new();

        public Chart Chart { get; set; }

        public async Task LoadAsync()
        {
            var items = await _apiService.GetItemsAsync();

            var rnd = new Random();

            var grouped = items
                .GroupBy(i => i.Category)
                .Select(g =>
                {
                    var color = SKColor.FromHsv(rnd.Next(0, 360), 70, 90);
                    return new ChartEntry(g.Count())
                    {
                        Label = g.Key,
                        ValueLabel = g.Count().ToString(),
                        Color = color
                    };
                })
                .ToList();


            Chart = new BarChart { Entries = grouped, LabelTextSize = 32 };
            OnPropertyChanged(nameof(Chart));
        }
    }
}