using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfLife
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.CGOL;
            textBox1.Text = Form1.WidthX.ToString();
            textBox2.Text = Form1.WidthY.ToString();
            textBox3.Text = Form1.cellSize.ToString();
            textBox4.Text = (Form1.gridSize - Form1.cellSize).ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool ist1running = false;
            if (Form1.timer1.Enabled)
                ist1running = true;

            if (ist1running)
            {
                Form1.timer1.Stop();
                Form1.timer2.Stop();
                Form1.T1wasRunning = true;
            }

            int newX;
            bool Xresult = int.TryParse(textBox1.Text, out newX);
            if (Xresult)
                Form1.WidthX = newX;

            int newY;
            bool Yresult = int.TryParse(textBox2.Text, out newY);
            if (Yresult)
                Form1.WidthY = newY;

            int newC, newG;
            bool Cresult = int.TryParse(textBox3.Text, out newC);
            bool Gresult = int.TryParse(textBox4.Text, out newG);

            if (Cresult && Gresult)
            {
                Form1.cellSize = newC;
                Form1.gridSize = newC + newG;
            }


            Form1.gridChanged = true;
            

            // stuff

            /*if (ist1running)
            {
                Form1.timer1.Start();
                Form1.timer2.Start();
            }*/

            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1.gridChanged = false;
            Form1.T1wasRunning = false;
            this.Close();
        }
    }
}
