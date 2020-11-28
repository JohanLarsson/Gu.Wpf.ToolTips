﻿namespace Gu.Wpf.ToolTips
{
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Exposes the internal PopupControlService via reflection.
    /// </summary>
    public static class PopupControlService
    {
        private static readonly object Service = typeof(FrameworkElement).GetProperty("PopupControlService", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null) ?? throw new InvalidOperationException("Did not find property PopupControlService");
#pragma warning disable REFL009, GU0006, INPC013 // The referenced member is not known to exist.
        private static readonly MethodInfo RaiseToolTipClosingEventMethod = GetMethod("RaiseToolTipClosingEvent");
        private static readonly MethodInfo RaiseToolTipOpeningEventMethod = GetMethod("RaiseToolTipOpeningEvent");
        private static readonly MethodInfo ResetToolTipTimerMethod = GetMethod("ResetToolTipTimer");

        private static readonly PropertyInfo LastObjectWithToolTipProperty = GetProperty("LastObjectWithToolTip");
        private static readonly PropertyInfo LastMouseOverWithToolTipProperty = GetProperty("LastMouseOverWithToolTip");
        private static readonly PropertyInfo LastMouseDirectlyOverProperty = GetProperty("LastMouseDirectlyOver");
        private static readonly PropertyInfo LastCheckedProperty = GetProperty("LastChecked");
#pragma warning restore REFL009, GU0006, INPC013  // The referenced member is not known to exist.

        private static DependencyObject? LastObjectWithToolTip
        {
            get => (DependencyObject?)LastObjectWithToolTipProperty.GetValue(Service);
            set => LastObjectWithToolTipProperty.SetValue(Service, value);
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static DependencyObject? LastMouseOverWithToolTip
        {
            get => (DependencyObject?)LastMouseOverWithToolTipProperty.GetValue(Service);
            set => LastMouseOverWithToolTipProperty.SetValue(Service, value);
        }

        private static DependencyObject? LastMouseDirectlyOver
        {
            get => (DependencyObject?)LastMouseDirectlyOverProperty.GetValue(Service);
            set => LastMouseDirectlyOverProperty.SetValue(Service, value);
        }

        private static DependencyObject? LastChecked
        {
            get => (DependencyObject?)LastCheckedProperty.GetValue(Service);
            set => LastCheckedProperty.SetValue(Service, value);
        }
#pragma warning restore IDE0052 // Remove unread private members

        /// <summary>
        /// Shows the <see cref="ToolTip"/> for <paramref name="element"/> like if it was hovered with mouse.
        /// </summary>
        /// <param name="element">The <see cref="UIElement"/>.</param>
        public static void ShowToolTip(UIElement element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (LastObjectWithToolTip is { })
            {
                RaiseToolTipClosingEvent(reset: true);
                LastMouseOverWithToolTip = null;
            }

            LastMouseDirectlyOver = element;
            LastChecked = element;
            LastObjectWithToolTip = element;
            ResetToolTipTimerMethod.Invoke(Service, null);
            RaiseToolTipOpeningEvent(fromKeyboard: false);
        }

        private static void RaiseToolTipClosingEvent(bool reset) => RaiseToolTipClosingEventMethod.Invoke(Service, new object[] { reset });

        private static void RaiseToolTipOpeningEvent(bool fromKeyboard) => RaiseToolTipOpeningEventMethod.Invoke(Service, new object[] { fromKeyboard });

        private static MethodInfo GetMethod(string name) => Service.GetType().GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance)
                                                            ?? throw new InvalidOperationException($"Did not find method {name}");

        private static PropertyInfo GetProperty(string name) => Service.GetType().GetProperty(name, BindingFlags.NonPublic | BindingFlags.Instance)
                                                                ?? throw new InvalidOperationException($"Did not find method {name}");
    }
}
