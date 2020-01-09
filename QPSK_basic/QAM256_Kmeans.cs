using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Numerics;
using MathNet.Numerics.Distributions;

namespace QPSK_basic
{
    class QAM256_Kmeans
    {
        public double[,] init_center = new double[2, 256]; // 초기 기준점

        public Complex[] Com_init_center = new Complex[256];
        public double[,] hk_center = new double[2, 4]; // 채널 추정을 위한 센터

        public Complex temp_hk;
        public double[,] dist;
        public Complex[] real_Y;
        public Complex[,] temp_Y;
        public int[] position;
        public int ylength;

        public QAM256_Kmeans(Complex[] realY, int cluster, Complex hk)
        {
            ylength = realY.Length;
            dist = new double[256, ylength]; // 거리를 저장
            real_Y = realY;

            position = new int[ylength]; // 위치를 저장 (SER을 구하기 위해 필수)
            temp_hk = hk;

            QAM64_Kmeans y_kmeans = new QAM64_Kmeans(realY, cluster, hk);

            Make_init_center(y_kmeans.init_center);

            Kmeans_256qam(y_kmeans.temp_Y, real_Y, cluster);
        }

        public void Make_init_center(double[,] center)
        {
            double[] pi = new double[4] { Math.PI / 4, 3 * Math.PI / 4, 5 * Math.PI / 4, 7 * Math.PI / 4 };
            for (int i = 0; i < 64; i++)
            {
                int startpos = i * 4; // 0 4 8 12
                for (int j = 0; j < 4; j++)
                {
                    init_center[0, startpos + j] = Math.Cos(pi[j]) + center[0, i];
                    init_center[1, startpos + j] = Math.Sin(pi[j]) + center[1, i];
                }
            }
        }

        public void Kmeans_256qam(Complex[,] temp_y, Complex[] real_y, int cluster)
        {
            int pos = 0;
            int pos_temp = 0;

            for (int start = 0; start < cluster; start++) // cluster 부분
            {
                for (int startpos = 0; startpos < 64; startpos++)
                {
                    pos = startpos * 4;

                    for (int i = 0; i < ylength; i++) // 배열의 열
                    {
                        for (int j = 0; j < 4; j++) // 배열의 행
                        {
                            pos_temp = pos + j; // startpos가 0일 때 pos = 0 1 2 3
                            //Console.WriteLine(pos_temp);
                            if (temp_y[startpos, i].Real != 0 && temp_y[startpos, i].Imaginary != 0)
                            {
                                double part_Real = Math.Pow(temp_y[startpos, i].Real - init_center[0, pos_temp], 2);
                                double part_Imag = Math.Pow(temp_y[startpos, i].Imaginary - init_center[1, pos_temp], 2);
                                dist[pos_temp, i] = Math.Sqrt(part_Real + part_Imag); // dist는 거리가 저장된다. 4xN행열
                                //Console.WriteLine("count = " + i + " " + pos_temp + " " + dist[pos_temp, i]);
                            }
                        }
                    }
                }
                position = Min_dist(dist, ylength);

                double[,] count = new double[3, 256]; // 0 real 1 imag 2 count
                temp_Y = new Complex[256, ylength]; // y를 군집별로 저장

                for (int k = 0; k < ylength; k++)
                {
                    temp_Y[position[k], k] = real_y[k]; // temp_y를 만들어주고

                    for (int q = 0; q < 256; q++)
                    {
                        if (position[k] == q)
                        {
                            count[0, q] += real_y[k].Real;
                            count[1, q] += real_y[k].Imaginary;
                            count[2, q]++;
                        }
                    }
                }
                for (int v = 0; v < 256; v++)
                {
                    if (count[2, v] != 0)
                    {
                        init_center[0, v] = count[0, v] / count[2, v];
                        init_center[1, v] = count[1, v] / count[2, v];
                    }
                }
            }
            // 11_25 추가부분 - 채널 추정을 위한 센터 생성
            for (int i = 0; i < 4; i++)
            {
                int startpos = i * 64; // 0 16 32 48
                double sum_real = 0;
                double sum_imag = 0;
                for (int j = 0; j < 64; j++)
                {
                    sum_real += init_center[0, startpos + j];
                    sum_imag += init_center[1, startpos + j];

                    // test용
                    Com_init_center[startpos + j] = new Complex(init_center[0, startpos + j], init_center[1, startpos + j]);

                }
                sum_real /= 64;
                sum_imag /= 64;
                hk_center[0, i] = sum_real;
                hk_center[1, i] = sum_imag;
            }
            // 여기까지
        }




        public int[] Min_dist(double[,] dist, int length) // 거리 dist와 dist의 2번째 행열 길이를 받음
        {
            double temp = 0;
            int pos = 0;
            int[] position = new int[length];

            for (int startpos = 0; startpos < 64; startpos++) // 사분면
            {
                for (int i = 0; i < length; i++) // 각 심볼
                {
                    // 최솟값을 찾는 알고리즘
                    for (int j = 0; j < 4; j++) // 4개씩
                    {
                        pos = startpos * 4 + j; // 0 1 2 3 / 4 5 6 7 / 8 9 10 11 / 12 13 14 15

                        if (dist[pos, i] != 0)
                        {
                            if (pos % 4 == 0)
                            {
                                temp = dist[pos, i];
                                position[i] = pos;
                            }
                            else if (temp > dist[pos, i])
                            {
                                temp = dist[pos, i];
                                position[i] = pos;
                            }
                            // Console.WriteLine(dist[j, i]);
                        }
                    }

                }
            }
            return position;
        }
    }
}
