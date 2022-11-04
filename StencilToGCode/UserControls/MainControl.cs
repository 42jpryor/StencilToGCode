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
            if (pictureBox1.Image != null)
            {
                // Convert image to black and white
                Bitmap bmp = new Bitmap(pictureBox1.Image);

                int detailScale = 4;

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

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            // Change label to show the value of the trackbar
            lblColorLimit.Text = trackBar1.Value.ToString();
        }
    }
}
