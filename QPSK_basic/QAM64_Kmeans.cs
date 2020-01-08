using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Numerics;
using MathNet.Numerics.Distributions;


using Inventors.IO.Matlab;

namespace QPSK_basic
{
    class QAM64_Kmeans
    {
        public double[,] init_center = new double[2, 64]; // 초기 기준점

        public Complex[] Com_init_center = new Complex[64];
        public double[,] hk_center = new double[2, 4]; // 채널 추정을 위한 센터 

        public Complex temp_hk;
        public double[,] dist;
        public Complex[] real_Y;
        public Complex[,] temp_Y;
        public int[] position;
        public int ylength;

        public QAM64_Kmeans(Complex[] realY, int cluster, Complex hk)
        {
            ylength = realY.Length;
            dist = new double[64, ylength]; // 거리를 저장
            real_Y = realY;
            
            position = new int[ylength]; // 위치를 저장 (SER을 구하기 위해 필수)
            temp_hk = hk;

            QAM16_Kmeans y_kmeans = new QAM16_Kmeans(realY, cluster, hk);

            Make_init_center(y_kmeans.init_center);
            
            /*
            //test
            MatlabFile file = new MatlabFile("16qam.mat", true);

            Complex[] a = new Complex[ylength];
            Complex[] b = new Complex[ylength];
            Complex[] c = new Complex[ylength];
            Complex[] d = new Complex[ylength];
            Complex[] e = new Complex[ylength];
            Complex[] f = new Complex[ylength];
            Complex[] g = new Complex[ylength];
            Complex[] h = new Complex[ylength];
            Complex[] i = new Complex[ylength];
            Complex[] j = new Complex[ylength];
            Complex[] k = new Complex[ylength];
            Complex[] l = new Complex[ylength];
            Complex[] m = new Complex[ylength];
            Complex[] n = new Complex[ylength];
            Complex[] o = new Complex[ylength];
            Complex[] p = new Complex[ylength];

            double[] are = new double[ylength];
            double[] aim = new double[ylength];
            double[] bre = new double[ylength];
            double[] bim = new double[ylength];
            double[] cre = new double[ylength];
            double[] cim = new double[ylength];
            double[] dre = new double[ylength];
            double[] dim = new double[ylength];

            double[] ere = new double[ylength];
            double[] eim = new double[ylength];
            double[] fre = new double[ylength];
            double[] fim = new double[ylength];
            double[] gre = new double[ylength];
            double[] gim = new double[ylength];
            double[] hre = new double[ylength];
            double[] him = new double[ylength];

            double[] ire = new double[ylength];
            double[] iim = new double[ylength];
            double[] jre = new double[ylength];
            double[] jim = new double[ylength];
            double[] kre = new double[ylength];
            double[] kim = new double[ylength];
            double[] lre = new double[ylength];
            double[] lim = new double[ylength];

            double[] mre = new double[ylength];
            double[] mim = new double[ylength];
            double[] nre = new double[ylength];
            double[] nim = new double[ylength];
            double[] ore = new double[ylength];
            double[] oim = new double[ylength];
            double[] pre = new double[ylength];
            double[] pim = new double[ylength];

            for (int x = 0; x < ylength; x++)
            {
                a[x] = y_kmeans.temp_Y[0, x];
                b[x] = y_kmeans.temp_Y[1, x];
                c[x] = y_kmeans.temp_Y[2, x];
                d[x] = y_kmeans.temp_Y[3, x];
                e[x] = y_kmeans.temp_Y[4, x];
                f[x] = y_kmeans.temp_Y[5, x];
                g[x] = y_kmeans.temp_Y[6, x];
                h[x] = y_kmeans.temp_Y[7, x];
                i[x] = y_kmeans.temp_Y[8, x];
                j[x] = y_kmeans.temp_Y[9, x];
                k[x] = y_kmeans.temp_Y[10, x];
                l[x] = y_kmeans.temp_Y[11, x];
                m[x] = y_kmeans.temp_Y[12, x];
                n[x] = y_kmeans.temp_Y[13, x];
                o[x] = y_kmeans.temp_Y[14, x];
                p[x] = y_kmeans.temp_Y[15, x];

            }

            ComplexToDouble(a, ref are, ref aim);
            ComplexToDouble(b, ref bre, ref bim);
            ComplexToDouble(c, ref cre, ref cim);
            ComplexToDouble(d, ref dre, ref dim);

            ComplexToDouble(e, ref ere, ref eim);
            ComplexToDouble(f, ref fre, ref fim);
            ComplexToDouble(g, ref gre, ref gim);
            ComplexToDouble(h, ref hre, ref him);

            ComplexToDouble(i, ref ire, ref iim);
            ComplexToDouble(j, ref jre, ref jim);
            ComplexToDouble(k, ref kre, ref kim);
            ComplexToDouble(l, ref lre, ref lim);

            ComplexToDouble(m, ref mre, ref mim);
            ComplexToDouble(n, ref nre, ref nim);
            ComplexToDouble(o, ref ore, ref oim);
            ComplexToDouble(p, ref pre, ref pim);

            Matrix acent = new Matrix("a", are, aim);
            Matrix bcent = new Matrix("b", bre, bim);
            Matrix ccent = new Matrix("c", cre, cim);
            Matrix dcent = new Matrix("d", dre, dim);

            Matrix ecent = new Matrix("e", ere, eim);
            Matrix fcent = new Matrix("f", fre, fim);
            Matrix gcent = new Matrix("g", gre, gim);
            Matrix hcent = new Matrix("h", hre, him);

            Matrix icent = new Matrix("i", ire, iim);
            Matrix jcent = new Matrix("j", jre, jim);
            Matrix kcent = new Matrix("k", kre, kim);
            Matrix lcent = new Matrix("l", lre, lim);

            Matrix mcent = new Matrix("m", mre, mim);
            Matrix ncent = new Matrix("n", nre, nim);
            Matrix ocent = new Matrix("o", ore, oim);
            Matrix pcent = new Matrix("p", pre, pim);

            file.Write(acent);
            file.Write(bcent);
            file.Write(ccent);
            file.Write(dcent);
            file.Write(ecent);
            file.Write(fcent);
            file.Write(gcent);
            file.Write(hcent);
            file.Write(icent);
            file.Write(jcent);
            file.Write(kcent);
            file.Write(lcent);
            file.Write(mcent);
            file.Write(ncent);
            file.Write(ocent);
            file.Write(pcent);
            // test
            */

            Kmeans_64qam(y_kmeans.temp_Y, real_Y, cluster);
            
        }

        public void Make_init_center(double[,] center)
        {
            double[] pi = new double[4] { Math.PI / 4, 3 * Math.PI / 4, 5 * Math.PI / 4, 7 * Math.PI / 4 };
            //Complex[] temp_center = new Complex[64];
            for (int i = 0; i < 16; i++)
            {
                int startpos = i * 4;
                for (int j = 0; j < 4; j++)
                {
                    init_center[0, startpos + j] = Math.Cos(pi[j]) + center[0, i];
                    init_center[1, startpos + j] = Math.Sin(pi[j]) + center[1, i];
                }
            }
        }
        public void Kmeans_64qam(Complex[,] temp_y, Complex[] real_y, int cluster)
        {
            int pos = 0;
            int pos_temp = 0;

            for (int start = 0; start < cluster; start++) // cluster 부분
            {
                for (int startpos = 0; startpos < 16; startpos++)
                {
                    pos = startpos * 4;

                    for (int i = 0; i < ylength; i++) // 배열의 열
                    {
                        for (int j = 0; j < 4; j++) // 배열의 행
                        {
                            pos_temp = pos + j;
                            if (temp_y[startpos, i].Real != 0 && temp_y[startpos, i].Imaginary != 0)
                            {
                                double part_Real = Math.Pow(temp_y[startpos, i].Real - init_center[0, pos_temp], 2);
                                double part_Imag = Math.Pow(temp_y[startpos, i].Imaginary - init_center[1, pos_temp], 2);
                                dist[pos_temp, i] = Math.Sqrt(part_Real + part_Imag);// dist는 거리가 저장된다. 4xN행열
                                // Console.WriteLine("count = " + i + " " + pos_temp + " " + dist[pos_temp, i]);
                            }
                        }
                    }
                }
                position = Min_dist(dist, ylength);

                double[,] count = new double[3, 64]; // 0 real 1 imag 2 count
                temp_Y = new Complex[64, ylength]; // y를 군집별로 저장

                for (int k = 0; k < ylength; k++)
                {
                    temp_Y[position[k], k] = real_y[k]; // temp_y를 만들어주고

                    for (int q = 0; q < 64; q++)
                    {
                        if (position[k] == q)
                        {
                            count[0, q] += real_y[k].Real;
                            count[1, q] += real_y[k].Imaginary;
                            count[2, q]++;
                        }
                    }
                }
                for (int v = 0; v < 64; v++)
                {
                    if (count[2, v] != 0)
                    {
                        init_center[0, v] = count[0, v] / count[2, v];
                        init_center[1, v] = count[1, v] / count[2, v];
                    }
                }
            }
            double[] Sreal = new double[64];
            double[] Simag = new double[64];

            // 11_25 추가부분 - 채널 추정을 위한 센터 생성
            for (int i = 0; i < 4; i++)
            {
                int startpos = i * 16; // 0 16 32 48
                double sum_real = 0;
                double sum_imag = 0;
                for (int j = 0; j < 16; j++)
                {
                    sum_real += init_center[0, startpos + j];
                    sum_imag += init_center[1, startpos + j];

                    // test용
                    Com_init_center[startpos + j] = new Complex(init_center[0, startpos + j], init_center[1, startpos + j]);

                }
                sum_real /= 16;
                sum_imag /= 16;
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

            for (int startpos = 0; startpos < 16; startpos++) // 사분면
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
        static void ComplexToDouble(Complex[] com, ref double[] real, ref double[] imag)
        {
            for (int i = 0; i < com.Length; i++)
            {
                real[i] = com[i].Real;
                imag[i] = com[i].Imaginary;
            }
        }
    }
}