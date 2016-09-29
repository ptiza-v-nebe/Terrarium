//------------------------------------------------------------------------------  
//      Copyright (c) Microsoft Corporation.  All rights reserved.                                                             
//------------------------------------------------------------------------------


using DxVBLib;
using System;
using System.Drawing;
using Terrarium.Renderer.Engine;

namespace Terrarium.Renderer.DirectX
{
    /// <summary>
    ///  Encapsulate the logic for implementing a sprite surface capable
    ///  of rendering animated sprites.
    /// </summary>
    public class DirectDrawSpriteSurface
    {
        /// <summary>
        ///  The number of animation frames on the
        ///  sheet.
        /// </summary>
        private readonly int animationFrames;

        /// <summary>
        ///  The number of separate animation types
        ///  on the sheet.
        /// </summary>
        private readonly int animationTypes;

        /// <summary>
        ///  The real DirectDraw surface pointer
        /// </summary>
        private readonly DirectDrawSurface ddsurface;

        /// <summary>
        ///  The height of a single frame of animation.
        /// </summary>
        private readonly int frameHeight;

        /// <summary>
        ///  The width of a single frame of animation.
        /// </summary>
        private readonly int frameWidth;

        /// <summary>
        ///  The name of this sprite sheet
        /// </summary>
        private readonly string spriteName;

        /// <summary>
        ///  Creates a new sprite sheet with the given name from an image.
        ///  The sheet is broken into sprites given the number of horizontal
        ///  and vertical frames available on this sheet.
        /// </summary>
        /// <param name="spriteName">The name of the sprite sheet</param>
        /// <param name="spriteImagePath">The path to the image to load.</param>
        /// <param name="xFrames">The number of horizontal frames on this sheet.</param>
        /// <param name="yFrames">The number of vertical frames on this sheet.</param>
        public DirectDrawSpriteSurface(string spriteName, string spriteImagePath, int xFrames, int yFrames)
        {
#if TRACE
            GraphicsEngine.Profiler.Start("DirectDrawSpriteSurface..ctor()");
#endif
            this.spriteName = spriteName;

            ddsurface = new DirectDrawSurface(spriteImagePath);
            if (ddsurface == null)
            {
                ddsurface = new DirectDrawSurface(spriteImagePath, DirectDrawSurface.SystemMemorySurfaceDescription);
            }
            ddsurface.TransparencyKey = new ColorKey(DirectDrawSurface.DefaultColorKey);

            animationFrames = xFrames;
            animationTypes = yFrames;

            frameHeight = ddsurface.Rect.Bottom/animationTypes;
            frameWidth = ddsurface.Rect.Right/animationFrames;
#if TRACE
            GraphicsEngine.Profiler.End("DirectDrawSpriteSurface..ctor()");
#endif
        }

        /// <summary>
        ///  The height of each frame of animation.
        /// </summary>
        public int FrameHeight
        {
            get { return frameHeight; }
        }

        /// <summary>
        ///  The width of each from of animation.
        /// </summary>
        public int FrameWidth
        {
            get { return frameWidth; }
        }

        /// <summary>
        ///  Access to the real DirectDraw surface used to create this sprite surface.
        /// </summary>
        public DirectDrawSurface SpriteSurface
        {
            get { return ddsurface; }
        }

        /// <summary>
        ///  The name of this sprite surface.
        /// </summary>
        public string SpriteName
        {
            get { return spriteName; }
        }

        /// <summary>
        ///  Grab a sprite given the x,y frame offset
        /// </summary>
        /// <param name="xFrame">Retrieve the Xth horizontal frame.</param>
        /// <param name="yFrame">Retrieve the Yth vertical frame.</param>
        /// <returns></returns>
        public RECT GrabSprite(int xFrame, int yFrame)
        {
#if TRACE
            GraphicsEngine.Profiler.Start("DirectDrawSpriteSurface.GrabSprite(int, int)");
#endif
            if (xFrame < 0 || xFrame >= animationFrames ||
                yFrame < 0 || yFrame >= animationTypes)
            {
                throw new Exception("Sprite request is out of range");
            }

            var spriteRect = new RECT();
            spriteRect.Top = yFrame*frameHeight;
            spriteRect.Bottom = spriteRect.Top;
            spriteRect.Bottom += frameHeight;

            spriteRect.Left = xFrame*frameWidth;
            spriteRect.Right = spriteRect.Left;
            spriteRect.Right += frameWidth;

#if TRACE
            GraphicsEngine.Profiler.End("DirectDrawSpriteSurface.GrabSprite(int, int)");
#endif
            return spriteRect;
        }

        /// <summary>
        ///  Retreive a sprite that has to be drawn within the given destination
        ///  rectangle, given the bounds of the viewport.
        /// </summary>
        /// <param name="xFrame">Retrieve the Xth horizontal frame.</param>
        /// <param name="yFrame">Retrieve the Yth vertical frame.</param>
        /// <param name="dest">The destination rectangle for the sprite.</param>
        /// <param name="bounds">The view rectangle bounds.</param>
        /// <returns></returns>
        public DirectDrawClippedRect GrabSprite(int xFrame, int yFrame, Rectangle dest, Rectangle bounds)
        {
#if TRACE
            GraphicsEngine.Profiler.Start("DirectDrawSpriteSurface.GrabSprite(int, int, RECT, RECT)");
#endif
            if (xFrame < 0 || xFrame >= animationFrames ||
                yFrame < 0 || yFrame >= animationTypes)
            {
                throw new Exception("Sprite request is out of range");
            }

            var spriteRect = new RECT();
            spriteRect.Top = yFrame * frameHeight;
            spriteRect.Bottom = spriteRect.Top;
            spriteRect.Bottom += frameHeight;

            spriteRect.Left = xFrame * frameWidth;
            spriteRect.Right = spriteRect.Left;
            spriteRect.Right += frameWidth;

            var ddClipRect = new DirectDrawClippedRect();
            ddClipRect.Destination = dest;
            ddClipRect.Source = Rectangle.FromLTRB(spriteRect.Left, spriteRect.Top, spriteRect.Right, spriteRect.Bottom);

            if (dest.Left >= bounds.Right || dest.Right <= bounds.Left || dest.Top >= bounds.Bottom ||
                dest.Bottom <= bounds.Top)
            {
                ddClipRect.Invisible = true;
#if TRACE
                GraphicsEngine.Profiler.End("DirectDrawSpriteSurface.GrabSprite(int, int, RECT, RECT)");
#endif
                return ddClipRect;
            }

            ddClipRect.ClipRectBounds(dest, bounds);
#if TRACE
            GraphicsEngine.Profiler.End("DirectDrawSpriteSurface.GrabSprite(int, int, RECT, RECT)");
#endif
            return ddClipRect;
        }

        /// <summary>
        ///  Retreive a sprite that has to be drawn within the given destination
        ///  rectangle, given the bounds of the viewport.  Also contains a scaling
        ///  factor.
        /// </summary>
        /// <param name="xFrame">Retrieve the Xth horizontal frame.</param>
        /// <param name="yFrame">Retrieve the Yth vertical frame.</param>
        /// <param name="dest">The destination rectangle for the sprite.</param>
        /// <param name="bounds">The view rectangle bounds.</param>
        /// <param name="factor">A scaling factor.</param>
        /// <returns></returns>
        public DirectDrawClippedRect GrabSprite(int xFrame, int yFrame, Rectangle dest, Rectangle bounds, int factor)
        {
#if TRACE
            GraphicsEngine.Profiler.Start("DirectDrawSpriteSurface.GrabSprite(int, int, RECT, RECT, int)");
#endif
            if (xFrame < 0 || xFrame >= animationFrames ||
                yFrame < 0 || yFrame >= animationTypes)
            {
                throw new Exception("Sprite request is out of range");
            }

            var spriteRect = new Rectangle(xFrame, yFrame, frameWidth, frameHeight);

            var ddClipRect = new DirectDrawClippedRect();
            ddClipRect.Destination  = dest;
            ddClipRect.Source       = spriteRect;

            if (dest.Left >= bounds.Right || dest.Right <= bounds.Left || dest.Top >= bounds.Bottom ||
                dest.Bottom <= bounds.Top)
            {
                ddClipRect.Invisible = true;
#if TRACE
                GraphicsEngine.Profiler.End("DirectDrawSpriteSurface.GrabSprite(int, int, RECT, RECT, int)");
#endif
                return ddClipRect;
            }
            ddClipRect.ClipRectBounds(dest, bounds, factor);

#if TRACE
            GraphicsEngine.Profiler.End("DirectDrawSpriteSurface.GrabSprite(int, int, RECT, RECT, int)");
#endif
            return ddClipRect;
        }
    }
}