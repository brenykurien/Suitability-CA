using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BitMiracle.LibTiff.Classic;

namespace Suitablity_based_CA
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        // Author : Breny Kurien
        // Work done as a part of M.Tech project at ADRIN

        // Constraints in Tab1
        private void txtAttributeCnttb1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!Char.IsControl(e.KeyChar) && !Char.IsDigit(e.KeyChar))
                { e.Handled = true; }
        }
        private void txtLayerWeighttb1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsControl(e.KeyChar) && !Char.IsDigit(e.KeyChar) && (e.KeyChar!='.'))
            { e.Handled = true; }
            // Allow only one decimal point
            if((e.KeyChar=='.') && ((sender as TextBox).Text.IndexOf('.')>-1))
            { e.Handled = true; }
        }
        private void txtLayerWeighttb1_Validating(object sender, CancelEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if(tb!=null)
            {
                double i;
                if(double.TryParse(tb.Text,out i))
                {if (i > 0 && i <= 100) return;}
                MessageBox.Show("Weight should be between 0 & 100.");
                e.Cancel = true;
            }
        }
        private void txtGrowthRatetb1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsControl(e.KeyChar) && !Char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != '-'))
            { e.Handled = true; }
            // Allow only one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            { e.Handled = true; }
            // Allow only one negative sign
            if ((e.KeyChar == '-') && ((sender as TextBox).Text.Length > 0))
            { e.Handled = true; }
        }
        private void txtGrowthRatetb1_Validating(object sender, CancelEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                double i;
                if (double.TryParse(tb.Text, out i))
                { if (i >=-100 && i <= 100) return; }
                MessageBox.Show("Growth % should be between -100 and 100.");
                e.Cancel = true;
            }
        }

        // Constraints for Tab2
        private void txtGentb2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsControl(e.KeyChar) && !Char.IsDigit(e.KeyChar))
            { e.Handled = true; }
        }
        private void txtPrbtytb2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsControl(e.KeyChar) && !Char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            { e.Handled = true; }
            // Allow only one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            { e.Handled = true; }
        }
        private void txtPrbtytb2_Validating(object sender, CancelEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                double i;
                if (double.TryParse(tb.Text, out i))
                { if (i >= 0 && i <= 1) return; }
                MessageBox.Show("Probability should be between 0 & 1.");
                e.Cancel = true;
            }
        }
        private void txtαSDtb2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsControl(e.KeyChar) && !Char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            { e.Handled = true; }
            // Allow only one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            { e.Handled = true; }
        }
        private void txtαSDtb2_Validating(object sender, CancelEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                double i;
                if (double.TryParse(tb.Text, out i))
                { if (i >= 0 && i <= 10) return; }
                MessageBox.Show(" α should be between 0 & 10.");
                e.Cancel = true;
            }
        }

        // Constraints for Tab4
        private void txtFinalYearstb4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsControl(e.KeyChar) && !Char.IsDigit(e.KeyChar))
            { e.Handled = true; }
        }

        // Declare global variables used
        List<Layer> list = new List<Layer>();
        public int attrno;
        public int layercnt = 0;

        // Click events for Tab1
        private void btnSettb1_Click(object sender, EventArgs e)
        {
            try { attrno = int.Parse(txtAttributeCnttb1.Text); }
            catch { MessageBox.Show("Number of layers cannot be null !!"); return; }
            if (attrno == 0) { MessageBox.Show("Number of layers cannot be 0 !!"); return; }
            MessageBox.Show("Number of layers have been set to: " + attrno +"\nLoad first layer along with its Name, Weight, Growth % & File Path...");
        }       
        private string loadnewfile()
        {
            OpenFileDialog o = new OpenFileDialog();
            o.Title = "Open geotiff images";
            o.Filter = "geotiff files(*.tif)|*.tif|jpg files(*.jpeg)|.jpeg|png files(*.png)|*.png";
            DialogResult dr = o.ShowDialog();
            if (dr == DialogResult.OK)
            {
                userfilepath = o.FileName;
            }
            return userfilepath;

        }
        public string userfilepath
        {
            get { return txtLayerPathtb1.Text; }
            set { txtLayerPathtb1.Text = value; }
        }
        private void btnBrowsetb1_Click(object sender, EventArgs e)
        {
            layercnt++;
            try { picbxtb1.Load(loadnewfile()); }
            catch { MessageBox.Show("Could not load file."); return; }
            Layer o = new Layer();
            if (layercnt < attrno)
            {
                list.Add(o.Load(txtLayerNametb1.Text, int.Parse(txtLayerWeighttb1.Text), txtLayerPathtb1.Text, double.Parse(txtGrowthRatetb1.Text)));
                MessageBox.Show("Load next layer along with its Name, Weight, Growth % & File Path..");
                txtLayerNametb1.Clear();
                txtLayerWeighttb1.Clear();
                txtLayerPathtb1.Clear();
                picbxtb1.Image = null;
                txtGrowthRatetb1.Clear();
            }
            else
            {
                layercnt = 0;
                list.Add(o.Load(txtLayerNametb1.Text, int.Parse(txtLayerWeighttb1.Text), txtLayerPathtb1.Text, double.Parse(txtGrowthRatetb1.Text)));
                MessageBox.Show("All layers have been saved.");
            }
        }
        private void btnShowAllLayerstb1_Click(object sender, EventArgs e)
        {
            string str = "";
            try
            {
                for (int i = 0; i < attrno; i++)
                {
                    str+="Layer Name: "+list.ElementAt(i).LayerName+", Weight assigned: "+list.ElementAt(i).LayerWeight+", Layer growth rate: "+list.ElementAt(i).LayerGrowth+"\n";
                }
                if (str == "") MessageBox.Show("No Data found.");
                else MessageBox.Show(str);
            }
            catch { MessageBox.Show("All layers have been cleared");return; }
        }
        private void btnCleartb1_Click(object sender, EventArgs e)
        {
            list.Clear();
            txtAttributeCnttb1.Clear();
            txtLayerNametb1.Clear();
            txtLayerWeighttb1.Clear();
            txtGrowthRatetb1.Clear();
            txtLayerPathtb1.Clear();
            picbxtb1.Image = null;
            picbxoptb2.Image = null;
            txtPrbtytb2.Clear();
            txtαSDtb2.Clear();
            α = 4;
            txtGentb2.Clear();
        }

        // Constants and default variables
        public double α;
        public const int p = 4;

        // Declaration of variables going to be used
        public int row, col; 
        double Pnu;

        // Click events for Tab2
        
        private void btnTesttb2_Click(object sender, EventArgs e)
        {
            picbxoptb2.Image = null;
            // Start stop watch to time
            Stopwatch s = new Stopwatch();
            s.Start();

            int nogen;
            try
            {
                nogen = int.Parse(txtGentb2.Text);
                Pnu = double.Parse(txtPrbtytb2.Text);
                α = double.Parse(txtαSDtb2.Text);
            }
            catch { MessageBox.Show("Enter number of years greater than 0.\nValue of Probability should be between 0 and 1.\nα value should be between 0 and 10."); return; }

            // Get value of rows & col
            if (list.Count == 0) { MessageBox.Show("Number of driving factors have not been assigned"); return; }
            using (Tiff image = Tiff.Open(list.ElementAt(0).LayerPath, "r"))
            {
                if (image == null)
                { MessageBox.Show("Could not open layer: " + list.ElementAt(0).LayerName); return; }

                // Check if it is a type we support
                FieldValue[] value = image.GetField(TiffTag.SAMPLESPERPIXEL);
                if (value == null)
                {
                    MessageBox.Show("Undefined number of samples per pixel");
                    return;
                }
                short spp = value[0].ToShort();
                if (spp != 1)
                {
                    MessageBox.Show("Samples per pixel is not 1");
                    return;
                }
                value = image.GetField(TiffTag.BITSPERSAMPLE);
                if (value == null)
                {
                    MessageBox.Show("Undefined number of bits per sample");
                    return;
                }
                short bps = value[0].ToShort();
                if (bps != 8)
                {
                    MessageBox.Show("Bits per sample is not 8");
                    return;
                }
                value = image.GetField(TiffTag.IMAGEWIDTH);
                if (value == null)
                {
                    MessageBox.Show("Image does not define its width");
                    return;
                }
                col = value[0].ToInt();
                value = image.GetField(TiffTag.IMAGELENGTH);
                if (value == null)
                {
                    MessageBox.Show("Image does not define its width");
                    return;
                }
                row = value[0].ToInt();
                image.Close();
            }

            // Start of computation           
            byte[] ca_check = new byte[row * col];
            for (int gencnt = 0; gencnt < nogen; gencnt++)
            {
                double[,] Dsp1 = new double[row, col];
                if (gencnt == 0)
                {
                    Read_Write r = new Read_Write();
                    byte[] ca_array = new byte[row * col];
                    ca_check = r.ReadImage(list.ElementAt(0).LayerPath,row,col);
                    if (ca_check == null)
                    { MessageBox.Show("Error in loading layer: " + list.ElementAt(0).LayerName); return; }
                    ca_array = CA(ca_check);
                    Dsp1 = CA_Suitability(ca_array);
                }
                else
                {
                    byte[] ca_array = new byte[row * col];
                    ca_array = CA(ca_check);
                    Dsp1 = CA_Suitability(ca_array);
                }

                for (int t = 1; t < attrno; t++)
                {
                    byte[] buf;
                    Read_Write ro = new Read_Write();
                    buf = ro.ReadImage(list.ElementAt(t).LayerPath,row, col);
                    if (buf == null)
                    { MessageBox.Show("Error in loading layer: " + list.ElementAt(t).LayerName); return; }
                    Parallel.For(0, row,
                        R =>
                        {
                            int r = R * col;
                            for (int y = 0; y < col; y++)
                            {
                                double gain = Math.Pow(list.ElementAt(t).LayerWeight, p);
                                double growth = ((list.ElementAt(t).LayerGrowth) / 100) + 1;
                                double groyr = Math.Pow(growth, gencnt);
                                double cellvalue = (double)(buf[r + y]) * groyr;
                                if (cellvalue <= 100)
                                {
                                    double change = Math.Pow(cellvalue, p);
                                    Dsp1[R, y] += gain * change;
                                }
                                else if (cellvalue > 100)
                                {
                                    double change = Math.Pow(100, p);
                                    Dsp1[R, y] += gain * change;
                                }
                            }
                        });
                }
                double DSpmax = 0;
                // Find DSpmax
                Parallel.For(0, row,
                    R =>
                    {
                        int r = R * col;
                        for (int j = 0; j < col; j++)
                        {
                            if (DSpmax < Dsp1[R, j])
                                DSpmax = Dsp1[R, j];
                        }
                    });

                // Compute and compare with cut of prob
                Parallel.For(0, row,
                    R =>
                    {
                        int r = R * col;
                        for (int y = 0; y < col; y++)
                        {
                            if (ca_check[r + y] == 50)
                            {
                                double val = α * (1 - Math.Pow(Dsp1[R, y] / DSpmax, 1.0 / p));
                                val = Math.Pow(2.71828, val);
                                Dsp1[R, y] = 1.0 / val;
                                if (Dsp1[R, y] > Pnu)
                                    ca_check[r + y] = 100;
                            }
                        }
                    });
            }
            Read_Write w = new Read_Write();
            try { picbxoptb2.Load(w.WriteImage(ca_check, row, col)); }
            catch { MessageBox.Show("Image is set to null.");}
            s.Stop();
            long end = s.ElapsedMilliseconds;
            MessageBox.Show("Time taken to complete "+end+" Milliseconds.");
        }
        public double[,] CA_Suitability(byte[] CAarray)
        {
            double[,] DS = new double[row, col];
            Parallel.For(0, row,
                R => {
                    int r = R * col;
                    for (int c = 0; c < col; c++)
                    {
                        if (CAarray[r + c] != 0)
                        {
                            double gain = Math.Pow(list.ElementAt(0).LayerWeight, p);
                            double change = Math.Pow((byte)(CAarray[r + c]), p);
                            DS[R, c] = gain * change; 
                        }
                        else DS[R, c] = 0;
                    }
                });
            return DS;
        }  
        public byte[] CA(byte[] array)
        {
            byte[] newcell = new byte[row * col];  // Lookup table

            // Preparation to create a lookup table
            // For centre elements in the image
            Parallel.For(1, (row - 1),
                R =>
                {
                    int r = R * col;
                    for (int c = 1; c < col-1; c++)
                    {
                        if(array[r+c]==100)
                            for (int i = -1; i <= 1; i++)
                            {
                                for (int j = -col; j <= col; j+=col)
                                {newcell[r + c + i + j] += 1;}
                                newcell[r + c] -= 1;
                            }
                    }

                });
            // Top row and bottom row
            for (int r = 0; r < row*col; r+=((row-1)*col))
            {
                for (int c = 0; c < col; c++)
                {
                    newcell[r + c] = array[r+c];
                    if(array[r+c]==100)
                    {
                        if(r==0)
                        {
                            if(c>1 && c<col-2)
                            { newcell[r + c + col - 1] += 1; newcell[r + c + col] += 1; newcell[r + c + col + 1] += 1; }
                            else if(c==col-2)
                            { newcell[2*col-2] += 1; newcell[2 * col - 3] += 1; }
                            else if(c==1)
                            { newcell[col+1] += 1; newcell[col +2] += 1; }
                            else if(c==0)
                            { newcell[col + 1] += 1; }
                            else if(c==col-1)
                            { newcell[2*col -2] += 1; }                            
                        }
                        else if(r==(row-1)*col)
                        {
                            if (c > 1 && c < col - 2)
                            { newcell[r + c - col - 1] += 1; newcell[r + c - col] += 1; newcell[r + c - col + 1] += 1; }
                            else if (c == col - 2)
                            { newcell[row * col - 2 - col] += 1; newcell[row * col - 2 - col - 1] += 1; }
                            else if (c == 1)
                            { newcell[(row -2)* col + 1] += 1; newcell[(row - 2) * col + 1 + 1] += 1; }
                            else if (c == 0)
                            { newcell[(row - 2) * col + 1] += 1; }
                            else if (c == col - 1)
                            { newcell[row * col - 2 - col] += 1; }                            
                        }                        
                    }
                }
            }
            // For side two coloumns
            for (int r = 0; r < col; r+=col-1)
            {
                for (int c = col; c < (row-1)*col; c+=col)
                {
                    newcell[r + c] = array[r + c];
                    if (array[r + c] == 100)
                    {
                        if (r == 0)
                        {
                            if (c > col && c < (row - 2) * col)
                            { newcell[r + c - col + 1] += 1; newcell[r + c + 1] += 1; newcell[r + c + col + 1] += 1; }
                            else if (c == col)
                            { newcell[col + 1] += 1; newcell[2 * col + 1] += 1; }
                            else if (c == (row - 2) * col)
                            { newcell[(row - 2) * col + 1] += 1; newcell[(row - 2) * col + 1 - col] += 1; }
                        }
                        else if (r == col - 1)
                        {
                            if (c > col && c < (row - 2) * col)
                            { newcell[r + c - col - 1] += 1; newcell[r + c - 1] += 1; newcell[r + c + col - 1] += 1; }
                            else if (c == col)
                            { newcell[2 * (col - 1)] += 1; newcell[2 * (col - 1) + col] += 1; }
                            else if (c == (row - 2) * col)
                            { newcell[(row - 1) * col - 1 - 1] += 1; newcell[(row - 1) * col - 1 - 1 - col] += 1; }
                        }
                    }                  
                }
            }
            // Check lookup table and decide new generation
            for (int r = col; r < (row-1)*col; r+=col)
            {
                for (int c = 1; c < col-1; c++)
                {
                    if (newcell[r + c] > 0)
                    {
                        if (array[r + c] == 50)
                            newcell[r + c] = 100;
                        else newcell[r + c] = array[r + c];
                    }
                    else newcell[r + c] = array[r + c];
                }
            }
            return newcell;
        }

        // For calibration
        private void btnC_Loadtb3_Click(object sender, EventArgs e)
        {
            string s = loadnewC_filetb3();
            if (s == "") { MessageBox.Show("Could not load file."); return; }

            // Get result of testing stage
            Read_Write test = new Read_Write();
            string file = "ans" + Read_Write.simcnt.ToString() + ".tif";
            byte[] predicted = test.ReadImage(file, row, col);
            if (predicted == null)
            { MessageBox.Show("No simulation image found.. !! Check if simulation was done."); return; }
            Read_Write calibrate = new Read_Write();

            byte[] orginal = calibrate.ReadImage(txtC_FilePathtb3.Text, row, col);
            if (orginal==null)
            { MessageBox.Show("Could not real image to be calibrated."); return; }           
            

            // Initialise counters
            int CPcnt = 0;int CError = 0;int OError = 0;int excluded_cell = 0;
            byte[] calibimage = new byte[row * col];

            for (int i = 0; i < row*col; i++)
            {
                // Correctly predicted
                if(predicted[i]==orginal[i] && orginal[i] != 0) { CPcnt++; calibimage[i] = 255; }
                // Comission error
                else if(predicted[i]==100 && orginal[i] == 50) { CError++;calibimage[i] = 150; }
                // Omission Error
                else if(predicted[i] == 50 && orginal[i] == 100) { OError++; calibimage[i] = 50; }
                // Excluded cells
                else if (orginal[i] == 0) { excluded_cell++;calibimage[i] = 0; }
            }

            // Set these values into textboxes
            txtC_CP_tb3.Text = CPcnt.ToString();
            txtC_CError_tb3.Text = CError.ToString();
            txtC_OError_tb3.Text = OError.ToString();
            // Compute % of correct cells
            int total = row * col - excluded_cell;
            double correct = ((double)((CPcnt*100.0)/total));
            double CE = ((double)((CError * 100.0) / total));
            double OE = ((double)((OError * 100.0) / total));
            txtC_CPercent_tb3.Text = correct.ToString();
            txtC_CEPercent_tb3.Text = CE.ToString();
            txtC_OEPercent_tb3.Text = OE.ToString();

            picbxC_op_tb3.Load(test.WriteImageCV(calibimage, row, col,1));
        }
        private string loadnewC_filetb3()
        {
            OpenFileDialog o = new OpenFileDialog();
            o.Title = "Open geotiff images";
            o.Filter = "geotiff files(*.tif)|*.tif|jpg files(*.jpeg)|.jpeg|png files(*.png)|*.png";
            DialogResult dr = o.ShowDialog();
            if (dr == DialogResult.OK)
            {
                userC_filepath = o.FileName;
            }
            return userC_filepath;

        }
        public string userC_filepath
        {
            get { return txtC_FilePathtb3.Text; }
            set { txtC_FilePathtb3.Text = value; }
        }

        // For validation
        private string loadnewV_filetb3()
        {
            OpenFileDialog o = new OpenFileDialog();
            o.Title = "Open geotiff images";
            o.Filter = "geotiff files(*.tif)|*.tif|jpg files(*.jpeg)|.jpeg|png files(*.png)|*.png";
            DialogResult dr = o.ShowDialog();
            if (dr == DialogResult.OK)
            {
                userV_filepath = o.FileName;
            }
            return userV_filepath;

        }
        public string userV_filepath
        {
            get { return txtV_FilePathtb3.Text; }
            set { txtV_FilePathtb3.Text = value; }
        }
        private void btnV_Loadtb3_Click(object sender, EventArgs e)
        {
            string s = loadnewV_filetb3();
            if (s == "") { MessageBox.Show("Could not load file."); return; }

            // Get result of testing stage
            Read_Write test = new Read_Write();
            string file = "ans" + Read_Write.simcnt.ToString() + ".tif";
            byte[] predicted = test.ReadImage(file, row, col);
            if (predicted == null)
            { MessageBox.Show("No simulation image found.. !! Check if simulation was done."); return; }

            Read_Write calibrate = new Read_Write();
            byte[] orginal = calibrate.ReadImage(txtV_FilePathtb3.Text,row,col);
            if (orginal == null)
            { MessageBox.Show("Could not real image to be calibrated."); return; }            

            // Initialise counters
            int CPcnt = 0; int CError = 0; int OError = 0; int excluded_cell = 0;
            byte[] calibimage = new byte[row * col];
            

            for (int i = 0; i < row * col; i++)
            {
                // Correctly predicted
                if (predicted[i] == orginal[i] && orginal[i] != 0) { CPcnt++; calibimage[i] = 255; }
                // Comission error
                else if (predicted[i] == 100 && orginal[i] == 50) { CError++; calibimage[i] = 150; }
                // Omission Error
                else if (predicted[i] == 50 && orginal[i] == 100) { OError++; calibimage[i] = 50; }
                // Excluded cells
                else if (orginal[i] == 0) { excluded_cell++; calibimage[i] = 0; }
            }

            // Set these values into textboxes
            txtV_CP_tb3.Text = CPcnt.ToString();
            txtV_CError_tb3.Text = CError.ToString();
            txtV_OError_tb3.Text = OError.ToString();
            // Compute % of correct cells
            int total = row * col - excluded_cell;
            double correct = ((double)((CPcnt * 100.0) / total));
            double CE = ((double)((CError * 100.0) / total));
            double OE = ((double)((OError * 100.0) / total));
            txtV_CPercent_tb3.Text = correct.ToString();
            txtV_CEPercent_tb3.Text = CE.ToString();
            txtV_OEPercent_tb3.Text = OE.ToString();

            picbxV_op_tb3.Load(test.WriteImageCV(calibimage,row,col,2));            
        }

        // For prediction
        List<string> finalpath = new List<string>();
        int finalcnt = 0;
        private void btnStarttb4_Click(object sender, EventArgs e)
        {
            finalcnt = 0;
            txtFinalYearstb4.Clear();
            txtFinalLayerPathtb4.Clear();
            finalpath.Clear();
            picbx_Output_tb4.Image = null;
            if (list.Count == 0) { MessageBox.Show("Driving factors have not been assigned"); return; }
            MessageBox.Show("Load first layer: "+list.ElementAt(0).LayerName+" with weight: "+list.ElementAt(0).LayerWeight+" for final prediction.");
        }
        private string loadfinallayertb4()
        {
            OpenFileDialog o = new OpenFileDialog();
            o.Title = "Open geotiff images";
            o.Filter = "geotiff files(*.tif)|*.tif|jpg files(*.jpeg)|.jpeg|png files(*.png)|*.png";
            DialogResult dr = o.ShowDialog();
            if (dr == DialogResult.OK)
            {
                finalpathtb4 = o.FileName;
            }
            return finalpathtb4;

        }
        public string finalpathtb4
        {
            get { return txtFinalLayerPathtb4.Text; }
            set { txtFinalLayerPathtb4.Text = value; }
        }
        private void btnLoadFinalPathtb4_Click(object sender, EventArgs e)
        {
            string path = loadfinallayertb4();
            if (path == "") { MessageBox.Show("Could not load file.");return; }
            if (finalcnt > attrno) { MessageBox.Show("All layers have been saved, to make changes click clear button.");return; }
            picbx_Output_tb4.Load(path);
            finalpath.Add(path);
            finalcnt++;
            if (finalcnt < attrno) { MessageBox.Show("Load next layer: "+list.ElementAt(finalcnt).LayerName+" with weight: "+list.ElementAt(finalcnt).LayerWeight+" for final prediction." );}
            else { MessageBox.Show("All layers have been loaded.");finalcnt++;return; }
            txtFinalLayerPathtb4.Clear();
            picbx_Output_tb4.Image = null;
        }
        private void btnClearAlltb4_Click(object sender, EventArgs e)
        {
            finalcnt = 0;
            txtFinalYearstb4.Clear();
            finalpath.Clear();
            txtFinalLayerPathtb4.Clear();
            picbx_Output_tb4.Image = null;
        }
        private void btnPredict_OP_tb4_Click(object sender, EventArgs e)
        {
            // Start stop watch to time
            Stopwatch s = new Stopwatch();
            s.Start();

            int finalgen;
            try { finalgen = int.Parse(txtFinalYearstb4.Text);}
            catch { MessageBox.Show("Enter number of years greater than 0."); return; }

            // Get value of rows & col
            if (list.Count == 0) { MessageBox.Show("Driving factors have not been assigned"); return; }
            if (finalpath.Count == 0) { MessageBox.Show("Load final layers to start prediction.");return; }
            using (Tiff image = Tiff.Open(finalpath.ElementAt(0), "r"))
            {
                if (image == null)
                { MessageBox.Show("Could not open layer: " + list.ElementAt(0).LayerName); return; }

                // Check if it is a type we support
                FieldValue[] value = image.GetField(TiffTag.SAMPLESPERPIXEL);
                if (value == null)
                {
                    MessageBox.Show("Undefined number of samples per pixel");
                    return;
                }
                short spp = value[0].ToShort();
                if (spp != 1)
                {
                    MessageBox.Show("Samples per pixel is not 1");
                    return;
                }
                value = image.GetField(TiffTag.BITSPERSAMPLE);
                if (value == null)
                {
                    MessageBox.Show("Undefined number of bits per sample");
                    return;
                }
                short bps = value[0].ToShort();
                if (bps != 8)
                {
                    MessageBox.Show("Bits per sample is not 8");
                    return;
                }
                value = image.GetField(TiffTag.IMAGEWIDTH);
                if (value == null)
                {
                    MessageBox.Show("Image does not define its width");
                    return;
                }
                col = value[0].ToInt();
                value = image.GetField(TiffTag.IMAGELENGTH);
                if (value == null)
                {
                    MessageBox.Show("Image does not define its width");
                    return;
                }
                row = value[0].ToInt();
                image.Close();
            }
            // Start of computation           
            byte[] ca_check = new byte[row * col];
            for (int gencnt = 0; gencnt < finalgen; gencnt++)
            {
                double[,] Dsp1 = new double[row, col];
                if (gencnt == 0)
                {
                    Read_Write r = new Read_Write();
                    byte[] ca_array = new byte[row * col];
                    ca_check = r.ReadImage(finalpath.ElementAt(0),row,col);
                    if (ca_check == null)
                    { MessageBox.Show("Error in loading layer: " + list.ElementAt(0).LayerName); return; }
                    ca_array = CA(ca_check);
                    Dsp1 = CA_Suitability(ca_array);
                }
                else
                {
                    byte[] ca_array = new byte[row * col];
                    ca_array = CA(ca_check);
                    Dsp1 = CA_Suitability(ca_array);
                }

                for (int t = 1; t < attrno; t++)
                {
                    byte[] buf;
                    Read_Write ro = new Read_Write();
                    buf = ro.ReadImage(finalpath.ElementAt(t),row,col);
                    if (buf == null)
                    { MessageBox.Show("Error in loading layer: " + finalpath.ElementAt(t)); return; }
                    Parallel.For(0, row,
                        R =>
                        {
                            int r = R * col;
                            for (int y = 0; y < col; y++)
                            {
                                double gain = Math.Pow(list.ElementAt(t).LayerWeight, p);
                                double growth = ((list.ElementAt(t).LayerGrowth) / 100) + 1;
                                double groyr = Math.Pow(growth, gencnt);
                                double cellvalue = (double)(buf[r + y]) * groyr;
                                if (cellvalue <= 100)
                                {
                                    double change = Math.Pow(cellvalue, p);
                                    Dsp1[R, y] += gain * change;
                                }
                                else if (cellvalue > 100)
                                {
                                    double change = Math.Pow(100, p);
                                    Dsp1[R, y] += gain * change;
                                }
                            }
                        });
                }
                double DSpmax = 0;
                // Find DSpmax
                Parallel.For(0, row,
                    R =>
                    {
                        int r = R * col;
                        for (int j = 0; j < col; j++)
                        {
                            if (DSpmax < Dsp1[R, j])
                                DSpmax = Dsp1[R, j];
                        }
                    });

                // Compute and compare with cut of prob
                Parallel.For(0, row,
                    R =>
                    {
                        int r = R * col;
                        for (int y = 0; y < col; y++)
                        {
                            if (ca_check[r + y] == 50)
                            {
                                double val = α * (1 - Math.Pow(Dsp1[R, y] / DSpmax, 1.0 / p));
                                val = Math.Pow(2.71828, val);
                                Dsp1[R, y] = 1.0 / val;
                                if (Dsp1[R, y] > Pnu)
                                    ca_check[r + y] = 100;
                            }
                        }
                    });
            }
            Read_Write w = new Read_Write();
            try { picbx_Output_tb4.Load(w.WriteImagePrediction(ca_check,row,col)); }
            catch { MessageBox.Show("Error in writing prediction image."); }
            s.Stop();
            long end = s.ElapsedMilliseconds;
            MessageBox.Show("Time taken to complete " + end + " Milliseconds.");
        }
    }
}
