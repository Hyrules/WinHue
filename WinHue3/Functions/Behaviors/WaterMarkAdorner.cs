using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace WinHue3.Functions.Behaviors
{
    public static class TextBoxFocusBehavior
    {
        #region AttachedProperties

       

        public static string GetWatermarkText(DependencyObject obj)
        {
            return (string)obj.GetValue(WatermarkTextProperty);
        }

        public static void SetWatermarkText(DependencyObject obj, string value)
        {
            obj.SetValue(WatermarkTextProperty, value);
        }

        // Using a DependencyProperty as the backing store for WatermarkText.  This enables animation, styling, binding, etc…
        public static readonly DependencyProperty WatermarkTextProperty =
            DependencyProperty.RegisterAttached("WatermarkText", typeof(string), typeof(TextBoxFocusBehavior),new UIPropertyMetadata(string.Empty, OnWatermarkTextChanged));

        private static void OnWatermarkTextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                tb.Text = (string)e.NewValue;
            }
        }

        public static bool GetIsWaterMarkEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsWaterMarkEnabledProperty);
        }

        public static void SetIsWaterMarkEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsWaterMarkEnabledProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsWaterMarkEnabled.  This enables animation, styling, binding, etc…
        public static readonly DependencyProperty IsWaterMarkEnabledProperty =
            DependencyProperty.RegisterAttached("IsWaterMarkEnabled", typeof(bool), typeof(TextBoxFocusBehavior),new UIPropertyMetadata(false, OnIsWatermarkEnabled));

        private static void OnIsWatermarkEnabled(object sender, DependencyPropertyChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                bool isEnabled = (bool)e.NewValue;
                if (isEnabled)
                {
                    tb.GotFocus += OnInputTextBoxGotFocus;
                    tb.LostFocus += OnInputTextBoxLostFocus;
                }
                else
                {
                    tb.GotFocus -= OnInputTextBoxGotFocus;
                    tb.LostFocus -= OnInputTextBoxLostFocus;
                }
            }
        }
        private static void OnInputTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            var tb = e.OriginalSource as TextBox;
            if (tb != null)
            {
                if (string.IsNullOrEmpty(tb.Text))
                {
                    tb.Text = GetWatermarkText(tb);
                }
            }
        }

        private static void OnInputTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            var tb = e.OriginalSource as TextBox;
            if (tb != null)
            {
                if (tb.Text == GetWatermarkText(tb))
                {
                    tb.Text = string.Empty;
                }
            }
        }
        #endregion AttachedProperties
    }

}
