//------------------------------------------------------------------------------
//      Copyright (c) Microsoft Corporation.  All rights reserved.                                                             
//------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Terrarium.Glass;

namespace Terrarium.Forms.Classes.Controls
{
    public partial class DeveloperPanel : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public DeveloperPanel()
        {
            InitializeComponent();
            this.BackColor = GlassStyleManager.Active.DialogColor;
        
            // Hook up the Leds
            leds[0] = this.ledOne;
            leds[1] = this.ledTwo;
            leds[2] = this.ledThree;
            leds[3] = this.ledFour;
            foreach (TerrariumLed led in leds)
                led.Enabled = false;
        }

        #region STATUS SUPPORT
        /// <summary>
        /// 
        /// </summary>
        public string GameModeText
        {
            get
            {
                return this.gameModeLabel.Text;
            }
            set
            {
                this.gameModeLabel.Text = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int AnimalCount
        {
            get
            {
                return Convert.ToInt32(this.animalCountLabel.Text);
            }
            set
            {
                this.animalCountLabel.Text = value.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MaximumAnimalCount
        {
            get
            {
                return Convert.ToInt32(this.maximumAnimalCountLabel.Text);
            }
            set
            {
                this.maximumAnimalCountLabel.Text = value.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int PlantCount
        {
            get
            {
                return Convert.ToInt32(this.teleportationsCountLabel.Text);
            }
            set
            {
                this.teleportationsCountLabel.Text = value.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int PeerCount
        {
            get
            {
                return Convert.ToInt32(this.peerCountLabel.Text);
            }
            set
            {
                this.peerCountLabel.Text = value.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string WebRoot
        {
            get
            {
                return this.webRootLabel.Text;
            }
            set
            {
                this.webRootLabel.Text = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Teleportations
        {
            get
            {
                return Convert.ToInt32(this.teleportationsCountLabel.Text);
            }
            set
            {
                this.teleportationsCountLabel.Text = value.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int FailedSends
        {
            get
            {
                return Convert.ToInt32(this.failedSendsCountLabel.Text);
            }
            set
            {
                this.failedSendsCountLabel.Text = value.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int FailedReceives
        {
            get
            {
                return Convert.ToInt32(this.failedReceivesCountLabel.Text);
            }
            set
            {
                this.failedReceivesCountLabel.Text = value.ToString();
            }
        }

        #endregion

        #region LED SUPPORT

        private TerrariumLed[] leds = new Terrarium.Forms.TerrariumLed[4];

        /// <summary>
        ///  Provides quick array based access to fall 5 
        ///  status LEDs within the TerrariumTopPanel.
        /// </summary>
        [Browsable(false)]
        public TerrariumLed[] Leds
        {
            get
            {
                return leds;
            }
        }

        #endregion

        #region MINI MAP SUPPORT

        /// <summary>
        ///  The actual land size in pixels as determined by
        ///  the Terrarium Game Engine
        /// </summary>
        private Size landSize;
        /// <summary>
        ///  The actual size of the viewport as determined
        ///  during initial form setup of the primary
        ///  Terrarium gaming form.
        /// </summary>
        private Size viewPortSize;
        /// <summary>
        ///  The size of the viewport scaled onto the map
        ///  and navigation window.
        /// </summary>
        private Size viewPortSizeOnMap;
        /// <summary>
        ///  The location of the current Terrarium viewport
        ///  with respect to the entire map.  Used to map
        ///  the viewport onto the mapping window.
        /// </summary>
        private Point mapLocation;
        /// <summary>
        ///  A bitmap used to display a small representation
        ///  of the game area on a minimap.  This Bitmap
        ///  is generated by the GraphicsEngine classes.
        /// </summary>
        private Bitmap minimap;

        /// <summary>
        ///  Access to the NavigatePictureBox
        /// </summary>
        [Browsable(false)]
        public PictureBox NavigatePictureBox
        {
            get
            {
                return navigatePictureBox;
            }
        }

        /// <summary>
        ///  Allows the setting of a bitmap image to use for the minimap
        /// </summary>
        [Browsable(false)]
        public Bitmap MiniMap
        {
            set
            {
                minimap = value;
            }
        }

        /// <summary>
        ///  Sets the size of the view port
        /// </summary>
        [Browsable(false)]
        public Size ViewPortSize
        {
            set
            {
                viewPortSize = value;
                InvalidateViewportRect();
            }
        }

        /// <summary>
        ///  Sets the actual size of the land
        /// </summary>
        [Browsable(false)]
        public Size LandSize
        {
            get
            {
                return landSize;
            }

            set
            {
                landSize = value;
                InvalidateViewportRect();
            }
        }

        /// <summary>
        ///  The current map location within the Terrarium game view
        /// </summary>
        [Browsable(false)]
        public Point MapLocation
        {
            get
            {
                return mapLocation;
            }

            set
            {
                mapLocation = new Point((value.X * this.navigatePictureBox.Width) / landSize.Width,
                    (value.Y * this.navigatePictureBox.Height) / landSize.Height);
                this.navigatePictureBox.Invalidate();
            }
        }

        private void InvalidateViewportRect()
        {
            viewPortSizeOnMap = new Size((viewPortSize.Width * this.navigatePictureBox.Width) / landSize.Width,
                (viewPortSize.Height * this.navigatePictureBox.Height) / landSize.Height);
        }

        /// <summary>
        ///  navigatePictureBox event handler that controls painting of the
        ///  map onto the mini map.
        /// </summary>
        /// <param name="sender">PictureBox</param>
        /// <param name="e">Graphics context objects</param>
        private void navigatePictureBox_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (minimap != null)
            {
                e.Graphics.DrawImage((Image)minimap, this.navigatePictureBox.ClientRectangle);
            }

            int width = ((viewPortSizeOnMap.Width + mapLocation.X) >= this.navigatePictureBox.Width) ? ((this.navigatePictureBox.Width - mapLocation.X) - 1) : viewPortSizeOnMap.Width;
            int height = ((viewPortSizeOnMap.Height + mapLocation.Y) >= this.navigatePictureBox.Height) ? ((this.navigatePictureBox.Height - mapLocation.Y) - 1) : viewPortSizeOnMap.Height;

            e.Graphics.DrawRectangle(Pens.White, mapLocation.X, mapLocation.Y, width-1, height-1);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="worldWidth"></param>
        /// <param name="worldHeight"></param>
        public void GenerateMiniMap(int worldWidth, int worldHeight)
        {
            Image background = Image.FromFile(System.IO.Path.Combine(Terrarium.Configuration.GameConfig.MediaDirectory, "background.bmp"));

            Bitmap mapImage = new Bitmap(background, navigatePictureBox.Size);

            int tileX = worldWidth / background.Width;
            int tileY = worldHeight / background.Height;

            int tileWidth = mapImage.Width / tileX;
            int tileHeight = mapImage.Height / tileY;

            Graphics mapGraphics = Graphics.FromImage(mapImage);

            for (int x = 0; x < tileX; ++x)
            {
                for (int y = 0; y < tileY; ++y)
                {
                    mapGraphics.DrawImage(background, (x * tileWidth), (y * tileHeight), tileWidth, tileHeight);
                }
            }

            mapGraphics.Dispose();
            background.Dispose();

            this.navigatePictureBox.Image = mapImage;
        }

        private void failedReceivesLabel_Click(object sender, EventArgs e)
        {

        }

        private void glassLabel4_Click(object sender, EventArgs e)
        {

        }

    }
}
