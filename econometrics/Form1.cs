using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ZedGraph;

namespace econometrics
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            comboBox1.Items.Add("Аддитивная");
            comboBox1.Items.Add("Мультипликативная");    
        }
            public List<double> list = new List<double>();
            public double[] x;
            public double[] y;
            public GraphPane myPane;
            int k;
            double[] sk;
        
         
        private void button1_Click(object sender, EventArgs e)
        {
            string line;
            
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                StreamReader file =new StreamReader(ofd.FileName);
                while ((line = file.ReadLine()) != null)
                {
                    double d = Double.Parse(line);
                    list.Add(d);
                }
                file.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            myPane = zedGraphControl1.GraphPane;
            x= new double[list.Count];
            y = new double[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                x[i] = i;
                y[i] = list[i];
            }
            zedGraphControl1.GraphPane.CurveList.Clear();
            PointPairList spl1 = new PointPairList(x, y);
            LineItem myCurve1 = myPane.AddCurve("Data", spl1, Color.Blue, SymbolType.Diamond);
            myCurve1.Line.Width = 3.0F;
            myPane.Title.Text = "***";  
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
            zedGraphControl1.Refresh();
        }

        public int Period
        {
            get
            {
                if (textBox1.Text == "")
                {
                    return 0;
                }
                else
                {
                    return int.Parse(textBox1.Text);
                }
            }
            set
            {
                textBox1.Text = "" + value;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            PointPairList ppl=new PointPairList();
            PointPair[] spl2 = new PointPair[list.Count - k];
            if (comboBox1.Text == "Аддитивная")
            {

                if (Period % 2 == 0)
                {//четный
                    k = (Period) / 2;
                    sk = new double[list.Count - k];
                    for (int i = k + 1; i < list.Count - k; i++)
                    {
                        sk[i] = 0;
                        for (int j = i - k; j < i + k + 1; j++)
                        {
                            sk[i] = y[j]/ (2*Period) + sk[i];
                            sk[i] = sk[i] ;
                            spl2[i] = new PointPair(i, sk[i]);
                            
                        }
                        ppl.Add(spl2[i]);
                    }
                }
                else
                {
                    k = (Period - 1) / 2;
                    sk = new double[list.Count - k];
                    for (int i = k + 1; i < list.Count - k; i++)
                    {
                        sk[i] = 0;
                        for (int j = i - k; j < i + k + 1; j++)
                        {
                            sk[i] = y[j]/Period  + sk[i];
                            spl2[i] = new PointPair(i, sk[i]);
                        }
                        ppl.Add(spl2[i]);
                    }

                }
            }
            if (comboBox1.Text == "Мультипликативная")
            {
                double g;
                if (Period % 2 != 0)
                {
                    MessageBox.Show(""+y[6]);
                    k = (Period - 1) / 2;
                    sk = new double[list.Count - k];
                    for (int i = k + 1; i < list.Count - k; i++)
                    {
                        sk[i] = 0;
                        for (int j = i - k; j < i + k + 1; j++)
                        {
                            g = y[j];
                            y[j] = Math.Log(y[j]);
                            if (Double.IsNaN(y[j]))
                            {
                                throw new Exception(y[j]+"-"+g);
                            }
                            sk[i] = y[j]/ Period + sk[i];
                            sk[i] = Math.Exp(sk[i]);
                            spl2[i] = new PointPair(i, sk[i]);
                        }
                        ppl.Add(spl2[i]);
                    }
                }
                else
                {
                    k = (Period) / 2;
                    sk = new double[list.Count - k];
                    for (int i = k + 1; i < list.Count - k; i++)
                    {
                        sk[i] = 0;
                        for (int j = i - k; j < i + k + 1; j++)
                        {
                            y[j] = Math.Log(y[j]);
                            sk[i] = y[j]/ (2 * Period) + sk[i];
                            sk[i] = Math.Exp(sk[i]);
                            spl2[i] = new PointPair(i, sk[i]);
                        }
                        ppl.Add(spl2[i]);
                    }
                }
            }
            
            LineItem myCurve2 = myPane.AddCurve("Skolz", ppl, Color.Red, SymbolType.Diamond);
            myCurve2.Line.Width = 3.0F;
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
            zedGraphControl1.Refresh();
        }
    }
}