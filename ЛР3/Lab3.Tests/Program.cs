using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Lab3;

namespace Lab3.Tests
{
    internal static class Program
    {
        private static int passed;
        private static int failed;
        private static readonly StringBuilder report = new StringBuilder();

        private static int Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            report.AppendLine("Протокол тестирования MiniList<T>");
            report.AppendLine("Лабораторная работа №3. Интерфейсы IEnumerable<T>, IEnumerator<T>, IEnumerable, IEnumerator.");
            report.AppendLine();
            report.AppendLine("План");
            report.AppendLine("1. Пустой список: MoveNext возвращает false, чтение Current до начала запрещено.");
            report.AppendLine("2. Append вставляет в голову: порядок перебора обратный порядку добавления, номера узлов возрастают.");
            report.AppendLine("3. Обобщённый foreach и необобщённый foreach дают одну и ту же последовательность.");
            report.AppendLine("4. Reset возвращает перечислитель в исходное положение, повторный проход совпадает с первым.");
            report.AppendLine("5. Current после завершения перебора запрещён; лишние MoveNext остаются false.");
            report.AppendLine("6. Dispose не выбрасывает исключение. Два перечислителя одного списка независимы.");
            report.AppendLine();
            report.AppendLine("Результаты");

            TestEmpty();
            TestAppendOrder();
            TestGenericAndNonGeneric();
            TestReset();
            TestAfterEnd();
            TestDisposeAndIndependentEnumerators();

            report.AppendLine();
            report.AppendLine("Итог: пройдено " + passed + ", не пройдено " + failed + ".");
            report.AppendLine();
            if (failed == 0)
            {
                report.AppendLine("Заключение");
                report.AppendLine("MiniList<T> строит односвязный список с вставкой в голову и отдаёт элементы через обобщённый и необобщённый перечислители. Контракт Current, MoveNext, Reset и Dispose выполняется на пустом списке, на списке из нескольких узлов и при повторном проходе. Реализация интерфейсов пригодна для перебора коллекции в приложении.");
            }
            else
            {
                report.AppendLine("Заключение");
                report.AppendLine("Есть непройденные проверки. Реализацию перечислителя нужно исправить до сдачи.");
            }

            string text = report.ToString();
            Console.Write(text);
            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "Протокол_тестирования.txt"), text, new UTF8Encoding(true));
            return failed == 0 ? 0 : 1;
        }

        private static void Check(string name, bool condition)
        {
            if (condition)
            {
                passed++;
                report.AppendLine("  [+] " + name);
            }
            else
            {
                failed++;
                report.AppendLine("  [-] " + name);
            }
        }

        private static void CheckThrows(string name, Action action)
        {
            bool thrown = false;
            try
            {
                action();
            }
            catch (InvalidOperationException)
            {
                thrown = true;
            }
            catch (Exception)
            {
                thrown = false;
            }

            Check(name, thrown);
        }

        private static void TestEmpty()
        {
            MiniList<string> list = new MiniList<string>();
            using (IEnumerator<string> enumerator = list.GetEnumerator())
            {
                CheckThrows("Пустой список: Current до MoveNext", delegate { string unused = enumerator.Current; });
                Check("Пустой список: MoveNext == false", enumerator.MoveNext() == false);
                CheckThrows("Пустой список: Current после MoveNext", delegate { string unused = enumerator.Current; });
            }
        }

        private static void TestAppendOrder()
        {
            MiniList<string> list = new MiniList<string>();
            int first = list.Append("a");
            int second = list.Append("b");
            int third = list.Append("c");

            Check("Append возвращает возрастающие номера", second == first + 1 && third == second + 1);
            Check("Голова списка — последний добавленный элемент", list.Top != null && list.Top.content == "c");
            Check("Второй узел хранит предыдущий элемент", list.Top.Next != null && list.Top.Next.content == "b");
            Check("Хвост хранит первый элемент и не имеет продолжения",
                list.Top.Next.Next != null && list.Top.Next.Next.content == "a" && list.Top.Next.Next.Next == null);
        }

        private static void TestGenericAndNonGeneric()
        {
            MiniList<int> list = new MiniList<int>();
            list.Append(1);
            list.Append(2);
            list.Append(3);

            List<int> generic = new List<int>();
            foreach (int value in list)
                generic.Add(value);

            List<int> nonGeneric = new List<int>();
            foreach (object value in (IEnumerable)list)
                nonGeneric.Add((int)value);

            Check("Обобщённый foreach: 3, 2, 1", generic.Count == 3 && generic[0] == 3 && generic[1] == 2 && generic[2] == 1);
            Check("Необобщённый foreach совпадает с обобщённым",
                nonGeneric.Count == generic.Count && nonGeneric[0] == generic[0] && nonGeneric[1] == generic[1] && nonGeneric[2] == generic[2]);

            IEnumerator raw = ((IEnumerable)list).GetEnumerator();
            raw.MoveNext();
            Check("IEnumerator.Current возвращает тот же объект, что и обобщённый Current", raw.Current is int && (int)raw.Current == 3);
            (raw as IDisposable).Dispose();
        }

        private static void TestReset()
        {
            MiniList<string> list = new MiniList<string>();
            list.Append("x");
            list.Append("y");

            using (IEnumerator<string> enumerator = list.GetEnumerator())
            {
                Check("Первый MoveNext", enumerator.MoveNext() && enumerator.Current == "y");
                Check("Второй MoveNext", enumerator.MoveNext() && enumerator.Current == "x");
                enumerator.Reset();
                CheckThrows("После Reset чтение Current запрещено", delegate { string unused = enumerator.Current; });
                Check("После Reset проход повторяется", enumerator.MoveNext() && enumerator.Current == "y");
            }
        }

        private static void TestAfterEnd()
        {
            MiniList<string> list = new MiniList<string>();
            list.Append("only");

            using (IEnumerator<string> enumerator = list.GetEnumerator())
            {
                enumerator.MoveNext();
                Check("Повторный MoveNext за последним элементом == false", enumerator.MoveNext() == false);
                Check("Ещё один MoveNext остаётся false", enumerator.MoveNext() == false);
                CheckThrows("Current после завершения запрещён", delegate { string unused = enumerator.Current; });
            }
        }

        private static void TestDisposeAndIndependentEnumerators()
        {
            MiniList<string> list = new MiniList<string>();
            list.Append("1");
            list.Append("2");

            IEnumerator<string> first = list.GetEnumerator();
            IEnumerator<string> second = list.GetEnumerator();
            first.MoveNext();
            second.MoveNext();
            second.MoveNext();

            Check("Два перечислителя независимы", first.Current == "2" && second.Current == "1");

            bool disposeOk = true;
            try
            {
                first.Dispose();
                second.Dispose();
            }
            catch (Exception)
            {
                disposeOk = false;
            }

            Check("Dispose не выбрасывает исключение", disposeOk);
        }
    }
}
