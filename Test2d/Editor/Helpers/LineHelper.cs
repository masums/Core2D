﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class LineHelper : Helper
    {
        private Editor _editor;
        private State _currentState = State.None;
        private XLine _shape;

        private ShapeStyle _style;
        private double _pointEllipseRadius = 3.0;
        private XEllipse _ellipseStart;
        private XEllipse _ellipseEnd;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="editor"></param>
        public LineHelper(Editor editor)
        {
            _editor = editor;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TryToConnectStart(XLine line, double x, double y)
        {
            var result = ShapeBounds.HitTest(_editor.Project.CurrentContainer, new Vector2(x, y), _editor.Project.Options.HitTreshold);
            if (result != null && result is XPoint)
            {
                line.Start = result as XPoint;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TryToConnectEnd(XLine line, double x, double y)
        {
            var result = ShapeBounds.HitTest(_editor.Project.CurrentContainer, new Vector2(x, y), _editor.Project.Options.HitTreshold);
            if (result != null && result is XPoint)
            {
                line.End = result as XPoint;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void LeftDown(double x, double y)
        {
            double sx = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case State.None:
                    {
                        _shape = XLine.Create(
                            sx, sy,
                            _editor.Project.CurrentStyleGroup.CurrentStyle,
                            _editor.Project.Options.PointShape);
                        if (_editor.Project.Options.TryToConnect)
                        {
                            TryToConnectStart(_shape as XLine, sx, sy);
                        }
                        _editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_shape);
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateOne();
                        Move(_shape);
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        _currentState = State.One;
                    }
                    break;
                case State.One:
                    {
                        var line = _shape as XLine;
                        if (line != null)
                        {
                            line.End.X = sx;
                            line.End.Y = sy;
                            if (_editor.Project.Options.TryToConnect)
                            {
                                TryToConnectEnd(_shape as XLine, sx, sy);
                            }
                            _editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_shape);
                            Remove();
                            Finalize(_shape);
                            _editor.History.Snapshot(_editor.Project);
                            _editor.Project.CurrentContainer.CurrentLayer.Shapes.Add(_shape);
                            //_editor.Project.CurrentContainer.Invalidate();
                            _currentState = State.None;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void LeftUp(double x, double y)
        {
            double sx = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void RightDown(double x, double y)
        {
            double sx = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case State.None:
                    break;
                case State.One:
                    {
                        _editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_shape);
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Remove();
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        _currentState = State.None;
                    }
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void RightUp(double x, double y)
        {
            double sx = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void Move(double x, double y)
        {
            double sx = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case State.None:
                    break;
                case State.One:
                    {
                        var line = _shape as XLine;
                        if (line != null)
                        {
                            line.End.X = sx;
                            line.End.Y = sy;
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_shape);
                            _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void ToStateOne()
        {
            _style = _editor.Project.Options.HelperStyle;
            _ellipseStart = XEllipse.Create(0, 0, _style, null, true);
            _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_ellipseStart);
            _ellipseEnd = XEllipse.Create(0, 0, _style, null, true);
            _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_ellipseEnd);
        }
        
        /// <summary>
        /// 
        /// </summary>
        public override void ToStateTwo()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override void ToStateThree()
        {
        }
        
        /// <summary>
        /// 
        /// </summary>
        public override void ToStateFour()
        {
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        public override void Move(BaseShape shape)
        {
            if (_ellipseStart != null)
            {
                _ellipseStart.TopLeft.X = _shape.Start.X - _pointEllipseRadius;
                _ellipseStart.TopLeft.Y = _shape.Start.Y - _pointEllipseRadius;
                _ellipseStart.BottomRight.X = _shape.Start.X + _pointEllipseRadius;
                _ellipseStart.BottomRight.Y = _shape.Start.Y + _pointEllipseRadius;
            }

            if (_ellipseEnd != null)
            {
                _ellipseEnd.TopLeft.X = _shape.End.X - _pointEllipseRadius;
                _ellipseEnd.TopLeft.Y = _shape.End.Y - _pointEllipseRadius;
                _ellipseEnd.BottomRight.X = _shape.End.X + _pointEllipseRadius;
                _ellipseEnd.BottomRight.Y = _shape.End.Y + _pointEllipseRadius;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        public override void Finalize(BaseShape shape)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Remove()
        {
            if (_ellipseStart != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_ellipseStart);
                _ellipseStart = null;
            }

            if (_ellipseEnd != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_ellipseEnd);
                _ellipseEnd = null;
            }
        }
    }
}
