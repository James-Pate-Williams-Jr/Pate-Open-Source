using System;
using System.Windows.Forms;

namespace Exterior_Ballistics_Page_41
{
    public partial class Form1 : Form
    {
        private const double stdDensity = 1.2034;   // SI units

        public Form1()
        {
            InitializeComponent();

            double humidity = 0.78;
            double[] diameters = { 3, 4, 5, 6, 8, 12, 14, 16 };
            double[] weights = { 13, 33, 50, 105, 200, 870, 1400, 2100 };
            double[] iData = { 1.0, 0.67, 0.59, 0.61, 0.61, 0.61, 0.7, 0.61 };
            double[] temp = { 61, 65, 57, 70, 85, 93, 69, 32 };
            double[] pressures = { 29.80, 29.60, 30.25, 30.50, 29.75, 30.20, 29.80, 30.15 };
            double[] height = { 1000, 18000, 8000, 13000, 15000 };
            
            textBox1.Text += "Diam\tWt\ti\tTemp\tBar\tC\t\tlog C\r\n\r\n";

            try
            {
                /*K = new double[log10K.Length];

                for (int i = 0; i < log10K.Length; i++)
                    K[i] = Math.Pow(10.0, log10K[i]);*/

                for (int i = 0; i < 8; i++)
                {
                    double pascals = pressures[i] / 0.00029529980;
                    double atm = pascals * 9.8692316931427E-6;
                    double temperature = (temp[i] - 32.0) / 1.8;
                    double density = NISTDensityHumidAir(temperature, humidity, atm * 101325);
                    double BC = weights[i] / ((density / stdDensity) * iData[i] * diameters[i] * diameters[i]);

                    textBox1.Text += diameters[i].ToString().PadLeft(2) + "\t" + weights[i].ToString().PadLeft(4) + "\t"
                        + iData[i].ToString("F2") + "\t" + temp[i].ToString("F2") + "\t" + pressures[i].ToString("F2")
                        + "\t" + BC.ToString("F5").PadLeft(8) + "\t" + Math.Log10(BC).ToString("F5").PadLeft(7) + "\r\n";
                }

                temp[0] = 65;
                temp[1] = 85;
                temp[2] = 57;
                temp[3] = 69;
                temp[4] = 32;
                pressures[0] = 29.00;
                pressures[1] = 22.76;
                pressures[2] = 30.25;
                pressures[3] = 29.80;
                pressures[4] = 30.15;

                textBox1.Text += "\r\nTemp\tBar\tH\tD\tRa\tRf\tva\r\n\r\n";

                double[] v1 = { 2680, 3140, 2870, 2590 };
                double[] v2 = { 2572, 3088, 2854, 2578 };
                double[] wt = { 13, 50, 870, 2100 };

                for (int i = 0; i < 4; i++)
                {
                    double v1sv2 = v1[i] - v2[i];
                    double v1av2 = v1[i] + v2[i];
                    double l2 = 1000;
                    double Ra = v1av2 * v1sv2 / l2;
                    double Rf = wt[i] * Ra / 32.16;
                    double va = v1av2 / 2.0;

                    textBox1.Text += weights[i].ToString().PadLeft(4) + "\t" + (l2 / 2).ToString("F2").PadLeft(5) + "\t"
                        + v1[i].ToString().PadLeft(4) + "\t" + v2[i].ToString().PadLeft(4) + "\t" + Rf.ToString("F2").PadLeft(7) + "\t"
                        + Ra.ToString("F2").PadLeft(7) + "\t" + va.ToString().PadLeft(4) + "\r\n";
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private double Density(double y)
        {
            return Math.Pow(10, -0.00001372 * y);
        }

        private int ComputeIndex(double v)
        {
            int index = 0;

            if (v > 3600)
                index = 6;

            else if (v > 2600 && v <= 3600)
                index = 5;

            else if (v > 1800 && v <= 2600)
                index = 4;

            else if (v > 1370 && v <= 1800)
                index = 3;

            else if (v > 1230 && v <= 1370)
                index = 2;

            else if (v > 790 && v <= 1230)
                index = 1;

            else
                index = 0;

            return index;
        }

        private double NISTDensityHumidAir(double t, double humidity, double p)
        {
            // t is the temperature in Celsius
            // 0 <= humidity <= 1
            // p is pressure in Pascals
            // density is returned in SI units kg / (m * m * m)
            // 15 <= t <= 27 for best results
            // 600 hPa <= p <= 1100 hPa or 60000 Pa <= p <= 110000 Pa for best results

            double T = 273.15 + t;
            double R = 8.314472;
            double factor = p / (R * T);
            double a0 = 1.58123e-6;
            double a1 = -2.9331e-8;
            double a2 = 1.1043e-10;
            double b0 = 5.707e-6;
            double b1 = -2.051e-8;
            double c0 = 1.9898e-4;
            double c1 = -2.376e-6;
            double d = 1.83e-11;
            double e = -0.765e-8;
            double Ma = 28.96546e-3;
            double c = 0.3780;
            double alpha = 1.00062;
            double beta = 3.14e-8;
            double gamma = 5.6e-7;
            double f = alpha + beta * p + gamma * t * t;
            double A = 1.2378847e-5;
            double B = -1.9121316e-2;
            double C = 33.93711047;
            double D = -6.3431645e3;
            double psv = Math.Exp(A * T * T + B * T + C + D / T);
            double xv = humidity * f * psv / p;
            double f1 = a0 + a1 * t + a2 * t * t + (b0 + b1 * t) * xv + (c0 + c1 * t) * xv * xv;
            double Z = 1.0 - (p / T) * f1 + (p * p) / (T * T) * (d + e * xv * xv);

            return factor * Ma * (1 - xv * c) / Z;
        }
    }
}