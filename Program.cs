using System;
using System.Collections.Generic;
using System.Text;

namespace Geometry
{
    /// <summary>
    /// Базовий абстрактний клас для представлення трикутника
    /// </summary>
    public abstract class Triangle
    {
        protected const double Epsilon = 1e-10;

        public double SideA { get; protected set; }
        public double SideB { get; protected set; }
        public double SideC { get; protected set; }

        public double AngleAlpha { get; protected set; }
        public double AngleBeta { get; protected set; }
        public double AngleGamma { get; protected set; }

        protected Triangle() { }

        /// <summary>
        /// Фабричний метод для створення трикутника за трьома сторонами
        /// </summary>
        public static Triangle FromThreeSides(double a, double b, double c)
        {
            if (a <= Epsilon || b <= Epsilon || c <= Epsilon)
                throw new ArgumentOutOfRangeException("Сторони мають бути додатними");

            if (a + b <= c + Epsilon || a + c <= b + Epsilon || b + c <= a + Epsilon)
                throw new ArgumentException("Не виконується нерівність трикутника");

            bool abEqual = Math.Abs(a - b) < Epsilon;
            bool bcEqual = Math.Abs(b - c) < Epsilon;
            bool acEqual = Math.Abs(a - c) < Epsilon;

            if (abEqual && bcEqual)
                return new EquilateralTriangle(a);
            else if (abEqual || bcEqual || acEqual)
            {
                // Визначаємо, яка сторона буде основою
                double baseLength = (!abEqual) ? c : (!acEqual ? b : a);
                double leg = abEqual ? a : (acEqual ? a : b);
                return new IsoscelesTriangle(baseLength, leg);
            }
            else
                return new ScaleneTriangle(a, b, c);
        }

        /// <summary>
        /// Обчислює кути за теоремою косинусів
        /// </summary>
        protected virtual void CalculateAngles()
        {
            double denomA = 2 * SideB * SideC;
            double denomB = 2 * SideA * SideC;
            double denomC = 2 * SideA * SideB;

            double cosA = (SideB * SideB + SideC * SideC - SideA * SideA) / denomA;
            double cosB = (SideA * SideA + SideC * SideC - SideB * SideB) / denomB;
            double cosC = (SideA * SideA + SideB * SideB - SideC * SideC) / denomC;

            AngleAlpha = Math.Acos(Math.Clamp(cosA, -1, 1)) * 180 / Math.PI;
            AngleBeta = Math.Acos(Math.Clamp(cosB, -1, 1)) * 180 / Math.PI;
            AngleGamma = Math.Acos(Math.Clamp(cosC, -1, 1)) * 180 / Math.PI;
        }

        public virtual double GetPerimeter() => SideA + SideB + SideC;

        public virtual void Print() => Console.WriteLine(ToString());

        public override string ToString()
        {
            return $"Трикутник: сторони ({SideA:F2}, {SideB:F2}, {SideC:F2}), " +
                   $"кути ({AngleAlpha:F2}°, {AngleBeta:F2}°, {AngleGamma:F2}°), " +
                   $"периметр ({GetPerimeter():F2})";
        }
    }

    public class RightTriangle : Triangle
    {
        public RightTriangle(double leg1, double leg2)
        {
            if (leg1 <= Epsilon)
                throw new ArgumentOutOfRangeException(nameof(leg1), "Катет має бути додатним");
            if (leg2 <= Epsilon)
                throw new ArgumentOutOfRangeException(nameof(leg2), "Катет має бути додатним");

            SideA = leg1;
            SideB = leg2;
            CalculateHypotenuse();
            CalculateAngles();
        }

        protected void CalculateHypotenuse()
        {
            SideC = Math.Sqrt(SideA * SideA + SideB * SideB);
        }

        public override string ToString()
        {
            return $"Прямокутний трикутник: катети ({SideA:F2}, {SideB:F2}), " +
                   $"гіпотенуза ({SideC:F2}), кути ({AngleAlpha:F2}°, {AngleBeta:F2}°, {AngleGamma:F2}°), " +
                   $"периметр ({GetPerimeter():F2})";
        }
    }

    public class IsoscelesTriangle : Triangle
    {
        public IsoscelesTriangle(double baseLength, double leg)
        {
            if (baseLength <= Epsilon)
                throw new ArgumentOutOfRangeException(nameof(baseLength), "Основа має бути додатною");
            if (leg <= Epsilon)
                throw new ArgumentOutOfRangeException(nameof(leg), "Бічна сторона має бути додатною");
            if (2 * leg <= baseLength + Epsilon)
                throw new ArgumentException("Бічна сторона занадто мала для побудови трикутника");

            SideA = baseLength;
            SideB = leg;
            SideC = leg;
            CalculateAngles();
        }

        public bool IsApexAngleObtuse() => AngleAlpha > 90;

        public double GetHeightToBase()
        {
            double halfBase = SideA / 2;
            double underSqrt = SideB * SideB - halfBase * halfBase;
            if (underSqrt < -Epsilon)
                throw new InvalidOperationException("Неможливо обчислити висоту: некоректна геометрія");
            return Math.Sqrt(Math.Max(0, underSqrt));
        }

        public override string ToString()
        {
            return $"Рівнобедрений трикутник: основа ({SideA:F2}), бічні сторони ({SideB:F2}), " +
                   $"кути ({AngleAlpha:F2}°, {AngleBeta:F2}°, {AngleGamma:F2}°), " +
                   $"висота ({GetHeightToBase():F2}), периметр ({GetPerimeter():F2})";
        }
    }

    public class EquilateralTriangle : Triangle
    {
        public EquilateralTriangle(double side)
        {
            if (side <= Epsilon)
                throw new ArgumentOutOfRangeException(nameof(side), "Сторона має бути додатною");

            SideA = SideB = SideC = side;
            CalculateAngles();
        }

        protected override void CalculateAngles()
        {
            AngleAlpha = AngleBeta = AngleGamma = 60.0;
        }

        public double GetArea() => (Math.Sqrt(3) / 4) * SideA * SideA;
        public double GetHeight() => (Math.Sqrt(3) / 2) * SideA;

        public override string ToString()
        {
            return $"Рівносторонній трикутник: сторона ({SideA:F2}), всі кути ({AngleAlpha:F2}°), " +
                   $"площа ({GetArea():F2}), висота ({GetHeight():F2}), периметр ({GetPerimeter():F2})";
        }
    }

    public class ScaleneTriangle : Triangle
    {
        public ScaleneTriangle(double a, double b, double c)
        {
            if (a <= Epsilon || b <= Epsilon || c <= Epsilon)
                throw new ArgumentOutOfRangeException("Сторони мають бути додатними");
            if (a + b <= c + Epsilon || a + c <= b + Epsilon || b + c <= a + Epsilon)
                throw new ArgumentException("Не виконується нерівність трикутника");

            SideA = a;
            SideB = b;
            SideC = c;
            CalculateAngles();
        }

        public bool IsAcute() => AngleAlpha < 90 && AngleBeta < 90 && AngleGamma < 90;

        public override string ToString()
        {
            return $"Різносторонній трикутник: сторони ({SideA:F2}, {SideB:F2}, {SideC:F2}), " +
                   $"кути ({AngleAlpha:F2}°, {AngleBeta:F2}°, {AngleGamma:F2}°), " +
                   $"периметр ({GetPerimeter():F2}), гострокутний: {IsAcute()}";
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            var triangles = new List<Triangle>
            {
                new RightTriangle(3, 4),
                Triangle.FromThreeSides(5, 5, 5),
                Triangle.FromThreeSides(3, 4, 5),
                new IsoscelesTriangle(6, 5),
                new EquilateralTriangle(7),
                new ScaleneTriangle(6, 7, 8)
            };

            Console.WriteLine("=== Демонстрація поліморфізму ===");
            foreach (var t in triangles)
                t.Print();
        }
    }
}
