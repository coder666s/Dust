using System;

namespace MNN
{
//  Структура 2 мерной точки
    public struct Point
    {
        public int x;
        public int y;
        public int type;
        public void Set(int X, int Y, int T)
        {
            x = X;
            y = Y;
            type = T;
        }
    }

//  Коллекция объектов
    public struct MyCollectionObject
    {
        public int index;
        public double dinstance;
    }

//  Класс 200 мерной точки
    public class Point200
    {
        public double[] axis = new double[200];
        public int type;
    }

    public struct answ
    {
        public int type;
        public double proc;
    }

    class Program
    {
//  Дистанция 2 мерная
        static double Distance(Point p1, Point p2)
        {
            return Math.Sqrt((p2.x - p1.x)* (p2.x - p1.x) + (p2.y - p1.y)*(p2.y - p1.y));
        }

//  Метод для 2 мерной плоскости
        static void Old()
        {
            Console.WriteLine("Hello World!");
            int N, k;
            N = 10;
            Point[] points = new Point[N];
            points[0].Set(1, 1, 1);
            points[1].Set(0, 5, 1);
            points[2].Set(1, 7, 1);
            points[3].Set(5, 1, 1);
            points[4].Set(3, 5, 2);
            points[5].Set(6, 7, 2);
            points[6].Set(8, 8, 2);
            points[7].Set(2, 2, 2);
            points[8].Set(7, 0, 2);
            points[9].Set(1, 3, 2);
            Point point = new Point();
            point.x = 3;
            point.y = 4;
            k = 4;
            if (k > N)
                return;
            MyCollectionObject[] mco = new MyCollectionObject[N];
            for (int i = 0; i < N; i++)
            {
                mco[i].index = i;
                mco[i].dinstance = Distance(points[i], point);
                Console.WriteLine(mco[i].index.ToString() + " " + mco[i].dinstance.ToString());
            }
            MyCollectionObject[] selected = new MyCollectionObject[N];

            double min = mco[0].dinstance;
            double minDinst = 0;
            //            int cnt = 0;
            for (int c = 1; c < k; c++)
            {
                min = mco[0].dinstance;
                for (int i = 0; i < N; i++)
                {
                    if (mco[i].dinstance < min && mco[i].dinstance > minDinst)
                    {
                        min = mco[i].dinstance;
                    }
                }
                minDinst = min;
            }
            MyCollectionObject my = new MyCollectionObject();
            for (int i = 0; i < N; i++)
            {
                if (mco[i].dinstance == min)
                {
                    my.index = mco[i].index;
                    my.dinstance = mco[i].dinstance;
                }
            }
            Console.WriteLine(my.dinstance.ToString());

            int cnt1 = 0;
            int cnt2 = 0;
            for (int i = 0; i < N; i++)
            {
                if (mco[i].dinstance <= my.dinstance)
                {
                    if (points[mco[i].index].type == 1)
                        cnt1++;
                    else
                        cnt2++;
                }
            }
            if (cnt1 >= cnt2)
                Console.WriteLine("1");
            else
                Console.WriteLine("2");
            Console.ReadKey();

        }

//  Дистанция 200 мерная
        static double Distance200(Point200 p1, Point200 p2)
        {
            double sigma = 0;
            for (int i = 0; i < 200; i++)
            {
                sigma += (p2.axis[i] - p1.axis[i]) * (p2.axis[i] - p1.axis[i]);
            }
            return Math.Sqrt(sigma);
        }

        //  Метод для 200 мерной плоскости
        static Point200[] New(int N, int k, Point200 point, Point200[] points)
        {
            /*            if (k > N)
                            return 0;*/
            MyCollectionObject[] mco = new MyCollectionObject[N];
            for (int i = 0; i < N; i++)
            {
                mco[i].index = i;
                mco[i].dinstance = Distance200(points[i], point);
            }
            MyCollectionObject[] selected = new MyCollectionObject[N];

            double min = mco[0].dinstance;
            double minDinst = 0;
            MyCollectionObject[] my1 = new MyCollectionObject[k];
            for (int c = 1; c < k; c++)
            {
                min = mco[0].dinstance;
                for (int i = 0; i < N; i++)
                {
                    if (mco[i].dinstance < min && mco[i].dinstance > minDinst)
                    {
                        min = mco[i].dinstance;
                        my1[c].index = mco[i].index;
                        my1[c].dinstance = mco[i].dinstance;
                    }
                }
                minDinst = min;
            }
            Point200[] ret = new Point200[k];  //!!!!
            for(int i = 0; i < k; i++)
            {
                ret[i].axis = points[my1[i].index].axis;
            }
            return ret;
        }

        static answ[] MP(int k, Point200[] points)
        {
            int[] cnt = new int[1];
            int[] num = new int[1];
            num[0] = points[0].type;
            cnt[0] = 1;
            for (int i = 0; i < k; i++)
            {
                bool find = false;
                for(int c = 0; c < cnt.Length; c++)
                {
                    if (points[i].type == num[c])
                    {
                        cnt[c]++;
                        find = true;
                    }
                }
                if (!find)
                {
                    Array.Resize(ref cnt, cnt.Length + 1);
                    cnt[cnt.Length - 1] = 1;
                    Array.Resize(ref num, num.Length + 1);
                    cnt[num.Length - 1] = points[i].type;
                }         
            }
            answ[] res = new answ[num.Length];
            for(int i = 0; i < res.Length; i++)
            {
                res[i].type = num[i];
                res[i].proc = cnt[i] / k * 100;
            }
            return res;
        }

        static void Main(string[] args)
        {

        }
    }
}
