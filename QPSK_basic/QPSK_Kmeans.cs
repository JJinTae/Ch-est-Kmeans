using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Numerics;
using MathNet.Numerics.Distributions;



namespace QPSK_basic
{
    public class QPSK_Kmeans
    {
        public double[,] init_center = new double[2, 4] { { 1, -1, -1, 1 }, { 1, 1, -1, -1 } }; // 초기 기준점
        public Complex[] Com_init_center = new Complex[4] { new Complex(1,1), new Complex(-1, 1) , new Complex(-1, -1) , new Complex(1, -1) };

        public double[,] dist;
        public Complex[,] temp_Y;
        public int[] position;

        public QPSK_Kmeans(Complex[] y, int cluster, Complex hk)
        {
            dist = new double[4, y.Length]; // 거리를 저장
            temp_Y = new Complex[4, y.Length]; // y를 군집별로 저장
            position = new int[y.Length]; // 위치를 저장 (SER을 구하기 위해 필수)

            Kmeans_qpsk_ch(y, cluster, hk);
        }
        public void Kmeans_qpsk_ch(Complex[] y, int cluster, Complex hk)
        {
            // 초기센터 초기화
            for (int i = 0; i < 4; i++)
            {
                Com_init_center[i] = Complex.Multiply(Com_init_center[i], hk);
                init_center[0, i] = Com_init_center[i].Real;
                init_center[1, i] = Com_init_center[i].Imaginary;
            }
            for (int start = 0; start < cluster; start++) // cluster 부분
            {
                for (int i = 0; i < y.Length; i++) // 배열의 열
                {
                    for (int j = 0; j < 4; j++) // 배열의 행
                    {
                        double part_Real = Math.Pow(y[i].Real - init_center[0, j], 2);
                        double part_Imag = Math.Pow(y[i].Imaginary - init_center[1, j], 2);
                        dist[j, i] = Math.Sqrt(part_Real + part_Imag); // dist는 거리가 저장된다. 4xN행열
                    }
                }

                position = Min_dist(dist, y.Length);

                // 새로운 센터 생성
                /*
                 * 1. 4xN 행렬에 각 Complex 넣기 (군집을 표현하기 위해 나중에 사용)
                 * 2. 각 행당 평균값을 init_center에 넣어주기
                 *  - temp_y 를 만들어주고
                 *  - 각 군집에 들어간 수를 세어주고
                 *  - 각 군집의 합을 Re, Imag별로 나누어 준다.
                 *  - init_center에 Re, Imag 부분에 넣어준다.
                 * 3. 다시 비교
                 */

                // 각 사분면에 들어간 심볼의 갯수와 Re값의 합 Imag값의 합
                double count1 = 0, count2 = 0, count3 = 0, count4 = 0;
                double count1_re = 0, count1_imag = 0, count2_re = 0, count2_imag = 0, count3_re = 0, count3_imag = 0, count4_re = 0, count4_imag = 0;
                for (int k = 0; k < y.Length; k++)
                {
                    temp_Y[position[k], k] = y[k]; // temp_y를 만들어주고

                    // 각 군집에 들어간 수를 세어주고
                    if (position[k] == 0)
                    {
                        count1_re += y[k].Real;
                        count1_imag += y[k].Imaginary;
                        count1 += 1;
                    }
                    else if (position[k] == 1)
                    {
                        count2_re += y[k].Real;
                        count2_imag += y[k].Imaginary;
                        count2 += 1;
                    }
                    else if (position[k] == 2)
                    {
                        count3_re += y[k].Real;
                        count3_imag += y[k].Imaginary;
                        count3 += 1;
                    }
                    else if (position[k] == 3)
                    {
                        count4_re += y[k].Real;
                        count4_imag += y[k].Imaginary;
                        count4 += 1;
                    }
                    else
                    {
                        Console.WriteLine("position이 비어있습니다.");
                    }
                    // init_center에 평균값을 넣어준다.
                    if (count1 > 0)
                    {
                        init_center[0, 0] = count1_re / count1;
                        init_center[1, 0] = count1_imag / count1;
                    }
                    if (count2 > 0)
                    {
                        init_center[0, 1] = count2_re / count2;
                        init_center[1, 1] = count2_imag / count2;
                    }
                    if (count3 > 0)
                    {
                        init_center[0, 2] = count3_re / count3;
                        init_center[1, 2] = count3_imag / count3;
                    }
                    if (count4 > 0)
                    {
                        init_center[0, 3] = count4_re / count4;
                        init_center[1, 3] = count4_imag / count4;
                    }
                }
            }
        }
        // dist의 최솟값의 Index를 배열 형태로 반환하는 메서드
        public int[] Min_dist(double[,] dist, int length) // 거리 dist와 dist의 2번째 행열 길이를 받음
        {
            double temp = 0;
            int[] position = new int[length];
            for(int i = 0; i < length; i++)
            {
                // 최솟값을 찾는 알고리즘
                for (int j = 0; j < 4; j++)
                { 
                    if (j == 0)
                    {
                        temp = dist[j,i];
                    }
                    else if( temp > dist[j,i])
                    {
                        temp = dist[j, i];
                    }
                    // Console.WriteLine(dist[j, i]);
                }

                for(int k = 0; k < 4; k++)
                {
                    
                    if(dist[k, i] == temp)
                    {
                        position[i] = k;
                        // Console.WriteLine("여기가 temp값" + temp + "hi" + position[i]);
                    }
                }
            }
            return position;
        }
    }
}