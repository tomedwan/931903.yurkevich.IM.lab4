using System;
using System.Drawing;
using System.Windows.Forms;

namespace Simulation_Lab_4
{
    public partial class Form1 : Form
    {
        Graphics graphics;
        int resolution;
        bool[,] field;
        int cols;
        int rows;
        int currentGeneration;


        public Form1()
        {
            InitializeComponent();
        }

        private void startGame()
        {
            if (timer1.Enabled) return;
            currentGeneration = 0;
            Text = $"Generation {currentGeneration}";

            nudResolution.Enabled = false;
            nudDensity.Enabled = false;

            resolution = (int)nudResolution.Value;
            rows = pictureBox1.Height / resolution;
            cols = pictureBox1.Width / resolution;

            field = new bool[cols, rows];

            Random random = new Random();
            for (int x = 0; x < cols; x++)
                for (int y = 0; y < rows; y++)
                    field[x, y] = random.Next((int)nudDensity.Value) == 0;

            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = Graphics.FromImage(pictureBox1.Image);
            timer1.Start();
        }

        private void nextGeneration()
        {
            graphics.Clear(Color.Black);

            var newField = new bool[cols, rows];

            for (int x = 0; x < cols; x++)
                for (int y = 0; y < rows; y++)
                {
                    var neigboursCount = countNeighbours(x, y);
                    bool hasLife = field[x, y];

                    if (!hasLife && neigboursCount == 3) newField[x, y] = true;
                    else if (hasLife && (neigboursCount < 2 || neigboursCount > 3)) newField[x, y] = false;
                    else newField[x, y] = field[x, y];

                    if (hasLife)
                        graphics.FillRectangle(Brushes.DarkOrange, x * resolution, y * resolution, resolution - 1, resolution - 1);
                }
            field = newField;
            pictureBox1.Refresh();
            Text = $"Generation {++currentGeneration}";
        }

        private int countNeighbours(int x, int y)
        {
            int count = 0;

            for (int i = -1; i < 2; i++)
                for (int j = -1; j < 2; j++)
                {
                    var col = (x + i + cols) % cols;
                    var row = (y + j + rows) % rows;
                    var isSelfChecking = col == x && row == y;
                    var hasLife = field[col, row];
                    if (hasLife && !isSelfChecking) count++;
                }

            return count;
        }

        private void stopGame()
        {
            if (!timer1.Enabled) return;
            timer1.Stop();
            nudResolution.Enabled = true;
            nudDensity.Enabled = true;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            startGame();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            stopGame();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            nextGeneration();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!timer1.Enabled) return;

            if (e.Button == MouseButtons.Left)
            {
                var x = e.Location.X / resolution;
                var y = e.Location.Y / resolution;

                if (validateMousePosition(x, y)) field[x, y] = true;
            }
            if (e.Button == MouseButtons.Right)
            {
                var x = e.Location.X / resolution;
                var y = e.Location.Y / resolution;
                if (validateMousePosition(x, y)) field[x, y] = false;
            }
        }

        private bool validateMousePosition(int x, int y)
        {
            return x >= 0 && y >= 0 && x < cols && y < rows;
        }
    }
}
