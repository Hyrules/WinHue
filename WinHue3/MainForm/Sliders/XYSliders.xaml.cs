using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using WinHue3.Colors;

namespace WinHue3.MainForm.Sliders
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>

    public partial class XYSliders : UserControl, ICommandSource
    {
        private string _modelid;

        public XYSliders()
        {
            InitializeComponent();
            XSlider.ValueChanged += XSlider_ValueChanged;
            XSlider.PreviewMouseUp += XSlider_PreviewMouseUp;
            XSlider.OnOldValueChanged += XSlider_OnOldValueChanged;
            YSlider.ValueChanged += YSlider_ValueChanged;
            YSlider.PreviewMouseUp += YSlider_PreviewMouseUp;
            YSlider.OnOldValueChanged += YSlider_OnOldValueChanged;
        }

        private void XSlider_OnOldValueChanged(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            OldXValue = XSlider.OldValue;
        }

        private void YSlider_OnOldValueChanged(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            OldYValue = YSlider.OldValue;
        }

        private void YSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            
            RunCommand();
        }

        private void XSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {

            RunCommand();
        }

        private void YSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            YValue = e.NewValue;
            HueColorConverter.ColorFromXY(new CGPoint(XValue, YValue), _modelid);
            recColorXY.Fill = new SolidColorBrush(HueColorConverter.ColorFromXY(new CGPoint((float) XValue,(float) YValue), _modelid));
        }

        private void XSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            XValue = e.NewValue;
            recColorXY.Fill = new SolidColorBrush(HueColorConverter.ColorFromXY(new CGPoint((float)XValue, (float)YValue), _modelid));
        }



        public double OldXValue
        {
            get => (int)GetValue(OldXValueProperty);
            set => SetValue(OldXValueProperty, value);
        }

        // Using a DependencyProperty as the backing store for OldXValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OldXValueProperty =
            DependencyProperty.Register("OldXValue", typeof(double), typeof(XYSliders), new PropertyMetadata(default(double)));

        public double OldYValue
        {
            get => (double)GetValue(OldYValueProperty);
            set => SetValue(OldYValueProperty, value);
        }

        // Using a DependencyProperty as the backing store for OldYValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OldYValueProperty =
            DependencyProperty.Register("OldYValue", typeof(double), typeof(XYSliders), new PropertyMetadata(default(double)));



        public string XSliderLabel
        {
            get => (string)GetValue(XSliderLabelProperty);
            set => SetValue(XSliderLabelProperty, value);
        }

        // Using a DependencyProperty as the backing store for XSliderLabel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XSliderLabelProperty = DependencyProperty.Register("XSliderLabel", typeof(string), typeof(XYSliders), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, XSliderLabelPropertyChanged));

        private static void XSliderLabelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            XYSliders control = (XYSliders)d;
            control.lblXColorSpace.Content = e.NewValue;
        }

        public string YSliderLabel
        {
            get => (string)GetValue(YSliderLabelProperty);
            set => SetValue(YSliderLabelProperty, value);
        }

        // Using a DependencyProperty as the backing store for YSliderLabel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YSliderLabelProperty = DependencyProperty.Register("YSliderLabel", typeof(string), typeof(XYSliders), new FrameworkPropertyMetadata(string.Empty,FrameworkPropertyMetadataOptions.BindsTwoWayByDefault ,YSliderLabelPropertyChanged) { BindsTwoWayByDefault = true, DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged});

        private static void YSliderLabelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            XYSliders control = (XYSliders)d;
            control.lblYColorSpace.Content = e.NewValue;
        }

        public double XValue
        {
            get => (double)GetValue(XSliderValueProperty);
            set => SetValue(XSliderValueProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XSliderValueProperty = DependencyProperty.Register("XValue", typeof(double), typeof(XYSliders), new PropertyMetadata(default(double), XSliderValuePropertyChanged));

        private static void XSliderValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            XYSliders slider = (XYSliders) d;
            slider.XSlider.Value = Convert.ToDouble(e.NewValue);
        }

        public double YValue
        {
            get => (double)GetValue(YValueProperty);
            set => SetValue(YValueProperty, value);
        }
      
        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YValueProperty = DependencyProperty.Register("YValue", typeof(double), typeof(XYSliders), new PropertyMetadata(default(double), YValuePropertyChanged));

        private static void YValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            XYSliders slider = (XYSliders) d;
            slider.YSlider.Value = Convert.ToDouble(e.NewValue);
        }

        #region COMMAND
        public ICommand Command
        {    
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => (object)GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public static readonly DependencyProperty CommandParameterProperty =
        DependencyProperty.Register(
        "CommandParameter",
        typeof(object),
        typeof(XYSliders),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, CommandParameterPropertyChanged) { DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged}
        );

        private static void CommandParameterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            XYSliders slider = (XYSliders) d;
            slider._modelid = e.NewValue?.ToString() ?? string.Empty;
        }

        public IInputElement CommandTarget
        {
            get => (IInputElement)GetValue(CommandTargetProperty);
            set => SetValue(CommandTargetProperty, value);
        }

        public static readonly DependencyProperty CommandTargetProperty =
        DependencyProperty.Register(
        "CommandTarget",
        typeof(IInputElement),
        typeof(XYSliders),
        new UIPropertyMetadata(null)
        );

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                "Command",
                typeof(ICommand),
                typeof(XYSliders),
                new PropertyMetadata(null,
                CommandChanged
                ));

        // Command dependency property change callback.

        private static void CommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            XYSliders cs = (XYSliders)d;
            cs.HookUpCommand((ICommand)e.OldValue, (ICommand)e.NewValue);
        }

        private void HookUpCommand(ICommand oldCommand, ICommand newCommand)
        {
            // If oldCommand is not null, then we need to remove the handlers.
            if (oldCommand != null)
            {
                RemoveCommand(oldCommand, newCommand);
            }
            AddCommand(oldCommand, newCommand);
        }

        // Remove an old command from the Command Property.
        private void RemoveCommand(ICommand oldCommand, ICommand newCommand)
        {
            EventHandler handler = CanExecuteChanged;
            oldCommand.CanExecuteChanged -= handler;
        }

        // Add the command.
        private void AddCommand(ICommand oldCommand, ICommand newCommand)
        {
            EventHandler handler = CanExecuteChanged;
            if (newCommand != null)
            {
                newCommand.CanExecuteChanged += handler;
            }
        }

        private void CanExecuteChanged(object sender, EventArgs e)
        {
            if (this.Command == null) return;
            RoutedCommand command = this.Command as RoutedCommand;

            // If a RoutedCommand.
            this.IsEnabled = command?.CanExecute(CommandParameter, CommandTarget) ?? Command.CanExecute(CommandParameter);
        }

        private void RunCommand()
        {
            if (this.Command == null) return;
            RoutedCommand command = Command as RoutedCommand;

            if (command != null)
            {
                command.Execute(CommandParameter, CommandTarget);
            }
            else
            {
                Command.Execute(CommandParameter);
            }
        }

        #endregion

    }
}
