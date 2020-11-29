using Hybridizer.Runtime.CUDAImports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using MathNet.Numerics.Distributions;
using System.Diagnostics;

namespace HybridizerSample1
{
    class Program
    {
        [EntryPoint]
        public static void Run(int N, int[] a, int[] b)
        {
            Parallel.For(0, N, i => { a[i] /= b[i]; });
        }

        static void Main(string[] args)
        {
            // 시간측정
            Stopwatch sw = new Stopwatch();

            /*==========CUDA==========*/
            cudaDeviceProp prop;
            cuda.GetDeviceProperties(out prop, 0);
            HybRunner runner = HybRunner.Cuda();
            
            dynamic wrapped = runner.Wrap(new Program());
            /*==========CUDA==========*/
            
            // 심볼 수
            int Symbol_num = 2304;
            // 반복 횟수
            int start_count = 20;
            // SNR 배열(탭수에 따라 0-20 또는 0-40)
            int SNR = 21;
            // 변수
            int QAM = 16;
            // 군집화 횟수
            int cluster = 20;
            
            int tab;
            
            // SER을 담는 변수
            double[] Ser = new double[SNR];
            // MSE을 담는 변수
            double[] MSE = new double[SNR]; 
            // 오류율
            double err = 0;

            // 시간측정 시작
            sw.Start();
            for (int start = 0; start < start_count; start++)
            {
                Complex[] ComArray_S = new Complex[Symbol_num];
                Complex[] ComArray_Y = new Complex[Symbol_num];

                Console.WriteLine("Total Count : " + start);

                // 심볼 S 객체 생성 ( 이때 채널이 생성됨 (곱해지지는 않음))
                gen_Symbol S = new gen_Symbol();

                // ComArray_S Complex 배열에 심볼 대입
                ComArray_S = S.Gen_S_Symbol(QAM, Symbol_num);

                // demode s symbol
                Demode nErr_s = new Demode(ComArray_S, QAM);

                // 채널 입력
                ComArray_Y = S.MultyCh(ComArray_S);

                for (int i = 0; i < SNR; i++) // SNR
                {
                    if (QAM == 4 || QAM == 16)
                        tab = i;
                    else
                        tab = i*2;
                    
                    Complex[] ComArray_R = new Complex[Symbol_num];
                    ComArray_R = S.AddNoise(ComArray_Y, Symbol_num, tab); // Y = Hx + n

                    Ch_Est est = new Ch_Est();
                    if (QAM==4)
                    {
                        QPSK_Kmeans y_kmeans = new QPSK_Kmeans();
                        y_kmeans.Kmeans_qpsk_ch(ComArray_R, cluster, S.Hk);
                        est.Est(y_kmeans.init_center, S.Hk);
                    }
                    else if (QAM == 16)
                    {
                        QAM16_Kmeans y_kmeans = new QAM16_Kmeans(ComArray_R, cluster, S.Hk);
                        est.Est(y_kmeans.hk_center, S.Hk); // 채널 추정 16-256
                    }
                    else if (QAM == 64)
                    {
                        QAM64_Kmeans y_kmeans = new QAM64_Kmeans(ComArray_R, cluster, S.Hk);
                        est.Est(y_kmeans.hk_center, S.Hk); // 채널 추정 16-256
                    }
                    else if (QAM == 256)
                    {
                        QAM256_Kmeans y_kmeans = new QAM256_Kmeans(ComArray_R, cluster, S.Hk);
                        est.Est(y_kmeans.hk_center, S.Hk); // 채널 추정 16-256
                    }
                    
                    ComArray_R = S.DividCh(ComArray_R, est.Est_ch); // 추정한 채널로 R 심볼을 나눈다.

                    Demode nErr_y = new Demode(ComArray_R, QAM);

                    err = 0;
                    // Demode 이용
                    for (int j = 0; j < ComArray_R.Length; j++)
                    {
                        if (!Complex.Equals(nErr_s.data_sum[j], nErr_y.data_sum[j]))
                        {
                            err++;
                        }
                    }
                    Ser[i] += err;
                    MSE[i] += est.MSE_result;
                }
            }
            // 시간측정 종료
            sw.Stop();

            // SER, MSE 출력
            for (int q = 0; q < SNR; q++)
            {
                Ser[q] = Ser[q] / (Symbol_num * start_count);
                MSE[q] = MSE[q] / start_count;
                Console.WriteLine(q * 2);
                Console.WriteLine("SER : " + Ser[q]);
                Console.WriteLine("MSE : " + MSE[q]);
            }
            Console.WriteLine("symbol : " + Symbol_num);

            //측정 시간 출력
            Console.WriteLine("CUDA {0} {1} ms", Symbol_num, sw.ElapsedMilliseconds / start_count / 2);
        }
    }
}