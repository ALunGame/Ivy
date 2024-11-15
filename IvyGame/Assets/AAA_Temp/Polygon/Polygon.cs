using System;

public class Point
{
    public double x, y;

    public Point(double x = 0, double y = 0)
    {
        this.x = x;
        this.y = y;
    }

    // 向量加法
    public static Point operator +(Point a, Point b)
    {
        return new Point(a.x + b.x, a.y + b.y);
    }

    // 向量减法
    public static Point operator -(Point a, Point b)
    {
        return new Point(a.x - b.x, a.y - b.y);
    }

    // 点积
    public double Dot(Point b)
    {
        return x * b.x + y * b.y;
    }

    // 叉积
    public double Cross(Point b)
    {
        return x * b.y - y * b.x;
    }
}

public class Polygon
{
    private const double EPS = 1e-6;
    public Point[] points;
    public int n;

    public Polygon(int n)
    {
        this.n = n;
        points = new Point[n + 1];
    }

    // 三态函数，判断两个 double 在 EPS 精度下的大小关系
    private int Dcmp(double x)
    {
        if (Math.Abs(x) < EPS) return 0;
        return x < 0 ? -1 : 1;
    }

    // 判断点 Q 是否在 P1 和 P2 的线段上
    private bool OnSegment(Point P1, Point P2, Point Q)
    {
        // 前一个判断点 Q 在 P1P2 直线上，后一个判断在 P1P2 范围上
        return Dcmp((P1 - Q).Cross(P2 - Q)) == 0 && Dcmp((P1 - Q).Dot(P2 - Q)) <= 0;
    }

    // 判断点 P 在多边形内 - 射线法
    public bool InPolygon(Point P)
    {
        bool flag = false; // 相当于计数
        Point P1, P2; // 多边形一条边的两个顶点
        for (int i = 0, j = n - 1; i < n; j = i++)
        {
            // points[] 是给出多边形的顶点
            P1 = points[i];
            P2 = points[j];
            // 点在多边形一条边上
            if (OnSegment(P1, P2, P)) 
                return true; 
            // 前一个判断 min(P1.y, P2.y) < P.y <= max(P1.y, P2.y)
            // 这个判断代码很精妙，是网上找的大师模板
            // 后一个判断被测点在射线与边交点的左边
            if ((Dcmp(P1.y - P.y) > 0 != Dcmp(P2.y - P.y) > 0) &&
                Dcmp(P.x - (P.y - P1.y) * (P1.x - P2.x) / (P1.y - P2.y) - P1.x) < 0)
                flag = !flag;
        }
        return flag;
    }
}

//public class Program
//{
//    public static void Main()
//    {
//        string input;
//        while ((input = Console.ReadLine()) != null)
//        {
//            int n = int.Parse(input);
//            Polygon polygon = new Polygon(n);

//            for (int i = 1; i <= n; i++)
//            {
//                string[] coords = Console.ReadLine().Split();
//                double x = double.Parse(coords[0]);
//                double y = double.Parse(coords[1]);
//                polygon.points[i] = new Point(x, y);
//            }

//            int m = int.Parse(Console.ReadLine());

//            for (int i = 0; i < m; i++)
//            {
//                string[] testCoords = Console.ReadLine().Split();
//                double x = double.Parse(testCoords[0]);
//                double y = double.Parse(testCoords[1]);
//                Point test = new Point(x, y);

//                Console.WriteLine(polygon.InPolygon(test) ? "Yes" : "No");
//            }
//        }
//    }
//}

