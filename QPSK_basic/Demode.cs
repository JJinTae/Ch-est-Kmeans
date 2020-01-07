using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace QPSK_basic
{
    public class Demode
    {
        double[] y_real;
        double[] y_imag;

        double[] data_real;
        double[] data_imag;
        public Complex[] data_sum;
        double Mean; // 16QAM 이상부터 sqrt 

        // 판별할 신호 complex symbol, 해당 신호의 QAM
        public Demode(Complex[] symbol, int qam)
        {
            y_real = new double[symbol.Length];
            y_imag = new double[symbol.Length];
            data_real = new double[symbol.Length];
            data_imag = new double[symbol.Length];
            data_sum = new Complex[symbol.Length];

            if (qam == 4)
            {
                Demode_QPSK(symbol);
            }
            else if (qam == 16)
            {
                Mean = Math.Sqrt(10);
                Demode_QAM16(symbol);
            }
            else if (qam == 64)
            {
                Mean = Math.Sqrt(42);
                Demode_QAM64(symbol);
            }
            else if (qam == 256)
            {
                Mean = Math.Sqrt(340);
                Demode_QAM256(symbol);
            }
            else
            {
                Console.WriteLine("QAM 숫자를 잘못 입력하였습니다.");
                Console.ReadKey();
            }
        }

        public void Demode_QPSK(Complex[] symbol)
        {
            for(int i = 0; i < symbol.Length; i++)
            {
                y_real[i] = symbol[i].Real;
                y_imag[i] = symbol[i].Imaginary;

                if(y_real[i] <= 0)
                    data_real[i] = -1;
                else
                    data_real[i] = 1;

                if(y_imag[i] <= 0)
                    data_imag[i] = -1;
                else
                    data_imag[i] = 1;

                data_sum[i] = new Complex(data_real[i], data_imag[i]);
            }
        }

        public void Demode_QAM16(Complex[] symbol)
        {
            for (int i = 0; i < symbol.Length; i++)
            {
                y_real[i] = symbol[i].Real;
                y_imag[i] = symbol[i].Imaginary;
                /*
                if (y_real[i] < -2 / Mean)
                    data_real[i] = -3;
                else if (y_real[i] > 2 / Mean)
                    data_real[i] = 3;
                else if (y_real[i] > -2 / Mean && y_real[i] <= 0)
                    data_real[i] = -1;
                else
                    data_real[i] = 1;

                if (y_imag[i] < -2 / Mean)
                    data_imag[i] = -3;
                else if (y_imag[i] > 2 / Mean)
                    data_imag[i] = 3;
                else if (y_imag[i] > -2 / Mean && y_imag[i] <= 0)
                    data_imag[i] = -1;
                else
                    data_imag[i] = 1;
                */
                
                if (y_real[i] < -2 / Mean)
                    data_real[i] = -3;
                else if (y_real[i] > 2 / Mean)
                    data_real[i] = 3;
                else if (y_real[i] > -2 / Mean && y_real[i] <=0 )
                    data_real[i] = -1;
                else if (y_real[i] > 0 && y_real[i] <= 2 / Mean)
                    data_real[i] = 1;

                if (y_imag[i] < -2 / Mean)
                    data_imag[i] = -3;
                else if (y_imag[i] > 2 / Mean)
                    data_imag[i] = 3;
                else if (y_imag[i] > -2 / Mean && y_imag[i] <= 0)
                    data_imag[i] = -1;
                else if (y_imag[i] > 0 && y_imag[i] <= 2 / Mean)
                    data_imag[i] = 1;

                data_sum[i] = new Complex(data_real[i], data_imag[i]);
            }
        }
        public void Demode_QAM64(Complex[] symbol)
        {
            for (int i = 0; i < symbol.Length; i++)
            {
                y_real[i] = symbol[i].Real;
                y_imag[i] = symbol[i].Imaginary;

                if (y_real[i] > 0 && y_real[i] <= 2 / Mean)
                    data_real[i] = 1;
                else if (y_real[i] > 2 / Mean && y_real[i] <= 4 / Mean)
                    data_real[i] = 3;
                else if (y_real[i] > 4 / Mean && y_real[i] <= 6 / Mean)
                    data_real[i] = 5;
                else if (y_real[i] > 6 / Mean)
                    data_real[i] = 7;

                if (y_real[i] <= -6 / Mean)
                    data_real[i] = -7;
                else if (y_real[i] <= -4 / Mean && y_real[i] > -6 / Mean)
                    data_real[i] = -5;
                else if (y_real[i] <= -2 / Mean && y_real[i] > -4 / Mean)
                    data_real[i] = -3;
                else if (y_real[i] <= 0 && y_real[i] > -2 / Mean)
                    data_real[i] = -1;

                if (y_imag[i] > 0 && y_imag[i] <= 2 / Mean)
                    data_imag[i] = 1;
                else if (y_imag[i] > 2 / Mean && y_imag[i] <= 4 / Mean)
                    data_imag[i] = 3;
                else if (y_imag[i] > 4 / Mean && y_imag[i] <= 6 / Mean)
                    data_imag[i] = 5;
                else if (y_imag[i] > 6)
                    data_imag[i] = 7;

                if (y_imag[i] <= -6 / Mean)
                    data_imag[i] = -7;
                else if (y_imag[i] <= -4 / Mean && y_imag[i] > -6 / Mean)
                    data_imag[i] = -5;
                else if (y_imag[i] <= -2 / Mean && y_imag[i] > -4 / Mean)
                    data_imag[i] = -3;
                else if (y_imag[i] <= 0 && y_imag[i] > -2 / Mean)
                    data_imag[i] = -1;


                data_sum[i] = new Complex(data_real[i], data_imag[i]);
            }
        }
        public void Demode_QAM256(Complex[] symbol)
        {
            for (int i = 0; i < symbol.Length; i++)
            {
                y_real[i] = symbol[i].Real;
                y_imag[i] = symbol[i].Imaginary;

                if (y_real[i] > 0 && y_real[i] <= 2 / Mean)
                    data_real[i] = 1;
                else if (y_real[i] > 2 / Mean && y_real[i] <= 4 / Mean)
                    data_real[i] = 3;
                else if (y_real[i] > 4 / Mean && y_real[i] <= 6 / Mean)
                    data_real[i] = 5;
                else if (y_real[i] > 6 / Mean && y_real[i] <= 8 / Mean)
                    data_real[i] = 7;
                else if (y_real[i] > 8 / Mean && y_real[i] <= 10 / Mean)
                    data_real[i] = 9;
                else if (y_real[i] > 10 / Mean && y_real[i] <= 12 / Mean)
                    data_real[i] = 11;
                else if (y_real[i] > 12 / Mean && y_real[i] <= 14 / Mean)
                    data_real[i] = 13;
                else
                    data_real[i] = 15;

                if (y_real[i] <= -14 / Mean)
                    data_real[i] = -15;
                else if (y_real[i] <= -12 / Mean && y_real[i] > -14 / Mean)
                    data_real[i] = -13;
                else if (y_real[i] <= -10 / Mean && y_real[i] > -12 / Mean)
                    data_real[i] = -11;
                else if (y_real[i] <= -8 / Mean && y_real[i] > -10 / Mean)
                    data_real[i] = -9;
                else if (y_real[i] <= -6 / Mean && y_real[i] > -8 / Mean)
                    data_real[i] = -7;
                else if (y_real[i] <= -4 / Mean && y_real[i] > -6 / Mean)
                    data_real[i] = -5;
                else if (y_real[i] <= -2 / Mean && y_real[i] > -4 / Mean)
                    data_real[i] = -3;
                else
                    data_real[i] = -1;



                if (y_imag[i] > 0 && y_imag[i] <= 2 / Mean)
                    data_imag[i] = 1;
                else if (y_imag[i] > 2 / Mean && y_imag[i] <= 4 / Mean)
                    data_imag[i] = 3;
                else if (y_imag[i] > 4 / Mean && y_imag[i] <= 6 / Mean)
                    data_imag[i] = 5;
                else if (y_imag[i] > 6 / Mean && y_imag[i] <= 8 / Mean)
                    data_imag[i] = 7;
                else if (y_imag[i] > 8 / Mean && y_imag[i] <= 10 / Mean)
                    data_imag[i] = 9;
                else if (y_imag[i] > 10 / Mean && y_imag[i] <= 12 / Mean)
                    data_imag[i] = 11;
                else if (y_imag[i] > 12 / Mean && y_imag[i] <= 14 / Mean)
                    data_imag[i] = 13;
                else
                    data_imag[i] = 15;

                if (y_imag[i] <= -14 / Mean)
                    data_imag[i] = -15;
                else if (y_imag[i] <= -12 / Mean && y_imag[i] > -14 / Mean)
                    data_imag[i] = -13;
                else if (y_imag[i] <= -10 / Mean && y_imag[i] > -12 / Mean)
                    data_imag[i] = -11;
                else if (y_imag[i] <= -8 / Mean && y_imag[i] > -10 / Mean)
                    data_imag[i] = -9;
                else if (y_imag[i] <= -6 / Mean && y_imag[i] > -8 / Mean)
                    data_imag[i] = -7;
                else if (y_imag[i] <= -4 / Mean && y_imag[i] > -6 / Mean)
                    data_imag[i] = -5;
                else if (y_imag[i] <= -2 / Mean && y_imag[i] > -4 / Mean)
                    data_imag[i] = -3;
                else
                    data_imag[i] = -1;


                data_sum[i] = new Complex(data_real[i], data_imag[i]);
            }
        }

    }
}

