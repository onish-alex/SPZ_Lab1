using System;
using SPZ_Lab1;

namespace SPZ_Lab1_Interface
{
    class MobileBalance_TestInterface
    { 
        
        public static int action = 0;           //идентификатор выбранного действия
        public static MobileBalance testBalance;    //ссылка для хранения мобильного баланса

        public static void Main()
        {
            while (action != 10)
            {
                PrintMenu();
                int.TryParse(Console.ReadLine(), out action);
                if (testBalance == null && action != 1)
                {
                    Console.WriteLine("Ошибка: Номер еще не создан.");
                    continue;
                }
                switch (action)
                {
                    case 1:
                        string numberLine;
                        Tariff tariff;
                        Console.WriteLine("\nВведите номер телефона в формате +380-ХХ-YYYYYYY,\nгде XX - код оператора (50/66/95/99), YY - цифры собственно номера телефона: ");
                       
                        numberLine = Console.ReadLine();
                        
                         if (!MobileBalance.isNumberCorrect(numberLine))
                         {
                             Console.WriteLine("\nОшибка: Введен неверный формат номера.");
                             continue; 
                             
                         }
                       
                        tariff = MobileBalance.ChooseTariff();
                        testBalance = new MobileBalance(tariff, 100f, numberLine);
                        Service[] serv = new Service[5];
                        Console.WriteLine("\nНовый мобильный баланс успешно создан!");
                        testBalance.PrintState();
                       

                break;
                    case 2:

                        testBalance.PrintState();

                        break;
                    case 3:
  
                        testBalance.PrintCallHistory();
                     
                        break;
                    case 4:

                        Console.WriteLine("Введите номер телефона для звонка:");
                        testBalance.CallPhone(Console.ReadLine());

                        break;
                    case 5:

                        Console.WriteLine("Введите номер телефона для звонка: ");
                        numberLine = Console.ReadLine();
                        Console.WriteLine("Введите длительность разговора (в минутах): ");
                        int duration;
                        if (!int.TryParse(Console.ReadLine(), out duration))
                        {
                            duration = 1;
                        }
                        testBalance.CallPhone(numberLine, duration);

                        break;
                    case 6:

                        tariff = MobileBalance.ChooseTariff();
                        testBalance.ChangeTariff(tariff);

                        break;
                    case 7:

                        Console.WriteLine("\nВведите сумму для пополнения: ");
                        float sum;
                        float.TryParse(Console.ReadLine(), out sum);
                        testBalance.RefillBalance(sum);

                        break;
                    case 8:

                        Service service;
                        service = MobileBalance.ChooseService();
                        testBalance.AddService(service);

                        break;
                    case 9:

                        service = MobileBalance.ChooseService();
                        testBalance.RemoveService(service);

                        break;
                    default: continue;
                }
            }
        }

        public static void PrintMenu()
        {
            Console.WriteLine(
                "\n1. Создать новый номер" +
                "\n2. Отобразить состояние счета" +
                "\n3. Отобразить историю звонков" +
                "\n4. Совершить звонок" +
                "\n5. Совершить звонок (с указанием длительности)" +
                "\n6. Перейти на новый тариф" +
                "\n7. Пополнить счет" +
                "\n8. Подключить услугу" +
                "\n9. Отключить услугу" +
                "\n10. Выход" +
                "\n/////////////////////////////////////");
        } 
    }
}
