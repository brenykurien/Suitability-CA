using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitMiracle.LibTiff.Classic;
using System.Windows.Forms;
using System.IO;

namespace Suitablity_based_CA
{
    class Read_Write
    {
        // Author : Breny Kurien
        // Work done as a part of M.Tech project at ADRIN

        public byte[] buffernull=null;   // return null array
        
        // Read and Write Images for simulation
        public byte[] ReadImage(string Layerpath,int row, int col)
        {
            byte[] buffer;
            // Open the TIFF image
            using (Tiff image = Tiff.Open(Layerpath, "r"))
            {
                if (image == null)
                {
                    MessageBox.Show("Could not open incoming image"+Layerpath);
                    //throw new System.InvalidOperationException("Could not open incoming image"+Layerpath);
                    return buffernull;
                }

                // Check that it is of a type that we support
                FieldValue[] value = image.GetField(TiffTag.BITSPERSAMPLE);
                if (value == null)
                {
                    MessageBox.Show("Undefined number of bits per sample");
                    return buffernull;
                }
                short bps = value[0].ToShort();
                if (bps != 8)
                {
                    MessageBox.Show("Bits per sample is not 8");
                    return buffernull;
                }

                value = image.GetField(TiffTag.SAMPLESPERPIXEL);
                if (value == null)
                {
                    MessageBox.Show("Undefined number of samples per pixel");
                    return buffernull;
                }
                short spp = value[0].ToShort();
                if (spp != 1)
                {
                    MessageBox.Show("Samples per pixel is not 1");
                    return buffernull;
                }

                value = image.GetField(TiffTag.IMAGEWIDTH);
                if (value == null)
                {
                    MessageBox.Show("Image does not define its width");
                    return buffernull;
                }
                int col1 = value[0].ToInt();

                value = image.GetField(TiffTag.IMAGELENGTH);
                if (value == null)
                {
                    MessageBox.Show("Image does not define its width");
                    return buffernull;
                }
                int row1 = value[0].ToInt();

                if (row != row1 || col != col1) { MessageBox.Show("All the images are not of the same size. Please verify this."); return buffernull; }
                buffer = new byte[row * col];
                int c = 0;
                for (int r = 0; r < row; r++)
                {
                    byte[] buf = new byte[col];
                    image.ReadScanline(buf, r);
                    for (int j = 0; j < col; j++)
                    {buffer[c + j] = buf[j];}
                    c += col;
                }                

                // Deal with photometric interpretations
                value = image.GetField(TiffTag.PHOTOMETRIC);
                if (value == null)
                {
                    MessageBox.Show("Image has an undefined photometric interpretation");
                    return buffernull;
                }

                Photometric photo = (Photometric)value[0].ToInt();
                if (photo != Photometric.MINISWHITE)
                {
                    // Flip bits
                    MessageBox.Show("Fixing the photometric interpretation");

                    for (int count = 0; count < row*col; count++)
                        buffer[count] = (byte)~buffer[count];
                }

                // Deal with fillorder
                value = image.GetField(TiffTag.FILLORDER);
                if (value == null)
                {
                    MessageBox.Show("Image has an undefined fillorder");
                    return buffernull;
                }

                FillOrder fillorder = (FillOrder)value[0].ToInt();
                if (fillorder != FillOrder.MSB2LSB)
                {
                    // We need to swap bits -- ABCDEFGH becomes HGFEDCBA
                    MessageBox.Show("Fixing the fillorder");

                    for (int count = 0; count < row*col; count++)
                    {
                        byte tempbyte = 0;
                        if ((buffer[count] & 128) != 0) tempbyte += 1;
                        if ((buffer[count] & 64) != 0) tempbyte += 2;
                        if ((buffer[count] & 32) != 0) tempbyte += 4;
                        if ((buffer[count] & 16) != 0) tempbyte += 8;
                        if ((buffer[count] & 8) != 0) tempbyte += 16;
                        if ((buffer[count] & 4) != 0) tempbyte += 32;
                        if ((buffer[count] & 2) != 0) tempbyte += 64;
                        if ((buffer[count] & 1) != 0) tempbyte += 128;
                        buffer[count] = tempbyte;
                    }
                }        
                image.Close();
            }
            return buffer;
        }
        
        public static int simcnt; // Counter for simulation 
        public string WriteImage(byte[] arr,int row,int col)
        {
            simcnt++;
            string opname = "ans"+ simcnt.ToString() +".tif";
            //string file = opname + filecounter.ToString() + ".tif";
            // Open the TIFF file
            using (Tiff image = Tiff.Open(opname, "w"))
            {
                if (image == null)
                {
                    MessageBox.Show("Could not open " + opname + " for writing");
                    return null;
                }
                image.SetField(TiffTag.IMAGEWIDTH, col);
                image.SetField(TiffTag.IMAGELENGTH, row);

                // Need some attention 1 for bilevel,grayscale and patellte colour images
                // 3 for RGB images
                image.SetField(TiffTag.SAMPLESPERPIXEL, 1);
                image.SetField(TiffTag.BITSPERSAMPLE, 8);                
                image.SetField(TiffTag.ROWSPERSTRIP, row);

                image.SetField(TiffTag.COMPRESSION, Compression.NONE);
                image.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISWHITE);
                image.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);
                image.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);

                image.SetField(TiffTag.XRESOLUTION, 3000);
                image.SetField(TiffTag.YRESOLUTION, 3000);
                image.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.CENTIMETER);

                int c = 0;
                for (int i = 0; i < row; i++)
                {
                    byte[] buf = new byte[col];
                    for (int j = 0; j < col; j++)
                    { buf[j] = arr[c + j]; }
                    image.WriteScanline(buf, i);
                    c += col;
                }
                image.WriteDirectory();
                image.Close();
            }
            return opname;
        }
        
        static int Ccnt,Vcnt; // Counter for calibration and validation
        public string WriteImageCV(byte[] array, int row, int col, int a)
        {
            
            string opname;
            if (a == 1) { Ccnt++; opname = "calibrated"+ Ccnt.ToString()+ ".tif"; }
            else { Vcnt++; opname = "validated"+ Vcnt.ToString()+ ".tif"; }
            
            using (Tiff output = Tiff.Open(opname, "w"))
            {
                output.SetField(TiffTag.IMAGEWIDTH, col);
                output.SetField(TiffTag.IMAGELENGTH, row);
                output.SetField(TiffTag.SAMPLESPERPIXEL, 3);
                output.SetField(TiffTag.BITSPERSAMPLE, 8);
                output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                output.SetField(TiffTag.PHOTOMETRIC, Photometric.RGB);
                output.SetField(TiffTag.COMPRESSION, Compression.DEFLATE);
                int temp = 0;
                byte[] buf = new byte[col * 3];
                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < col; j++)
                    {
                        if (array[temp + j] == 255) { buf[j * 3+0]= 0;buf[j * 3 + 1] = 255;buf[j * 3 + 2] = 0; }
                        else if (array[temp + j] == 50) { buf[j * 3 + 0] = 255; buf[j * 3 + 1] = 0; buf[j * 3 + 2] = 0; }
                        else if (array[temp + j] == 150) { buf[j * 3 + 0] = 0; buf[j * 3 + 1] = 0; buf[j * 3 + 2] = 255; }
                        else if (array[temp + j] == 0) { buf[j * 3 + 0] = 255; buf[j * 3 + 1] = 255; buf[j * 3 + 2] = 255; }
                        else { MessageBox.Show("Error in writing image CV.");}
                    }
                    temp += col;
                    output.WriteScanline(buf, i);
                }
                output.WriteDirectory();
                output.Close();            
            }
            return opname;
        }
        
        static int Pcnt; // Counter for prediction
        public string WriteImagePrediction(byte[] array, int row, int col)
        {
            Pcnt++;
            string opname="prediction"+Pcnt.ToString()+".tif"; ;
            //string file = opname + Predictiocnt.ToString() + ".tif";
            using (Tiff output = Tiff.Open(opname, "w"))
            {
                output.SetField(TiffTag.IMAGEWIDTH, col);
                output.SetField(TiffTag.IMAGELENGTH, row);
                output.SetField(TiffTag.SAMPLESPERPIXEL, 3);
                output.SetField(TiffTag.BITSPERSAMPLE, 8);
                output.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                output.SetField(TiffTag.PHOTOMETRIC, Photometric.RGB);
                output.SetField(TiffTag.COMPRESSION, Compression.DEFLATE);
                int temp = 0;
                byte[] buf = new byte[col * 3];
                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < col; j++)
                    {
                        if (array[temp + j] == 50) { buf[j * 3 + 0] = 255; buf[j * 3 + 1] = 198; buf[j * 3 + 2] = 150; }
                        else if (array[temp + j] == 100) { buf[j * 3 + 0] = 255; buf[j * 3 + 1] = 0; buf[j * 3 + 2] = 0; }
                        else if (array[temp + j] == 0) { buf[j * 3 + 0] = 0; buf[j * 3 + 1] = 0; buf[j * 3 + 2] = 0; }
                        else { MessageBox.Show("Error in writing prediction image."); }
                    }
                    temp += col;
                    output.WriteScanline(buf, i);
                }
                output.WriteDirectory();
                output.Close();
            }
            return opname;
        }
    }
}
