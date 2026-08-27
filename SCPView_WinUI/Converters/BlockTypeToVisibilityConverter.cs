using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using SCPView_WinUI.Data.Model;
using System;

namespace SCPView_WinUI.Converters
{
    public class BlockTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ContentBlockType type)
            {
                return type == ContentBlockType.Text
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class BlockquoteToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ContentBlockType type)
            {
                return type == ContentBlockType.Blockquote
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
