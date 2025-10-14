using System;

namespace Geometry
{
    /// <summary>
    /// Базовий абстрактний клас для представлення трикутника
    /// </summary>
    public abstract class Triangle
    {
        protected const double Epsilon = 1e-10;
        
        private double _sideA, _sideB, _sideC;
        private double _angleAlpha, _angleBeta, _angleGamma;
        
        /// <summary>
        /// Сторона A трикутника
        /// </summary>
        public double SideA => _sideA;
        
        /// <summary>
        /// Сторона B трикутника
        /// </summary>
        public double SideB => _sideB;
        
        /// <summary>
        /// Сторона C трикутника
        /// </summary>
        public double SideC => _sideC;
        
        /// <summary>
        /// Кут Alpha (у градусах)
        /// </summary>
        public double AngleAlpha => _angleAlpha;
        
        /// <summary>
        /// Кут Beta (у градусах)
        /// </summary>
        public double AngleBeta => _angleBeta;
        
        /// <summary>
        /// Кут Gamma (у градусах)
        /// </summary>
        public double AngleGamma => _angleGamma;
        
        protected double SideAProtected { get => _sideA; set => _sideA = value; }
        protected double SideBProtected { get => _sideB; set => _sideB = value; }
        protected double SideCProtected { get => _sideC; set => _sideC = value; }
        protected double AngleAlphaProtected { get => _angleAlpha; set => _angleAlpha = value; }
        protected double AngleBetaProtected { get => _angleBeta; set => _angleBeta = value; }
        protected double AngleGammaProtected { get => _angleGamma; set => _angleGamma = value; }
        
        /// <summary>
        /// Protected конструктор для запобігання прямого створення екземплярів
        /// </summary>
        protected Triangle() { }
        
        /// <summary>
        /// Фабричний метод для створення трикутника за трьома сторонами
        /// </summary>
        /// <param name="a">Довжина сторони A</param>
        /// <param name="b">Довжина сторони B</param>
        /// <param name="c">Довжина сторони C</param>
        /// <returns>Новий екземпляр ScaleneTriangle</returns>
        /// <exception cref="ArgumentException">Викидається при некоректних значеннях сторін</exception>
        public static Triangle FromThreeSides(double a, double b, double c)
        {
            return new ScaleneTriangle(a, b, c);
        }
        
        /// <summary>
        /// Обчислює кути трикутника за теоремою косинусів
        /// </summary>
        protected virtual void CalculateAngles()
        {
            double denomA = 2 * _sideB * _sideC;
            double denomB = 2 * _sideA * _sideC;
            double denomC = 2 * _sideA * _sideB;
            
            if (Math.Abs(denomA) < Epsilon || Math.Abs(denomB) < Epsilon || Math.Abs(denomC) < Epsilon)
            {
                throw new InvalidOperationException("Неможливо обчислити кути: одна зі сторін занадто мала");
            }
            
            double cosA = (_sideB * _sideB + _sideC * _sideC - _sideA * _sideA) / denomA;
            double cosB = (_sideA * _sideA + _sideC * _sideC - _sideB * _sideB) / denomB;
            double cosC = (_sideA * _sideA + _sideB * _sideB - _sideC * _sideC) / denomC;
            
            _angleAlpha = Math.Acos(Math.Max(-1, Math.Min(1, cosA))) * 180 / Math.PI;
            _angleBeta = Math.Acos(Math.Max(-1, Math.Min(1, cosB))) * 180 / Math.PI;
            _angleGamma = Math.Acos(Math.Max(-1, Math.Min(1, cosC))) * 180 / Math.PI;
        }
        
        /// <summary>
        /// Обчислює периметр трикутника
        /// </summary>
        /// <returns>Периметр трикутника</returns>
        public virtual double GetPerimeter()
        {
            return _sideA + _sideB + _sideC;
        }
        
        /// <summary>
        /// Виводить інформацію про трикутник
        /// </summary>
        public virtual void Print()
        {
            Console.WriteLine(ToString());
        }
        
        /// <summary>
        /// Повертає рядкове представлення трикутника
        /// </summary>
        public override string ToString()
        {
            return $"Трикутник: сторони ({_sideA:F2}, {_sideB:F2}, {_sideC:F2}), " +
                   $"кути ({_angleAlpha:F2}°, {_angleBeta:F2}°, {_angleGamma:F2}°), " +
                   $"периметр ({GetPerimeter():F2})";
        }
    }
    
    /// <summary>
    /// Клас для представлення прямокутного трикутника
    /// </summary>
    public class RightTriangle : Triangle
    {
        /// <summary>
        /// Створює прямокутний трикутник за двома катетами
        /// </summary>
        /// <param name="leg1">Перший катет</param>
        /// <param name="leg2">Другий катет</param>
        /// <exception cref="ArgumentException">Викидається при некоректних значеннях катетів</exception>
        public RightTriangle(double leg1, double leg2)
        {
            if (leg1 <= Epsilon)
                throw new ArgumentException("Катет має бути додатним", nameof(leg1));
            if (leg2 <= Epsilon)
                throw new ArgumentException("Катет має бути додатним", nameof(leg2));
            
            SideAProtected = leg1;
            SideBProtected = leg2;
            CalculateThirdSide();
            CalculateAngles();
        }
        
        /// <summary>
        /// Обчислює гіпотенузу за теоремою Піфагора
        /// </summary>
        protected void CalculateThirdSide()
        {
            SideCProtected = Math.Sqrt(SideA * SideA + SideB * SideB);
        }
        
        /// <summary>
        /// Повертає рядкове представлення прямокутного трикутника
        /// </summary>
        public override string ToString()
        {
            return $"Прямокутний трикутник: катети ({SideA:F2}, {SideB:F2}), " +
                   $"гіпотенуза ({SideC:F2}), кути ({AngleAlpha:F2}°, {AngleBeta:F2}°, {AngleGamma:F2}°), " +
                   $"периметр ({GetPerimeter():F2})";
        }
    }
    
    /// <summary>
    /// Клас для представлення рівнобедреного трикутника
    /// </summary>
    public class IsoscelesTriangle : Triangle
    {
        /// <summary>
        /// Створює рівнобедрений трикутник
        /// </summary>
        /// <param name="baseLength">Довжина основи</param>
        /// <param name="leg">Довжина бічної сторони</param>
        /// <exception cref="ArgumentException">Викидається при некоректних значеннях</exception>
        public IsoscelesTriangle(double baseLength, double leg)
        {
            if (baseLength <= Epsilon)
                throw new ArgumentException("Основа має бути додатною", nameof(baseLength));
            if (leg <= Epsilon)
                throw new ArgumentException("Бічна сторона має бути додатною", nameof(leg));
            
            if (2 * leg <= baseLength + Epsilon)
                throw new ArgumentException("Бічна сторона занадто мала для побудови трикутника");
            
            SideAProtected = baseLength;
            SideBProtected = leg;
            SideCProtected = leg;
            CalculateAngles();
        }
        
        /// <summary>
        /// Перевіряє, чи кут при вершині (між бічними сторонами) є тупим
        /// </summary>
        /// <returns>true, якщо кут при вершині > 90°</returns>
        public bool IsApexAngleObtuse()
        {
            return AngleAlpha > 90;
        }
        
        /// <summary>
        /// Обчислює висоту, опущену до основи
        /// </summary>
        /// <returns>Висота до основи</returns>
        public double GetHeightToBase()
        {
            double halfBase = SideA / 2;
            double underSqrt = SideB * SideB - halfBase * halfBase;
            
            // Захист від від'ємного значення через числову похибку
            if (underSqrt < 0)
            {
                if (underSqrt > -Epsilon)
                    underSqrt = 0;
                else
                    throw new InvalidOperationException("Неможливо обчислити висоту: некоректна геометрія");
            }
            
            return Math.Sqrt(underSqrt);
        }
        
        /// <summary>
        /// Повертає рядкове представлення рівнобедреного трикутника
        /// </summary>
        public override string ToString()
        {
            return $"Рівнобедрений трикутник: основа ({SideA:F2}), бічні сторони ({SideB:F2}), " +
                   $"кути ({AngleAlpha:F2}°, {AngleBeta:F2}°, {AngleGamma:F2}°), " +
                   $"висота до основи ({GetHeightToBase():F2}), периметр ({GetPerimeter():F2})";
        }
    }
    
    /// <summary>
    /// Клас для представлення рівностороннього трикутника
    /// </summary>
    public class EquilateralTriangle : Triangle
    {
        /// <summary>
        /// Створює рівносторонній трикутник
        /// </summary>
        /// <param name="side">Довжина сторони</param>
        /// <exception cref="ArgumentException">Викидається при некоректному значенні</exception>
        public EquilateralTriangle(double side)
        {
            if (side <= Epsilon)
                throw new ArgumentException("Сторона має бути додатною", nameof(side));
            
            SideAProtected = side;
            SideBProtected = side;
            SideCProtected = side;
            CalculateAngles();
        }
        
        /// <summary>
        /// Обчислення кутів для рівностороннього трикутника
        /// Всі кути дорівнюють 60°
        /// </summary>
        protected override void CalculateAngles()
        {
            AngleAlphaProtected = 60.0;
            AngleBetaProtected = 60.0;
            AngleGammaProtected = 60.0;
        }
        
        /// <summary>
        /// Обчислює площу рівностороннього трикутника
        /// </summary>
        /// <returns>Площа трикутника</returns>
        public double GetArea()
        {
            return (Math.Sqrt(3) / 4) * SideA * SideA;
        }
        
        /// <summary>
        /// Обчислює висоту рівностороннього трикутника
        /// </summary>
        /// <returns>Висота трикутника</returns>
        public double GetHeight()
        {
            return (Math.Sqrt(3) / 2) * SideA;
        }
        
        /// <summary>
        /// Повертає рядкове представлення рівностороннього трикутника
        /// </summary>
        public override string ToString()
        {
            return $"Рівносторонній трикутник: сторона ({SideA:F2}), " +
                   $"всі кути ({AngleAlpha:F2}°), площа ({GetArea():F2}), " +
                   $"висота ({GetHeight():F2}), периметр ({GetPerimeter():F2})";
        }
    }
    
    /// <summary>
    /// Клас для представлення різностороннього трикутника
    /// </summary>
    public class ScaleneTriangle : Triangle
    {
        /// <summary>
        /// Створює різносторонній трикутник
        /// </summary>
        /// <param name="a">Довжина сторони A</param>
        /// <param name="b">Довжина сторони B</param>
        /// <param name="c">Довжина сторони C</param>
        /// <exception cref="ArgumentException">Викидається при некоректних значеннях сторін</exception>
        public ScaleneTriangle(double a, double b, double c)
        {
            if (a <= Epsilon)
                throw new ArgumentException("Сторона має бути додатною", nameof(a));
            if (b <= Epsilon)
                throw new ArgumentException("Сторона має бути додатною", nameof(b));
            if (c <= Epsilon)
                throw new ArgumentException("Сторона має бути додатною", nameof(c));
            
            if (a + b <= c + Epsilon || a + c <= b + Epsilon || b + c <= a + Epsilon)
                throw new ArgumentException("Не виконується нерівність трикутника");
            
            SideAProtected = a;
            SideBProtected = b;
            SideCProtected = c;
            CalculateAngles();
        }
        
        /// <summary>
        /// Перевіряє, чи трикутник є гострокутним (всі кути менше 90°)
        /// </summary>
        /// <returns>true, якщо всі кути менше 90°</returns>
        public bool IsAcute()
        {
            return AngleAlpha < 90 && AngleBeta < 90 && AngleGamma < 90;
        }
        
        /// <summary>
        /// Повертає рядкове представлення різностороннього трикутника
        /// </summary>
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
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            try
            {
                Console.WriteLine("=== Тестування прямокутного трикутника ===");
                var rightTriangle = new RightTriangle(3, 4);
                rightTriangle.Print();
                Console.WriteLine($"Периметр: {rightTriangle.GetPerimeter():F2}");
                
                Console.WriteLine("\n=== Тестування різностороннього трикутника (5-5-5) ===");
                var triangle = Triangle.FromThreeSides(5, 5, 5);
                triangle.Print();
                
                Console.WriteLine("\n=== Тестування трикутника 3-4-5 ===");
                var triangle345 = Triangle.FromThreeSides(3, 4, 5);
                triangle345.Print();
                
                Console.WriteLine("\n=== Тестування рівнобедреного трикутника ===");
                var isosceles = new IsoscelesTriangle(6, 5);
                isosceles.Print();
                Console.WriteLine($"Кут при вершині тупий: {isosceles.IsApexAngleObtuse()}");
                Console.WriteLine($"Висота до основи: {isosceles.GetHeightToBase():F2}");
                
                Console.WriteLine("\n=== Тестування рівностороннього трикутника ===");
                var equilateral = new EquilateralTriangle(7);
                equilateral.Print();
                Console.WriteLine($"Площа: {equilateral.GetArea():F2}");
                Console.WriteLine($"Висота: {equilateral.GetHeight():F2}");
                
                Console.WriteLine("\n=== Тестування різностороннього трикутника (6-7-8) ===");
                var scalene = new ScaleneTriangle(6, 7, 8);
                scalene.Print();
                Console.WriteLine($"Гострокутний: {scalene.IsAcute()}");
                
                Console.WriteLine("\n=== Тест помилкових даних ===");
                try
                {
                    Triangle.FromThreeSides(1, 2, 10);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Очікувана помилка: {ex.Message}");
                }
                
                try
                {
                    var invalid = new RightTriangle(0, 5);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Очікувана помилка: {ex.Message} (параметр: {ex.ParamName})");
                }
                
                try
                {
                    var invalid2 = new IsoscelesTriangle(10, 3);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Очікувана помилка: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }
    }
}
