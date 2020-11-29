using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using MathNet.Numerics.Distributions;


namespace QPSK_basic
{
    class QPSK_MSE
    {
        public QPSK_MSE(double[,] center, Complex Hk)
        {
            double MSE = 0;
            Complex[] H = new Complex[4];
            Complex[] exp = new Complex[4];
            Complex[] temp_center = new Complex[4];
            double[] result = new double[4];


            exp[0] = new Complex(0, 1 * Math.PI / 4);
            exp[1] = new Complex(0, 3 * Math.PI / 4);
            exp[2] = new Complex(0, 5 * Math.PI / 4);
            exp[3] = new Complex(0, 7 * Math.PI / 4);

            for (int i = 0; i < 4; i++)
            {
                temp_center[i] = new Complex(center[0, i], center[1, i]);
                H[i] = Complex.Multiply(Hk, exp[i]);
            }

            for (int j = 0; j < 4; j++)
            {
                Complex a = Complex.Subtract(temp_center[j], H[0]);
                a = Complex.Multiply(a, a);
                result[j] = Complex.Abs(a);
            }
            Console.WriteLine(result.Min() + "");
        }
    }
}