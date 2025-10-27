using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geometry
{
    /// <summary>
    /// Базовий абстрактний клас для представлення трикутника
    /// </summary>
    public abstract class Triangle
    {
        protected const double Epsilon = 1e-10;

        /// <summary>
        /// Сторона A трикутника
        /// </summary>
        public double SideA { get; protected set; }

        /// <summary>
        /// Сторона B трикутника
        /// </summary>
        public double SideB { get; protected set; }

        /// <summary>
        /// Сторона C трикутника
        /// </summary>
        public double SideC { get; protected set; }

        /// <summary>
        /// Кут Alpha (протилежний стороні A) у градусах
        /// </summary>
        public double AngleAlpha { get; protected set; }

        /// <summary>
        /// Кут Beta (протилежний стороні B) у градусах
        /// </summary>
        public double AngleBeta { get; protected set; }

        /// <summary>
        /// Кут Gamma (протилежний стороні C) у градусах
        /// </summary>
        public double AngleGamma { get; protected set; }

        protected Triangle() { }

        /// <summary>
        /// Фабричний метод для створення трикутника за трьома сторонами
        /// </summary>
        /// <param name="a">Перша сторона</param>
        /// <param name="b">Друга сторона</param>
        /// <param name="c">Третя сторона</param>
        /// <returns>Конкретний тип трикутника залежно від співвідношення сторін</returns>
        public static Triangle FromThreeSides(double a, double b, double c)
        {
            // Перевірка на додатність
            if (a <= Epsilon)
                throw new ArgumentOutOfRangeException(nameof(a), a, "Сторона a має бути додатною");
            if (b <= Epsilon)
                throw new ArgumentOutOfRangeException(nameof(b), b, "Сторона b має бути додатною");
            if (c <= Epsilon)
                throw new ArgumentOutOfRangeException(nameof(c), c, "Сторона c має бути додатною");

            // Перевірка нерівності трикутника
            if (a + b <= c + Epsilon)
                throw new ArgumentException($"Не виконується нерівність трикутника: a({a}) + b({b}) <= c({c})");
            if (a + c <= b + Epsilon)
                throw new ArgumentException($"Не виконується нерівність трикутника: a({a}) + c({c}) <= b({b})");
            if (b + c <= a + Epsilon)
                throw new ArgumentException($"Не виконується нерівність трикутника: b({b}) + c({c}) <= a({a})");

            // Визначення типу трикутника
            bool abEqual = Math.Abs(a - b) < Epsilon;
            bool bcEqual = Math.Abs(b - c) < Epsilon;
            bool acEqual = Math.Abs(a - c) < Epsilon;

            // Рівносторонній: всі три сторони рівні
            if (abEqual && bcEqual)
            {
                return new EquilateralTriangle(a);
            }

            // Рівнобедрений: дві сторони рівні
            if (abEqual)
            {
                // a == b, c - основа, a і b - бічні сторони
                return new IsoscelesTriangle(c, a);
            }
            if (acEqual)
            {
                // a == c, b - основа, a і c - бічні сторони
                return new IsoscelesTriangle(b, a);
            }
            if (bcEqual)
            {
                // b == c, a - основа, b і c - бічні сторони
                return new IsoscelesTriangle(a, b);
            }

            // Різносторонній
            return new ScaleneTriangle(a, b, c);
        }

        /// <summary>
        /// Обчислює кути трикутника за теоремою косинусів
        /// </summary>
        protected virtual void CalculateAngles()
        {
            // Перевірка на нульові знаменники (не повинно статися при валідних сторонах)
            double denomA = 2 * SideB * SideC;
            double denomB = 2 * SideA * SideC;
            double denomC = 2 * SideA * SideB;

            if (Math.Abs(denomA) < Epsilon || Math.Abs(denomB) < Epsilon || Math.Abs(denomC) < Epsilon)
                throw new InvalidOperationException("Неможливо обчислити кути: вироджений трикутник");

            double cosA = (SideB * SideB + SideC * SideC - SideA * SideA) / denomA;
            double cosB = (SideA * SideA + SideC * SideC - SideB * SideB) / denomB;
            double cosC = (SideA * SideA + SideB * SideB - SideC * SideC) / denomC;

            AngleAlpha = Math.Acos(Math.Clamp(cosA, -1, 1)) * 180 / Math.PI;
            AngleBeta = Math.Acos(Math.Clamp(cosB, -1, 1)) * 180 / Math.PI;
            AngleGamma = Math.Acos(Math.Clamp(cosC, -1, 1)) * 180 / Math.PI;
        }

        /// <summary>
        /// Обчислює периметр трикутника
        /// </summary>
        public virtual double GetPerimeter() => SideA + SideB + SideC;

        /// <summary>
        /// Обчислює площу трикутника за формулою Герона
        /// </summary>
        public virtual double GetArea()
        {
            double s = GetPerimeter() / 2;
            double area = Math.Sqrt(s * (s - SideA) * (s - SideB) * (s - SideC));
            return area;
        }

        /// <summary>
        /// Виводить інформацію про трикутник на консоль
        /// </summary>
        public virtual void Print() => Console.WriteLine(ToString());

        public override string ToString()
        {
            return $"Трикутник: сторони ({SideA:F2}, {SideB:F2}, {SideC:F2}), " +
                   $"кути ({AngleAlpha:F2}°, {AngleBeta:F2}°, {AngleGamma:F2}°), " +
                   $"площа ({GetArea():F2}), периметр ({GetPerimeter():F2})";
        }
    }

    /// <summary>
    /// Представляє прямокутний трикутник
    /// </summary>
    public sealed class RightTriangle : Triangle
    {
        /// <summary>
        /// Створює прямокутний трикутник за двома катетами
        /// </summary>
        /// <param name="leg1">Перший катет</param>
        /// <param name="leg2">Другий катет</param>
        public RightTriangle(double leg1, double leg2)
        {
            if (leg1 <= Epsilon)
                throw new ArgumentOutOfRangeException(nameof(leg1), leg1, "Катет має бути додатним");
            if (leg2 <= Epsilon)
                throw new ArgumentOutOfRangeException(nameof(leg2), leg2, "Катет має бути додатним");

            SideA = leg1;
            SideB = leg2;
            CalculateHypotenuse();
            CalculateAngles();
            
            // Корекція прямого кута через можливі похибки округлення
            AdjustRightAngle();
        }

        /// <summary>
        /// Обчислює гіпотенузу за теоремою Піфагора
        /// </summary>
        private void CalculateHypotenuse()
        {
            SideC = Math.Sqrt(SideA * SideA + SideB * SideB);
        }

        /// <summary>
        /// Коригує прямий кут, щоб він був рівно 90°
        /// </summary>
        private void AdjustRightAngle()
        {
            // Найбільший кут у прямокутному трикутнику завжди 90°
            if (AngleGamma > AngleAlpha && AngleGamma > AngleBeta)
                AngleGamma = 90.0;
            else if (AngleAlpha > AngleBeta)
                AngleAlpha = 90.0;
            else
                AngleBeta = 90.0;
        }

        /// <summary>
        /// Обчислює площу прямокутного трикутника
        /// </summary>
        public override double GetArea()
        {
            return (SideA * SideB) / 2;
        }

        public override string ToString()
        {
            return $"Прямокутний трикутник: катети ({SideA:F2}, {SideB:F2}), " +
                   $"гіпотенуза ({SideC:F2}), кути ({AngleAlpha:F2}°, {AngleBeta:F2}°, {AngleGamma:F2}°), " +
                   $"площа ({GetArea():F2}), периметр ({GetPerimeter():F2})";
        }
    }

    /// <summary>
    /// Представляє рівнобедрений трикутник
    /// </summary>
    public sealed class IsoscelesTriangle : Triangle
    {
        /// <summary>
        /// Створює рівнобедрений трикутник
        /// </summary>
        /// <param name="baseLength">Довжина основи</param>
        /// <param name="leg">Довжина бічної сторони</param>
        public IsoscelesTriangle(double baseLength, double leg)
        {
            if (baseLength <= Epsilon)
                throw new ArgumentOutOfRangeException(nameof(baseLength), baseLength, "Основа має бути додатною");
            if (leg <= Epsilon)
                throw new ArgumentOutOfRangeException(nameof(leg), leg, "Бічна сторона має бути додатною");
            if (2 * leg <= baseLength + Epsilon)
                throw new ArgumentException($"Бічна сторона ({leg}) занадто мала для побудови трикутника з основою ({baseLength}). " +
                                           $"Має виконуватись: 2 * leg > base");

            SideA = baseLength;
            SideB = leg;
            SideC = leg;
            CalculateAngles();
        }

        /// <summary>
        /// Перевіряє, чи є кут при вершині тупим
        /// </summary>
        /// <returns>True, якщо кут при вершині більший за 90°</returns>
        public bool IsApexAngleObtuse() => AngleAlpha > 90 + Epsilon;

        /// <summary>
        /// Обчислює висоту, опущену на основу
        /// </summary>
        /// <returns>Довжина висоти</returns>
        public double GetHeightToBase()
        {
            double halfBase = SideA / 2;
            double underSqrt = SideB * SideB - halfBase * halfBase;
            
            if (underSqrt < -Epsilon)
                throw new InvalidOperationException($"Неможливо обчислити висоту: некоректна геометрія. " +
                                                   $"SideB² - (SideA/2)² = {underSqrt}");
            
            return Math.Sqrt(Math.Max(0, underSqrt));
        }

        /// <summary>
        /// Обчислює площу рівнобедреного трикутника
        /// </summary>
        public override double GetArea()
        {
            return (SideA * GetHeightToBase()) / 2;
        }

        public override string ToString()
        {
            return $"Рівнобедрений трикутник: основа ({SideA:F2}), бічні сторони ({SideB:F2}), " +
                   $"кути ({AngleAlpha:F2}°, {AngleBeta:F2}°, {AngleGamma:F2}°), " +
                   $"висота ({GetHeightToBase():F2}), площа ({GetArea():F2}), периметр ({GetPerimeter():F2})";
        }
    }

    /// <summary>
    /// Представляє рівносторонній трикутник
    /// </summary>
    public sealed class EquilateralTriangle : Triangle
    {
        /// <summary>
        /// Створює рівносторонній трикутник
        /// </summary>
        /// <param name="side">Довжина сторони</param>
        public EquilateralTriangle(double side)
        {
            if (side <= Epsilon)
                throw new ArgumentOutOfRangeException(nameof(side), side, "Сторона має бути додатною");

            SideA = SideB = SideC = side;
            CalculateAngles();
        }

        /// <summary>
        /// Встановлює всі кути рівними 60°
        /// </summary>
        protected override void CalculateAngles()
        {
            AngleAlpha = AngleBeta = AngleGamma = 60.0;
        }

        /// <summary>
        /// Обчислює площу рівностороннього трикутника
        /// </summary>
        public override double GetArea()
        {
            return (Math.Sqrt(3) / 4) * SideA * SideA;
        }

        /// <summary>
        /// Обчислює висоту рівностороннього трикутника
        /// </summary>
        /// <returns>Довжина висоти</returns>
        public double GetHeight()
        {
            return (Math.Sqrt(3) / 2) * SideA;
        }

        public override string ToString()
        {
            return $"Рівносторонній трикутник: сторона ({SideA:F2}), всі кути ({AngleAlpha:F2}°), " +
                   $"площа ({GetArea():F2}), висота ({GetHeight():F2}), периметр ({GetPerimeter():F2})";
        }
    }

    /// <summary>
    /// Представляє різносторонній трикутник
    /// </summary>
    public sealed class ScaleneTriangle : Triangle
    {
        /// <summary>
        /// Створює різносторонній трикутник
        /// </summary>
        /// <param name="a">Перша сторона</param>
        /// <param name="b">Друга сторона</param>
        /// <param name="c">Третя сторона</param>
        public ScaleneTriangle(double a, double b, double c)
        {
            if (a <= Epsilon)
                throw new ArgumentOutOfRangeException(nameof(a), a, "Сторона a має бути додатною");
            if (b <= Epsilon)
                throw new ArgumentOutOfRangeException(nameof(b), b, "Сторона b має бути додатною");
            if (c <= Epsilon)
                throw new ArgumentOutOfRangeException(nameof(c), c, "Сторона c має бути додатною");

            if (a + b <= c + Epsilon)
                throw new ArgumentException($"Не виконується нерівність трикутника: a({a}) + b({b}) <= c({c})");
            if (a + c <= b + Epsilon)
                throw new ArgumentException($"Не виконується нерівність трикутника: a({a}) + c({c}) <= b({b})");
            if (b + c <= a + Epsilon)
                throw new ArgumentException($"Не виконується нерівність трикутника: b({b}) + c({c}) <= a({a})");

            SideA = a;
            SideB = b;
            SideC = c;
            CalculateAngles();
        }

        /// <summary>
        /// Перевіряє, чи є трикутник гострокутним
        /// </summary>
        /// <returns>True, якщо всі кути менші за 90°</returns>
        public bool IsAcute()
        {
            return AngleAlpha < 90 - Epsilon && 
                   AngleBeta < 90 - Epsilon && 
                   AngleGamma < 90 - Epsilon;
        }

        /// <summary>
        /// Перевіряє, чи є трикутник тупокутним
        /// </summary>
        /// <returns>True, якщо один з кутів більший за 90°</returns>
        public bool IsObtuse()
        {
            return AngleAlpha > 90 + Epsilon || 
                   AngleBeta > 90 + Epsilon || 
                   AngleGamma > 90 + Epsilon;
        }

        public override string ToString()
        {
            string type = IsAcute() ? "гострокутний" : (IsObtuse() ? "тупокутний" : "прямокутний");
            
            return $"Різносторонній трикутник: сторони ({SideA:F2}, {SideB:F2}, {SideC:F2}), " +
                   $"кути ({AngleAlpha:F2}°, {AngleBeta:F2}°, {AngleGamma:F2}°), " +
                   $"площа ({GetArea():F2}), периметр ({GetPerimeter():F2}), тип: {type}";
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            try
            {
                // Створення різних типів трикутників
                var triangles = new List<Triangle>
                {
                    new RightTriangle(3, 4),
                    Triangle.FromThreeSides(5, 5, 5),
                    Triangle.FromThreeSides(3, 4, 5),
                    new IsoscelesTriangle(6, 5),
                    new EquilateralTriangle(7),
                    new ScaleneTriangle(6, 7, 8),
                    Triangle.FromThreeSides(5, 5, 8),  // Рівнобедрений через фабрику
                    Triangle.FromThreeSides(7, 8, 9)   // Різносторонній через фабрику
                };

                Console.WriteLine("=== Демонстрація поліморфізму ===\n");
                
                foreach (var triangle in triangles)
                {
                    triangle.Print();
                }

                // Демонстрація роботи з площами
                Console.WriteLine("\n=== Сортування за площею ===\n");
                var sortedByArea = triangles.OrderBy(t => t.GetArea()).ToList();
                
                foreach (var triangle in sortedByArea)
                {
                    Console.WriteLine($"Площа: {triangle.GetArea():F2}, Тип: {triangle.GetType().Name}");
                }

                // Обчислення сумарної площі
                double totalArea = triangles.Sum(t => t.GetArea());
                Console.WriteLine($"\nСумарна площа всіх трикутників: {totalArea:F2}");

                // Демонстрація специфічних методів
                Console.WriteLine("\n=== Специфічні методи класів ===\n");
                
                var isosceles = triangles.OfType<IsoscelesTriangle>().FirstOrDefault();
                if (isosceles != null)
                {
                    Console.WriteLine($"Рівнобедрений трикутник: висота до основи = {isosceles.GetHeightToBase():F2}, " +
                                    $"тупий кут при вершині: {isosceles.IsApexAngleObtuse()}");
                }

                var scalene = triangles.OfType<ScaleneTriangle>().FirstOrDefault();
                if (scalene != null)
                {
                    Console.WriteLine($"Різносторонній трикутник: гострокутний = {scalene.IsAcute()}, " +
                                    $"тупокутний = {scalene.IsObtuse()}");
                }

                var equilateral = triangles.OfType<EquilateralTriangle>().FirstOrDefault();
                if (equilateral != null)
                {
                    Console.WriteLine($"Рівносторонній трикутник: висота = {equilateral.GetHeight():F2}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }

            Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }
    }
}
