using System;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace Kide.Utils
{
    public class Icon : MarkupExtension
    {
        public Icon(object g) => Geometry = g;

        public byte Size { get; set; } = 12;
        public object Geometry { get; set; }

        public override object ProvideValue(IServiceProvider
            serviceProvider)
        {
            if (Geometry is not Avalonia.Media.Geometry) throw new InvalidOperationException("Geometry is required");
            return new Avalonia.Controls.PathIcon
            {
                Data = Geometry as Geometry,
                Width = Size,
                Height = Size
            };
        }
    }
}