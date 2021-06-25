using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfLife
{
    public partial class Form1 : Form
    {
        const int healthCondition1 = 2; // two adjacent dots required to survive
        const int healthCondition2 = 3; // three adjacent dots required to grow

        static int speed = 50; // time in ms between cycles

        const int WidthX = 345; // X dimension of cell grid
        const int WidthY = 185; // Y dimension of cell grid

        const int Xoffset = 160; //X offset from upper left corner of window
        const int Yoffset = 40;

        const int gridSize = 5; // distance between grids

        const int cellSize = 4; //radius of dots


        uint drawMode = 0;

        Bitmap bmp = new Bitmap((WidthX * gridSize) + (gridSize), (WidthY * gridSize) + (gridSize), System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

        static string patternCustomFileName = "pictures\\GOL1.bmp";

        static Tuple<int, int> mousePos = new Tuple<int, int>(0, 0);
        static Tuple<int, int> oldMousePos = new Tuple<int, int>(0, 0);

        static bool isMouseOverPic = false;

        static bool picWasSaved = false;

        Stopwatch frameStopwatch = new Stopwatch();
        double frameCounter = 0;
        double drawAvg = 0;
        double calcAvg = 0;


        SolidBrush dotcolor = new SolidBrush(Color.LavenderBlush);
        SolidBrush backcolor = new SolidBrush(Color.DarkGoldenrod);
        SolidBrush shadowcolor = new SolidBrush(Color.LightSalmon);


        static bool[,] board = new bool[WidthX, WidthY];
        static bool[,] oldboard = new bool[WidthX, WidthY];
        static bool[,] oldoldboard = new bool[WidthX, WidthY];
        static bool[][,] rgbboardhistory = new bool[7][,]; //[ new bool[WidthX,WidthY]; // = new bool[,]>();
        static Color[] ShadowColors = new Color[] { Color.FromArgb(237,213,186),
                                                    Color.FromArgb(228,200,156),
                                                    Color.FromArgb(219,186,127),
                                                    Color.FromArgb(210,173,98),
                                                    Color.FromArgb(201,160,68),
                                                    Color.FromArgb(192,146,39),
                                                    Color.DarkGoldenrod

        };
        
        

        static Random rand = new Random();


        static Timer timer1 = new Timer();

        static Timer timer2 = new Timer();

        static Timer timer3 = new Timer();

        Bitmap image1;

        public Form1()
        {
            InitializeComponent();

            resizePicBox();

            using (var g = Graphics.FromImage(bmp))
            {

                Rectangle rect = new Rectangle(0, 0, (WidthX * gridSize) + 2*gridSize, (WidthY * gridSize) + 2*gridSize);

                // Fill rectangle to screen.
                g.FillRectangle(backcolor, rect);

                this.pictureBox1.Image = bmp;
            }
            
            

            for (int n = 0; n < 7; n++)
                rgbboardhistory[n] = new bool[WidthX, WidthY];



            for (int i = 0; i < WidthX; i++)
                for (int j = 0; j < WidthY; j++)
                {

                    board[i, j] = false;
                }

            for (int n = 0; n < rgbboardhistory.Length; n++)
                for (int i = 0; i < WidthX; i++)
                {
                    for (int j = 0; j < WidthY; j++)
                    {
                        rgbboardhistory[n][i, j] = false;
                    }
                }
            drawBoard();

            //createRandomBoard();

            timer1.Interval = speed;
            timer1.Tick += Timer1_Tick;

            timer2.Interval = 333;
            timer2.Tick += Timer2_Tick;

            timer3.Interval = 5;
            timer3.Tick += Timer3_Tick;

        }

        private void resizePicBox()
        {
            this.Size = new Size(WidthX * gridSize - 220, WidthY * gridSize - 120);

            pictureBox1.Width = (WidthX * gridSize);
            pictureBox1.Height = (WidthY * gridSize);
            pictureBox1.Refresh();

            //this.Width =  WidthX * gridSize;
            //this.Height = (WidthY * gridSize) - 30;
            
            //this.ClientSize = new Size(WidthX * gridSize, WidthY * gridSize);
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (drawMode >= 1)
            {
                for (int n = 6; n > 0; n--)
                {
                    rgbboardhistory[n] = rgbboardhistory[n - 1];
                }
                rgbboardhistory[0] = board;
            }

            oldboard = board;

            frameStopwatch.Start();
            //bool[,] oldboard = board;

           
            //bool[,] newboard = calculateNextBoard();
            //board = newboard;
            board = calculateNextBoard();

            frameStopwatch.Stop();
            calcAvg += (double)frameStopwatch.Elapsed.TotalMilliseconds;
            frameStopwatch.Reset();

            frameStopwatch.Start();
            if (drawMode == 0)
                drawChangedCells(oldboard, board);
                //drawBoard();
            else if (drawMode >= 1 )
                drawChangedCellsShadowed(oldboard, board);
            
            frameStopwatch.Stop();
            drawAvg += (double)frameStopwatch.Elapsed.TotalMilliseconds;
            frameStopwatch.Reset();
            
            frameCounter++;

            //drawBoard();
            
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {

            double drawavg = drawAvg / frameCounter;
            double calcavg = calcAvg / frameCounter;
            label5.Text = calcavg.ToString("0.000");
            label6.Text = drawavg.ToString("0.000");
            label4.Text = ((drawAvg + calcAvg) / frameCounter).ToString("0.000");
            frameCounter = 0;
            drawAvg = 0;
            calcAvg = 0;

            //drawBoard();

        }

        private void Timer3_Tick(object sender, EventArgs e)
        {
            drawBoard();
            using (var g = Graphics.FromImage(bmp))
            {
                delPreviewImage(g, oldMousePos.Item1, oldMousePos.Item2);
                

                oldMousePos = mousePos;

                if (isMouseOverPic == true)
                {
                    previewImage(g, oldMousePos.Item1, oldMousePos.Item2);
                    
                }
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled)
            {
                timer1.Stop();
                timer2.Stop();
            }
            else
            {
                //drawBoard();
                timer1.Start();
                timer2.Start();
            }
            
        }

        void createRandomBoard()
        {
            Random rand = new Random();
            for (int i = 0; i < WidthX-1; i++)
                for (int j = 0; j < WidthY-1; j++)
                {
                    bool rnd = rand.NextDouble() > 0.6;
                    board[i, j] = rnd;
                }

        }

        void drawBoard()
        {
            //Graphics myGraphics = base.CreateGraphics();
            // Pen myPen = new Pen(Color.Red);
            // Pen blankPen = new Pen(BackColor);
            //SolidBrush mySolidBrush = dotcolor;
            //SolidBrush myBlankBrush = backcolor;
            //myGraphics.DrawEllipse(myPen, 50, 50, 150, 150);

           
            using (var g = Graphics.FromImage(bmp))
            {
                for (int i = 0; i < WidthX; i++)
                    for (int j = 0; j < WidthY; j++)
                    {

                        if (board[i, j] == true)
                            GraphicsExtensions.FillRectangle(g, dotcolor, i * gridSize + cellSize, j * gridSize + cellSize, cellSize);
                        //GraphicsExtensions.FillCircle(myGraphics, mySolidBrush, Xoffset + i * gridSize, Yoffset + j * gridSize, circleSize);
                        else
                            GraphicsExtensions.FillRectangle(g, backcolor, i * gridSize + cellSize, j * gridSize + cellSize, cellSize);
                        //GraphicsExtensions.FillCircle(myGraphics, myBlankBrush, Xoffset + i * gridSize, Yoffset + j * gridSize, circleSize);
                    }
                this.pictureBox1.Image = bmp;
            }
            
        }



        void drawChangedCells(bool[,] oldboard, bool[,] Tempboard)
        {
            //Graphics myGraphics = base.CreateGraphics();
            // Pen myPen = new Pen(Color.Red);
            // Pen blankPen = new Pen(BackColor);
            //SolidBrush mySolidBrush = dotcolor;
            //SolidBrush myBlankBrush = backcolor;
            //myGraphics.DrawEllipse(myPen, 50, 50, 150, 150);

            //var bmp = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            using (var g = Graphics.FromImage(bmp))
            {
                delPreviewImage(g, oldMousePos.Item1, oldMousePos.Item2);
                

                for (int i = 0; i < WidthX; i++)
                {
                    for (int j = 0; j < WidthY; j++)
                    {

                        if ((oldboard[i, j] == false) && (Tempboard[i, j] == true))
                            GraphicsExtensions.FillRectangle(g, dotcolor, i * gridSize + cellSize, j * gridSize + cellSize, cellSize);
                        //GraphicsExtensions.FillCircle(myGraphics, mySolidBrush,);
                        else if ((oldboard[i, j] == true) && (Tempboard[i, j] == false))
                            GraphicsExtensions.FillRectangle(g, backcolor, i * gridSize + cellSize, j * gridSize + cellSize, cellSize);
                        //GraphicsExtensions.FillCircle(myGraphics, myBlankBrush, Xoffset + i * gridSize, Yoffset + j * gridSize, circleSize);
                    }
                }

                oldMousePos = mousePos;

                if (isMouseOverPic == true)
                {
                    previewImage(g, oldMousePos.Item1, oldMousePos.Item2);
                    
                }


            }
            this.pictureBox1.Image = bmp;
            //board = Tempboard;
        }


        void drawChangedCellsShadowed(bool[,] oldboard, bool[,] Tempboard)
        {
            //Graphics myGraphics = base.CreateGraphics();
            // Pen myPen = new Pen(Color.Red);
            // Pen blankPen = new Pen(BackColor);
            //SolidBrush mySolidBrush = dotcolor;
            //SolidBrush myBlankBrush = backcolor;
            //myGraphics.DrawEllipse(myPen, 50, 50, 150, 150);

            //var bmp = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            using (var g = Graphics.FromImage(bmp))
            {
                
                delPreviewImage(g, oldMousePos.Item1, oldMousePos.Item2);

                for (int i = 0; i < WidthX; i++)
                {
                    for (int j = 0; j < WidthY; j++)
                    {
                        for (int n = 1; n < 5; n++)
                        {
                            if ((rgbboardhistory[n][i, j] == true) && (rgbboardhistory[n + 1][i, j] == false))
                            {
                                if (drawMode == 1)
                                    GraphicsExtensions.FillRectangle(g, new SolidBrush(ShadowColors[n + 2]), i * gridSize + cellSize, j * gridSize + cellSize, cellSize);
                                else if (drawMode == 2)
                                    GraphicsExtensions.FillRectangle(g, new SolidBrush(ShadowColors[n + 1]), i * gridSize + cellSize, j * gridSize + cellSize, cellSize);
                                else if (drawMode == 3)
                                    GraphicsExtensions.FillRectangle(g, new SolidBrush(ShadowColors[n]), i * gridSize + cellSize, j * gridSize + cellSize, cellSize);
                            }
                        }

                        if ((oldboard[i, j] == true) && (Tempboard[i, j] == false))
                            GraphicsExtensions.FillRectangle(g, backcolor, i * gridSize + cellSize, j * gridSize + cellSize, cellSize);
                        else if ((oldboard[i, j] == false) && (Tempboard[i, j] == true))
                            GraphicsExtensions.FillRectangle(g, dotcolor, i * gridSize + cellSize, j * gridSize + cellSize, cellSize);

                        

                        
                       
                        //else if ((oldboard[i,j] == false))// && (oldboard[i,j] == false))
                            //GraphicsExtensions.FillRectangle(g, backcolor, i * gridSize + cellSize, j * gridSize + cellSize, cellSize);

                        

                       

                    }
                }

                oldMousePos = mousePos;

                if (isMouseOverPic == true)
                {
                    previewImage(g, oldMousePos.Item1, oldMousePos.Item2);
                    
                }
            }
            this.pictureBox1.Image = bmp;
            //board = Tempboard;
        }




        static bool[,] calculateNextBoard()
        {
            bool[,] Tempboard = new bool[WidthX, WidthY];

            //for (int i = 0; i < WidthX; i++)
             
            Parallel.For(0, WidthX, i =>
            {
                for (int j = 0; j < WidthY; j++)
                {
                    int willLive = 0;
                    if (j > 0) //upper row
                    {
                        if (i > 0) //left of
                            willLive += ToInt(board[i - 1, j - 1]);
                        willLive += ToInt(board[i, j - 1]);
                        if (i < WidthX - 1)
                            willLive += ToInt(board[i + 1, j - 1]);
                    }

                    if (i > 0) //same row
                        willLive += ToInt(board[i - 1, j]); //left of
                    if (i < WidthX - 1)
                        willLive += ToInt(board[i + 1, j]);

                    if (j < WidthY - 1)//lower row
                    {
                        if (i > 0) //left of
                            willLive += ToInt(board[i - 1, j + 1]);
                        willLive += ToInt(board[i, j + 1]);
                        if (i < WidthX - 1)
                            willLive += ToInt(board[i + 1, j + 1]);
                    }


                    if (board[i, j] == true)
                    {
                        if (willLive >= healthCondition1 && willLive <= healthCondition2)
                            Tempboard[i, j] = true;
                        else
                            Tempboard[i, j] = false;
                    }
                    else // if original cell was empty
                    {
                        if (willLive == healthCondition2)
                            Tempboard[i, j] = true;
                        else
                            Tempboard[i, j] = false;
                    }




                }
            });
            
            return Tempboard;
        }


      

        private void loadImagefromBMP(string BMPname)
        {
            image1 = new Bitmap(BMPname, true);
            label1.Text = "BMP format: " + Environment.NewLine + image1.PixelFormat.ToString() + Environment.NewLine;
            label1.Text += image1.Height + "x" + image1.Width;

            int x, y;

            // Loop through the images pixels to reset color.
            for (x = 0; x < image1.Width; x++)
            {
                for (y = 0; y < image1.Height; y++)
                {
                    int pixelColor = image1.GetPixel(x, y).ToArgb();
                    int empty = Color.Empty.ToArgb();
                    //Debug.WriteLine("Pixel: " + x + ":" + y + " - pixelcolor: " + pixelColor + " - empty: " + empty);
                    if (pixelColor < -65794)
                        board[x, y] = true;
                    else
                        board[x, y] = false;

                }
            }
        }

        private void loadImagetoPos(string BMPname, int Xoffset, int Yoffset)
        {
            
            
            image1 = new Bitmap(BMPname, true);

            label1.Text = Path.GetFileName(BMPname) + Environment.NewLine + image1.PixelFormat.ToString().Substring(6) + Environment.NewLine;
            label1.Text += image1.Height + "x" + image1.Width;

            Debug.WriteLine("(Xoffset / gridSize): {0}", (Xoffset / gridSize));
            Debug.WriteLine("(Yoffset / gridSize): {0}", (Yoffset / gridSize));
            Debug.WriteLine("image1 Width: {0}", image1.Width);
            Debug.WriteLine("image1 Height: {0}", image1.Height);
            Debug.WriteLine("WidthX: {0}", WidthX);
            Debug.WriteLine("WidthY: {0}", WidthY);

            if (
                //((Xoffset / gridSize) > (image1.Width)) && 
                //((Yoffset / gridSize) > (image1.Height)) && 
                (((Xoffset / gridSize) + (image1.Width)) < (WidthX)) &&
                (((Yoffset / gridSize) + (image1.Height)) < (WidthY))

                //((Xoffset / gridSize) < (WidthX - (image1.Width / 2) - 1)) && 
                //((Yoffset / gridSize) < (WidthY - (image1.Height / 2) - 1))
                )
            {
               
                    Debug.WriteLine("BMP fits into pos");
                    

                    int x, y;

                    // Loop through the images pixels
                    for (y = 0; y < image1.Height; y++)
                    {
                        for (x = 0; x < image1.Width; x++)
                        {
                            int pixelColor = image1.GetPixel(x, y).ToArgb();
                            //int empty = Color.Empty.ToArgb();
                            int posX = (Xoffset / gridSize) + x;
                            int posY = (Yoffset / gridSize) + y;
                            //Debug.WriteLine("PosX: {0}   PosY: {1}", posX, posY);
                            //Debug.WriteLine("Pixel: " + x + ":" + y + " - pixelcolor: " + pixelColor + " - empty: " + empty);
                            if (pixelColor < -65794)
                                board[posX, posY] = true;
                            else
                                board[posX, posY] = false;

                        }
                    }
                
            }
            else
            {
                Debug.WriteLine("doesn't fit: ");
               

            }
        }


        private void previewImage(Graphics g, int Xoffset, int Yoffset)
        {

            string BMPname = "";

            if (radioButton12.Checked)
                BMPname = @patternCustomFileName;
            else if (radioButton5.Checked)
                BMPname = "1pxblack.bmp";

            if (BMPname == "")
                return;

            image1 = new Bitmap(BMPname, true);

           

            if (
                //((Xoffset / gridSize) > (image1.Width)) && 
                //((Yoffset / gridSize) > (image1.Height)) && 
                (((Xoffset / gridSize) + (image1.Width)) < (WidthX)) &&
                (((Yoffset / gridSize) + (image1.Height)) < (WidthY))

                //((Xoffset / gridSize) < (WidthX - (image1.Width / 2) - 1)) && 
                //((Yoffset / gridSize) < (WidthY - (image1.Height / 2) - 1))
                )
            {

                //Debug.WriteLine("BMP fits into pos");




                int x, y;

                // Loop through the images pixels
                
                    for (y = 0; y < image1.Height; y++)
                    {
                        for (x = 0; x < image1.Width; x++)
                        {
                            int pixelColor = image1.GetPixel(x, y).ToArgb();
                            int empty = Color.Empty.ToArgb();
                            int posX = (Xoffset / gridSize) + x;
                            int posY = (Yoffset / gridSize) + y;
                            //Debug.WriteLine("PosX: {0}   PosY: {1}", posX, posY);
                            //Debug.WriteLine("Pixel: " + x + ":" + y + " - pixelcolor: " + pixelColor + " - empty: " + empty);
                            if (pixelColor < -65794)

                                GraphicsExtensions.FillRectangle(g, new SolidBrush(Color.DarkGray), Xoffset + x * gridSize + cellSize, Yoffset + y * gridSize + cellSize, cellSize);

                            //else
                                //GraphicsExtensions.FillRectangle(g, backcolor, Xoffset + x * gridSize + cellSize, Yoffset + y * gridSize + cellSize, cellSize);

                        }
                    }
                    
                
                this.pictureBox1.Image = bmp;

            }
            else
            {
                //Debug.WriteLine("doesn't fit: ");


            }
        }



        private void delPreviewImage(Graphics g, int Xoffset, int Yoffset)
        {
            string BMPname = "";

            if (radioButton12.Checked)
                BMPname = @patternCustomFileName;
            else if (radioButton5.Checked)
                BMPname = "1pxblack.bmp";

            if (BMPname == "")
                return;

            image1 = new Bitmap(BMPname, true);



            if (
                //((Xoffset / gridSize) > (image1.Width)) && 
                //((Yoffset / gridSize) > (image1.Height)) && 
                (((Xoffset / gridSize) + (image1.Width)) < (WidthX)) &&
                (((Yoffset / gridSize) + (image1.Height)) < (WidthY))

                //((Xoffset / gridSize) < (WidthX - (image1.Width / 2) - 1)) && 
                //((Yoffset / gridSize) < (WidthY - (image1.Height / 2) - 1))
                )
            {

                //Debug.WriteLine("BMP fits into pos");




                int x, y;

                // Loop through the images pixels

                for (y = 0; y < image1.Height; y++)
                {
                    for (x = 0; x < image1.Width; x++)
                    {
                        int pixelColor = image1.GetPixel(x, y).ToArgb();
                        int empty = Color.Empty.ToArgb();
                        int posX = (Xoffset / gridSize) + x;
                        int posY = (Yoffset / gridSize) + y;
                        //Debug.WriteLine("PosX: {0}   PosY: {1}", posX, posY);
                        //Debug.WriteLine("Pixel: " + x + ":" + y + " - pixelcolor: " + pixelColor + " - empty: " + empty);
                        if (pixelColor < -65794)
                            GraphicsExtensions.FillRectangle(g, backcolor, Xoffset + x * gridSize + cellSize, Yoffset + y * gridSize + cellSize, cellSize);

                    }
                }


                this.pictureBox1.Image = bmp;

            }
            else
            {
                //Debug.WriteLine("doesn't fit: ");


            }
        }




        public static int ToInt(bool value)
        {
            return value ? 1 : 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //timer1.Stop();
            //createBoard();
            loadImagefromBMP("pictures\\GOL1.bmp");
            if (!(timer1.Enabled))
                drawBoard();
        }



        private void button3_Click(object sender, EventArgs e)
        {
            createRandomBoard();
            if (!(timer1.Enabled))
                drawBoard();
        }


        //protected override void OnPaint(PaintEventArgs e)
        //{
            //base.OnPaint(e);

            //var bmp = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            

        //}

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            timer1.Interval = trackBar1.Value + 1;
            label3.Text = trackBar1.Value + " ms";
        }

        

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
                drawMode = 0;
           
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
                drawMode = 1;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
                drawMode = 2;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
                drawMode = 3;
        }

       

        private void button6_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < WidthX; i++)
                for (int j = 0; j < WidthY; j++)
                {
                    
                    board[i, j] = false;
                }

            for (int n = 0; n < rgbboardhistory.Length; n++)
                for (int i = 0; i < WidthX; i++)
                {
                    for (int j = 0; j < WidthY; j++)
                    {
                        rgbboardhistory[n][i, j] = false;
                    }
                }
            drawBoard();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            Point coordinates = me.Location;
            Debug.WriteLine(coordinates.ToString());
            if (me.Button == MouseButtons.Left)
                //if (radioButton5.Checked)
                  //  loadImagetoPos("1pxblack.BMP", coordinates.X, coordinates.Y);
                /*else*/ if (radioButton12.Checked)
                    loadImagetoPos(@patternCustomFileName, coordinates.X, coordinates.Y);

            //if (timer1.Enabled == false)
                drawBoard();
            


        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //MouseEventArgs me = (MouseEventArgs)e;
            Point coordinates = e.Location;
            //Debug.WriteLine(coordinates.ToString());
            if (e.Button == MouseButtons.Left)
                if (radioButton5.Checked)
                    loadImagetoPos("1pxblack.BMP", coordinates.X, coordinates.Y);
            mousePos = new Tuple<int, int>(coordinates.X, coordinates.Y);
            if (picWasSaved)
            {
                picWasSaved = false;
                
            }
            
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            isMouseOverPic = true;
            Cursor myCursor = new Cursor(Application.StartupPath + "\\Cursor1.cur");
            pictureBox1.Cursor = myCursor;
            if (timer1.Enabled == false)
                timer3.Start();
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            isMouseOverPic = false;
            pictureBox1.Cursor = Cursors.Default;
            if (timer1.Enabled == false)
                timer3.Stop();
            using (var g = Graphics.FromImage(bmp))
            {
                delPreviewImage(g, oldMousePos.Item1, oldMousePos.Item2);
                oldMousePos = mousePos;

            }
            }

        private void button4_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                
                openFileDialog.InitialDirectory = Path.Combine(Application.StartupPath,@"Pictures");
                openFileDialog.Filter = "Bitmap (*.bmp)|*.bmp|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    string filePath = openFileDialog.FileName;
                    radioButton12.Enabled = true;
                    radioButton12.Checked = true;
                    patternCustomFileName = filePath;

                    using (image1 = new Bitmap(filePath, true))
                    {
                        label1.Text = Path.GetFileName(patternCustomFileName) + Environment.NewLine + image1.PixelFormat.ToString() + Environment.NewLine;
                        label1.Text += image1.Height + " x " + image1.Width;
                    }
                    Debug.WriteLine(patternCustomFileName);
                    //Read the contents of the file into a stream
                    //var fileStream = openFileDialog.OpenFile();

                    /*using (StreamReader reader = new StreamReader(fileStream))
                    {
                        fileContent = reader.ReadToEnd();
                    }*/
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            bool isTimer1Enabled = false;
            if (timer1.Enabled)
                isTimer1Enabled = true;

            if (isTimer1Enabled)
                timer1.Stop();
            drawBoard();
            if (isTimer1Enabled)
                timer1.Start();
        }
    }

    public static class GraphicsExtensions
    {
        public static void DrawCircle(this Graphics g, Pen pen,
                                      float centerX, float centerY, float radius)
        {
            g.DrawEllipse(pen, centerX - radius, centerY - radius,
                          radius + radius, radius + radius);
        }

        public static void FillCircle(this Graphics g, Brush brush,
                                      float centerX, float centerY, float radius)
        {
            g.FillEllipse(brush, centerX - radius, centerY - radius,
                          radius + radius, radius + radius);
        }

        public static void FillRectangle(this Graphics g, Brush brush, int x, int y, int size)
        {
            // Create rectangle.
            Rectangle rect = new Rectangle(x - (size / 2), y - (size /2), size, size);

            // Fill rectangle to screen.
            g.FillRectangle(brush, rect);
        }
        

    }
}
