using System.Threading.Channels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ClajureLab2
{

    /*
     Напишите функцию, которая принимает на вход канал состоящий из последовательности чисел, 
     первое из которых является количеством последующих элементов, которые нужно поместить в массив, 
     а за ней следуют элементы этого массива, и возвращающая отдельные массивы. 
     Например 3, 4, 0, 2, 1, 2, 2, 4, 5 будет превращено в [4, 0, 2], [2], [4, 5]
     */

    internal class Program
    {
        static async Task Main(string[] args)
        {

            List<int> list = new List<int>() { 1, 2, 2, 5, 6, 3, 9, 9, 9 };

            /*var source1 = Try(list);

            foreach(var x in source1)
            {
                x.ForEach(i => Console.Write("{0} ", i));
                Console.WriteLine();
            }*/

            var myChannel = Channel.CreateUnbounded<int>() ;

            _ = Task.Factory.StartNew(async () =>
            {
                foreach (var j in list)
                {
                    await myChannel.Writer.WriteAsync(j);
                    await Task.Delay(100); // just to see
                }
            }); // данные в канале


            Console.WriteLine($"Start");
            await foreach (var item in FetchItems2(myChannel, list.Count))
            {
                //Console.WriteLine($"{item}");
                foreach(var x in item)
                {
                    Console.Write($"{x} ");
                }
                Console.WriteLine();
            }
            Console.WriteLine($"End");

        }

        static async IAsyncEnumerable<int[]> FetchItems2(Channel<int> data, int size, int d = 0)
        {
            List<int> innerList = new List<int>();

            for(int i = 0; i < size; i++)
            {
                innerList.Add(await data.Reader.ReadAsync());
            }

            for (int p = 0; p < innerList.Count;)
            {

                d = innerList[p++];    // Получаем размер массива

                d = (d <= 0) ? 0 : d; // Если размер не корректный, то правим
                int[] arr = new int[d]; // Создаём массив
                                        // create new list
/*              List<int> list = new List<int>(new int[d]);*/
                for (var i = 0; i < d && p < innerList.Count; i++, p++)
                    arr[i] = innerList[p];   // Заполняем массив данными

                await Task.Delay(750);
                yield return arr;           // Результат
            }
        }

        static List<List<int>> Try(List<int> data, int d = 0)
        {

            var myChannel = Channel.CreateUnbounded<int>();

            List<List<int>> outList = new List<List<int>>();
            for (int p = 0; p < data.Count;)
            {
                d = data[p++];    // Получаем размер массива

                d = (d <= 0) ? 0 : d; // Если размер не корректный, то правим

                List<int> tempList = new List<int>(new int[d]); //создаём промежуточный лист
                for (var i = 0; i < d && p < data.Count; i++, p++)
                    tempList[i] = data[p];   // Заполняем массив данными

                outList.Add(tempList);           // Результат
            }
            return outList;
        }

    }
}
