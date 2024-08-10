using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;


namespace ConsoleApp1
{
    public static class Program
    {
        static Mutex mtx = new Mutex();
        public static int[] MultiplyPolynomialsKaratsuba(int[] poly1, int[] poly2)
        {
            int n = Math.Max(poly1.Length, poly2.Length);
            int middle = n / 2;
            if (poly1.Length < 2 || poly2.Length < 2)
            {
                int m = poly1.Length;
                int k = poly2.Length;
                int[] bc = new int[m + k - 1];
                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < k; j++)
                    {
                        bc[i + j] += poly1[i] * poly2[j];
                    }
                }

                return bc;
            }


            int[] Poly1Part1 = poly1[middle..]; //high Part
            int[] Poly1Part2 = poly1[..middle]; //low Part



            int[] Poly2Part1 = poly2[middle..];
            int[] Poly2Part2 = poly2[..middle];


            int[] Z0 = MultiplyPolynomialsKaratsuba(Poly1Part2, Poly2Part2);
            int[] Z2 = MultiplyPolynomialsKaratsuba(Poly1Part1, Poly2Part1);

            int[] e1 = new int[Math.Max(Poly1Part1.Length, Poly1Part2.Length)];
            for (int i = 0; i < Math.Max(Poly1Part1.Length, Poly1Part2.Length); i++)
            {
                int value1 = (i < Poly1Part1.Length) ? Poly1Part1[i] : 0;
                int value2 = (i < Poly1Part2.Length) ? Poly1Part2[i] : 0;

                e1[i] = value1 + value2;
            }

            int[] e2 = new int[Math.Max(Poly2Part1.Length, Poly2Part2.Length)];
            for (int j = 0; j < Math.Max(Poly2Part1.Length, Poly2Part2.Length); j++)
            {
                int value1 = (j < Poly2Part1.Length) ? Poly2Part1[j] : 0;
                int value2 = (j < Poly2Part2.Length) ? Poly2Part2[j] : 0;

                e2[j] = value1 + value2;
            }


            int[] Z1 = MultiplyPolynomialsKaratsuba(e1, e2);

            int[] partial_rez = new int[Math.Max(Z1.Length, Z0.Length)];
            for (int i = 0; i < Math.Max(Z1.Length, Z0.Length); i++)
            {
                int value1 = (i < Z1.Length) ? Z1[i] : 0;
                int value2 = (i < Z0.Length) ? Z0[i] : 0;

                partial_rez[i] = value1 - value2;
            }

            int[] rez = new int[Math.Max(partial_rez.Length, Z2.Length)];
            for (int i = 0; i < Math.Max(partial_rez.Length, Z2.Length); i++)
            {
                int value1 = (i < partial_rez.Length) ? partial_rez[i] : 0;
                int value2 = (i < Z2.Length) ? Z2[i] : 0;

                rez[i] = value1 - value2;
            }



            int[] result = new int[2 * n - 1];

            for (int i = 0; i < Z0.Length; i++)
            {
                result[i] += Z0[i];
            }
            for (int i = 0; i < rez.Length; i++)
            {
                result[i + middle] += rez[i];
            }
            for (int i = 0; i < Z2.Length; i++)
            {
                result[i + 2 * middle] += Z2[i];
            }

            return result;
        }

        public static int[] MultiplyPolynomialsKaratsubaParallel(int[] poly1, int[] poly2, int depth,int t)
        {
            int n = Math.Max(poly1.Length, poly2.Length);
            int middle = n / 2;
            if (poly1.Length < 2 || poly2.Length < 2)
            {
                int m = poly1.Length;
                int k = poly2.Length;
                int[] bc = new int[m + k - 1];
                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < k; j++)
                    {
                        bc[i + j] += poly1[i] * poly2[j];
                    }
                }

                return bc;
            }

            int[] Poly1Part1 = poly1[middle..]; //high Part
            int[] Poly1Part2 = poly1[..middle]; //low Part

            int[] Poly2Part1 = poly2[middle..];
            int[] Poly2Part2 = poly2[..middle];

            int[] Z0=new int[0];
            int[] Z1=new int[0];
            int[] Z2=new int[0];
            int[] rez= new int[0];


            if (depth == t)
            {
                Task taskZ0 = Task.Run(() => Z0 = MultiplyPolynomialsKaratsubaParallel(Poly1Part2, Poly2Part2, depth + 1, t));
                Task taskZ2 = Task.Run(() => Z2 = MultiplyPolynomialsKaratsubaParallel(Poly1Part1, Poly2Part1, depth + 1, t));

                int[] e1 = new int[Math.Max(Poly1Part1.Length, Poly1Part2.Length)];
                for (int i = 0; i < Math.Max(Poly1Part1.Length, Poly1Part2.Length); i++)
                {
                   int value1 = (i < Poly1Part1.Length) ? Poly1Part1[i] : 0;
                   int value2 = (i < Poly1Part2.Length) ? Poly1Part2[i] : 0;

                   e1[i] = value1 + value2;
                }

                int[] e2 = new int[Math.Max(Poly2Part1.Length, Poly2Part2.Length)];
                for (int j = 0; j < Math.Max(Poly2Part1.Length, Poly2Part2.Length); j++)
                {
                   int value1 = (j < Poly2Part1.Length) ? Poly2Part1[j] : 0;
                   int value2 = (j < Poly2Part2.Length) ? Poly2Part2[j] : 0;

                   e2[j] = value1 + value2;
                }
                Z1=MultiplyPolynomialsKaratsubaParallel(e1, e2, depth + 1, t);

                Task.WaitAll(taskZ0, taskZ2);

                int[] partial_rez = new int[Math.Max(Z1.Length, Z0.Length)];
                for (int i = 0; i < Math.Max(Z1.Length, Z0.Length); i++)
                {
                    int value1 = (i < Z1.Length) ? Z1[i] : 0;
                    int value2 = (i < Z0.Length) ? Z0[i] : 0;

                    partial_rez[i] = value1 - value2;
                }

                int[] Z1_final = new int[Math.Max(partial_rez.Length, Z2.Length)];
                for (int i = 0; i < Math.Max(partial_rez.Length, Z2.Length); i++)
                {
                    int value1 = (i < partial_rez.Length) ? partial_rez[i] : 0;
                    int value2 = (i < Z2.Length) ? Z2[i] : 0;

                    Z1_final[i] = value1 - value2;
                }

 
               
                


                int[] result = new int[2 * n - 1];

                for (int i = 0; i < Z0.Length; i++)
                {
                    result[i] += Z0[i];
                }
                for (int i = 0; i < Z1_final.Length; i++)
                {
                    result[i + middle] += Z1_final[i];
                }
                for (int i = 0; i < Z2.Length; i++)
                {
                    result[i + 2 * middle] += Z2[i];
                }

                return result;
            }
            else
            {
                Z0 = MultiplyPolynomialsKaratsubaParallel(Poly1Part2, Poly2Part2, depth + 1, t);
                Z2 = MultiplyPolynomialsKaratsubaParallel(Poly1Part1, Poly2Part1, depth + 1, t);

                int[] e1 = new int[Math.Max(Poly1Part1.Length, Poly1Part2.Length)];
                for (int i = 0; i < Math.Max(Poly1Part1.Length, Poly1Part2.Length); i++)
                {
                    int value1 = (i < Poly1Part1.Length) ? Poly1Part1[i] : 0;
                    int value2 = (i < Poly1Part2.Length) ? Poly1Part2[i] : 0;

                    e1[i] = value1 + value2;
                }

                int[] e2 = new int[Math.Max(Poly2Part1.Length, Poly2Part2.Length)];
                for (int j = 0; j < Math.Max(Poly2Part1.Length, Poly2Part2.Length); j++)
                {
                    int value1 = (j < Poly2Part1.Length) ? Poly2Part1[j] : 0;
                    int value2 = (j < Poly2Part2.Length) ? Poly2Part2[j] : 0;

                    e2[j] = value1 + value2;
                }


                Z1 = MultiplyPolynomialsKaratsubaParallel(e1, e2, depth + 1, t);

                int[] partial_rez = new int[Math.Max(Z1.Length, Z0.Length)];
                for (int i = 0; i < Math.Max(Z1.Length, Z0.Length); i++)
                {
                    int value1 = (i < Z1.Length) ? Z1[i] : 0;
                    int value2 = (i < Z0.Length) ? Z0[i] : 0;

                    partial_rez[i] = value1 - value2;
                }

                int[] Z1_rez = new int[Math.Max(partial_rez.Length, Z2.Length)];
                for (int i = 0; i < Math.Max(partial_rez.Length, Z2.Length); i++)
                {
                    int value1 = (i < partial_rez.Length) ? partial_rez[i] : 0;
                    int value2 = (i < Z2.Length) ? Z2[i] : 0;

                    Z1_rez[i] = value1 - value2;
                }

                int[] result = new int[2 * n - 1];

                for (int i = 0; i < Z0.Length; i++)
                {
                    result[i] += Z0[i];
                }
                for (int i = 0; i < Z1_rez.Length; i++)
                {
                    result[i + middle] += Z1_rez[i];
                }
                for (int i = 0; i < Z2.Length; i++)
                {
                    result[i + 2 * middle] += Z2[i];
                }

                return result;
            }
           
        }

        public static int[] MultiplyPolynomials(int[] poly1, int[] poly2)
        {
            int m = poly1.Length;
            int n = poly2.Length;
            int[] result = new int[m + n - 1];

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    result[i + j] += poly1[i] * poly2[j];
                }
            }

            return result;
        }

        public static int[] MultiplyPolynomialsParallel(int[] poly1, int[] poly2)
        {
            int m = poly1.Length;
            int n = poly2.Length;
            int[] result = new int[m + n - 1];

            List<Task> tasks = new List<Task>();

            for (int i = 0; i < m; i++)
            {
                int start = i;
                tasks.Add(Task.Run(() =>
                {
                    for (int j = 0; j < n; j++)
                    {
                        Interlocked.Add(ref result[start + j], poly1[start] * poly2[j]);
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            return result;
        }


        /*public static void pararel_task(int[] poly1, int[] poly2, int[] rez, int idxThread, int nrThreads, int size)
        {
            int n = poly1.Length;
            int m = poly2.Length;

            int beginIdx = (idxThread * rez.Length) / nrThreads;
            int endIdx = ((idxThread + 1) * rez.Length) / nrThreads;

            for (int i = beginIdx; i < endIdx; i++)
            {
                int coefficient = 0;
                for (int j = 0; j <= i; j++)
                {
                    if (j < n && (i - j) < m)
                    {

                        coefficient += poly1[j] * poly2[i - j];
                    }
                }
                Interlocked.Add(ref rez[i], coefficient);
            }
        }*/



        public static void Main()
        {

            int n = 10000;
            int m = 10000;
            int[] poly1 = new int[n];
            int[] poly2 = new int[m];
            //int[] result = new int[m + n - 1];
            for (int i = 0; i < m; i++)
            {
                poly1[i] = 1;
            }

            for (int i = 0; i < m; i++)
            {
                poly2[i] = 1;
            }

            Stopwatch watch = new Stopwatch();
            watch.Start();

            int[] result = MultiplyPolynomials(poly1, poly2);

            watch.Stop();
            long elapsedTicks = watch.ElapsedTicks;
            double mircosec = (double)elapsedTicks / Stopwatch.Frequency * 1000000;

            //foreach (int i in result)
            //{
            //    Console.Write(i + " ");
            //}

            Console.WriteLine();
            Console.WriteLine($"Execution Time For Regular No Paralelism: {mircosec} mircroseconds");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            Stopwatch watch2 = new Stopwatch();
            watch2.Start();

            int[] result2 = MultiplyPolynomialsKaratsuba(poly1, poly2);

            watch2.Stop();
            long elapsedTicks2 = watch2.ElapsedTicks;
            double mircosec2 = (double)elapsedTicks2 / Stopwatch.Frequency * 1000000;

            //foreach (int i in result2)
            //{
            //    Console.Write(i + " ");
            //}

            Console.WriteLine();
            Console.WriteLine($"Execution Time For Karastuba No Paralelism: {mircosec2} mircroseconds");

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            Stopwatch watch3 = new Stopwatch();
            watch3.Start();

            int[] rezult3 = MultiplyPolynomialsParallel(poly1,poly2);

            watch3.Stop();
            long elapsedTicks3 = watch3.ElapsedTicks;
            double mircosec3 = (double)elapsedTicks3 / Stopwatch.Frequency * 1000000;

            //for (int i = 0; i < rezult3.Length; i++)
            //{
            //    Console.Write(rezult3[i] + " ");
            //}
            Console.WriteLine();

            Console.WriteLine($"Execution Time For Regula Paralalized : {mircosec3} mircroseconds");

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            //
            Console.WriteLine($"Enter nr of threads: ");
            var num = int.Parse(Console.ReadLine());
            double dresult = Math.Log(num, 3);
            int intResult = (int)dresult;

            Stopwatch watch4 = new Stopwatch();
            watch4.Start();

            int[] rezult4 = MultiplyPolynomialsKaratsubaParallel(poly1, poly2,0, intResult);

            watch4.Stop();
            long elapsedTicks4 = watch4.ElapsedTicks;
            double mircosec4 = (double)elapsedTicks4 / Stopwatch.Frequency * 1000000;

            //for (int i = 0; i < rezult4.Length; i++)
            //{
            //    Console.Write(rezult4[i] + " ");
            //}
            Console.WriteLine();

            Console.WriteLine($"Execution Time For Karatsuba Paralalized: {mircosec4} mircroseconds");

            Console.ReadLine();
        }


    }
}