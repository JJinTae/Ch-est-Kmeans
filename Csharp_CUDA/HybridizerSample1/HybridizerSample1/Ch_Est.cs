using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Numerics;
using MathNet.Numerics.Distributions;

namespace HybridizerSample1
{
    class Ch_Est
    {
        public Complex Est_ch = new Complex();
        public double MSE_result;

        // 채널 추정
        public void Est(double[,] center, Complex Hk)
        {
            Complex[] Com_center = new Complex[4];
            // Complex exp = new Complex(0, Math.Exp(1 * Math.PI / 4));
            double[] theta = new double[4];
            double[] dist = new double[4];
            
            Complex[] MSE = new Complex[4];
            
            Complex[] tmp_com = new Complex[4] { new Complex(1, 0), new Complex(-1, 0), new Complex(0, 1), new Complex(0, -1) };
            Complex sqrt2 = new Complex(Math.Sqrt(2), 0);

            // 중심점을 Complex 배열로 만들어 준다.
            for (int i = 0; i < 4; i++)
            {
                Com_center[i] = new Complex(center[0, i], center[1, i]);
                theta[i] = Math.Atan2(Com_center[i].Real, Com_center[i].Imaginary);
            }
            // 각도로 오름차순 정렬한다.
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (theta[i] < theta[j])
                    {
                        double theta_Temp = theta[i];
                        theta[i] = theta[j];
                        theta[j] = theta_Temp;

                        Complex center_Temp = Com_center[i];
                        Com_center[i] = Com_center[j];
                        Com_center[j] = center_Temp;
                    }
                }
            }
            Complex C = Complex.Divide(Com_center[1] + Com_center[2], sqrt2);

            for (int i = 0; i < 4; i++)
            {
                MSE[i] = Complex.Multiply(C, tmp_com[i]);
                dist[i] = Complex.Abs(Complex.Pow(MSE[i] - Hk, 2));
            }
            
            Est_ch = MSE[dist.ToList().IndexOf(dist.Min())];
            MSE_result = dist.Min();
        }
    }
}