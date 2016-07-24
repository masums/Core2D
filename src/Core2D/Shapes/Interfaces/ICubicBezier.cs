﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Shapes.Interfaces
{
    /// <summary>
    /// Defines cubic bezier shape contract.
    /// </summary>
    public interface ICubicBezier
    {
        /// <summary>
        /// Gets or sets start point.
        /// </summary>
        XPoint Point1 { get; set; }

        /// <summary>
        /// Gets or sets first control point.
        /// </summary>
        XPoint Point2 { get; set; }

        /// <summary>
        /// Gets or sets second control point.
        /// </summary>
        XPoint Point3 { get; set; }

        /// <summary>
        /// Gets or sets end point.
        /// </summary>
        XPoint Point4 { get; set; }
    }
}
