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

        }


        private void btnSelectImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Image Files (.bmp, .jpg, .png)|*.bmp;*.jpg;*.png";
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
                Bitmap bmp2 = new Bitmap(bmp.Width, bmp.Height);

                for (int x = 0; x < bmp.Width; x++)
                {
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        Color c = bmp.GetPixel(x, y);

                        if (c.R < 200 && c.G < 200 && c.B < 200)
                        {
                            bmp2.SetPixel(x, y, Color.Black);
                        }
                        else
                        {
                            bmp2.SetPixel(x, y, Color.White);
                        }
                    }
                }

                // Create an outline of the image
                Bitmap bmp3 = new Bitmap(bmp2.Width, bmp2.Height);

                // If the pixel is white, check the surrounding pixels, if the pixel is black set it to white
                // If any of the surrounding pixels are black, set the pixel to black
                for (int x = 0; x < bmp2.Width; x++)
                {
                    for (int y = 0; y < bmp2.Height; y++)
                    {
                        Color c = bmp2.GetPixel(x, y);

                        if (c.R == 255 && c.G == 255 && c.B == 255)
                        {
                            // Check surrounding pixels
                            bool foundBlack = false;

                            for (int x2 = x - 1; x2 <= x + 1; x2++)
                            {
                                for (int y2 = y - 1; y2 <= y + 1; y2++)
                                {
                                    if (x2 >= 0 && x2 < bmp2.Width && y2 >= 0 && y2 < bmp2.Height)
                                    {
                                        Color c2 = bmp2.GetPixel(x2, y2);

                                        if (c2.R == 0 && c2.G == 0 && c2.B == 0)
                                        {
                                            foundBlack = true;
                                        }
                                    }
                                }
                            }

                            if (foundBlack)
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
                }



                pictureBox1.Image = bmp3;
            }
        }

    }
}
