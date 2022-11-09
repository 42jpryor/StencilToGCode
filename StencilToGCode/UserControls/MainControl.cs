using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StencilToGCode.UserControls
{
    public partial class MainControl : UserControl
    {
        public MainControl()
        {
            InitializeComponent();

            tableLayoutPanel1.Dock = DockStyle.Fill;

            // Set picturebox to zoom
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;


            // Load saved settings
            txtPrinterWidth.Value = Properties.Settings.Default.PrinterWidth;
            txtPrinterHeight.Value = Properties.Settings.Default.PrinterHeight;
            txtPrinterLength.Value = Properties.Settings.Default.PrinterLength;
            txtXOffset.Value = Properties.Settings.Default.XOffset;
            txtYOffset.Value = Properties.Settings.Default.YOffset;
            txtZOffset.Value = Properties.Settings.Default.ZOffset;
            trackBar1.Value = Properties.Settings.Default.EdgeTrackbar;
            trackBarImageDetailScale.Value = Properties.Settings.Default.ResolutionTrackbar;
        }

        private void MainControl_Load(object sender, EventArgs e)
        {
            trackBar1.Value = 220;
            lblColorLimit.Text = trackBar1.Value.ToString();
        }


        private void btnSelectImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Image Files (.bmp, .jpg, .png)|*.bmp;*.jpg;*.png;";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Multiselect = false;

            openFileDialog1.Title = "Select an Image File";

            DialogResult userClickedOK = openFileDialog1.ShowDialog();

            if (userClickedOK == DialogResult.OK)
            {
                // Open the selected file to read.
                System.IO.Stream fileStream = openFileDialog1.OpenFile();

                using (System.IO.StreamReader reader = new System.IO.StreamReader(fileStream))
                {
                    // Read the first line from the file and write it the textbox.
                    btnSelectImage.Text = openFileDialog1.FileName;
                    
                    pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
                }
            }
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            // Must have width, length, and height greater than 0
            if (txtPrinterWidth.Value <= 0 || txtPrinterLength.Value <= 0 || txtPrinterHeight.Value <= 0)
            {
                MessageBox.Show("Printer Width, Length, and Height must be greater than 0");
                return;
            }

            if (pictureBox1.Image != null)
            {
                // Convert image to black and white
                Bitmap bmp = new Bitmap(pictureBox1.Image);

                int detailScale = trackBarImageDetailScale.Value;

                // Scale image
                int width = bmp.Width;
                int height = bmp.Height;

                if (width > height)
                {
                    height = detailScale * (int)(txtPrinterLength.Value / width * height);
                    width = detailScale * (int)txtPrinterWidth.Value;
                }
                else
                {
                    width = detailScale * (int)(txtPrinterWidth.Value / height * width);
                    height = detailScale * (int)txtPrinterLength.Value;
                }

                Bitmap bmp2 = new Bitmap(bmp, width, height);

                // Convert to black and white
                for (int x = 0; x < bmp2.Width; x++)
                {
                    for (int y = 0; y < bmp2.Height; y++)
                    {
                        Color c = bmp2.GetPixel(x, y);

                        if (c.R + c.G + c.B > trackBar1.Value)
                        {
                            bmp2.SetPixel(x, y, Color.White);
                        }
                        else
                        {
                            bmp2.SetPixel(x, y, Color.Black);
                        }
                    }
                }
                

                // Create an outline of the image
                Bitmap bmp3 = new Bitmap(bmp2.Width, bmp2.Height);

                // If the pixel is black, check the surrounding pixels
                // If any of the surrounding pixels are white, set the pixel to black
                for (int x = 0; x < bmp2.Width; x++)
                {
                    for (int y = 0; y < bmp2.Height; y++)
                    {
                        Color c = bmp2.GetPixel(x, y);

                        if (c.R == 0 && c.G == 0 && c.B == 0)
                        {
                            // Check surrounding pixels
                            if (x > 0 && y > 0 && x < bmp2.Width - 1 && y < bmp2.Height - 1)
                            {
                                Color c2 = bmp2.GetPixel(x - 1, y);
                                Color c3 = bmp2.GetPixel(x + 1, y);
                                Color c4 = bmp2.GetPixel(x, y - 1);
                                Color c5 = bmp2.GetPixel(x, y + 1);

                                if (c2.R == 255 && c2.G == 255 && c2.B == 255)
                                {
                                    bmp3.SetPixel(x, y, Color.Black);
                                }
                                else if (c3.R == 255 && c3.G == 255 && c3.B == 255)
                                {
                                    bmp3.SetPixel(x, y, Color.Black);
                                }
                                else if (c4.R == 255 && c4.G == 255 && c4.B == 255)
                                {
                                    bmp3.SetPixel(x, y, Color.Black);
                                }
                                else if (c5.R == 255 && c5.G == 255 && c5.B == 255)
                                {
                                    bmp3.SetPixel(x, y, Color.Black);
                                }
                                else
                                {
                                    bmp3.SetPixel(x, y, Color.White);
                                }
                            }
                            else
                            {
                                bmp3.SetPixel(x, y, Color.White);
                            }
                        }
                        else
                        {
                            bmp3.SetPixel(x, y, Color.White);
                        }
                    }
                }



                pictureBox1.Image = bmp3;
            }
        }


        private void btnFindIntersections_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(pictureBox1.Image);
            Bitmap bmp2 = new Bitmap(bmp, bmp.Width, bmp.Height);
            // Loop through each pixel in the image
            for (int x = 0; x < bmp2.Width; x++)
            {
                for (int y = 0; y < bmp2.Height; y++)
                {
                    // If the pixel is black, check the surrounding pixels and if 3 or more are black, set the pixel to red, if not than set it to blue
                    Color c = bmp2.GetPixel(x, y);

                    if (c.R == 0 && c.G == 0 && c.B == 0)
                    {
                        // Check surrounding pixels including diagnols
                        if (x > 0 && y > 0 && x < bmp2.Width - 1 && y < bmp2.Height - 1)
                        {
                            // Top pixel
                            Color c2 = bmp2.GetPixel(x, y - 1);
                            // Bottom pixel
                            Color c3 = bmp2.GetPixel(x, y + 1);
                            // Left pixel
                            Color c4 = bmp2.GetPixel(x - 1, y);
                            // Right pixel
                            Color c5 = bmp2.GetPixel(x + 1, y);
                            // Top left pixel
                            Color c6 = bmp2.GetPixel(x - 1, y - 1);
                            // Top right pixel
                            Color c7 = bmp2.GetPixel(x + 1, y - 1);
                            // Bottom left pixel
                            Color c8 = bmp2.GetPixel(x - 1, y + 1);
                            // Bottom right pixel
                            Color c9 = bmp2.GetPixel(x + 1, y + 1);

                            int blackCount = 0;

                            if (c2.R == 0)
                                blackCount++;
                            if (c3.R == 0)
                                blackCount++;
                            if (c4.R == 0)
                                blackCount++;
                            if (c5.R == 0)
                                blackCount++;
                            if (c6.R == 0)
                                blackCount++;
                            if (c7.R == 0)
                                blackCount++;
                            if (c8.R == 0)
                                blackCount++;
                            if (c9.R == 0)
                                blackCount++;

                            if (blackCount >= 4)
                            {
                                bmp2.SetPixel(x, y, Color.Red);
                            }
                            else
                            {
                                bmp2.SetPixel(x, y, Color.Blue);
                            }

                        }
                        else
                        {
                            bmp2.SetPixel(x, y, Color.White);
                        }
                    }
                    else
                    {
                        bmp2.SetPixel(x, y, Color.White);
                    }
                }
            }

            pictureBox1.Image = bmp2;
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            // Change label to show the value of the trackbar
            lblColorLimit.Text = trackBar1.Value.ToString();
            Properties.Settings.Default.EdgeTrackbar = trackBar1.Value;
            Properties.Settings.Default.Save();
        }

        private void trackBarImageDetailScale_ValueChanged(object sender, EventArgs e)
        {
            lblImageDetailScale.Text = trackBarImageDetailScale.Value.ToString();
            Properties.Settings.Default.ResolutionTrackbar = trackBarImageDetailScale.Value;
            Properties.Settings.Default.Save();
        }

        private void txtPrinterWidth_ValueChanged(object sender, EventArgs e)
        {
            // Save settings
            Properties.Settings.Default.PrinterWidth = txtPrinterWidth.Value;
            Properties.Settings.Default.Save();
        }

        private void txtPrinterLength_ValueChanged(object sender, EventArgs e)
        {
            // Save settings
            Properties.Settings.Default.PrinterLength = txtPrinterLength.Value;
            Properties.Settings.Default.Save();
        }

        private void txtPrinterHeight_ValueChanged(object sender, EventArgs e)
        {
            // Save settings
            Properties.Settings.Default.PrinterHeight = txtPrinterHeight.Value;
            Properties.Settings.Default.Save();
        }

        private void txtXOffset_ValueChanged(object sender, EventArgs e)
        {
            // Save settings
            Properties.Settings.Default.XOffset = txtXOffset.Value;
            Properties.Settings.Default.Save();
        }

        private void txtYOffset_ValueChanged(object sender, EventArgs e)
        {
            // Save settings
            Properties.Settings.Default.YOffset = txtYOffset.Value;
            Properties.Settings.Default.Save();
        }

        private void txtZOffset_ValueChanged(object sender, EventArgs e)
        {
            // Save settings
            Properties.Settings.Default.ZOffset = txtZOffset.Value;
            Properties.Settings.Default.Save();
        }

    }
}
