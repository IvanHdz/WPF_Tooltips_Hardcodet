using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Markup;
using System.Windows.Threading;

namespace Hardcodet.ToolTips
{
    /// <summary>
    /// Provides a way to quickly configure a tooltip for arbitrary framework elements.
    /// </summary>
    [ContentProperty("Content")]
    [DefaultProperty("Content")]
    public class ToolTipBehavior : Behavior<FrameworkElement>
    {
        private readonly DispatcherTimer timer;
        private Action timerAction;
        private Popup popup;
        private HeaderedToolTip toolTip;

        #region Content

        /// <summary>
        /// Content Dependency Property
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(ToolTipBehavior),
                                        new FrameworkPropertyMetadata(String.Empty));

        [Description("The main tooltip content.")]
        [Category("Common")]
        [DisplayName("Content")]
        [Bindable(true)]
        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }


        #endregion

        #region ContentStringFormat

        /// <summary>
        ///  ContentStringFormat Dependency Property
        /// </summary>
        public static readonly DependencyProperty ContentStringFormatProperty =
            DependencyProperty.Register(" ContentStringFormat", typeof(string), typeof(ToolTipBehavior),
                new FrameworkPropertyMetadata((string)null));


        [Description("Formatting of a content string.")]
        [Category("Common")]
        [DisplayName("Content StringFormat")]
        [Bindable(true)]
        public string ContentStringFormat
        {
            get { return (string)GetValue(ContentStringFormatProperty); }
            set { SetValue(ContentStringFormatProperty, value); }
        }

        #endregion

        #region Header

        /// <summary>
        /// Header Dependency Property
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Header", typeof (object), typeof (ToolTipBehavior),
                                        new FrameworkPropertyMetadata(String.Empty));

        [Description("Optional tooltip header")]
        [Category("Common")]
        [DisplayName("Header")]
        [Bindable(true)]
        public object Header
        {
            get { return GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        #endregion

        #region HeaderStringFormat

        /// <summary>
        /// HeaderStringFormat Dependency Property
        /// </summary>
        public static readonly DependencyProperty TitleStringFormatProperty =
            DependencyProperty.Register("TitleStringFormat", typeof(string), typeof(ToolTipBehavior),
                new FrameworkPropertyMetadata((string)null));

        [Description("Formatting of the header string.")]
        [Category("Common")]
        [DisplayName("Header StringFormat")]
        public string TitleStringFormat
        {
            get { return (string)GetValue(TitleStringFormatProperty); }
            set { SetValue(TitleStringFormatProperty, value); }
        }

        #endregion

        #region Category

        /// <summary>
        /// Category Dependency Property
        /// </summary>
        public static readonly DependencyProperty CategoryProperty =
            DependencyProperty.Register("Category", typeof(ToolTipCategory), typeof(ToolTipBehavior),
                new FrameworkPropertyMetadata(ToolTipCategory.None));

        /// <summary>
        /// Gets or sets the Category property. This dependency property 
        /// indicates ....
        /// </summary>
        [Bindable(true)]
        [Description("Severity and tooltip icon.")]
        [Category("Common")]
        [DisplayName("Category / Theme")]
        public ToolTipCategory Category
        {
            get { return (ToolTipCategory)GetValue(CategoryProperty); }
            set { SetValue(CategoryProperty, value); }
        }

        #endregion

        #region MaxWidth

        /// <summary>
        /// MaxWidth Dependency Property
        /// </summary>
        public static readonly DependencyProperty MaxWidthProperty =
            DependencyProperty.Register("MaxWidth", typeof(double), typeof(ToolTipBehavior),
                new FrameworkPropertyMetadata(300.0));

        /// <summary>
        /// Gets or sets the MaxWidth property. This dependency property 
        /// indicates the maximum width of a popup.
        /// </summary>
        [Bindable(true)]
        [Description("Maximum width of the popup (text will break to next line).")]
        [DisplayName("Max ToolTip Width")]
        public double MaxWidth
        {
            get { return (double)GetValue(MaxWidthProperty); }
            set { SetValue(MaxWidthProperty, value); }
        }

        #endregion
  
        #region Delay

        /* Keep in mind that changing the delay might mess with the animations */

        public static readonly DependencyProperty DelayProperty =
            DependencyProperty.Register("Delay", typeof(int), typeof(ToolTipBehavior),
                new FrameworkPropertyMetadata(500, OnDelayChanged), ValidateDelay);

        private static bool ValidateDelay(object value)
        {
            var delay = (int) value;
            return delay >= 100;
        }

        private static void OnDelayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var b = (ToolTipBehavior) d;
            b.timer.Interval = TimeSpan.FromMilliseconds(b.Delay);
        }


        [Description("Delay to show / hide tooltips.")]
        [Bindable(true)]
        [DisplayName("ToolTip Delay")]
        public int Delay
        {
            get { return (int)GetValue(DelayProperty); }
            set { SetValue(DelayProperty, value); }
        }

        #endregion

        #region IsInteractive


        public static readonly DependencyProperty IsInteractiveProperty =
            DependencyProperty.Register("IsInteractive", typeof(bool), typeof(ToolTipBehavior),
                new FrameworkPropertyMetadata(true, OnIsInteractiveChanged));

        [Bindable(true)]
        [DisplayName("Clickable ToolTip")]
        [Description("Whether the ToolTip can be selected / clicked on.")]
        public bool IsInteractive
        {
            get { return (bool)GetValue(IsInteractiveProperty); }
            set { SetValue(IsInteractiveProperty, value); }
        }

        /// <summary>
        /// Handles changes to the IsInteractive property.
        /// </summary>
        private static void OnIsInteractiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ToolTipBehavior)d;
            if (target.toolTip == null) return;

            target.toolTip.IsHitTestVisible = target.IsInteractive;
        }

        #endregion

        #region IsEnabled

        /// <summary>
        /// IsEnabled Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register("IsEnabled", typeof(bool), typeof(ToolTipBehavior),
                new FrameworkPropertyMetadata(true, OnIsEnabledChanged));


        [Bindable(true)]
        [Description("Optionally deactivates ToolTips.")]
        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (ToolTipBehavior) d;
            if (!behavior.IsEnabled)
            {
                //hide tooltip
                behavior.HideAndClose();
            }
        }

        #endregion

        


        public ToolTipBehavior()
        {
            timer = new DispatcherTimer(TimeSpan.FromMilliseconds(Delay), DispatcherPriority.Normal, OnTimerElapsed,
                                        Dispatcher);
            timer.IsEnabled = false;
        }


        /// <summary>
        /// Inits the helper popup and tooltip controls.
        /// </summary>
        private void InitControls()
        {
            popup = new Popup
                {
                    Placement = PlacementMode.Mouse, //note: set popup.PlacementTarget to the AssociatedObject, the tooltip will derive formatting
                    HorizontalOffset = 5,
                    VerticalOffset = 5,
                    AllowsTransparency = true
                };

            popup.MouseEnter += OnPopupMouseEnter;
            popup.MouseLeave += OnPopupMouseLeave;

            //hook up the popup with the data context of it's associated object
            //in case we have content with bindings
            var binding = new Binding
            {
                Path = new PropertyPath(FrameworkElement.DataContextProperty),
                Mode = BindingMode.OneWay,
                Source = AssociatedObject
            };
            BindingOperations.SetBinding(popup, FrameworkElement.DataContextProperty, binding);

            //if the content is itself already a tooltip control, don't wrap it
            //-> any other properties will be ignored!
            toolTip = Content as HeaderedToolTip ?? CreateWrapperControl();

            popup.Child = toolTip;
            toolTip.IsHitTestVisible = IsInteractive;

            //force template application so we can switch visual states
            toolTip.ApplyTemplate();
            VisualStateManager.GoToState(toolTip, "Closed", false);
        }


        /// <summary>
        /// Creates a <see cref="HeaderedToolTip"/> control and wires it's properties
        /// with the behavior through data binding.
        /// </summary>
        private HeaderedToolTip CreateWrapperControl()
        {
            //create wrapper tooltip control
            var tt = new HeaderedToolTip();

            //wire tooltip content
            CreateBinding(ContentProperty, ContentControl.ContentProperty, tt);

            //wire tooltip content string format
            CreateBinding(ContentStringFormatProperty, ContentControl.ContentStringFormatProperty, tt);

            //wire tooltip title
            CreateBinding(TitleProperty, HeaderedContentControl.HeaderProperty, tt);

            //wire title string format
            CreateBinding(TitleStringFormatProperty, HeaderedContentControl.HeaderStringFormatProperty, tt);

            //wire category
            CreateBinding(CategoryProperty, HeaderedToolTip.CategoryProperty, tt);

            //wire max width
            CreateBinding(MaxWidthProperty, FrameworkElement.MaxWidthProperty, tt);

            return tt;
        }

        private void CreateBinding(DependencyProperty behaviorProperty, DependencyProperty targetProperty, DependencyObject target)
        {
            //wire max width
            var binding = new Binding
            {
                Path = new PropertyPath(behaviorProperty),
                Mode = BindingMode.OneWay,
                Source = this
            };
            BindingOperations.SetBinding(target, targetProperty, binding);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.MouseEnter += OnMouseEnter;
            AssociatedObject.MouseLeave += OnMouseLeave;
        }

        
        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (!IsEnabled) return;

            //if the popup is still open, open it immediatly
            if (popup != null && popup.IsOpen)
            {
                Schedule(() => { }); //reset schedule
                VisualStateManager.GoToState(toolTip, "Showing", true);
            }
            else
            {
                Schedule(() =>
                    {
                        popup.IsOpen = true; //show, then transition
                        VisualStateManager.GoToState(toolTip, "Showing", true);
                    });
            }
        }

        private void OnPopupMouseEnter(object sender, MouseEventArgs e)
        {
            timer.Stop(); //suppress any other actions
            VisualStateManager.GoToState(toolTip, "Active", true); //the popup is still open

            //register an event handler for the deactivation event of the parent window
            //-> prevents the popup from remaining open if the window is being deactivated
            var parentWindow = Window.GetWindow(AssociatedObject);
            if (parentWindow != null)
            {
                parentWindow.Deactivated += OnParentWindowDeactivated;
            }
        }


        /// <summary>
        /// Closes the popup immediately, if the parent window was deactivated.
        /// Prevents the popup from hovering over other applications (popups
        /// are topmost).
        /// </summary>
        private void OnParentWindowDeactivated(object sender, EventArgs e)
        {
            //immediately close popup
            timer.Stop();

            var parentWindow = (Window)sender;
            parentWindow.Deactivated -= OnParentWindowDeactivated;

            popup.IsOpen = false;
            VisualStateManager.GoToState(toolTip, "Closed", false);
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            HideAndClose();
        }
        
        /// <summary>
        /// Transitions the popup into a closed state.
        /// </summary>
        private void HideAndClose()
        {
            if (popup == null) return;

            //start fading immediately if animation is programmed that way
            VisualStateManager.GoToState(toolTip, "Hiding", true);
            
            Schedule(() =>
                {
                    VisualStateManager.GoToState(toolTip, "Closed", true);
                    popup.IsOpen = false;
                });
        }

        private void OnPopupMouseLeave(object sender, MouseEventArgs e)
        {
            //remove event handler for the deactivation event of the parent window
            var parentWindow = Window.GetWindow(AssociatedObject);
            if (parentWindow != null)
            {
                parentWindow.Deactivated -= OnParentWindowDeactivated;
            }

            Schedule(() =>
            {
                VisualStateManager.GoToState(toolTip, "Hiding", true); //switch with a delay when leaving the popup
                Schedule(() =>
                    {
                        popup.IsOpen = false;
                    }); //close with yet another delay
            });
        }


        /// <summary>
        /// Schedules an action to be executed on the next timer tick. Resets the timer
        /// and replaces any other pending action.
        /// </summary>
        /// <remarks>
        /// Customize this if you need custom delays to show / hide / fade tooltips by
        /// simply changing the timer interval depending on the state change.
        /// </remarks>
        private void Schedule(Action action)
        {
            lock (timer)
            {
                timer.Stop();
                if (popup == null) InitControls();

                timerAction = action;
                timer.Start();
            }
        }


        /// <summary>
        /// Performs a pending action.
        /// </summary>
        private void OnTimerElapsed(object sender, EventArgs eventArgs)
        {
            lock (timer)
            {
                timer.Stop();

                var action = timerAction;
                timerAction = null;

                //cache action and set timer action to null before invoking
                //(that action could cause the timeraction to be reassigned)
                if (action != null) action();
            }
        }
    }
}