using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using WSplitTimer.Properties;

namespace WSplitTimer
{
    public partial class BackgroundImageDialog : Form
    {
        private WSplit wsplit;

        private Bitmap image;
        private RectangleF imageDisplayRectangle;
        private Timer animationTimer;
        private int currentFrame;

        private Rectangle selectionRectangle;
        private RectangleF selectionDisplayRectangle;

        private float scale;

        private bool imageLoaded = false;
        private string imageFilename = "";
        private readonly Brush grayScreen = new SolidBrush(Color.FromArgb(128, Color.LightGray));

        // Members related to the PictureBox events management
        private bool scrolling = false;

        private Point scrollingCursorStartPoint;
        private PointF scrollingImageStartPoint;
        private PointF scrollingSelectionStartPoint;

        private bool moving = false;
        private Point movingCursorStartPoint;
        private Point movingUnscaledSelectionStartPoint;

        private bool resizingLeft = false;
        private bool resizingRight = false;
        private bool resizingTop = false;
        private bool resizingBottom = false;
        private Rectangle resizingSelectionStartRect;

        private bool overLeft = false;
        private bool overRight = false;
        private bool overTop = false;
        private bool overBottom = false;

        public BackgroundImageDialog()
        {
            InitializeComponent();

            // The following prevents the mouse wheel event from having any effect on the trackbars:
            trackBarZoom.MouseWheel += ((o, e) => ((HandledMouseEventArgs)e).Handled = true);
            trackBarOpacity.MouseWheel += ((o, e) => ((HandledMouseEventArgs)e).Handled = true);

            picBoxImageSelectionModifier.MouseWheel += picBoxImageSelectionModifier_MouseWheel;
        }

        public DialogResult ShowDialog(Form caller, WSplit wsplit)
        {
            this.wsplit = wsplit;

            // If for some reason, a value is not compatible with WSplit, the settings
            // will automatically be brought back to default.
            try
            {
                PopulateSettings();
            }
            catch (Exception)   // Any kind of exception
            {
                RestoreDefaults();
                MessageBoxEx.Show(this,
                    "An error has occurred and your settings were brought back to defaults.",
                    "Defaults Restored", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return base.ShowDialog(wsplit);
        }

        private void PopulateSettings()
        {
            checkBoxUseImageBg.Checked = Settings.Profile.BackgroundImage;
            changeUsedImage(Settings.Profile.BackgroundImageFilename, false);
            selectionRectangle = Settings.Profile.BackgroundImageSelection;
            if (image != null)
            {
                if (selectionRectangle == Rectangle.Empty)
                    ResetSelection();
                FitZoom();
            }

            trackBarOpacity.Value = Settings.Profile.BackgroundOpacity;
            if (Settings.Profile.BackgroundPlain)
                radioButtonPlain.Checked = true;
            else if (Settings.Profile.BackgroundBlack)
                radioButtonBlack.Checked = true;
            else
                radioButtonDefault.Checked = true;
        }

        private void RestoreDefaults()
        {
            Settings.Profile.Reset();
            Settings.Profile.FirstRun = false;
            PopulateSettings();
        }

        public void ApplyChanges()
        {
            Settings.Profile.BackgroundImage = checkBoxUseImageBg.Checked;
            Settings.Profile.BackgroundImageFilename = imageFilename;
            Settings.Profile.BackgroundImageSelection = selectionRectangle;

            Settings.Profile.BackgroundOpacity = trackBarOpacity.Value;
            Settings.Profile.BackgroundPlain = radioButtonPlain.Checked;
            Settings.Profile.BackgroundBlack = radioButtonBlack.Checked;
        }

        private void changeUsedImage(string filename, bool newSelection = true)
        {
            // Image is gonna be changed.
            // The current image and values related to it are set back to default values
            image = null;
            imageLoaded = false;

            if (animationTimer != null)
            {
                animationTimer.Dispose();
                animationTimer = null;
                currentFrame = 0;
            }

            if (filename == "")
            {
                imageFilename = filename;
                textBoxImagePath.ForeColor = SystemColors.WindowText;
                textBoxImagePath.Text = "No image selected";

                labelZoom.Enabled = false;
                labelZoomDisplay.Enabled = false;
                trackBarZoom.Enabled = false;
                buttonZoomFit.Enabled = false;
                buttonAutoSelect.Enabled = false;
                buttonResetSelect.Enabled = false;
                picBoxImageSelectionModifier.Enabled = false;
            }
            else
            {
                try
                {
                    image = new Bitmap(filename);
                    imageFilename = filename;
                    textBoxImagePath.ForeColor = SystemColors.WindowText;
                    textBoxImagePath.Text = filename;

                    // If it's an animated image, sets up the animation correctly.
                    if (image.FrameDimensionsList.Any(fd => fd.Equals(FrameDimension.Time.Guid))
                        && image.GetFrameCount(FrameDimension.Time) > 1)
                    {
                        PropertyItem gifDelay = image.GetPropertyItem(0x5100);

                        animationTimer = new Timer();
                        animationTimer.Interval = BitConverter.ToInt16(gifDelay.Value, 0) * 10;
                        animationTimer.Tick += (o, e) =>
                            {
                                ++currentFrame;
                                if (currentFrame >= image.GetFrameCount(FrameDimension.Time))
                                    currentFrame = 0;

                                image.SelectActiveFrame(FrameDimension.Time, currentFrame);
                                picBoxImageSelectionModifier.Invalidate();
                            };
                        animationTimer.Start();
                    }

                    labelZoom.Enabled = true;
                    labelZoomDisplay.Enabled = true;
                    trackBarZoom.Enabled = true;
                    buttonZoomFit.Enabled = true;
                    buttonAutoSelect.Enabled = true;
                    buttonResetSelect.Enabled = true;
                    picBoxImageSelectionModifier.Enabled = true;

                    if (newSelection)
                    {
                        ResetSelection();
                        FitZoom();
                    }

                    imageLoaded = true;
                }
                catch (Exception)
                {
                    imageFilename = "";
                    textBoxImagePath.ForeColor = Color.Red;
                    textBoxImagePath.Text = "Cannot load image: " + filename;

                    labelZoom.Enabled = false;
                    labelZoomDisplay.Enabled = false;
                    trackBarZoom.Enabled = false;
                    buttonZoomFit.Enabled = false;
                    buttonAutoSelect.Enabled = false;
                    buttonResetSelect.Enabled = false;
                    picBoxImageSelectionModifier.Enabled = false;
                }
            }

            picBoxImageSelectionModifier.Invalidate();
        }

        private void FitZoom()
        {
            // Automatically sets up the optimal way to display the picture and the selection

            // First creates a rectangle that contains both the image and the selection rectangle,
            // at 100% scale, with (0;0) being the top-left corner of the image
            Rectangle contentRectangle = new Rectangle();
            contentRectangle.X = Math.Min(0, selectionRectangle.X);
            contentRectangle.Y = Math.Min(0, selectionRectangle.Y);
            contentRectangle.Width = Math.Max(image.Width, selectionRectangle.X + selectionRectangle.Width) - contentRectangle.X;
            contentRectangle.Height = Math.Max(image.Height, selectionRectangle.Y + selectionRectangle.Height) - contentRectangle.Y;

            // Calculates the scale needed to show the whole picture. Scale is at least 5%, and at most 200%
            scale = Math.Max(5, 39800 / (new int[] { contentRectangle.Width, contentRectangle.Height, 199 }).Max()) / 100f;
            trackBarZoom.Value = (int)(100 * scale);

            // Sets the display rectangles according to the previously calculated scale
            PointF contentDisplayPosition = new PointF(
                (398 - contentRectangle.Width * scale) / 2f,
                (398 - contentRectangle.Height * scale) / 2f);

            imageDisplayRectangle.Width = image.Width * scale;
            imageDisplayRectangle.Height = image.Height * scale;
            imageDisplayRectangle.X = -contentRectangle.X * scale + contentDisplayPosition.X;
            imageDisplayRectangle.Y = -contentRectangle.Y * scale + contentDisplayPosition.Y;

            selectionDisplayRectangle.Width = selectionRectangle.Width * scale;
            selectionDisplayRectangle.Height = selectionRectangle.Height * scale;
            selectionDisplayRectangle.X = (selectionRectangle.X - contentRectangle.X) * scale + contentDisplayPosition.X;
            selectionDisplayRectangle.Y = (selectionRectangle.Y - contentRectangle.Y) * scale + contentDisplayPosition.Y;

            // Tell the PictureBox it needs to refresh
            picBoxImageSelectionModifier.Invalidate();
        }

        private void ResetSelection()
        {
            // For the selection rectangle, (0;0) is the top-left corner of the image.
            selectionRectangle = new Rectangle(0, 0, image.Width, image.Height);
            selectionDisplayRectangle = imageDisplayRectangle;

            // Tell the PictureBox it needs to refresh
            picBoxImageSelectionModifier.Invalidate();
        }

        private void ZoomOnPoint(PointF zoomPoint, float newScale)
        {
            // The zoomPoint must remain in the same spot
            // First find what the zoomPoint is in 100% scale for both the image and the selection
            PointF imageFixPoint = new PointF(((zoomPoint.X - imageDisplayRectangle.X) / scale) * newScale,
                                              ((zoomPoint.Y - imageDisplayRectangle.Y) / scale) * newScale);
            PointF selectionFixPoint = new PointF(((zoomPoint.X - selectionDisplayRectangle.X) / scale) * newScale,
                                                  ((zoomPoint.Y - selectionDisplayRectangle.Y) / scale) * newScale);

            // We can safely change the scale now
            scale = newScale;

            // Then, change the display rectangles to fit the needs
            // Point imageFixPoint has to be at zoomPoint
            imageDisplayRectangle.X = zoomPoint.X - imageFixPoint.X;
            imageDisplayRectangle.Y = zoomPoint.Y - imageFixPoint.Y;
            imageDisplayRectangle.Width = image.Width * scale;
            imageDisplayRectangle.Height = image.Height * scale;

            selectionDisplayRectangle.X = zoomPoint.X - selectionFixPoint.X;
            selectionDisplayRectangle.Y = zoomPoint.Y - selectionFixPoint.Y;
            selectionDisplayRectangle.Width = selectionRectangle.Width * scale;
            selectionDisplayRectangle.Height = selectionRectangle.Height * scale;

            // Tell the PictureBox it needs to refresh
            picBoxImageSelectionModifier.Invalidate();
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                changeUsedImage(openFileDialog.FileName);
        }

        private void picBoxImageSelectionModifier_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.White);

            if (imageLoaded)
            {
                e.Graphics.DrawImage(image, imageDisplayRectangle);

                // Draw 4 semi-transparent rectangles around the selection rectangle:
                e.Graphics.FillRectangle(grayScreen, new RectangleF(
                    0, 0, picBoxImageSelectionModifier.ClientSize.Width,
                    selectionDisplayRectangle.Top));
                e.Graphics.FillRectangle(grayScreen, new RectangleF(
                    0, selectionDisplayRectangle.Top, selectionDisplayRectangle.Left,
                    picBoxImageSelectionModifier.ClientSize.Height - selectionDisplayRectangle.Top));
                e.Graphics.FillRectangle(grayScreen, new RectangleF(
                    selectionDisplayRectangle.Right, selectionDisplayRectangle.Top,
                    picBoxImageSelectionModifier.ClientSize.Width - selectionDisplayRectangle.Right,
                    picBoxImageSelectionModifier.ClientSize.Height - selectionDisplayRectangle.Top));
                e.Graphics.FillRectangle(grayScreen, new RectangleF(
                    selectionDisplayRectangle.Left, selectionDisplayRectangle.Bottom,
                    selectionDisplayRectangle.Width,
                    picBoxImageSelectionModifier.ClientSize.Height - selectionDisplayRectangle.Bottom));

                // There is no method for drawing a single RectangleF...
                e.Graphics.DrawRectangles(Pens.Red, new RectangleF[] { selectionDisplayRectangle });
            }

            // If the picture box is disabled, a semi-transparent dray screen is drawn on it
            if (!picBoxImageSelectionModifier.Enabled)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.LightGray)),
                    picBoxImageSelectionModifier.ClientRectangle);
            }
        }

        private void checkBoxUseImageBg_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxImageBg.Enabled = checkBoxUseImageBg.Checked;

            trackBarOpacity.Enabled = checkBoxUseImageBg.Checked;
            labelOpacity.Enabled = checkBoxUseImageBg.Checked;
            labelOpacityDisplay.Enabled = checkBoxUseImageBg.Checked;

            bool radioButtonsState = !checkBoxUseImageBg.Checked || trackBarOpacity.Value != 0;
            radioButtonDefault.Enabled = radioButtonsState;
            radioButtonPlain.Enabled = radioButtonsState;
            radioButtonBlack.Enabled = radioButtonsState;
        }

        private void trackBarOpacity_ValueChanged(object sender, EventArgs e)
        {
            bool valueNotZero = trackBarOpacity.Value != 0;
            radioButtonDefault.Enabled = valueNotZero;
            radioButtonPlain.Enabled = valueNotZero;
            radioButtonBlack.Enabled = valueNotZero;

            labelOpacityDisplay.Text = (valueNotZero) ? trackBarOpacity.Value + "%" : "Transparent";
        }

        private void trackBarZoom_Scroll(object sender, EventArgs e)
        {
            ZoomOnPoint(new PointF(
                picBoxImageSelectionModifier.ClientSize.Width / 2f,
                picBoxImageSelectionModifier.ClientSize.Height / 2f),
                trackBarZoom.Value / 100f);
        }

        private void trackBarZoom_ValueChanged(object sender, EventArgs e)
        {
            labelZoomDisplay.Text = trackBarZoom.Value + "%";
        }

        private void buttonZoomFit_Click(object sender, EventArgs e)
        {
            FitZoom();
        }

        private void buttonResetSelect_Click(object sender, EventArgs e)
        {
            ResetSelection();
        }

        private void buttonAutoSelect_Click(object sender, EventArgs e)
        {
            selectionRectangle.Size = wsplit.Size;
            selectionRectangle.X = image.Width / 2 - selectionRectangle.Width / 2;
            selectionRectangle.Y = image.Height / 2 - selectionRectangle.Height / 2;

            selectionDisplayRectangle.Width = selectionRectangle.Width * scale;
            selectionDisplayRectangle.Height = selectionRectangle.Height * scale;
            selectionDisplayRectangle.X = selectionRectangle.X * scale + imageDisplayRectangle.X;
            selectionDisplayRectangle.Y = selectionRectangle.Y * scale + imageDisplayRectangle.Y;

            picBoxImageSelectionModifier.Invalidate();
        }

        private void picBoxImageSelectionModifier_MouseEnter(object sender, EventArgs e)
        {
            if (!picBoxImageSelectionModifier.Focused)
                picBoxImageSelectionModifier.Focus();
        }

        private void picBoxImageSelectionModifier_MouseLeave(object sender, EventArgs e)
        {
            if (picBoxImageSelectionModifier.Focused)
                picBoxImageSelectionModifier.Parent.Focus();
        }

        private void picBoxImageSelectionModifier_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!moving)
            {
                int newValue = Math.Max(
                    Math.Min((e.Delta / 30) + trackBarZoom.Value, trackBarZoom.Maximum),
                    trackBarZoom.Minimum);

                trackBarZoom.Value = newValue;
                ZoomOnPoint(e.Location, newValue / 100f);
            }
        }

        private void picBoxImageSelectionModifier_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                scrolling = true;
                scrollingCursorStartPoint = e.Location;
                scrollingImageStartPoint = imageDisplayRectangle.Location;
                scrollingSelectionStartPoint = selectionDisplayRectangle.Location;

                picBoxImageSelectionModifier.Cursor = Cursors.SizeAll;
            }
            else if (e.Button == MouseButtons.Left)
            {
                if (!scrolling)
                {
                    // Check if the mouse is over any of the sides of the rectangle
                    if (overLeft)
                    {
                        resizingSelectionStartRect = selectionRectangle;
                        resizingLeft = true;

                        if (overTop)
                        {
                            resizingTop = true;
                            picBoxImageSelectionModifier.Cursor = Cursors.SizeNWSE;
                        }
                        else if (overBottom)
                        {
                            resizingBottom = true;
                            picBoxImageSelectionModifier.Cursor = Cursors.SizeNESW;
                        }
                        else
                            picBoxImageSelectionModifier.Cursor = Cursors.SizeWE;
                    }
                    else if (overRight)
                    {
                        resizingSelectionStartRect = selectionRectangle;
                        resizingRight = true;

                        if (overTop)
                        {
                            resizingTop = true;
                            picBoxImageSelectionModifier.Cursor = Cursors.SizeNESW;
                        }
                        else if (overBottom)
                        {
                            resizingBottom = true;
                            picBoxImageSelectionModifier.Cursor = Cursors.SizeNWSE;
                        }
                        else
                            picBoxImageSelectionModifier.Cursor = Cursors.SizeWE;
                    }
                    else if (overTop)
                    {
                        resizingSelectionStartRect = selectionRectangle;
                        resizingTop = true;
                        picBoxImageSelectionModifier.Cursor = Cursors.SizeNS;
                    }
                    else if (overBottom)
                    {
                        resizingSelectionStartRect = selectionRectangle;
                        resizingBottom = true;
                        picBoxImageSelectionModifier.Cursor = Cursors.SizeNS;
                    }

                    // At last, if it's over none of the borders, check if the cursor is in the rectangle for movement
                    else if (selectionDisplayRectangle.Left < e.X && e.X < selectionDisplayRectangle.Right
                        && selectionDisplayRectangle.Top < e.Y && e.Y < selectionDisplayRectangle.Bottom)
                    {
                        moving = true;
                        movingCursorStartPoint = e.Location;
                        movingUnscaledSelectionStartPoint = selectionRectangle.Location;
                    }
                }
            }
        }

        private void picBoxImageSelectionModifier_MouseUp(object sender, MouseEventArgs e)
        {
            // Stop every interaction with the picture box
            if (e.Button == MouseButtons.Middle)
                scrolling = false;
            else if (e.Button == MouseButtons.Left)
            {
                resizingLeft = false;
                resizingRight = false;
                resizingTop = false;
                resizingBottom = false;
                moving = false;
            }
        }

        private void picBoxImageSelectionModifier_MouseMove(object sender, MouseEventArgs e)
        {
            if (scrolling)
            {
                SizeF cursorLocationDifference = new SizeF(
                    scrollingCursorStartPoint.X - e.X, scrollingCursorStartPoint.Y - e.Y);

                // Move both display rectangles
                imageDisplayRectangle.Location = PointF.Subtract(scrollingImageStartPoint, cursorLocationDifference);
                selectionDisplayRectangle.Location = PointF.Subtract(scrollingSelectionStartPoint, cursorLocationDifference);

                picBoxImageSelectionModifier.Invalidate();
            }
            else if (moving)
            {
                // For the sake of limiting movement, the modifications have to first be applied to the
                // unscaled selection rectangle.
                selectionRectangle.X = movingUnscaledSelectionStartPoint.X - (int)((movingCursorStartPoint.X - e.X) / scale);
                selectionRectangle.Y = movingUnscaledSelectionStartPoint.Y - (int)((movingCursorStartPoint.Y - e.Y) / scale);

                if (selectionRectangle.Right < 5)
                    selectionRectangle.X = -selectionRectangle.Width + 5;
                else if (selectionRectangle.Left > image.Width - 5)
                    selectionRectangle.X = image.Width - 5;

                if (selectionRectangle.Bottom < 5)
                    selectionRectangle.Y = -selectionRectangle.Height + 5;
                else if (selectionRectangle.Top > image.Height - 5)
                    selectionRectangle.Y = image.Height - 5;

                // Calculate the new selection display rectangle from the selection rectangle
                selectionDisplayRectangle.X = (selectionRectangle.X * scale) + imageDisplayRectangle.X;
                selectionDisplayRectangle.Y = (selectionRectangle.Y * scale) + imageDisplayRectangle.Y;

                picBoxImageSelectionModifier.Invalidate();
            }
            else if (resizingLeft || resizingRight || resizingTop || resizingBottom)
            {
                // If the Shift key is pressed, keep the aspect ratio
                if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    if (resizingLeft || resizingRight)
                    {
                        float newOldRatio = Math.Max((resizingLeft)
                                ? Math.Max(resizingSelectionStartRect.Right - (e.X - imageDisplayRectangle.X) / scale,
                                    resizingSelectionStartRect.Right - image.Width + 5f)
                                : Math.Max((e.X - selectionDisplayRectangle.X) / scale, -resizingSelectionStartRect.X + 5f),
                            5f) / resizingSelectionStartRect.Width;

                        // Change the behavior depending on what borders are also being resized
                        if (resizingTop)
                        {
                            newOldRatio = Math.Max(newOldRatio, new float[] {
                                    resizingSelectionStartRect.Bottom - (e.Y - imageDisplayRectangle.Y) / scale,
                                    resizingSelectionStartRect.Bottom - image.Height + 5f, 5f
                                }.Max() / resizingSelectionStartRect.Height);

                            selectionRectangle.Y = resizingSelectionStartRect.Bottom -
                                (int)(resizingSelectionStartRect.Height * newOldRatio);
                        }
                        else if (resizingBottom)
                        {
                            newOldRatio = Math.Max(newOldRatio, new float[] {
                                    (e.Y - selectionDisplayRectangle.Y) / scale,
                                    -resizingSelectionStartRect.Y + 5f, 5f
                                }.Max() / resizingSelectionStartRect.Height);
                        }
                        else
                        {
                            newOldRatio = Math.Max(newOldRatio, new float[] {
                                    resizingSelectionStartRect.Height - 2 * ((image.Height - 5f) - resizingSelectionStartRect.Top),
                                    resizingSelectionStartRect.Height - 2 * (resizingSelectionStartRect.Bottom - 5f), 5f
                                }.Max() / resizingSelectionStartRect.Height);

                            selectionRectangle.Y = resizingSelectionStartRect.Y + (resizingSelectionStartRect.Height -
                                (int)(resizingSelectionStartRect.Height * newOldRatio)) / 2;
                        }

                        // The Width and Height calculation is the same for all cases
                        selectionRectangle.Width = (int)(resizingSelectionStartRect.Width * newOldRatio);
                        selectionRectangle.Height = (int)(resizingSelectionStartRect.Height * newOldRatio);

                        if (resizingLeft)
                            selectionRectangle.X = resizingSelectionStartRect.Right - selectionRectangle.Width;
                    }

                    // Here, it's either top or bottom, without the other borders
                    else
                    {
                        float newOldRatio = Math.Max(
                            Math.Max(
                                (resizingTop)
                                    ? Math.Max(resizingSelectionStartRect.Bottom - (e.Y - imageDisplayRectangle.Y) / scale,
                                        resizingSelectionStartRect.Bottom - image.Height + 5f)
                                    : Math.Max((e.Y - selectionDisplayRectangle.Y) / scale, -resizingSelectionStartRect.Y + 5f),
                                5f) / resizingSelectionStartRect.Height,
                            new float[] {
                                resizingSelectionStartRect.Width - 2 * ((image.Width - 5f) - resizingSelectionStartRect.Left),
                                resizingSelectionStartRect.Width - 2 * (resizingSelectionStartRect.Right - 5f), 5f
                            }.Max() / resizingSelectionStartRect.Width);

                        selectionRectangle.X = resizingSelectionStartRect.X + (resizingSelectionStartRect.Width -
                            (selectionRectangle.Width = (int)(resizingSelectionStartRect.Width * newOldRatio))) / 2;
                        selectionRectangle.Height = (int)(resizingSelectionStartRect.Height * newOldRatio);

                        if (resizingTop)   // This line only applies to Top
                            selectionRectangle.Y = resizingSelectionStartRect.Bottom - selectionRectangle.Height;
                    }
                }
                // Change size without caring about the aspect ratio
                else
                {
                    // Left side or more
                    if (resizingLeft)
                    {
                        int previousX = selectionRectangle.X;
                        selectionRectangle.X = (int)((e.X - imageDisplayRectangle.X) / scale);
                        if (selectionRectangle.Left >= image.Width)
                            selectionRectangle.X = image.Width - 1;
                        selectionRectangle.Width -= selectionRectangle.X - previousX;
                    }

                    // Right side or more
                    else if (resizingRight)
                    {
                        selectionRectangle.Width = (int)((e.X - selectionDisplayRectangle.X) / scale);
                        if (selectionRectangle.Right <= 0)
                            selectionRectangle.Width = -selectionRectangle.X + 1;
                    }

                    // Top side or more
                    if (resizingTop)
                    {
                        int previousY = selectionRectangle.Y;
                        selectionRectangle.Y = (int)((e.Y - imageDisplayRectangle.Y) / scale);
                        if (selectionRectangle.Top >= image.Height)
                            selectionRectangle.Y = image.Height - 1;
                        selectionRectangle.Height -= selectionRectangle.Y - previousY;
                    }

                    // Bottom side or more
                    else if (resizingBottom)
                    {
                        selectionRectangle.Height = (int)((e.Y - selectionDisplayRectangle.Y) / scale);
                        if (selectionRectangle.Bottom <= 0)
                            selectionRectangle.Height = -selectionRectangle.Y + 1;
                    }

                    // Apply size limitations
                    if (selectionRectangle.Width < 5)
                    {
                        if (resizingLeft)
                            selectionRectangle.X = selectionRectangle.Right - 5;
                        selectionRectangle.Width = 5;
                    }

                    if (selectionRectangle.Height < 5)
                    {
                        if (resizingTop)
                            selectionRectangle.Y = selectionRectangle.Bottom - 5;
                        selectionRectangle.Height = 5;
                    }
                }

                // Convert selection rectangle to selection display rectangle
                selectionDisplayRectangle.X = (selectionRectangle.X * scale) + imageDisplayRectangle.X;
                selectionDisplayRectangle.Y = (selectionRectangle.Y * scale) + imageDisplayRectangle.Y;
                selectionDisplayRectangle.Width = selectionRectangle.Width * scale;
                selectionDisplayRectangle.Height = selectionRectangle.Height * scale;

                picBoxImageSelectionModifier.Invalidate();
            }
            else
            {
                // Assume the cursor may not be over any border anymore
                overLeft = false;
                overTop = false;
                overBottom = false;
                overRight = false;

                // Check if the cursor is over any border of the selection rectangle
                if (selectionDisplayRectangle.Left - 5 <= e.X && e.X <= selectionDisplayRectangle.Left + 5
                    && selectionDisplayRectangle.Top - 5 <= e.Y && e.Y <= selectionDisplayRectangle.Bottom + 5)
                    overLeft = true;
                else if (selectionDisplayRectangle.Right - 5 <= e.X && e.X <= selectionDisplayRectangle.Right + 5
                    && selectionDisplayRectangle.Top - 5 <= e.Y && e.Y <= selectionDisplayRectangle.Bottom + 5)
                    overRight = true;

                if (selectionDisplayRectangle.Left - 5 <= e.X && e.X <= selectionDisplayRectangle.Right + 5
                    && selectionDisplayRectangle.Top - 5 <= e.Y && e.Y <= selectionDisplayRectangle.Top + 5)
                    overTop = true;
                else if (selectionDisplayRectangle.Left - 5 <= e.X && e.X <= selectionDisplayRectangle.Right + 5
                    && selectionDisplayRectangle.Bottom - 5 <= e.Y && e.Y <= selectionDisplayRectangle.Bottom + 5)
                    overBottom = true;

                // Sets the cursor according to what border it is over.
                if (overLeft)
                {
                    if (overTop)
                        picBoxImageSelectionModifier.Cursor = Cursors.SizeNWSE;
                    else if (overBottom)
                        picBoxImageSelectionModifier.Cursor = Cursors.SizeNESW;
                    else
                        picBoxImageSelectionModifier.Cursor = Cursors.SizeWE;
                }
                else if (overRight)
                {
                    if (overTop)
                        picBoxImageSelectionModifier.Cursor = Cursors.SizeNESW;
                    else if (overBottom)
                        picBoxImageSelectionModifier.Cursor = Cursors.SizeNWSE;
                    else
                        picBoxImageSelectionModifier.Cursor = Cursors.SizeWE;
                }
                else if (overTop || overBottom)
                    picBoxImageSelectionModifier.Cursor = Cursors.SizeNS;
                else    // If it is over none of the borders, default cursor
                    picBoxImageSelectionModifier.Cursor = Cursors.Default;
            }
        }

        private void linkLabelHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBoxEx.Show(this,
                "Help Window",
                "Help", MessageBoxButtons.OK);
        }

        private void linkLabelGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Nitrofski/WSplit");
        }
    }
}