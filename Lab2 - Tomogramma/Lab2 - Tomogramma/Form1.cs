using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab2___Tomogramma {
    public partial class Form1 : Form {
        Bin bin = new Bin();
        View view = new View();

        bool loaded = false;
        int currentLayer = 0;
        bool needReload = false;

        int tfMin = 0;
        int tfWidth = 1;

        int FrameCount;
        DateTime NextFPSUpdate = DateTime.Now.AddSeconds(1);

        enum DrawMode{
            QUADS, TEXTURE, QUADSTRIP
        }

        DrawMode drawMode = DrawMode.QUADS;
        public Form1() {
            InitializeComponent();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK) {
                string str = dialog.FileName;
                bin.readBIN(str);
                view.SetupView(glControl1.Width, glControl1.Height);
                loaded = true;
                glControl1.Invalidate();
            }
        }

        private void glControl1_Paint(object sender, PaintEventArgs e) {
            if (loaded) {
                switch (drawMode) {
                    case DrawMode.QUADS:
                        view.DrawQuads(currentLayer, tfMin, tfWidth);
                        break;
                    case DrawMode.TEXTURE:
                        if (needReload) {
                            view.generateTextureImage(currentLayer, tfMin, tfWidth);
                            view.Load2DTexture();
                            needReload = false;
                        }
                        view.DrawTexture();
                        break;
                    case DrawMode.QUADSTRIP:
                        view.DrawQuadStrip(currentLayer, tfMin, tfWidth);
                        break;
                }
                
                glControl1.SwapBuffers();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e) {
            currentLayer = trackBar1.Value;
            needReload = true;
        }

        void Application_Idle(object sender, EventArgs e) {
            while (glControl1.IsIdle) {
                displayFPS();
                glControl1.Invalidate();
            }
        }

        private void Form1_Load(object sender, EventArgs e) {
            Application.Idle += Application_Idle;
        }

        void displayFPS() {
            if (DateTime.Now >= NextFPSUpdate) {
                Text = String.Format("CT Visualizer (fps={0})", FrameCount);
                NextFPSUpdate = DateTime.Now.AddSeconds(1);
                FrameCount = 0;
            }
            ++FrameCount;
        }

        private void QuadsRadioButton_CheckedChanged(object sender, EventArgs e) {
            drawMode = DrawMode.QUADS;
        }

        private void textureRadioButton_CheckedChanged(object sender, EventArgs e) {
            drawMode = DrawMode.TEXTURE;
        }

        private void trackBar2_Scroll(object sender, EventArgs e) {
            tfMin = trackBar2.Value * 25;
            needReload = true;
            //glControl1.Invalidate();
        }

        private void trackBar3_Scroll(object sender, EventArgs e) {
            tfWidth = trackBar3.Value * 350;
            needReload = true;
            //glControl1.Invalidate();
        }

        private void QuadStripRadioButton_CheckedChanged(object sender, EventArgs e) {
            drawMode = DrawMode.QUADSTRIP;
        }
    }
}
