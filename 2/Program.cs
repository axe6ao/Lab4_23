using System;
using System.Collections.Generic;

// Клас для передачі даних від видавця до підписників
public class DataEventArgs : EventArgs
{
    public int Data { get; private set; }
    public int Priority { get; private set; }

    public DataEventArgs(int data, int priority)
    {
        Data = data;
        Priority = priority;
    }
}

// Клас видавця
public class Publisher
{
    // Подія для передачі даних підписникам
    public event EventHandler<DataEventArgs> DataPublished;

    // Метод для публікації даних
    public void PublishData(int data, int priority)
    {
        // Створення екземпляру аргументів події
        DataEventArgs args = new DataEventArgs(data, priority);

        // Виклик події з аргументами
        OnDataPublished(args);
    }

    // Метод для виклику події з аргументами
    protected virtual void OnDataPublished(DataEventArgs args)
    {
        // Перевірка, чи є підписники на подію
        if (DataPublished != null)
        {
            // Список підписників з пріоритетами
            SortedDictionary<int, List<EventHandler<DataEventArgs>>> subscribers =
                new SortedDictionary<int, List<EventHandler<DataEventArgs>>>();

            // Додавання підписників до списку згідно їх пріоритету
            foreach (EventHandler<DataEventArgs> subscriber in DataPublished.GetInvocationList())
            {
                int priority = 0;
                if (subscriber.Target is Subscriber)
                {
                    priority = ((Subscriber)subscriber.Target).Priority;
                }

                if (!subscribers.ContainsKey(priority))
                {
                    subscribers.Add(priority, new List<EventHandler<DataEventArgs>>());
                }

                subscribers[priority].Add(subscriber);
            }

            // Виклик підписників згідно пріоритету
            foreach (KeyValuePair<int, List<EventHandler<DataEventArgs>>> entry in subscribers)
            {
                foreach (EventHandler<DataEventArgs> subscriber in entry.Value)
                {
                    subscriber(this, args);
                }
            }
        }
    }
}

// Клас підписника
public class Subscriber
{
    public int Priority { get; private set; }

    public Subscriber(int priority)
    {
        Priority = priority;
    }

    // Метод-обробник події
    public void HandleData(object sender, DataEventArgs args)
    {
        Console.WriteLine("Data {0} with priority {1} received by subscriber with priority {2}",
            args.Data, args.Priority, Priority);
    }
}

// Приклад використання
class Program
{
    static void Main(string[] args)
    {
        Publisher publisher = new Publisher();
        Subscriber subscriber1 = new Subscriber(1);
        Subscriber subscriber2 = new Subscriber(2);
        Subscriber subscriber3 = new Subscriber(3);
    }
}
