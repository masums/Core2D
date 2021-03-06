﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Input;
using Avalonia.VisualTree;
using Core2D.Editor;
using Core2D.Editor.Input;

namespace Core2D.UI.Avalonia.Controls
{
    /// <summary>
    /// Provides input events for <see cref="IInputTarget"/>.
    /// </summary>
    public class InputControl : Border
    {
        /// <summary>
        /// Input target property.
        /// </summary>
        public static readonly StyledProperty<IInputTarget> InputTargetProperty =
            AvaloniaProperty.Register<InputControl, IInputTarget>(nameof(InputTarget));

        /// <summary>
        /// Gets or sets input target.
        /// </summary>
        public IInputTarget InputTarget
        {
            get { return GetValue(InputTargetProperty); }
            set { SetValue(InputTargetProperty, value); }
        }

        /// <summary>
        /// Relative to visual property.
        /// </summary>
        public static readonly StyledProperty<IVisual> RelativeToProperty =
            AvaloniaProperty.Register<InputControl, IVisual>(nameof(RelativeTo));

        /// <summary>
        /// Gets or sets relative to visual.
        /// </summary>
        public IVisual RelativeTo
        {
            get { return GetValue(RelativeToProperty); }
            set { SetValue(RelativeToProperty, value); }
        }

        /// <summary>
        /// Transformed visual property.
        /// </summary>
        public static readonly StyledProperty<IVisual> TransformedProperty =
            AvaloniaProperty.Register<InputControl, IVisual>(nameof(Transformed));

        /// <summary>
        /// Gets or sets transformed visual.
        /// </summary>
        public IVisual Transformed
        {
            get { return GetValue(TransformedProperty); }
            set { SetValue(TransformedProperty, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputControl"/> class.
        /// </summary>
        public InputControl()
        {
            PointerPressed += (sender, e) => HandlePointerPressed(e);
            PointerReleased += (sender, e) => HandlePointerReleased(e);
            PointerMoved += (sender, e) => HandlePointerMoved(e);
        }

        private void Capture()
        {
            var mouseDevice = (this.GetVisualRoot() as IInputRoot)?.MouseDevice;
            if (mouseDevice.Captured == null)
            {
                mouseDevice.Capture(this);
            }
        }

        private void Release()
        {
            var mouseDevice = (this.GetVisualRoot() as IInputRoot)?.MouseDevice;
            if (mouseDevice.Captured != null)
            {
                mouseDevice.Capture(null);
            }
        }

        private ModifierFlags GetModifier(InputModifiers inputModifiers)
        {
            var modifier = ModifierFlags.None;

            if (inputModifiers.HasFlag(InputModifiers.Alt))
            {
                modifier |= ModifierFlags.Alt;
            }

            if (inputModifiers.HasFlag(InputModifiers.Control))
            {
                modifier |= ModifierFlags.Control;
            }

            if (inputModifiers.HasFlag(InputModifiers.Shift))
            {
                modifier |= ModifierFlags.Shift;
            }

            return modifier;
        }

        private Point AdjustGetPosition(Point point)
        {
            if (Transformed?.RenderTransform != null)
            {
                return MatrixHelper.TransformPoint(Transformed.RenderTransform.Value.Invert(), point);
            }
            return point;
        }

        private void HandlePointerPressed(PointerPressedEventArgs e)
        {
            if (e.MouseButton == MouseButton.Left)
            {
                if (InputTarget != null)
                {
                    var point = AdjustGetPosition(e.GetPosition(RelativeTo));
                    var args = new InputArgs(point.X, point.Y, GetModifier(e.InputModifiers));
                    if (InputTarget.IsLeftDownAvailable())
                    {
                        InputTarget.LeftDown(args);
                    }
                }
            }
            else if (e.MouseButton == MouseButton.Right)
            {
                if (InputTarget != null)
                {
                    var point = AdjustGetPosition(e.GetPosition(RelativeTo));
                    var args = new InputArgs(point.X, point.Y, GetModifier(e.InputModifiers));
                    if (InputTarget.IsRightDownAvailable())
                    {
                        InputTarget.RightDown(args);
                    }
                }
            }
        }

        private void HandlePointerReleased(PointerReleasedEventArgs e)
        {
            if (e.MouseButton == MouseButton.Left)
            {
                if (InputTarget != null)
                {
                    var point = AdjustGetPosition(e.GetPosition(RelativeTo));
                    var args = new InputArgs(point.X, point.Y, GetModifier(e.InputModifiers));
                    if (InputTarget.IsLeftUpAvailable())
                    {
                        InputTarget.LeftUp(args);
                    }
                }
            }
            else if (e.MouseButton == MouseButton.Right)
            {
                if (InputTarget != null)
                {
                    var point = AdjustGetPosition(e.GetPosition(RelativeTo));
                    var args = new InputArgs(point.X, point.Y, GetModifier(e.InputModifiers));
                    if (InputTarget.IsRightUpAvailable())
                    {
                        InputTarget.RightUp(args);
                    }
                }
            }
        }

        private void HandlePointerMoved(PointerEventArgs e)
        {
            if (InputTarget != null)
            {
                var point = AdjustGetPosition(e.GetPosition(RelativeTo));
                var args = new InputArgs(point.X, point.Y, GetModifier(e.InputModifiers));
                if (InputTarget.IsMoveAvailable())
                {
                    InputTarget.Move(args);
                }
            }
        }
    }
}
