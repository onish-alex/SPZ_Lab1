using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SPZ_Lab1
{ 
    //перечисление тарифов
    public enum Tariff
    {
        VFSuperNetUnlim,
        VFSuperNetPro,
        VF_RED_EXTRA_L,
        VF_RED_EXTRA_S
    }

    //перечисление услуг
    public enum Service
    {
        VFMusic,
        OnlinePASS,
        VideoPASS,
        InstaPASS,
        YearWithoutPayment,
    }

    //класс "мобильного баланса"
    class MobileBalance
    {
        //структура для хранения информации о звонке
        struct Call
        {
            //поля для хранения данных о номере и длительности звонка
            private readonly string _number;
            private readonly int _callDuration;

            //инициализирующий конструктор
            public Call(string number, int duration)
            {
                _number = number;
                _callDuration = duration;
            }

            public override string ToString()
            {
                return string.Format("Номер: {0}, продолжительность звонка: {1} минут(-ы).\n", _number, _callDuration);
            }
        }
        
        private Tariff _tarifName;      //название текущего тарифа
        private float _minutePrice;     //стоимость разговора в минуту с учетом тарифа
        private float _currentBalance;  //сумма на счету
        private string _phoneNumber;    //номер телефона
        private List<Service> _services = new List<Service>(); //контейнер для хранения подключенных услуг
        private List<Call> _callHistory = new List<Call>(); //контейнер для хранения истории вызовов

        private static readonly float[] _tariffPrices =  {0.1f, 0.2f, 0.5f, 2f};      //стоимость разговора на разных тарифам
        private static readonly float[] _servicePrices = { 15f, 20f, 30f, 50f, 300f }; //стоимость подключения услуг
       
        //реализация свойств из интерфейса
        public Tariff TariffName
        {
            get
            {
                return _tarifName;
            }
            set
            {
                Array values = Enum.GetValues(typeof(Tariff));

                if (((int[])values).Contains((int)value)) {
                    _tarifName = value;
                }
                else
                {
                    _tarifName = Tariff.VF_RED_EXTRA_L;
                }
            }
        }
        public float MinutePrice
        {
            get
            {
                return _minutePrice;
            }  
            private set
            {
                _minutePrice = value;
            }
            
        }
        public float CurrentBalance
        {
            get
            {
                return _currentBalance;
            }
            set
            {
                if (value >= 0f && value <= 1000f)
                {
                    _currentBalance = value;
                }
                else if (value > 1000f)
                {
                    Console.WriteLine("\nСостояние счета не может превышать максимально допустимое значение (1000). На баланс была переведена недостающая до максимума сумма");
                    _currentBalance = 1000f;
                }
                else
                {
                    Console.WriteLine("\nНа счету недостаточно средств!");
                }
            }
        }
        public string PhoneNumber
        {
          get
            {
                return _phoneNumber;
            }
            
           private set
            {
                if (isNumberCorrect(value))
                {
                    _phoneNumber = value;
                } else {
                    _phoneNumber = "+380-99-9999999";
                }
                
            }
        }

        //инициализирующий конструктор
        public MobileBalance(
            Tariff tarifName,
            float currentBalance,
            string phoneNumber,
            params Service[] services
            )
        {

            TariffName = tarifName;
            MinutePrice = _tariffPrices[(int)TariffName];
            CurrentBalance = currentBalance;
            PhoneNumber = phoneNumber;
            foreach(Service a in services)
            {
                _services.Add(a);
            }
            
        }

        //конструктор по умолчанию
        public MobileBalance() : this(Tariff.VF_RED_EXTRA_L, 
                                      0f, 
                                      "+380-99-999-9999" 
                                      ) {}

        //вспомогательная функция проверки корректности формата номера телефона
        public static bool isNumberCorrect(string number)
        {
            int temp;
            string[] numberParts = number.Split('-');
            if (number.Length == 15 && numberParts.Length == 3 && numberParts[0] == "+380" && "66-50-99-95".Contains(numberParts[1]) && int.TryParse(numberParts[2], out temp) /*&& int.TryParse(numberParts[3], out temp)*/)
            {
                return true;
            }
            return false;
        }

        //вспомогательный метод выбора услуги по названию через консоль
        public static Service ChooseService()
        {
            Service service;
            Console.WriteLine("\nВведите название услуги \n(VFMusic / OnlinePASS / VideoPASS / InstaPASS / YearWithoutPayment):");
            string str = Console.ReadLine();
            if (!Enum.TryParse(str, out service))
            {
                Console.WriteLine("\nОшибка: Введено неверное название услуги.");
                return (Service)(-1);
            }
            return service;
        }

        //вспомогательный метод выбора тарифа по названию через консоль
        public static Tariff ChooseTariff()
        {
            Tariff tariff;
            Console.WriteLine("\nВведите название тарифа \n(VFSuperNetUnlim / VFSuperNetPro / VF_RED_EXTRA_L / VF_RED_EXTRA_S):");
            string str = Console.ReadLine();
            if (!Enum.TryParse(str, out tariff))
            {
                Console.WriteLine("\nОшибка: Введено неверное название тарифа. Установлен тариф по умолчанию.");
                return Tariff.VF_RED_EXTRA_L;
            } 
            return tariff;
        }

        //изменение текущего тарифа
        public void ChangeTariff(Tariff t)
        {
            TariffName = t;
            MinutePrice = _tariffPrices[(int)t];
        }
        
        //звонок с указанием номера и длительности разговора
        public float CallPhone(string phone, int minutes)
        {
            if (!isNumberCorrect(phone))
            {
                Console.WriteLine("\nНеверно набранный номер!");
                return 0f;
            }
            if (minutes < 0)
            {
                Console.WriteLine("\nНекорректное значение длительности разговора!");
                return 0f;
            }

            float CurrentSum =  minutes * MinutePrice;

            //если на указанну длительность разговора не хватает средств, 
            //совершить звонок на всю имеющуюся на счету сумму, рассчитать на сколько минут хватило этой суммы
            if (CurrentSum > CurrentBalance)
            {
                minutes = (int)(CurrentBalance / MinutePrice);
                CurrentSum = CurrentBalance;
            }

            CurrentBalance -= CurrentSum;
            _callHistory.Add(new Call(phone, minutes));

            Console.WriteLine("\nСовершен звонок на номер {0} длительностью {1} минут(-ы)\nСо счета списано {2} грн.", phone, minutes, CurrentSum);

            return CurrentSum;
        }

        //звонок с указанием номера и случайной длительностью (1 - 180 минут)
        public float CallPhone(string phone)
        {
            Random rand = new Random();
            return CallPhone(phone, rand.Next(1, 180));
        }

        //пополнение счета на указанную сумму
        public void RefillBalance(float sum)
        {
            if (sum > 0)
            {
                CurrentBalance += sum;
                Console.WriteLine("\nБаланс успешно пополнен!");
            } else
            {
                Console.WriteLine("\nУказана некорректная сумма!");
            }
        }

        //подключить услугу
        public void AddService(Service serviceName)
        {
            if (_services.Contains(serviceName))
            {
                Console.WriteLine("\nУслуга {0} уже подключена для данного номера ({1})", serviceName.ToString(), PhoneNumber);
            }
            else if ((int)serviceName == -1)
            {
                return;
            }
            else if (((int)serviceName) > _servicePrices.Length - 1 || ((int)serviceName) < 0)
            {
                Console.WriteLine("Имя услуги указано неверно!");
            }
            else if (CurrentBalance < _servicePrices[(int)serviceName]) 
            {
                Console.WriteLine("недостаточно средств для подключения услуги!");
            } 
            else
            {
                _services.Add(serviceName);
                CurrentBalance -= _servicePrices[(int)serviceName];
                Console.WriteLine("\nПо номеру {0} была успешно подключена услуга {1}.\nСо счета была списана плата за данную услугу за месяц использования ({2} грн)", PhoneNumber, serviceName.ToString(), _servicePrices[(int)serviceName]);
            }
        }

        //отключить услугу
        public void RemoveService(Service serviceName)
        {
            if (_services.Contains(serviceName))
            {
                _services.Remove(serviceName);
                Console.WriteLine("\nУслуга {0} была успешно отключена", serviceName.ToString());
            }
            else if ((int)serviceName == -1)
            {
                return;
            }
            else
            {
                Console.WriteLine("\nУслуга {0} не подключена!", serviceName.ToString());
            }
        }

        //вывод состояния баланса на консоль
        public void PrintState()
        {
           
            Service[] arr = _services.ToArray();
            //int a = arr.Length;
            StringBuilder serviceNames = new StringBuilder("");
            if (arr.Length > 0)
            {
                serviceNames.Append(arr[0]);
                for (int i = 1; i < arr.Length; i++)
                {
                    serviceNames.Append(", " + arr[i]);
                }
            }

            Console.WriteLine("\n*******************************************************************************\n" +
                "Номер телефона: {0}\n" +
                "Текущий баланс: {1} грн\n" +
                "Текущий тариф: {2} (Стоимость разговора в минуту - {3} грн)\n" +
                "Подключенные услуги: {4}\n" +
                "*******************************************************************************",
                PhoneNumber, CurrentBalance, TariffName, MinutePrice, serviceNames.ToString()); 
        }

        //вывод истории звонков на консоль
        public void PrintCallHistory()
        {
            if (_callHistory.Count == 0)
            {
                Console.WriteLine("\nИстория вызовов пуста!");
                return;
            }

            StringBuilder callNames = new StringBuilder("");
            for (int i = 0; i < _callHistory.Count; i++)
            {
                callNames.Append(_callHistory[i].ToString());
            }
            
            Console.WriteLine("\nИстория звонков:\n{0}", callNames);
        }
    }
}
