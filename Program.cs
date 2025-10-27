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

        // Кешування площі для оптимізації
        private double? _cachedArea;

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
            // Перевірка на додатність сторін
            if (a <= Epsilon)
                throw new ArgumentOutOfRangeException(nameof(a), a, 
                    "Сторона 'a' має бути додатним числом");
            if (b <= Epsilon)
                throw new ArgumentOutOfRangeException(nameof(b), b, 
                    "Сторона 'b' має бути додатним числом");
            if (c <= Epsilon)
                throw new ArgumentOutOfRangeException(nameof(c), c, 
                    "Сторона 'c' має бути додатним числом");

            // Перевірка нерівності трикутника з інформативними повідомленнями
            if (a + b <= c + Epsilon)
                throw new ArgumentException(
                    $"Не виконується нерівність трикутника: сума сторін a ({a:F2}) + b ({b:F2}) = {a + b:F2} " +
                    $"повинна бути більшою за c ({c:F2})");
            if (a + c <= b + Epsilon)
                throw new ArgumentException(
                    $"Не виконується нерівність трикутника: сума сторін a ({a:F2}) + c ({c:F2}) = {a + c:F2} " +
                    $"повинна бути більшою за b ({b:F2})");
            if (b + c <= a + Epsilon)
                throw new ArgumentException(
                    $"Не виконується нерівність трикутника: сума сторін b ({b:F2}) + c ({c:F2}) = {b + c:F2} " +
                    $"повинна бути більшою за a ({a:F2})");

            // Визначення типу трикутника за співвідношенням сторін
            bool abEqual = Math.Abs(a - b) < Epsilon;
            bool bcEqual = Math.Abs(b - c) < Epsilon;
            bool acEqual = Math.Abs(a - c) < Epsilon;

            // Рівносторонній: всі три сторони рівні
            if (abEqual && bcEqual)
            {
                return new EquilateralTriangle(a);
            }

            // Рівнобедрений: рівно дві сторони рівні
            if (abEqual)
            {
                // a == b (бічні сторони), c - основа
                return new IsoscelesTriangle(c, a);
            }
            if (acEqual)
            {
                // a == c (бічні сторони), b - основа
                return new IsoscelesTriangle(b, a);
            }
            if (bcEqual)
            {
                // b == c (бічні сторони), a - основа
                return new IsoscelesTriangle(a, b);
            }

            // Різносторонній: всі сторони різні
            return new ScaleneTriangle(a, b, c);
        }

        /// <summary>
        /// Обчислює кути трикутника за теоремою косинусів
        /// </summary>
        protected virtual void CalculateAngles()
        {
            // Перевірка на вироджений трикутник
            double denomA = 2 * SideB * SideC;
            double denomB = 2 * SideA * SideC;
            double denomC = 2 * SideA * SideB;

            if (Math.Abs(denomA) < Epsilon || Math.Abs(denomB) < Epsilon || Math.Abs(denomC) < Epsilon)
                throw new InvalidOperationException(
                    "Неможливо обчислити кути: вироджений трикутник (одна або більше сторін дорівнює нулю)");

            // Теорема косинусів: cos(A) = (b² + c² - a²) / (2bc)
            double cosA = (SideB * SideB + SideC * SideC - SideA * SideA) / denomA;
            double cosB = (SideA * SideA + SideC * SideC - SideB * SideB) / denomB;
            double cosC = (SideA * SideA + SideB * SideB - SideC * SideC) / denomC;

            // Обмеження значень косинусів діапазоном [-1, 1] для уникнення помилок Math.Acos
            AngleAlpha = Math.Acos(Math.Clamp(cosA, -1, 1)) * 180 / Math.PI;
            AngleBeta = Math.Acos(Math.Clamp(cosB, -1, 1)) * 180 / Math.PI;
            AngleGamma = Math.Acos(Math.Clamp(cosC, -1, 1)) * 180 / Math.PI;
        }

        /// <summary>
        /// Обчислює периметр трикутника
        /// </summary>
        public virtual double GetPerimeter() => SideA + SideB + SideC;

        /// <summary>
        /// Обчислює площу трикутника за формулою Герона з кешуванням
        /// </summary>
        public virtual double GetArea()
        {
            if (_cachedArea.HasValue)
                return _cachedArea.Value;

            double s = GetPerimeter() / 2;
            _cachedArea = Math.Sqrt(s * (s - SideA) * (s - SideB) * (s - SideC));
            return _cachedArea.Value;
        }

        /// <summary>
        /// Скидає кешовані значення (викликається при зміні параметрів трикутника)
        /// </summary>
        protected void InvalidateCache()
        {
            _cachedArea = null;
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
                throw new ArgumentOutOfRangeException(nameof(leg1), leg1, 
                    "Катет 'leg1' має бути додатним числом");
            if (leg2 <= Epsilon)
                throw new ArgumentOutOfRangeException(nameof(leg2), leg2, 
                    "Катет 'leg2' має бути додатним числом");

            SideA = leg1;
            SideB = leg2;
            CalculateHypotenuse();
            CalculateAngles();
            
            // Корекція прямого кута для уникнення числових похибок
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
        /// Коригує прямий кут на основі найбільшої сторони (гіпотенузи)
        /// У прямокутному трикутнику кут, протилежний найбільшій стороні, завжди дорівнює 90°
        /// </summary>
        private void AdjustRightAngle()
        {
            // Визначаємо найбільшу сторону (гіпотенузу)
            double maxSide = Math.Max(SideA, Math.Max(SideB, SideC));
            
            // Кут, протилежний найбільшій стороні, має бути 90°
            if (Math.Abs(maxSide - SideA) < Epsilon)
                AngleAlpha = 90.0;
            else if (Math.Abs(maxSide - SideB) < Epsilon)
                AngleBeta = 90.0;
            else
                AngleGamma = 90.0;
        }

        /// <summary>
        /// Обчислює площу прямокутного трикутника (оптимізована формула)
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
        // Кешування висоти
        private double? _cachedHeight;

        /// <summary>
        /// Створює рівнобедрений трикутник
        /// </summary>
        /// <param name="baseLength">Довжина основи</param>
        /// <param name="leg">Довжина бічної сторони</param>
        public IsoscelesTriangle(double baseLength, double leg)
        {
            if (baseLength <= Epsilon)
                throw new ArgumentOutOfRangeException(nameof(baseLength), baseLength, 
                    "Основа трикутника має бути додатним числом");
            if (leg <= Epsilon)
                throw new ArgumentOutOfRangeException(nameof(leg), leg, 
                    "Бічна сторона трикутника має бути додатним числом");
            if (2 * leg <= baseLength + Epsilon)
                throw new ArgumentException(
                    $"Бічна сторона ({leg:F2}) занадто мала для побудови трикутника з основою ({baseLength:F2}). " +
                    $"Має виконуватись нерівність: 2 × leg > base, тобто {2 * leg:F2} > {baseLength:F2}");

            SideA = baseLength;
            SideB = leg;
            SideC = leg;
            CalculateAngles();
        }

        /// <summary>
        /// Перевіряє, чи є кут при вершині тупим (більше 90°)
        /// </summary>
        /// <returns>True, якщо кут при вершині більший за 90°</returns>
        public bool IsApexAngleObtuse() => AngleAlpha > 90 + Epsilon;

        /// <summary>
        /// Обчислює висоту, опущену на основу, з кешуванням
        /// </summary>
        /// <returns>Довжина висоти</returns>
        public double GetHeightToBase()
        {
            if (_cachedHeight.HasValue)
                return _cachedHeight.Value;

            double halfBase = SideA / 2;
            double underSqrt = SideB * SideB - halfBase * halfBase;
            
            if (underSqrt < -Epsilon)
                throw new InvalidOperationException(
                    $"Неможливо обчислити висоту: некоректна геометрія трикутника. " +
                    $"Значення під коренем: leg² - (base/2)² = {SideB * SideB:F2} - {halfBase * halfBase:F2} = {underSqrt:F2}");
            
            _cachedHeight = Math.Sqrt(Math.Max(0, underSqrt));
            return _cachedHeight.Value;
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
        // Кешування висоти
        private double? _cachedHeight;

        /// <summary>
        /// Створює рівносторонній трикутник
        /// </summary>
        /// <param name="side">Довжина сторони</param>
        public EquilateralTriangle(double side)
        {
            if (side <= Epsilon)
                throw new ArgumentOutOfRangeException(nameof(side), side, 
                    "Сторона трикутника має бути додатним числом");

            SideA = SideB = SideC = side;
            CalculateAngles();
        }

        /// <summary>
        /// Встановлює всі кути рівними 60° (властивість рівностороннього трикутника)
        /// </summary>
        protected override void CalculateAngles()
        {
            AngleAlpha = AngleBeta = AngleGamma = 60.0;
        }

        /// <summary>
        /// Обчислює площу рівностороннього трикутника за оптимізованою формулою
        /// </summary>
        public override double GetArea()
        {
            return (Math.Sqrt(3) / 4) * SideA * SideA;
        }

        /// <summary>
        /// Обчислює висоту рівностороннього трикутника з кешуванням
        /// </summary>
        /// <returns>Довжина висоти</returns>
        public double GetHeight()
        {
            if (_cachedHeight.HasValue)
                return _cachedHeight.Value;

            _cachedHeight = (Math.Sqrt(3) / 2) * SideA;
            return _cachedHeight.Value;
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
                throw new ArgumentOutOfRangeException(nameof(a), a, 
                    "Сторона 'a' має бути додатним числом");
            if (b <= Epsilon)
                throw new ArgumentOutOfRangeException(nameof(b), b, 
                    "Сторона 'b' має бути додатним числом");
            if (c <= Epsilon)
                throw new ArgumentOutOfRangeException(nameof(c), c, 
                    "Сторона 'c' має бути додатним числом");

            if (a + b <= c + Epsilon)
                throw new ArgumentException(
                    $"Не виконується нерівність трикутника: a ({a:F2}) + b ({b:F2}) = {a + b:F2} повинно бути > c ({c:F2})");
            if (a + c <= b + Epsilon)
                throw new ArgumentException(
                    $"Не виконується нерівність трикутника: a ({a:F2}) + c ({c:F2}) = {a + c:F2} повинно бути > b ({b:F2})");
            if (b + c <= a + Epsilon)
                throw new ArgumentException(
                    $"Не виконується нерівність трикутника: b ({b:F2}) + c ({c:F2}) = {b + c:F2} повинно бути > a ({a:F2})");

            SideA = a;
            SideB = b;
            SideC = c;
            CalculateAngles();
        }

        /// <summary>
        /// Перевіряє, чи є трикутник гострокутним (всі кути менше 90°)
        /// </summary>
        /// <returns>True, якщо всі кути менші за 90°</returns>
        public bool IsAcute()
        {
            return AngleAlpha < 90 - Epsilon && 
                   AngleBeta < 90 - Epsilon && 
                   AngleGamma < 90 - Epsilon;
        }

        /// <summary>
        /// Перевіряє, чи є трикутник тупокутним (один кут більше 90°)
        /// </summary>
        /// <returns>True, якщо один з кутів більший за 90°</returns>
        public bool IsObtuse()
        {
            return AngleAlpha > 90 + Epsilon || 
                   AngleBeta > 90 + Epsilon || 
                   AngleGamma > 90 + Epsilon;
        }

        /// <summary>
        /// Визначає тип трикутника за кутами
        /// </summary>
        /// <returns>Рядок з описом типу трикутника</returns>
        private string GetAngleType()
        {
            if (IsAcute())
                return "гострокутний";
            if (IsObtuse())
                return "тупокутний";
            return "прямокутний";
        }

        public override string ToString()
        {
            return $"Різносторонній трикутник: сторони ({SideA:F2}, {SideB:F2}, {SideC:F2}), " +
                   $"кути ({AngleAlpha:F2}°, {AngleBeta:F2}°, {AngleGamma:F2}°), " +
                   $"площа ({GetArea():F2}), периметр ({GetPerimeter():F2}), тип: {GetAngleType()}";
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            try
            {
                Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║        Демонстрація роботи з геометричними фігурами       ║");
                Console.WriteLine("╚════════════════════════════════════════════════════════════╝\n");

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

                // Демонстрація роботи фабричного методу
                DemonstrateFactoryMethod();

                // Демонстрація обробки помилок
                DemonstrateErrorHandling();

                Console.WriteLine("=== Демонстрація поліморфізму: виведення всіх трикутників ===\n");
                
                int counter = 1;
                foreach (var triangle in triangles)
                {
                    Console.WriteLine($"{counter}. {triangle}");
                    counter++;
                }

                // Демонстрація роботи з площами
                Console.WriteLine("\n=== Сортування трикутників за площею (від меншої до більшої) ===\n");
                var sortedByArea = triangles.OrderBy(t => t.GetArea()).ToList();
                
                counter = 1;
                foreach (var triangle in sortedByArea)
                {
                    Console.WriteLine($"{counter}. Площа: {triangle.GetArea():F2} кв.од., " +
                                    $"Тип: {triangle.GetType().Name}");
                    counter++;
                }

                // Обчислення сумарної площі та периметру
                double totalArea = triangles.Sum(t => t.GetArea());
                double totalPerimeter = triangles.Sum(t => t.GetPerimeter());
                Console.WriteLine($"\n┌─────────────────────────────────────────────┐");
                Console.WriteLine($"│ Сумарна площа всіх трикутників: {totalArea,10:F2} │");
                Console.WriteLine($"│ Сумарний периметр всіх трикутників: {totalPerimeter,7:F2} │");
                Console.WriteLine($"│ Середня площа: {totalArea / triangles.Count,24:F2} │");
                Console.WriteLine($"└─────────────────────────────────────────────┘");

                // Демонстрація специфічних методів класів
                Console.WriteLine("\n=== Демонстрація специфічних методів підкласів ===\n");
                
                var isosceles = triangles.OfType<IsoscelesTriangle>().FirstOrDefault();
                if (isosceles != null)
                {
                    Console.WriteLine($"► Рівнобедрений трикутник:");
                    Console.WriteLine($"  • Висота до основи: {isosceles.GetHeightToBase():F2}");
                    Console.WriteLine($"  • Чи тупий кут при вершині? {(isosceles.IsApexAngleObtuse() ? "Так" : "Ні")}");
                }

                var scalene = triangles.OfType<ScaleneTriangle>().FirstOrDefault();
                if (scalene != null)
                {
                    Console.WriteLine($"\n► Різносторонній трикутник:");
                    Console.WriteLine($"  • Гострокутний? {(scalene.IsAcute() ? "Так" : "Ні")}");
                    Console.WriteLine($"  • Тупокутний? {(scalene.IsObtuse() ? "Так" : "Ні")}");
                }

                var equilateral = triangles.OfType<EquilateralTriangle>().FirstOrDefault();
                if (equilateral != null)
                {
                    Console.WriteLine($"\n► Рівносторонній трикутник:");
                    Console.WriteLine($"  • Висота: {equilateral.GetHeight():F2}");
                    Console.WriteLine($"  • Площа: {equilateral.GetArea():F2}");
                }

                var rightTriangle = triangles.OfType<RightTriangle>().FirstOrDefault();
                if (rightTriangle != null)
                {
                    Console.WriteLine($"\n► Прямокутний трикутник:");
                    Console.WriteLine($"  • Катети: {rightTriangle.SideA:F2}, {rightTriangle.SideB:F2}");
                    Console.WriteLine($"  • Гіпотенуза: {rightTriangle.SideC:F2}");
                    Console.WriteLine($"  • Площа: {rightTriangle.GetArea():F2}");
                }

                // Пошук трикутника з найбільшою площею
                var largestTriangle = triangles.OrderByDescending(t => t.GetArea()).First();
                Console.WriteLine($"\n=== Трикутник з найбільшою площею ===");
                Console.WriteLine($"{largestTriangle}");

                // Статистика за типами трикутників
                PrintStatistics(triangles);

                // Порівняння двох трикутників
                if (triangles.Count >= 2)
                {
                    CompareTriangles(triangles[0], triangles[1]);
                }

            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"\n❌ Помилка вхідних даних: {ex.Message}");
                Console.WriteLine($"   Параметр: {ex.ParamName}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"\n❌ Помилка побудови трикутника: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Неочікувана помилка: {ex.Message}");
                Console.WriteLine($"   Тип помилки: {ex.GetType().Name}");
            }

            Console.WriteLine("\n" + new string('═', 60));
            Console.WriteLine("Натисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }

        /// <summary>
        /// Демонструє роботу фабричного методу FromThreeSides
        /// </summary>
        static void DemonstrateFactoryMethod()
        {
            Console.WriteLine("=== Демонстрація фабричного методу FromThreeSides ===\n");

            var testCases = new[]
            {
                (a: 5.0, b: 5.0, c: 5.0, expected: "EquilateralTriangle"),
                (a: 5.0, b: 5.0, c: 8.0, expected: "IsoscelesTriangle"),
                (a: 3.0, b: 4.0, c: 5.0, expected: "ScaleneTriangle"),
                (a: 6.0, b: 8.0, c: 10.0, expected: "ScaleneTriangle")
            };

            foreach (var (a, b, c, expected) in testCases)
            {
                var triangle = Triangle.FromThreeSides(a, b, c);
                string actual = triangle.GetType().Name;
                string status = actual == expected ? "✓" : "✗";
                Console.WriteLine($"{status} Сторони ({a}, {b}, {c}) → {actual}");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Демонструє обробку помилок при створенні некоректних трикутників
        /// </summary>
        static void DemonstrateErrorHandling()
        {
            Console.WriteLine("=== Демонстрація обробки помилок ===\n");

            var invalidCases = new[]
            {
                (a: 1.0, b: 2.0, c: 10.0, description: "Порушення нерівності трикутника"),
                (a: -5.0, b: 3.0, c: 4.0, description: "Від'ємна сторона"),
                (a: 0.0, b: 5.0, c: 5.0, description: "Нульова сторона")
            };

            foreach (var (a, b, c, description) in invalidCases)
            {
                try
                {
                    var triangle = Triangle.FromThreeSides(a, b, c);
                    Console.WriteLine($"✗ {description}: очікувалась помилка, але трикутник створено");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"✓ {description}: {ex.Message.Split('\n')[0]}");
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Виводить статистику про колекцію трикутників
        /// </summary>
        static void PrintStatistics(List<Triangle> triangles)
        {
            Console.WriteLine("\n=== Статистика трикутників ===\n");

            var grouped = triangles.GroupBy(t => t.GetType().Name);
            
            foreach (var group in grouped)
            {
                Console.WriteLine($"► {group.Key}:");
                Console.WriteLine($"  • Кількість: {group.Count()}");
                Console.WriteLine($"  • Середня площа: {group.Average(t => t.GetArea()):F2}");
                Console.WriteLine($"  • Середній периметр: {group.Average(t => t.GetPerimeter()):F2}");
            }
        }

        /// <summary>
        /// Порівнює два трикутники за різними параметрами
        /// </summary>
        static void CompareTriangles(Triangle t1, Triangle t2)
        {
            Console.WriteLine("\n=== Порівняння двох трикутників ===\n");
            Console.WriteLine($"Трикутник 1: {t1.GetType().Name}");
            Console.WriteLine($"Трикутник 2: {t2.GetType().Name}\n");

            Console.WriteLine($"Площа:     {t1.GetArea():F2} vs {t2.GetArea():F2} " +
                            $"({(t1.GetArea() > t2.GetArea() ? "Перший більший" : "Другий більший")})");
            Console.WriteLine($"Периметр:  {t1.GetPerimeter():F2} vs {t2.GetPerimeter():F2} " +
                            $"({(t1.GetPerimeter() > t2.GetPerimeter() ? "Перший більший" : "Другий більший")})");
        }
    }
}
