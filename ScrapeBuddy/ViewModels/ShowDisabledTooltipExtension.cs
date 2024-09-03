using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using System.Linq;

namespace ScrapeBuddy.ViewModels
{
    /// <summary>
    /// Provides an attached property to show tooltips on disabled controls in Avalonia.
    /// </summary>
    public static class ShowDisabledTooltipExtension
    {
        #region Constructors

        /// <summary>
        /// Initializes static members of the <see cref="ShowDisabledTooltipExtension"/> class.
        /// </summary>
        static ShowDisabledTooltipExtension()
        {
            ShowOnDisabledProperty.Changed.AddClassHandler<Control>((x, y) => HandleShowOnDisabledChanged(x, y));
        }

        #endregion

        #region Properties

        #region ShowOnDisabled AvaloniaProperty

        /// <summary>
        /// Gets the value of the ShowOnDisabled attached property for a specified <see cref="AvaloniaObject"/>.
        /// </summary>
        /// <param name="obj">The object from which to retrieve the value.</param>
        /// <returns><c>true</c> if tooltips should be shown on disabled controls; otherwise, <c>false</c>.</returns>
        public static bool GetShowOnDisabled(AvaloniaObject obj)
        {
            return obj.GetValue(ShowOnDisabledProperty);
        }

        /// <summary>
        /// Sets the value of the ShowOnDisabled attached property for a specified <see cref="AvaloniaObject"/>.
        /// </summary>
        /// <param name="obj">The object on which to set the value.</param>
        /// <param name="value">The value indicating whether tooltips should be shown on disabled controls.</param>
        public static void SetShowOnDisabled(AvaloniaObject obj, bool value)
        {
            obj.SetValue(ShowOnDisabledProperty, value);
        }

        /// <summary>
        /// Identifies the ShowOnDisabled attached property.
        /// </summary>
        public static readonly AttachedProperty<bool> ShowOnDisabledProperty =
            AvaloniaProperty.RegisterAttached<object, Control, bool>(
                "ShowOnDisabled",
                false, false);

        /// <summary>
        /// Handles changes to the ShowOnDisabled attached property.
        /// </summary>
        /// <param name="control">The control for which the property changed.</param>
        /// <param name="e">The event data.</param>
        private static void HandleShowOnDisabledChanged(Control control, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool isEnabledVal && isEnabledVal)
            {
                control.DetachedFromVisualTree += AttachedControl_DetachedFromVisualOrExtension;
                control.AttachedToVisualTree += AttachedControl_AttachedToVisualTree;
                if (control.IsInitialized)
                {
                    // Enable tooltip after visual is attached
                    AttachedControl_AttachedToVisualTree(control, null);
                }
            }
            else
            {
                AttachedControl_DetachedFromVisualOrExtension(control, null);
            }
        }

        /// <summary>
        /// Handles the control's attachment to the visual tree.
        /// </summary>
        /// <param name="sender">The control that was attached.</param>
        /// <param name="e">The event data.</param>
        private static void AttachedControl_AttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            if (sender is not Control control)
            {
                return;
            }

            var tl = TopLevel.GetTopLevel(control);
            // NOTE: PointerMoved needs to be tunneled in some cases.
            tl.AddHandler(TopLevel.PointerMovedEvent, TopLevel_PointerMoved, RoutingStrategies.Tunnel);
        }

        /// <summary>
        /// Handles the control's detachment from the visual tree.
        /// </summary>
        /// <param name="s">The control that was detached.</param>
        /// <param name="e">The event data.</param>
        private static void AttachedControl_DetachedFromVisualOrExtension(object s, VisualTreeAttachmentEventArgs e)
        {
            if (s is not Control control)
            {
                return;
            }

            control.DetachedFromVisualTree -= AttachedControl_DetachedFromVisualOrExtension;
            control.AttachedToVisualTree -= AttachedControl_AttachedToVisualTree;

            if (TopLevel.GetTopLevel(control) is not TopLevel tl)
            {
                return;
            }

            tl.RemoveHandler(TopLevel.PointerMovedEvent, TopLevel_PointerMoved);
        }

        /// <summary>
        /// Handles the pointer moved event on the top-level control.
        /// </summary>
        /// <param name="sender">The top-level control.</param>
        /// <param name="e">The pointer event data.</param>
        private static void TopLevel_PointerMoved(object sender, global::Avalonia.Input.PointerEventArgs e)
        {
            if (sender is not Control tl)
            {
                return;
            }

            var attachedControls = tl.GetVisualDescendants()
                                     .Where(x => GetShowOnDisabled(x))
                                     .Cast<Control>();

            // Find disabled children under the pointer with this extension enabled
            var disabledChildUnderPointer = attachedControls
                .FirstOrDefault(x =>
                    x.Bounds.Contains(e.GetPosition(x.Parent as Visual)) &&
                    x.IsEffectivelyVisible &&
                    !x.IsEffectivelyEnabled);
            if (disabledChildUnderPointer != null)
            {
                // Manually show the tooltip
                ToolTip.SetIsOpen(disabledChildUnderPointer, true);
            }

            var disabledTooltipsToHide = attachedControls
                .Where(x =>
                    ToolTip.GetIsOpen(x) &&
                    x != disabledChildUnderPointer &&
                    !x.IsEffectivelyEnabled);

            foreach (var dcst in disabledTooltipsToHide)
            {
                ToolTip.SetIsOpen(dcst, false);
            }
        }

        #endregion

        #endregion
    }
}

           
