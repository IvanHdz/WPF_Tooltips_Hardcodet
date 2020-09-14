using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Hardcodet.ToolTips
{
    /// <summary>
    /// Headered content control that is being styled as a ToolTip. Used
    /// visual states for transitions.
    /// </summary>
    [TemplateVisualState(GroupName = "ToolTip", Name = "Closed")]
    [TemplateVisualState(GroupName = "ToolTip", Name = "Showing")]
    [TemplateVisualState(GroupName = "ToolTip", Name = "Hiding")]
    [TemplateVisualState(GroupName = "ToolTip", Name = "Active")]
    [TemplateVisualState(GroupName = "Severity", Name = "None")]
    [TemplateVisualState(GroupName = "Severity", Name = "Informational")]
    [TemplateVisualState(GroupName = "Severity", Name = "Warning")]
    [TemplateVisualState(GroupName = "Severity", Name = "Error")]
    public class HeaderedToolTip : HeaderedContentControl
    {
        static HeaderedToolTip()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HeaderedToolTip), new FrameworkPropertyMetadata(typeof(HeaderedToolTip)));
        }

        #region Category

        /// <summary>
        /// Category Dependency Property
        /// </summary>
        public static readonly DependencyProperty CategoryProperty =
            DependencyProperty.Register("Category", typeof(ToolTipCategory), typeof(HeaderedToolTip),
                                        new FrameworkPropertyMetadata(ToolTipCategory.None, OnCategoryChanged));


        [Description("Sets icon and tooltip color theme.")]
        [Category("Common")]
        [Bindable(true)]
        public ToolTipCategory Category
        {
            get { return (ToolTipCategory)GetValue(CategoryProperty); }
            set { SetValue(CategoryProperty, value); }
        }

        private static void OnCategoryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tt = (HeaderedToolTip)d;

            string stateName = tt.Category.ToString();
            VisualStateManager.GoToState(tt, stateName, true);
        }

        #endregion


        /// <summary>
        /// Triggers visual state once the control template was applied
        /// (switching state too early doesn't trigger anything, since the state
        /// management is in the control templates).
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            string stateName = Category.ToString();
            VisualStateManager.GoToState(this, stateName, true);
        }
    }
}