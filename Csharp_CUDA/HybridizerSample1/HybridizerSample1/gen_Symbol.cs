using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using MathNet.Numerics.Distributions;

namespace HybridizerSample1
{
    class gen_Symbol
    {
        static Random rnd = new Random();
        public Complex Hk;
        // S 심볼 생성(몇qam인지, 심볼의 갯수)
        public Complex[] Gen_S_Symbol(int qam, int SymbolNumber)
        {
            // Complex 배열 생성
            Complex[] ComArray = new Complex[SymbolNumber];
            // 더블형의 실수부와 허수부
            double Re, Im; 
            int[] Symbol = new int[] { 1, -1, 3, -3, 5, -5, 7, -7, 9, -9, 11, -11, 13, -13, 15, -15 };
            Hk = new Complex();
            Hk = Gen_ch();
            
            // QPSK
            if (qam == 4)
            {
                double Mean = Math.Sqrt(2);
                for (int i = 0; i < SymbolNumber; i++)
                {
                    Re = Symbol[rnd.Next(0, 2)] / Mean;
                    Im = Symbol[rnd.Next(0, 2)] / Mean;
                    ComArray[i] = new Complex(Re, Im);
                }
            }
            // 16QAM
            else if (qam == 16)
            {
                double Mean = Math.Sqrt(10);
                for (int i = 0; i < SymbolNumber; i++)
                {
                    Re = Symbol[rnd.Next(0, 4)] / Mean;
                    Im = Symbol[rnd.Next(0, 4)] / Mean;
                    ComArray[i] = new Complex(Re, Im);
                }
            }
            // 64QAM
            else if (qam == 64)
            {
                double Mean = Math.Sqrt(42);
                for (int i = 0; i < SymbolNumber; i++)
                {
                    Re = Symbol[rnd.Next(0, 8)] / Mean;
                    Im = Symbol[rnd.Next(0, 8)] / Mean;
                    ComArray[i] = new Complex(Re, Im);
                }
            }
            // 256QAM
            else if (qam == 256)
            {
                double Mean = Math.Sqrt(340);
                for (int i = 0; i < SymbolNumber; i++)
                {
                    Re = Symbol[rnd.Next(0, 16)] / Mean;
                    Im = Symbol[rnd.Next(0, 16)] / Mean;
                    ComArray[i] = new Complex(Re, Im);
                }
            }
            return ComArray;
        }

        public Complex[] AddNoise(Complex[] s, int length, double SNR)
        {
            // noise 배열 생성
            Complex[] noise = new Complex[length];
            Complex[] y = new Complex[length];
            double temp = Math.Pow(10, -SNR / 20);

            // 입력받은 s의 배열길이 만큼 real 부분의 정규분포 난수 배열 생성
            Normal gen_re_noise = new Normal(0, 1);
            double[] re_noise = new double[length];
            gen_re_noise.Samples(re_noise);

            // 입력받은 s의 배열길이 만큼 imaginary 부분의 정규분포 난수 배열 생성
            Normal gen_imag_noise = new Normal(0, 1);
            double[] imag_noise = new double[length];
            gen_imag_noise.Samples(imag_noise);

            double Mean = Math.Sqrt(2);

            // 정규분포 난수에 sqrt(2)를 나누고 
            for (int i = 0; i < length; i++)
            {
                re_noise[i] = re_noise[i] / Mean * temp;
                imag_noise[i] = imag_noise[i] / Mean * temp;

                noise[i] = new Complex(re_noise[i], imag_noise[i]);

                y[i] = noise[i] + s[i];
            }
            return y;
        }

        // 채널 생성
        public Complex Gen_ch()
        {
            Normal CH_RE = new Normal(0, 1);
            Normal CH_IM = new Normal(0, 1);
            double ch_Re = CH_RE.Sample() / Math.Sqrt(2);
            double ch_Im = CH_IM.Sample() / Math.Sqrt(2);
            Complex ch = new Complex(ch_Re, ch_Im);

            return ch;
        }

        public Complex[] MultyCh(Complex[] s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                s[i] = Complex.Multiply(s[i], Hk);
            }
            return s;
        }

        public Complex[] DividCh(Complex[] s, Complex est_ch)
        {
            for (int i = 0; i < s.Length; i++)
            {
                s[i] = Complex.Divide(s[i], est_ch);
            }
            return s;
        }
    }
}
