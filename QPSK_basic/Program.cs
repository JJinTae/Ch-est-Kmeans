using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Inventors.IO.Matlab;
using System.Numerics;
using MathNet.Numerics.Distributions;
using Excel = Microsoft.Office.Interop.Excel; // 엑셀시트 사용


namespace QPSK_basic
{
    class Program
    {
        static Random r = new Random();
        static void Main(string[] args)
        {
            // Mat 파일 생성
            MatlabFile file = new MatlabFile("symbol_ser_mse.mat", true);

            // S심볼 만들기
            int Symbol_num = 36864;
            int start_count = 20;
            int SNR = 21;
            int QAM = 16;

            double num = Convert.ToDouble(Symbol_num);
            double[] Ser = new double[SNR]; // SER을 담는 변수
            double[] MSE = new double[SNR]; // MSE을 담는 변수
            double err = 0;

            for (int start = 0; start < start_count; start++)
            {
                Complex[] ComArray_S = new Complex[Symbol_num];
                Complex[] ComArray_Y = new Complex[Symbol_num];
                
                Console.WriteLine("Total Count : " + start);

                // 심볼 S 객체 생성 ( 이때 채널이 생성됨 (곱해지지는 않음))
                gen_Symbol S = new gen_Symbol();

                // ComArray_S Complex 배열에 심볼 대입
                ComArray_S = S.Gen_S_Symbol(QAM, Symbol_num);


                /*
                //-----------------s 심볼 테스트----------------------//
                double[] Sreal = new double[Symbol_num];
                double[] Simag = new double[Symbol_num];
                ComplexToDouble(ComArray_S, ref Sreal, ref Simag);
                Matrix symbol_s = new Matrix("symbol_s", Sreal, Simag);
                file.Write(symbol_s);
                //---------------------------------------//
                */
                
                // demode s symbol
                Demode nErr_s = new Demode(ComArray_S, QAM);

                // 채널 입력
                ComArray_Y = S.MultyCh(ComArray_S);
                
                for (int i = 0; i < SNR; i++) // SNR
                {
                    Complex[] ComArray_R = new Complex[Symbol_num];
                    ComArray_R = S.AddNoise(ComArray_Y, Symbol_num, i); // Y = Hx + n

                    /*
                    // y symbol + noise * hk
                    double[] Yreal = new double[Symbol_num];
                    double[] Yimag = new double[Symbol_num];
                    ComplexToDouble(ComArray_R, ref Yreal, ref Yimag);
                    Matrix symbol_y = new Matrix("symbol_y", Yreal, Yimag);
                    file.Write(symbol_y);
                    // symbol test end
                    */

                    // 채널 추정 객체 생성
                    Ch_Est est = new Ch_Est();
                    // 군집화
                    if (QAM == 4)
                    {
                        QPSK_Kmeans y_kmeans = new QPSK_Kmeans(ComArray_R, 20, S.Hk);
                        est.Est(y_kmeans.init_center, S.Hk); // 채널 추정
                    }
                    else if(QAM == 16)
                    {
                        QAM16_Kmeans y_kmeans = new QAM16_Kmeans(ComArray_R, 20, S.Hk);
                        est.Est(y_kmeans.hk_center, S.Hk); // 채널 추정
                    }
                    else if(QAM == 64)
                    {
                        QAM64_Kmeans y_kmeans = new QAM64_Kmeans(ComArray_R, 20, S.Hk);
                        est.Est(y_kmeans.hk_center, S.Hk); // 채널 추정
                    }
                    else
                    {
                        QAM256_Kmeans y_kmeans = new QAM256_Kmeans(ComArray_R, 20, S.Hk);
                        est.Est(y_kmeans.hk_center, S.Hk); // 채널 추정
                    }

                    ComArray_R = S.DividCh(ComArray_R, est.Est_ch); // 추정한 채널로 R 심볼을 나눈다.
                    // Console.WriteLine(i+ " : "  + S.Hk + " : " + est.Est_ch);

                    /*
                    // kmeans 후 센터값
                    double[] rcenter = new double[QAM];
                    double[] icenter = new double[QAM];
                    ComplexToDouble(y_kmeans.Com_init_center, ref rcenter, ref icenter);
                    Matrix center = new Matrix("kmeans_center", rcenter, icenter);
                    file.Write(center);

                    // symbol r = y / hk
                    double[] Rreal = new double[Symbol_num];
                    double[] Rimag = new double[Symbol_num];
                    ComplexToDouble(ComArray_R, ref Rreal, ref Rimag);
                    Matrix symbol_r = new Matrix("symbol_r", Rreal, Rimag);
                    file.Write(symbol_r);
                    // symbol test end                    
                    */
                    

                    Demode nErr_y = new Demode(ComArray_R, QAM);
                    // Console.WriteLine("SNR : " + i);
                    // Console.WriteLine("MSE : " + est.MSE_result);
                    
                    err = 0;
                    // Demode 이용
                    for (int j = 0; j < ComArray_R.Length; j++)
                    {
                        if(!Complex.Equals(nErr_s.data_sum[j], nErr_y.data_sum[j]))
                        {
                            err++;
                        }
                    }
                    Ser[i] += err;
                    MSE[i] += est.MSE_result;
                }
            }

            for (int q = 0; q < SNR; q++)
            {
                Ser[q] = Ser[q] / (Symbol_num * start_count);
                MSE[q] = MSE[q] / start_count;
                Console.WriteLine(q);
                Console.WriteLine("SER : " + Ser[q]);
                Console.WriteLine("MSE : " + MSE[q]);
            }

            // ser, mse 생성
            Matrix ser = new Matrix("ser", Ser);
            Matrix mse = new Matrix("mse", MSE);
            file.Write(ser);
            file.Write(mse);

            DisplayInExcel(Ser, MSE);
            Console.ReadKey();
        }

        
        static void DisplayInExcel(double[] ser, double[] mse)
        {
            var excelApp = new Excel.Application();
            // Make the object visible.
            excelApp.Visible = true;

            // Create a new, empty workbook and add it to the collection returned 
            // by property Workbooks. The new workbook becomes the active workbook.
            // Add has an optional parameter for specifying a praticular template. 
            // Because no argument is sent in this example, Add creates a new workbook. 
            excelApp.Workbooks.Add();

            // This example uses a single workSheet. The explicit type casting is
            // removed in a later procedure.
            Excel._Worksheet workSheet = (Excel.Worksheet)excelApp.ActiveSheet;

            workSheet.Cells[1, "A"] = "ser";
            workSheet.Cells[1, "B"] = "mse";

            var row = 1;
            foreach (double acct in ser)
            {
                row++;
                workSheet.Cells[row, "A"] = acct;
                // workSheet.Cells[row, "B"] = acct.Imaginary;
            }
            row = 1;
            foreach (double acct in mse)
            {
                row++;
                workSheet.Cells[row, "B"] = acct;
                // workSheet.Cells[row, "B"] = acct.Imaginary;
            }

            // 열 너비를 콘텐츠에 맞게 조정
            workSheet.Columns[1].AutoFit();
            workSheet.Columns[2].AutoFit();
        }
        /*
        static void DisplayInExcel(Complex[] ser)
        {
            var excelApp = new Excel.Application();
            // Make the object visible.
            excelApp.Visible = true;

            // Create a new, empty workbook and add it to the collection returned 
            // by property Workbooks. The new workbook becomes the active workbook.
            // Add has an optional parameter for specifying a praticular template. 
            // Because no argument is sent in this example, Add creates a new workbook. 
            excelApp.Workbooks.Add();

            // This example uses a single workSheet. The explicit type casting is
            // removed in a later procedure.
            Excel._Worksheet workSheet = (Excel.Worksheet)excelApp.ActiveSheet;

            workSheet.Cells[1, "A"] = "ser";
            workSheet.Cells[1, "B"] = "mse";

            var row = 1;
            foreach (Complex acct in ser)
            {
                row++;
                workSheet.Cells[row, "A"] = acct.Real;
                workSheet.Cells[row, "B"] = acct.Imaginary;
            }

            // 열 너비를 콘텐츠에 맞게 조정
            workSheet.Columns[1].AutoFit();
            workSheet.Columns[2].AutoFit();
        }
        */


        // 
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
